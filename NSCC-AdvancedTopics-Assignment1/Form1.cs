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
                Invoke(new Action(() => rtbHistory.AppendText(message + "\n")));
            else
                rtbHistory.AppendText(message + "\n");

        }

        private void OnMessageReceived(string message)
        {
            // Update the UI thread-safely
            if (InvokeRequired)
                Invoke(new Action(() => rtbHistory.AppendText(message + "\n")));
            else
                rtbHistory.AppendText(message + "\n");

        }

        private void OnMessageSent(string message)
        {
            if (InvokeRequired)
                Invoke(new Action(() => rtbHistory.AppendText(message + "\n")));
            else
                rtbHistory.AppendText(message + "\n");
        }

        Socket client  = null;
        string ip      = "127.0.0.1";
        int port       = 5000;
        string username = "Client";
        private readonly object socketLock = new object();
        private ChatClient chatClient = new ChatClient(); // My Custom Library

        private void button1_Click(object sender, EventArgs e) // should be btnSend_Click
        {
            string message = tbUsername.Text + ": " + tbMessage.Text;
            chatClient.SendMessage(message);
            tbMessage.Clear();

        }

        public void receiveMessage(IAsyncResult iasync)
        {
            chatClient.ReceiveCallback(iasync);
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
            chatClient.Connect(ip, port, receiveMessage);
        }

        private void mnDisconnect_Click(object sender, EventArgs e)
        {
            chatClient.Disconnect();
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
