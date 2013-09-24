﻿using Server.Model;
using Server.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Server.Control
{
    ///<summary>
    ///The base class for the Control.
    ///</summary>
    class ServerControl
    {
        private ServerModel serverModel;
        private ServerView serverView;
        private TcpListener tcpListener;

        public ServerControl()
        {
            serverModel = new ServerModel();

            //test doctor list:
            serverModel.doctors = new List<DoctorCredentials> {new DoctorCredentials("Jim","Kanarie") };

            serverView = new ServerView(serverModel);
            this.tcpListener = new TcpListener(System.Net.IPAddress.Any, 31337);
            listenForClients();
        }

        ///<summary>
        ///Start listening for connecting TcpClients.
        ///</summary>
        public void listenForClients()
        {
            tcpListener.Start();
            TcpClient tempClient;

            for (; ; )
            {
                try
                {
                    serverView.writeToConsole("Listening..");
                    tempClient = tcpListener.AcceptTcpClient();
                    acceptAClient(tempClient);
                    serverView.writeToConsole("Connected.");
                    serverView.writeToConsole("Online clients in list: " + serverModel.onlineClients.Count.ToString());
                }
                catch (Exception)
                {
                }
               
                Thread.Sleep(10);
            }
        }

        ///<summary>
        ///Instantiate a Clientobject for a connected TcpClient.
        ///</summary>
        public void acceptAClient(TcpClient client)
        {
            new Client(client, this);
        }

        ///<summary>
        ///Forward client to servermodel's dictionary.
        ///</summary>
        public void addClientToList(Client client)
        {
            serverModel.writeBikeData(client, null);
        }

        ///<summary>
        ///Change desired online/offline status for a client.
        ///</summary>
        public void changeClientStatus(Client client, String status)
        {
            serverModel.changeClientStatus(client, status);
        }

        ///<summary>
        ///Forward chatmessage received from client to servermodel.
        ///</summary>
        public void forwardMessage(Kettler_X7_Lib.Objects.Packet pack)
        {
            Kettler_X7_Lib.Objects.ChatMessage message = (Kettler_X7_Lib.Objects.ChatMessage)pack.Data;

            serverView.writeToConsole(message.Message);

            foreach ( Client client in serverModel.onlineClients)
            {
                if (client.userName == message.Receiver)
                {
                    client.sendHandler(pack);
                }
            }
        }

        ///<summary>
        ///Forward bikecommand received from client to servermodel.
        ///</summary>
        public void forwardBikeCommand(Kettler_X7_Lib.Objects.Packet pack)
        {
            Kettler_X7_Lib.Objects.BikeControl bikeControl = (Kettler_X7_Lib.Objects.BikeControl)pack.Data;
            foreach (Client client in serverModel.onlineClients)
            {
                if (client.userName == bikeControl.Receiver)
                {
                    client.sendHandler(pack);
                }
            }
        }

        ///<summary>
        ///Check the Handshake Request for login information and respond.
        /// </summary>
        public void handshakeResponse(Client client, Kettler_X7_Lib.Objects.Handshake shake)
        {
            Kettler_X7_Lib.Objects.ResponseHandshake response = new Kettler_X7_Lib.Objects.ResponseHandshake();
            response.Result = Kettler_X7_Lib.Objects.ResponseHandshake.ResultType.RESULTTYPE_ACCESSDENIED;

            switch (shake.ClientFlag)
            {
                case Kettler_X7_Lib.Objects.Client.ClientFlag.CLIENTFLAG_DOCTORAPP:
                    DoctorCredentials doc = serverModel.doctors.Find(x => x.username == shake.Username);

                    if (doc == null)
                        response.Result = Kettler_X7_Lib.Objects.ResponseHandshake.ResultType.RESULTTYPE_INVALIDCREDENTIALS;
                    else
                    {
                        if (doc.password == shake.Password)
                            response.Result = Kettler_X7_Lib.Objects.ResponseHandshake.ResultType.RESULTTYPE_OK;
                        else
                        {
                            doc.logins.Add(new Log(DateTime.Now, false));
                            if (doc.logins.Count > 5)
                            {
                                bool timeOutAccount = true;
                                TimeSpan logSpan = new TimeSpan(1, 0, 0);
                                for (int i = doc.logins.Count; i <= doc.logins.Count - 5; i--)
                                {
                                    if (!doc.logins[i].accepted)
                                    {
                                        if (DateTime.Now.Subtract(doc.logins[i].login) >= logSpan)
                                            timeOutAccount = false;
                                    }
                                    else
                                        timeOutAccount = false;
                                }

                                if (timeOutAccount)
                                    response.Result = Kettler_X7_Lib.Objects.ResponseHandshake.ResultType.RESULTTYPE_ACCESSDENIED;
                                else
                                    response.Result = Kettler_X7_Lib.Objects.ResponseHandshake.ResultType.RESULTTYPE_INVALIDCREDENTIALS;
                            }
                            else
                                response.Result = Kettler_X7_Lib.Objects.ResponseHandshake.ResultType.RESULTTYPE_INVALIDCREDENTIALS;
                        }
                    }
                    break;
                case Kettler_X7_Lib.Objects.Client.ClientFlag.CLIENTFLAG_CUSTOMERAPP:
                    if (serverModel.onlineClients.Contains(client))
                    {
                        //is logged in already, send message back!
                        response.Result = Kettler_X7_Lib.Objects.ResponseHandshake.ResultType.RESULTTYPE_INVALIDCREDENTIALS;
                    }
                    else
                    {
                        // everything's alright!
                        addClientToList(client);
                        response.Result = Kettler_X7_Lib.Objects.ResponseHandshake.ResultType.RESULTTYPE_OK;
                    }
                    break;
                default:
                    break;
            }

            Kettler_X7_Lib.Objects.Packet responsePack = new Kettler_X7_Lib.Objects.Packet();
            responsePack.Data = response;
            responsePack.Flag = Kettler_X7_Lib.Objects.Packet.PacketFlag.PACKETFLAG_RESPONSE_HANDSHAKE;
            client.sendHandler(responsePack);
        }

        ///<summary>
        ///Forward client data to servermodel.
        ///</summary>
        public void writeToModel(Client client, Object data)
        {
            serverModel.writeBikeData(client, data);
        }

        ///<summary>
        ///Tell servermodel to finalize data after a client has disconnected.
        ///</summary>
        public void finalizeClient()
        {
            serverModel.finalizeData();
        }
    }
}
