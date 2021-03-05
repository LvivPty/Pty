using Pty.Network.Core;
using Pty.Network.Extensions;
using Pty.Network.Models;
using Pty.Network.Models.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientApp
{
    class Program
    {
        private const string host = "127.0.0.1";
        private const int port = 8888;
        static TcpClient client;
        static NetworkStream stream;

        static RequestModel CreateRequest()
        {
            RequestModel request = null;
            int command = -1;
            do
            {
                Console.WriteLine("Enter command: ");
                Console.WriteLine("1. Ping");
                Console.WriteLine("2. Echo");
                Console.WriteLine("3. Send file");
                Console.WriteLine("4. Send message");
                Console.WriteLine("5. Get connected users");

                command = int.Parse(Console.ReadLine());
            } while (command > 5 && command < 1) ;
            switch (command)
            {
                case 1:request = new RequestModel() { Data = CommandFactory.CreatePing().Serialize(), Command = Command.Ping };break;
                case 2: request = new RequestModel() { Data = CommandFactory.CreateEcho().Serialize(), Command = Command.Echo }; break;
                case 3: request = new RequestModel() { Data = CommandFactory.CreateFile().Serialize(), Command = Command.File }; break;
                case 4: request = new RequestModel() { Data = CommandFactory.CreateMsg().Serialize(), Command = Command.Msg }; break;
                case 5: request = new RequestModel() { Data = CommandFactory.CreateList().Serialize(), Command = Command.List }; break;
            }

            return request;
        }

        static void Main(string[] args)
        {
            client = new TcpClient();
            try
            {
                client.Connect(host, port); 
                stream = client.GetStream(); 

                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start(); 
                SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        static void SendMessage()
        {
            while (true)
            {
                var res = CreateRequest();

                byte[] data = Encoding.Unicode.GetBytes(res.Serialize());
                stream.Write(data, 0, data.Length);
            }
        }

        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64]; 
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    var response = message.Deserialize<ResponseModel>();
                    if(response.StatusCode != StatusCode.Successed)
                    {
                        Console.WriteLine($"Error: {response.StatusCode} by Command: {response.Command}");
                        return;
                    }
                    switch (response.Command)
                    {
                        case Command.Ping:
                            Console.WriteLine($"Return Status: {response.StatusCode} by Ping Command");
                            break;
                        case Command.Echo:
                            Console.WriteLine($"Status: {response.StatusCode}");
                            break;
                        case Command.List:
                            Console.WriteLine("List of connected users:");
                            if(response.StatusCode == StatusCode.Successed)
                                response.Data.Deserialize<IEnumerable<string>>().ToList().ForEach(f => Console.WriteLine(f));
                            else
                                Console.WriteLine($"Status: {response.StatusCode}");
                            break;
                        case Command.Msg:
                            Console.WriteLine(response.Data);
                            break;
                        case Command.File:
                            if (response.StatusCode == StatusCode.Successed)
                                Console.WriteLine(response.Data);
                            else
                                Console.WriteLine($"Status: {response.StatusCode}");
                            break;
                    }
                    //Console.WriteLine(message);
                }
                catch
                {
                    Console.WriteLine(StatusCode.WrongSize);
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        static void Disconnect()
        {
            if (stream != null)
                stream.Close();
            if (client != null)
                client.Close();
            Environment.Exit(0);
        }
    }
}
