using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace asdbg_ui
{
	public partial class FormMain : Form
	{
		private const int MARKER_CURRENTLINE = 0;
		private const int MARKER_BREAKPOINT = 1;

		private AngelscriptLexer m_lexer;

		private TcpClient m_client;

		private NetworkStream m_ns;
		private BinaryWriter m_writer;
		private BinaryReader m_reader;

		private Thread m_clientThread;

		private string m_currentPath;
		private string m_currentFile;

		private string m_brokenFilename;
		private int m_brokenLine;

		private List<ScriptBreakpoint> m_breakpoints = new List<ScriptBreakpoint>();

		public FormMain()
		{
			InitializeComponent();

			m_lexer = new AngelscriptLexer();

			foreach (var style in editor.Styles) {
				style.Font = "Consolas";
				style.Size = 10;
			}
			editor.Styles[AngelscriptLexer.StyleIdentifier].ForeColor = Color.DimGray;
			editor.Styles[AngelscriptLexer.StyleConstant].ForeColor = Color.DeepPink;
			editor.Styles[AngelscriptLexer.StyleString].ForeColor = Color.DarkRed;
			editor.Styles[AngelscriptLexer.StyleStorageModifier].ForeColor = Color.DarkBlue;
			editor.Styles[AngelscriptLexer.StyleStorageType].ForeColor = Color.DarkTurquoise;
			editor.Styles[AngelscriptLexer.StyleVariable].ForeColor = Color.DarkBlue;
			editor.Styles[AngelscriptLexer.StyleControl].ForeColor = Color.Red;

			editor.TabWidth = 2;

			foreach (var m in editor.Margins) {
				m.Type = MarginType.BackColor;
				m.Width = 0;
			}

			editor.Margins[0].Type = MarginType.Symbol;
			editor.Margins[0].Cursor = MarginCursor.Arrow;
			editor.Margins[0].Width = 24;
			editor.Margins[0].Mask = (1u << 1);
			editor.Margins[0].Sensitive = true;

			editor.Margins[1].Type = MarginType.Number;
			editor.Margins[1].Width = 32;
			editor.Margins[1].Mask = (1u << 0);

			editor.Markers[MARKER_CURRENTLINE].Symbol = MarkerSymbol.Arrow;
			editor.Markers[MARKER_CURRENTLINE].SetBackColor(Color.Yellow);
			editor.Markers[MARKER_CURRENTLINE].SetForeColor(Color.Black);

			editor.Markers[MARKER_BREAKPOINT].Symbol = MarkerSymbol.Circle;
			editor.Markers[MARKER_BREAKPOINT].SetBackColor(Color.Red);
			editor.Markers[MARKER_BREAKPOINT].SetForeColor(Color.Black);
		}

		private string ProperPath(string path)
		{
			return path.Replace('\\', '/');
		}

		private int FindIndexOfLine(int line)
		{
			if (line == 1) {
				return 0;
			}

			string text = editor.Text;
			int curLine = 1;
			for (int i = 0; i < text.Length; i++) {
				if (text[i] == '\n') {
					if (++curLine == line) {
						return i + 1;
					}
				}
			}
			return 0;
		}

		private void SetStatus(string status)
		{
			Invoke(new Action(() => {
				labelStatus.Text = status;
			}));
		}

		private void AddPathNodes(string path, TreeNodeCollection nodes)
		{
			var dirs = Directory.GetDirectories(path);

			foreach (var dir in dirs) {
				if (Directory.GetFiles(dir, "*.as", SearchOption.AllDirectories).Length == 0) {
					continue;
				}

				var node = nodes.Add(Path.GetFileName(dir));
				node.Tag = ProperPath(dir);
				node.SelectedImageKey = node.ImageKey = "Folder";
				AddPathNodes(dir, node.Nodes);
			}

			var files = Directory.GetFiles(path, "*.as");

			foreach (var file in files) {
				var node = nodes.Add(Path.GetFileName(file));
				node.Tag = ProperPath(file);
				node.SelectedImageKey = node.ImageKey = "Script";
			}
		}

		private void SetCurrentPath(string path)
		{
			Invoke(new Action(() => {
				if (m_currentPath == path) {
					return;
				}

				m_currentPath = path;
				listFiles.Nodes.Clear();
				AddPathNodes(path, listFiles.Nodes);
			}));
		}

		private void SetCurrentLine(int line)
		{
			editor.MarkerDeleteAll(MARKER_CURRENTLINE);
			editor.Lines[line - 1].MarkerAdd(MARKER_CURRENTLINE);
		}

		private void SetCurrentFile(string filename)
		{
			if (m_currentFile == filename) {
				return;
			}

			m_currentFile = filename;

			editor.MarkerDeleteAll(MARKER_BREAKPOINT);
			editor.MarkerDeleteAll(MARKER_CURRENTLINE);

			string absolutePath = filename;
			if (!Path.IsPathRooted(absolutePath)) {
				absolutePath = ProperPath(m_currentPath + "/" + filename);
			}
			editor.Text = File.ReadAllText(absolutePath);

			foreach (var bp in m_breakpoints) {
				if (bp.Filename != filename) {
					break;
				}
				editor.Lines[bp.Line - 1].MarkerAdd(MARKER_BREAKPOINT);
			}

			//TODO: Focus on file in tree view here

			if (m_brokenFilename == filename) {
				SetCurrentLine(m_brokenLine);
			}
		}

		private void SetCurrentPosition(string filename, int line, int column)
		{
			Invoke(new Action(() => {
				m_brokenFilename = filename;
				m_brokenLine = line;

				SetCurrentFile(filename);
				SetCurrentLine(line);

				editor.SelectionEnd = editor.SelectionStart = FindIndexOfLine(line) + (column - 1);
				editor.Focus();
				editor.ScrollCaret();
			}));
		}

		private void ServerPacket_Path()
		{
			ushort pathLength = m_reader.ReadUInt16();
			string path = Encoding.UTF8.GetString(m_reader.ReadBytes(pathLength));

			SetCurrentPath(path);
		}

		private void ServerPacket_Location()
		{
			ushort filenameLength = m_reader.ReadUInt16();
			string filename = Encoding.UTF8.GetString(m_reader.ReadBytes(filenameLength));
			int line = m_reader.ReadInt32();
			int column = m_reader.ReadInt32();

			SetStatus(filename + " (line " + line + ", col " + column + ")");
			SetCurrentPosition(filename, line, column);
		}

		private void ServerPacket_ClearLocalVariables()
		{
			Invoke(new Action(() => {
				gridLocals.Rows.Clear();
			}));
		}

		private void ServerPacket_LocalVariable()
		{
			ushort nameLength = m_reader.ReadUInt16();
			string name = Encoding.UTF8.GetString(m_reader.ReadBytes(nameLength));

			ushort typeNameLength = m_reader.ReadUInt16();
			string typeName = Encoding.UTF8.GetString(m_reader.ReadBytes(typeNameLength));

			ushort valueLength = m_reader.ReadUInt16();
			string value = Encoding.UTF8.GetString(m_reader.ReadBytes(valueLength));

			Invoke(new Action(() => {
				gridLocals.Rows.Add(name, typeName, value);
			}));
		}

		private void ClientThreadFunction()
		{
			while (true) {
				try {
					ushort packetType = m_reader.ReadUInt16();
					switch (packetType) {
						case 1: ServerPacket_Location(); break;
						case 2: ServerPacket_ClearLocalVariables(); break;
						case 3: ServerPacket_LocalVariable(); break;
						case 4: ServerPacket_Path(); break;
						default: SetStatus("Invalid packet type " + packetType + " received!"); break;
					}
				} catch (IOException) {
					SetStatus("Disconnected!");
					break;
				}
			}
		}

		private void FormMain_Load(object sender, EventArgs e)
		{
			SetStatus("Connecting...");

			m_client = new TcpClient();
			m_client.BeginConnect("localhost", 8912, (ar) => {
				if (!ar.IsCompleted || !m_client.Connected) {
					SetStatus("Connection failed.");
					return;
				}

				m_ns = m_client.GetStream();
				m_writer = new BinaryWriter(m_ns);
				m_reader = new BinaryReader(m_ns);

				SetStatus("Connected.");

				m_clientThread = new Thread(ClientThreadFunction);
				m_clientThread.Start();
			}, null);
		}

		private void buttonStep_Click(object sender, EventArgs e)
		{
			m_writer.Write((ushort)1);
			editor.MarkerDeleteAll(MARKER_CURRENTLINE);
		}

		private void buttonStepOver_Click(object sender, EventArgs e)
		{
			m_writer.Write((ushort)7);
		}

		private void buttonStepOut_Click(object sender, EventArgs e)
		{
			m_writer.Write((ushort)8);
		}

		private void buttonPause_Click(object sender, EventArgs e)
		{
			m_writer.Write((ushort)2);
		}

		private void buttonResume_Click(object sender, EventArgs e)
		{
			m_writer.Write((ushort)3);
			editor.MarkerDeleteAll(MARKER_CURRENTLINE);

			m_brokenFilename = null;
			m_brokenLine = 0;
			gridLocals.Rows.Clear();

			SetStatus("Resumed.");
		}

		private void editor_MarginClick(object sender, MarginClickEventArgs e)
		{
			if (m_currentFile == null) {
				return;
			}

			if (e.Margin == 0) {
				var lineIndex = editor.LineFromPosition(e.Position);
				var line = editor.Lines[lineIndex];

				if ((line.MarkerGet() & (1u << MARKER_BREAKPOINT)) != 0) {
					var bpIndex = m_breakpoints.FindIndex(bp => bp.Line == lineIndex + 1 && bp.Filename == m_currentFile);
					if (bpIndex != -1) {
						m_breakpoints.RemoveAt(bpIndex);
					}

					line.MarkerDelete(MARKER_BREAKPOINT);
					m_writer.Write((ushort)5);
				} else {
					m_breakpoints.Add(new ScriptBreakpoint() {
						Filename = m_currentFile,
						Line = lineIndex + 1
					});

					line.MarkerAdd(MARKER_BREAKPOINT);
					m_writer.Write((ushort)4);
				}

				m_writer.Write((ushort)m_currentFile.Length);
				m_writer.Write(Encoding.UTF8.GetBytes(m_currentFile));
				m_writer.Write(lineIndex + 1);
			}
		}

		private void listFiles_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node == null || e.Node.ImageKey != "Script") {
				return;
			}

			var filename = (string)e.Node.Tag;
			if (filename == null) {
				return;
			}

			SetCurrentFile(filename);
		}

		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			m_client.Close();
		}

		private void gridLocals_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			var row = gridLocals.Rows[e.RowIndex];
			var name = (string)row.Cells[0].Value;
			var value = (string)row.Cells[2].Value;

			m_writer.Write((ushort)6);

			m_writer.Write((ushort)name.Length);
			m_writer.Write(Encoding.UTF8.GetBytes(name));

			m_writer.Write((ushort)value.Length);
			m_writer.Write(Encoding.UTF8.GetBytes(value));
		}

		private void editor_StyleNeeded(object sender, StyleNeededEventArgs e)
		{
			m_lexer.Style(editor, editor.GetEndStyled(), e.Position);
		}
	}
}
