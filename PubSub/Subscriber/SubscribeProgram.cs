using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Subscriber
{
    class SubscriberProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Started Subscriber...");
            Subscriber s = new Subscriber();
            s.Initialize();
            s.Subscribe();
            s.Shutdown();
        }
    }

    class Subscriber
     {

         private ConnectionFactory Factory;
         private IConnection Connection;
         private IModel Channel;
         private string QueueName;
         private EventingBasicConsumer Consumer;


         public void Initialize()
         {
             try
             {
                 Console.WriteLine("Subscriber Initializing..");
                 Factory = new ConnectionFactory();
                 Factory.HostName = "localhost";
                 Factory.UserName = "guest";
                 Factory.Password = "guest";
                 Connection = Factory.CreateConnection();
                 Channel = Connection.CreateModel();
                 Channel.ExchangeDeclare("MyExchange", "fanout");
                 Consumer = new EventingBasicConsumer(Channel);
                 Consumer.Received += Consumer_Received;
                 Console.WriteLine("Subscriber Intialized");
             }
             catch (Exception)
             {

                 throw;
             }  
         }

         public void Subscribe()
         {
            // BasicDeliverEventArgs result;

             try
             {
                 QueueName = Channel.QueueDeclare().QueueName;
                 Console.WriteLine("Queue Name is " + QueueName);
                 Channel.QueueBind(QueueName, "MyExchange", "");
                 Console.WriteLine("Consumer waiting for Messages...");

                 while (true)
                 {
                     Channel.BasicConsume(QueueName, true, Consumer);
                     Console.WriteLine("Waiting on subscription....");
                     Console.ReadLine();
                     break;
                 }

                 //for (int i = 0; i < 100; i++)
                 //{
                 //    Channel.BasicConsume(QueueName, true, Consumer);
                 //    System.Threading.Thread.Sleep(400);
                 //}

                 Console.WriteLine("Subscriber Subscribed");
             }
             catch (Exception e)
             {
                 Console.WriteLine(e.Message.ToString());
                 throw;
             }
         }

         private void Consumer_Received(object sender, BasicDeliverEventArgs e)
         {
             Console.WriteLine("Subscriber reieved Message");
             Console.WriteLine(Encoding.UTF8.GetString(e.Body));

         }

         public void Shutdown()
         {
             Channel.Close();
             Channel.Dispose();

             Connection.Close();
             Connection.Dispose();

             Console.ReadLine();
         }
     }
    
}
