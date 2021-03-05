using Pty.Network.Models.Commands;
using System;

namespace ClientApp
{
    public class CommandFactory
    {
        public static EchoModel CreateEcho()
        {
            EchoModel model = new EchoModel();

            Console.WriteLine("Input echo msg to server: ");
            model.Message = Console.ReadLine();

            return model;
        }
        public static PingModel CreatePing()
        {
            PingModel model = new PingModel();

            return model;
        }

        public static FileModel CreateFile()
        {
            FileModel model = new FileModel();

            Console.WriteLine("Input login: ");
            model.Login = Console.ReadLine();
            Console.WriteLine("Input password: ");
            model.Password = Console.ReadLine();

            Console.WriteLine("File name: ");
            model.Name = Console.ReadLine();
            Console.WriteLine("File path: ");
            model.Path = Console.ReadLine();

            return model;
        }

        public static MsgModel CreateMsg()
        {
            MsgModel model = new MsgModel();

            Console.WriteLine("Input login: ");
            model.Login = Console.ReadLine();
            Console.WriteLine("Input password: ");
            model.Password = Console.ReadLine();

            Console.WriteLine("Input msg: ");
            model.Message = Console.ReadLine();

            return model;
        }

        public static ListModel CreateList()
        {
            ListModel model = new ListModel();

            Console.WriteLine("Input login: ");
            model.Login = Console.ReadLine();
            Console.WriteLine("Input password: ");
            model.Password = Console.ReadLine();

            return model;
        }
    }
}
