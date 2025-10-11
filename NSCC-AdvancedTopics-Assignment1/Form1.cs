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
using ChatApplicationLibraryV3; //My Custom Library. I used --> https://youtu.be/5JNS0-dZy7E?si=xQtPI0G5WDqOHVnn as a reference

namespace NSCC_AdvancedTopics_Assignment1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            chatClient.StatusMessage += OnStatusMessage;
            chatClient.MessageReceived += OnMessageReceived;
            chatClient.MessageSent += OnMessageSent;
        }

        private void OnStatusMessage(string message)
        {
            // Update the UI thread-safely
            if (InvokeRequired)
            {
                Invoke(new Action(() => rtbHistory.AppendText(message + Environment.NewLine)));
            }
            else
            {
                rtbHistory.AppendText(message + Environment.NewLine);
            }
        }

        // Add this method to handle received messages from the chat client
        private void OnMessageReceived(string message)
        {
            // Update the UI thread-safely
            if (InvokeRequired)
            {
                Invoke(new Action(() => rtbHistory.AppendText(message + Environment.NewLine)));
            }
            else
            {
                rtbHistory.AppendText(message + Environment.NewLine);
            }
        }

        private void OnMessageSent(string message)
        {
            if (InvokeRequired)
                Invoke(new Action(() => rtbHistory.AppendText(message + Environment.NewLine)));
            else
                rtbHistory.AppendText(message + Environment.NewLine);
        }

        Socket client  = null;
        string ip      = "127.0.0.1";
        int port       = 5000;
        string username = "Client";
        private readonly object socketLock = new object(); // Co-Pilot suggestion for thread safety
        private ChatClient chatClient = new ChatClient(); // My Custom Library

        private void button1_Click(object sender, EventArgs e) // should be btnSend_Click
        {
            string message = tbUsername.Text + ": " + tbMessage.Text;
            chatClient.SendMessage(message);
            //rtbHistory.AppendText(message + Environment.NewLine);
            tbMessage.Clear();
            //string message = tbUsername.Text + ": " + tbMessage.Text;
            //byte[] data = Encoding.ASCII.GetBytes(message);
            //lock (socketLock)
            //{
            //    if (client != null && client.Connected)
            //    {
            //        //client.Send(data);
            //        //rtbHistory.AppendText(message + Environment.NewLine);
            //        //tbMessage.Clear();
            //        client.BeginSend(data, 0, data.Length, SocketFlags.None, ar => //Co-Pilot suggestion
            //        {
            //            // Optionally handle send completion here
            //            this.Invoke((MethodInvoker)delegate
            //            {
            //                rtbHistory.AppendText(message + Environment.NewLine);
            //                tbMessage.Clear();
            //            });
            //        }, null);
            //    }
            //    else
            //    {
            //        MessageBox.Show("Not connected to server");
            //    }

            //}

        }

        //private void btnConnect_Click(object sender, EventArgs e)
        //{
        //    ip = tbIP.Text;
        //    int.TryParse(tbPort.Text, out port);

        //    try
        //    {
        //        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //        client.Connect(new IPEndPoint(IPAddress.Parse(ip), port));// just (__ip, __port) in net core
        //        if (client.Connected)
        //        {
        //            MessageBox.Show("Connected");

        //            StateObject state = new StateObject();
        //            state.socket = client;
        //            client.BeginReceive(state.buffer, 0, StateObject.bufferSize, SocketFlags.None, new AsyncCallback(receiveMessage), state);

        //        }
        //        else
        //        {
        //            MessageBox.Show("Not Connected");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        public void receiveMessage(IAsyncResult iasync)
        {
            chatClient.ReceiveCallback(iasync);
            //StateObject state = (StateObject)iasync.AsyncState;
            //Socket handler = state.socket;

            //try
            //{
            //    //int read = handler.EndReceive(iasync);
            //    //if (read > 0)
            //    //{
            //    //    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, read));
            //    //    handler.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(receiveMessage), state);
            //    //}
            //    //else
            //    //{
            //    //    if (state.sb.Length > 1)
            //    //    {
            //    //        string message = state.sb.ToString();
            //    //        rtbHistory.Invoke((MethodInvoker)delegate
            //    //        {
            //    //            rtbHistory.AppendText("Server: " + message + Environment.NewLine);
            //    //        });
            //    //    }
            //    //    handler.Close();
            //    //}

            //    int i = handler.EndReceive(iasync);

            //    string data = Encoding.ASCII.GetString(state.buffer, 0, i);

            //    rtbHistory.Invoke(new MethodInvoker(delegate()
            //    {
            //        rtbHistory.AppendText(data + Environment.NewLine); //"Server: " + 
            //    }));

            //    handler.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(receiveMessage), state);

            //}
            //catch (ObjectDisposedException)
            //{
            //    // Socket was closed, ignore this exception as it's expected on disconnect
            //}
            //catch (SocketException _e)
            //{
            //    MessageBox.Show(_e.Message);
            //}
            //catch (Exception _e)
            //{
            //    MessageBox.Show(_e.Message);
            //}


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnUsername_Click(object sender, EventArgs e)
        {
            username = tbUsername.Text;
            MessageBox.Show($"Username set to: {username}");
        }

        // Connect
        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chatClient.Connect(ip, 5000, receiveMessage);
            //ip = "127.0.0.1";
            ////int.TryParse(tbPort.Text, out port);

            //try
            //{
            //    client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //    client.Connect(new IPEndPoint(IPAddress.Parse(ip), port));// just (__ip, __port) in net core
            //    if (client.Connected)
            //    {
            //        MessageBox.Show("Connected");

            //        StateObject state = new StateObject();
            //        state.socket = client;
            //        client.BeginReceive(state.buffer, 0, StateObject.bufferSize, SocketFlags.None, new AsyncCallback(receiveMessage), state);

            //    }
            //    else
            //    {
            //        MessageBox.Show("Not Connected");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void mnDisconnect_Click(object sender, EventArgs e)
        {
            chatClient.Disconnect();
            //disconnect
            //lock (socketLock)
            //{
            //    if (client != null && client.Connected)
            //    {
            //        client.Shutdown(SocketShutdown.Both);
            //        client.Close();
            //        MessageBox.Show("Disconnected");
            //    }
            //    else
            //    {
            //        MessageBox.Show("Not connected to server");
            //    }
            //}

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
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
