using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatApplicationLibrary
{
    public class ChatClient
    {
        private Socket client;
        private readonly object socketLock = new object();
        public event Action<string> MessageReceived;
        public event Action<string> StatusMessage;
        public event Action<string> MessageSent;

        public void Connect(string ip, int port, AsyncCallback receiveMessage)
        {
            lock (socketLock)
            {
                try
                {
                    client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    client.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                    if (client.Connected)
                    {
                        StatusMessage?.Invoke("Connected");

                        StateObject state = new StateObject();
                        state.socket = client;
                        client.BeginReceive(state.buffer, 0, StateObject.bufferSize, SocketFlags.None, receiveMessage, state);
                    }
                    else
                    {
                        StatusMessage?.Invoke("Not Connected");
                    }
                }
                catch (Exception ex)
                {
                    StatusMessage?.Invoke(ex.Message);
                }
            }
        }

        public void SendMessage(string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            lock (socketLock)
            {
                if (client != null && client.Connected)
                {
                    client.BeginSend(data, 0, data.Length, SocketFlags.None, ar =>
                    {
                        // Notify UI that the message was sent
                        MessageSent?.Invoke(message);
                    }, null);
                }
                else
                {
                    StatusMessage?.Invoke("Not connected to server");
                }
            }
        }

        public void Disconnect()
        {
            lock (socketLock)
            {
                if (client != null && client.Connected)
                {
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                    client = null;
                    StatusMessage?.Invoke("Disconnected");
                }
                else
                {
                    StatusMessage?.Invoke("Not connected to server");
                }
            }
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.socket;
            try
            {
                int i = handler.EndReceive(ar);
                string data = Encoding.ASCII.GetString(state.buffer, 0, i);
                MessageReceived?.Invoke(data);
                handler.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, ReceiveCallback, state);
            }
            catch (ObjectDisposedException)
            {
                // Socket was closed, ignore this exception as it's expected on disconnect
            }
            catch (SocketException _e)
            {
                StatusMessage?.Invoke(_e.Message);
            }
            catch (Exception _e)
            {
                StatusMessage?.Invoke(_e.Message);
            }
        }

        private class StateObject
        {
            public Socket socket = null;
            public const int bufferSize = 1024;
            public byte[] buffer = new byte[bufferSize];
            //public StringBuilder sb = new StringBuilder();
        }
    }
}
