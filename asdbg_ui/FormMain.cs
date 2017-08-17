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

		private TcpClient m_client;

		private NetworkStream m_ns;
		private BinaryWriter m_writer;
		private BinaryReader m_reader;

		private Thread m_clientThread;

		private string m_currentFile;

		public FormMain()
		{
			InitializeComponent();

			editor.Styles[0].Font = "Consolas";

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

		private void SetCurrentPosition(string filename, int line, int column)
		{
			Invoke(new Action(() => {
				if (m_currentFile != filename) {
					m_currentFile = filename;
					editor.Text = File.ReadAllText(filename);
				}

				editor.MarkerDeleteAll(MARKER_CURRENTLINE);
				editor.Lines[line - 1].MarkerAdd(MARKER_CURRENTLINE);

				editor.SelectionEnd = editor.SelectionStart = FindIndexOfLine(line) + (column - 1);
				editor.Focus();
			}));
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
				listLocals.Items.Clear();
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
				var lvi = listLocals.Items.Add(name);
				lvi.SubItems.Add(typeName);
				lvi.SubItems.Add(value);
			}));
		}

		private void ClientThreadFunction()
		{
			while (true) {
				ushort packetType = m_reader.ReadUInt16();
				switch (packetType) {
					case 1: ServerPacket_Location(); break;
					case 2: ServerPacket_ClearLocalVariables(); break;
					case 3: ServerPacket_LocalVariable(); break;
					default: SetStatus("Invalid packet type " + packetType + " received!"); break;
				}
			}
		}

		private void FormMain_Load(object sender, EventArgs e)
		{
			SetStatus("Connecting...");

			m_client = new TcpClient();
			m_client.BeginConnect("localhost", 8912, (ar) => {
				if (!ar.IsCompleted) {
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
		}

		private void buttonPause_Click(object sender, EventArgs e)
		{
			m_writer.Write((ushort)2);
		}

		private void buttonResume_Click(object sender, EventArgs e)
		{
			m_writer.Write((ushort)3);
		}

		private void editor_MarginClick(object sender, MarginClickEventArgs e)
		{
			if (e.Margin == 0) {
				var line = editor.Lines[editor.LineFromPosition(e.Position)];
				if ((line.MarkerGet() & (1u << MARKER_BREAKPOINT)) != 0) {
					line.MarkerDelete(MARKER_BREAKPOINT);
					m_writer.Write((ushort)5);
				} else {
					line.MarkerAdd(MARKER_BREAKPOINT);
					m_writer.Write((ushort)4);
				}
			}
		}
	}
}
