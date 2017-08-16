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
		private TcpClient m_client;

		private NetworkStream m_ns;
		private BinaryWriter m_writer;
		private BinaryReader m_reader;

		private Thread m_clientThread;

		public FormMain()
		{
			InitializeComponent();
		}

		private void SetStatus(string status)
		{
			Invoke(new Action(() => {
				labelStatus.Text = status;
			}));
		}

		private void ServerPacket_Location()
		{
			ushort filenameLength = m_reader.ReadUInt16();
			string filename = Encoding.UTF8.GetString(m_reader.ReadBytes(filenameLength));
			int line = m_reader.ReadInt32();
			int column = m_reader.ReadInt32();

			SetStatus(filename + " (line " + line + ", col " + column + ")");
		}

		private void ClientThreadFunction()
		{
			while (true) {
				ushort packetType = m_reader.ReadUInt16();
				switch (packetType) {
					case 1: ServerPacket_Location(); break;
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
	}
}
