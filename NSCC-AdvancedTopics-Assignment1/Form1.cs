using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace NSCC_AdvancedTopics_Assignment1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Socket client  = null;
        string ip      = string.Empty;
        int port       = 1200;

        private void button1_Click(object sender, EventArgs e) // should be btnSend_Click
        {

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            ip = tbIP.Text;
            int.TryParse(tbPort.Text, out port);

            try
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(new IPEndPoint(IPAddress.Parse(ip), port));// just (__ip, __port) in net core
                if (client.Connected)
                {
                    MessageBox.Show("Connected");

                    StateObject state = new StateObject();
                    state.socket = client;
                    client.BeginReceive(state.buffer, 0, StateObject.bufferSize, SocketFlags.None, new AsyncCallback(receiveMessage), state);

                }
                else
                {
                    MessageBox.Show("Not Connected");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void receiveMessage(IAsyncResult iasync)
        {
            StateObject state = (StateObject)iasync.AsyncState;
            Socket handler = state.socket;

            try
            {
                //int read = handler.EndReceive(iasync);
                //if (read > 0)
                //{
                //    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, read));
                //    handler.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(receiveMessage), state);
                //}
                //else
                //{
                //    if (state.sb.Length > 1)
                //    {
                //        string message = state.sb.ToString();
                //        rtbHistory.Invoke((MethodInvoker)delegate
                //        {
                //            rtbHistory.AppendText("Server: " + message + Environment.NewLine);
                //        });
                //    }
                //    handler.Close();
                //}

                int i = handler.EndReceive(iasync);

                string data = Encoding.ASCII.GetString(state.buffer, 0, i);

                rtbHistory.Invoke(new MethodInvoker(delegate()
                {
                    rtbHistory.AppendText("Server: " + data + Environment.NewLine);
                }));

                handler.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(receiveMessage), state);

            }
            catch (SocketException _e)
            {
                MessageBox.Show(_e.Message);
            }
            catch (Exception _e)
            {
                MessageBox.Show(_e.Message);
            }


        }
    }

    public class StateObject
    {
        public Socket socket = null;
        public const int bufferSize = 1024;
        public byte[] buffer = new byte[bufferSize];
        public StringBuilder sb = new StringBuilder();
    }
}
