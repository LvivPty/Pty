using Pty.Network.Extensions;
using Pty.Network.Models;
using Pty.Network.Models.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Pty.Network.Services
{
    public class ClientObject
    {
        public static List<(string Login, string Password)> FakeDb
            => new List<(string Login, string Password)>() 
            {
                ("lpnu", "123"),
                ("someLogin", "somePass"),
                ("qwe", "1233"),
                ("www", "33"),
                ("aa", "aa")
            };

        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }

        TcpClient client;
        ServerObject server;

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        private bool IsAuth(string login, string pass)
        {
            return FakeDb.Where(p => p.Login.Contains(login) && p.Password.Contains(pass)).Count() > 0;
        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();

                Console.WriteLine($"Client with id: {Id} connected");

                while (true)
                {
                    var message = GetMessage();
                    if (string.IsNullOrEmpty(message))
                        continue;

                    var res = message.Deserialize<RequestModel>();
                    try
                    {
                        

                        switch (res.Command)
                        {
                            case Core.Command.Ping:
                                server.SendMessage(new ResponseModel() { Command = res.Command, StatusCode = Core.StatusCode.Successed }.Serialize(), Id);
                                break;
                            case Core.Command.Echo:
                                Console.WriteLine($"Echo: {res.Data.Deserialize<EchoModel>().Message}");
                                server.SendMessage(new ResponseModel() { Command = res.Command, StatusCode = Core.StatusCode.Successed }.Serialize(), Id);
                                break;
                            case Core.Command.List:
                                var listModel = res.Data.Deserialize<ListModel>();
                                if (!IsAuth(listModel.Login, listModel.Password))
                                    server.SendMessage(new ResponseModel() { Command = res.Command, StatusCode = Core.StatusCode.NotLoggedIn }.Serialize(), Id);
                                server.SendMessage(new ResponseModel() { Command = res.Command, StatusCode = Core.StatusCode.Successed, Data = server.clients.Select(s => s.Id).Serialize() }.Serialize(), Id);
                                break;
                            case Core.Command.Msg:
                                var msgModel = res.Data.Deserialize<MsgModel>();
                                if (!IsAuth(msgModel.Login, msgModel.Password))
                                    server.SendMessage(new ResponseModel() { Command = res.Command, StatusCode = Core.StatusCode.NotLoggedIn }.Serialize(), Id);
                                server.BroadcastMessage(new ResponseModel() { Command = res.Command, StatusCode = Core.StatusCode.Successed, Data = $"(From: {msgModel.Login})Msg: {msgModel.Message}" }.Serialize(), Id);
                                break;
                            case Core.Command.File:
                                var fileModel = res.Data.Deserialize<FileModel>();
                                if (!IsAuth(fileModel.Login, fileModel.Password))
                                    server.SendMessage(new ResponseModel() { Command = res.Command, StatusCode = Core.StatusCode.NotLoggedIn }.Serialize(), Id);
                                server.BroadcastMessage(new ResponseModel() { Command = res.Command, StatusCode = Core.StatusCode.Successed, Data = $"(User: {fileModel.Login}) added file" }.Serialize(), Id);
                                break;
                        }
                    }
                    catch
                    {
                        server.SendMessage(new ResponseModel() { Command = res.Command, StatusCode = Core.StatusCode.ServerError }.Serialize(), Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("User logout");
            }
            finally
            {
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        private string GetMessage()
        {
            byte[] data = new byte[64]; 
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}
