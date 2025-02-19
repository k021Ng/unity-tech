﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

using Lidgren.Network;

using SamplesCommon;

namespace ChatServer
{
    public class DATA_TYPE
    {
        public const int DAT_CONNECT = 0;
        public const int DAT_UNET = 1;
    }

    static class Program
	{
		private static Form1 s_form;
		private static NetServer s_server;
		private static NetPeerSettingsWindow s_settingsWindow;

        private class Room
        {
            public string id;
            public int timeCreate;

            //public string ipHost;
            //public int portHost;

            //public string ipClient;
            //public int portClient;
            public NetConnection host;
            public NetConnection client;
        }

        private static Dictionary<string, Room> s_rooms = new Dictionary<string, Room>();
        private static Dictionary<string, string> s_refRooms = new Dictionary<string, string>();
		
		[STAThread]
		static void Main()
	{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			s_form = new Form1();

			// set up network
			NetPeerConfiguration config = new NetPeerConfiguration("chat");
			config.MaximumConnections = 100;
			config.Port = 14242;
			s_server = new NetServer(config);

			StartServer();

			Application.Idle += new EventHandler(Application_Idle);
			Application.Run(s_form);
		}

		private static void Output(string text)
		{
			NativeMethods.AppendText(s_form.richTextBox1, text);
		}

		private static void Application_Idle(object sender, EventArgs e)
		{
			while (NativeMethods.AppStillIdle)
			{
				NetIncomingMessage im;
				while ((im = s_server.ReadMessage()) != null)
				{
					// handle incoming message
					switch (im.MessageType)
					{
						case NetIncomingMessageType.DebugMessage:
						case NetIncomingMessageType.ErrorMessage:
						case NetIncomingMessageType.WarningMessage:
						case NetIncomingMessageType.VerboseDebugMessage:
							string text = im.ReadString();
							Output(text);
							break;

						case NetIncomingMessageType.StatusChanged:
							NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

							string reason = im.ReadString();
							Output(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

                            if (status == NetConnectionStatus.Connected)
                            {
                                string roomId = im.SenderConnection.RemoteHailMessage.ReadString();
                                Output("Remote hail: roomId>" + roomId);

                                // create host
                                if (s_rooms.ContainsKey(roomId) == false)
                                {
                                    Room room = new Room();
                                    room.id = roomId;
                                    room.host = im.SenderConnection;
                                    s_rooms[room.id] = room;

                                    s_refRooms[im.SenderConnection.RemoteUniqueIdentifier.ToString()] = room.id;
                                }
                                // create client
                                else 
                                {
                                    Room room = s_rooms[roomId];
                                    room.client = im.SenderConnection;

                                    s_refRooms[im.SenderConnection.RemoteUniqueIdentifier.ToString()] = room.id;

                                    // send connect to host
                                    NetOutgoingMessage om = s_server.CreateMessage(im.Data.Length);
                                    om.Write(DATA_TYPE.DAT_CONNECT);
                                    om.Write(im.SenderConnection.RemoteUniqueIdentifier.ToString());
                                    s_server.SendMessage(om, room.host, NetDeliveryMethod.ReliableOrdered, 0);
                                }
                            }
                            else if (status == NetConnectionStatus.Disconnected)
                            {
                                s_refRooms.Clear();
                                s_rooms.Clear();
                            }

							UpdateConnectionsList();
							break;
						case NetIncomingMessageType.Data:
                            // incoming chat message from a client

                            // broadcast this to all connections, except sender
                            //List<NetConnection> all = s_server.Connections; // get copy
                            //all.Remove(im.SenderConnection);

                            //if (all.Count > 0)
                            //{
                            //    NetOutgoingMessage om = s_server.CreateMessage(im.Data.Length);
                            //    om.Write(im.Data);
                            //    s_server.SendMessage(om, all, NetDeliveryMethod.ReliableOrdered, 0);
                            //}

                            string remote_id = im.SenderConnection.RemoteUniqueIdentifier.ToString();
                            Console.Out.Write("remote id >" + remote_id);

                            if (s_refRooms.ContainsKey(remote_id))
                            {
                                string room_id = s_refRooms[remote_id];
                                Room room = s_rooms[room_id];

                                NetOutgoingMessage om = s_server.CreateMessage(im.Data.Length);
                                om.Write(im.Data);
                                if (remote_id == room.host.RemoteUniqueIdentifier.ToString() && room.client != null)
                                {
                                    s_server.SendMessage(om, room.client, im.DeliveryMethod, im.SequenceChannel);
                                }
                                else if (room.client != null && remote_id == room.client.RemoteUniqueIdentifier.ToString())
                                {
                                    s_server.SendMessage(om, room.host, im.DeliveryMethod, im.SequenceChannel);
                                }
								Output("xx-- data > " + im.DeliveryMethod.ToString() + " - " + im.SequenceChannel.ToString());
                            }
                            else
                            {
                                Output("ERROR: cant find room for remote id>" + remote_id);
                            }

                            /*
							string chat = im.ReadString();

							Output("Broadcasting '" + chat + "'");

							// broadcast this to all connections, except sender
							List<NetConnection> all = s_server.Connections; // get copy
							all.Remove(im.SenderConnection);

							if (all.Count > 0)
							{
								NetOutgoingMessage om = s_server.CreateMessage();
								om.Write(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " said: " + chat);
								s_server.SendMessage(om, all, NetDeliveryMethod.ReliableOrdered, 0);
							}
                            */
                            break;
						default:
							Output("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes " + im.DeliveryMethod + "|" + im.SequenceChannel);
							break;
					}
					s_server.Recycle(im);
				}
				Thread.Sleep(1);
			}
		}

		private static void UpdateConnectionsList()
		{
			s_form.listBox1.Items.Clear();

			foreach (NetConnection conn in s_server.Connections)
			{
				string str = NetUtility.ToHexString(conn.RemoteUniqueIdentifier) + " from " + conn.RemoteEndPoint.ToString() + " [" + conn.Status + "]";
				s_form.listBox1.Items.Add(str);
			}
		}

		// called by the UI
		public static void StartServer()
		{
			s_server.Start();
		}

		// called by the UI
		public static void Shutdown()
		{
			s_server.Shutdown("Requested by user");
		}

		// called by the UI
		public static void DisplaySettings()
		{
			if (s_settingsWindow != null && s_settingsWindow.Visible)
			{
				s_settingsWindow.Hide();
			}
			else
			{
				if (s_settingsWindow == null || s_settingsWindow.IsDisposed)
					s_settingsWindow = new NetPeerSettingsWindow("Chat server settings", s_server);
				s_settingsWindow.Show();
			}
		}
	}
}
