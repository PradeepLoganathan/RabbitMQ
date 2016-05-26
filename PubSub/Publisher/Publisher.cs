using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;

namespace Publisher
{

    //This program uses an exchange to send and recieve messages
    class PublisherProgram
    {
        static void Main(string[] args)
        {
            Publisher p = new Publisher();
            p.Initialize();
            p.Publish();
            p.Shutdown();            
        }


    }

    class Publisher
    {
        private ConnectionFactory Factory;
        private IConnection Connection;
        private IModel Channel;


        public void Initialize()
        {
            try
            {
                Factory = new ConnectionFactory();
                Factory.HostName = "localhost";
                Factory.UserName = "guest";
                Factory.Password = "guest";

                Connection = Factory.CreateConnection();

                Channel = Connection.CreateModel();

                Channel.ExchangeDeclare("MyExchange", "fanout");

                Console.WriteLine("Publisher Initialized");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
                throw e;
            }

        }


        public void Publish()
        {
            string Message;
            Random r;
            byte[] bMessage;
            int length = 100;
            for (int i = 0; i < length; i++)
            {
                r = new Random();
                Message = "Message - " + r.Next().ToString();
                bMessage = Encoding.UTF8.GetBytes(Message);
                Channel.BasicPublish("MyExchange", "", null, bMessage);
                Console.WriteLine("Publishing Message -- " + Message);
                System.Threading.Thread.Sleep(800);

            }

            Console.WriteLine("Publisher published");
        }


        public void Shutdown()
        {
            Console.ReadLine();

            Channel.Close();
            Channel.Dispose();

            Connection.Close();
            Connection.Dispose();
            Console.WriteLine("Publisher Shutdown");
            Console.ReadLine();
        }

    }
}
