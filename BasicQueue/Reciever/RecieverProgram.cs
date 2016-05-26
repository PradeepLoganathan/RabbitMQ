using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Reciever
{
    class RecieverProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Started Reciever....");
            Consumer C = new Consumer();
            C.Initialize();
            C.Consume();
            Console.ReadLine();
            C.ShutDown();
        }
    }

    class Consumer
    {

        private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;

        public Consumer()
        {

        }

        public bool Initialize()
        {
            try
            {
                factory = new ConnectionFactory();
                factory.HostName = "localhost";
                

                connection = factory.CreateConnection();
                channel = connection.CreateModel();
                channel.QueueDeclare("MyQ", false, false, false, null);
            }
            catch
            {
                return false;
            }

            return true;

        }

        public bool Consume()
        {
            try
            {
                EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                consumer.Received += Consumer_Received;
                channel.BasicConsume("MyQ", true, consumer);

            }
            catch
            {
                return false;
            }

            return true;
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            byte[]body = e.Body;
            string Message = Encoding.UTF8.GetString(body);
            Console.WriteLine("Recieved Message - " + Message);
        }

        internal void ShutDown()
        {
            if (connection.IsOpen)
                connection.Close();

            if (channel.IsOpen)
                channel.Close();

            connection.Dispose();
            channel.Dispose();

            channel = null;
            connection = null;
        }
    }
}
