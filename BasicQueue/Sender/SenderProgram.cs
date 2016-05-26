using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Sender
{
    class SenderProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Started Sender....");
            
            Producer producer = new Producer();
            producer.Initialize();
            for(int i=0;i<100;i++)
                producer.SendMessage();
            producer.ShutDown();
            Console.ReadLine();

        }
    }


    class Producer
    {

        private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;
        private static string QIdentifier;
        private static int Qid;

        public Producer()
        {
            QIdentifier = " Noise Generator - ";
            Qid = DateTime.Now.Second;            
                
        }

        public bool Initialize()
        {
            try
            {
                factory = new ConnectionFactory();
                factory.HostName = "localhost";
                factory.UserName = "guest";
                factory.Password = "guest";

                connection = factory.CreateConnection();
                Console.WriteLine("Created Connection....");
                channel = connection.CreateModel();
                Console.WriteLine("Created Channel....");
                channel.QueueDeclare("MyQ", false, false, false, null);
                Console.WriteLine("Created Queue..." + QIdentifier + Qid.ToString());

            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool SendMessage()
        {
            try
            {
                Random r;
                r = new Random();

                string message = "Message" + r.Next().ToString();
                byte[] body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish("", "MyQ", null, body);
                Console.WriteLine("Published Message - " + message);
                
            }
            catch
            {
                return false;
            }

            return true;
        }

        internal void ShutDown()
        {
            if(connection.IsOpen)
                connection.Close();
            
            if(channel.IsOpen)
                channel.Close();

            connection.Dispose();
            channel.Dispose();

            channel = null;
            connection = null;
        }
    }

}
