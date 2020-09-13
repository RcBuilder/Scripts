using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Install-Package RabbitMQ.Client
using RabbitMQ.Client; 
using RabbitMQ.Client.Events;

namespace TestConsole7
{
    public class RabbitMQ
    {
        class Producer : IDisposable {
            private IConnection connection { set; get; }
            private IModel channel { set; get; }
            private const string QueueName = "testQueue";

            public Producer() {

                // init rabbitMQ
                var factory = new ConnectionFactory {
                    HostName = "localhost"
                };

                this.connection = factory.CreateConnection();
                this.channel = connection.CreateModel();
                channel.QueueDeclare(Producer.QueueName, false, false, false, null);  // create a Queue (if not exists)
            }

            public void Run() {
                Console.WriteLine("Producer has started...");

                var rand = new Random();
                Enumerable.Range(1, 30).ToList().AsParallel().ForAll(x => {
                    channel.BasicPublish("", Producer.QueueName, null, Encoding.UTF8.GetBytes($"Hello World {x}"));  // send message
                    Thread.Sleep(rand.Next(500, 1500));  // random sleep between 0.5 to 1.5 seconds
                });
            }

            public void Dispose()
            {
                this.channel.Dispose();
                this.connection.Dispose();
            }
        }

        class Consumer : IDisposable
        {
            private IConnection connection { set; get; }
            private IModel channel { set; get; }
            private const string QueueName = "testQueue";

            protected string Name { set; get; } = "Anonymous";

            public Consumer(string Name) {
                this.Name = Name;

                // init rabbitMQ
                var factory = new ConnectionFactory {
                    HostName = "localhost"
                };

                this.connection = factory.CreateConnection();
                this.channel = connection.CreateModel();
                channel.QueueDeclare(Consumer.QueueName, false, false, false, null);  // create a Queue (if not exists)
            }

            public void Run() {
                Console.WriteLine("Consumer has started...");
                
                var consumer = new EventingBasicConsumer(channel);  // create a consumer
                consumer.Received += (s, e) => {
                    var bMessage = e.Body.ToArray();
                    Console.WriteLine($"{this.Name}: {Encoding.UTF8.GetString(bMessage)}");
                };

                channel.BasicConsume(Consumer.QueueName, true, consumer);  // read messages 
            }

            public void Dispose()
            {
                this.channel.Dispose();
                this.connection.Dispose();
            }
        }

        public static void Run() {            
            Task.Factory.StartNew(() =>
            {
                new Producer().Run();
            });

            Task.Factory.StartNew(() =>
            {
                new Consumer("Consumer-1").Run();
            });

            Task.Factory.StartNew(() =>
            {
                new Consumer("Consumer-2").Run();
            });
            
            Console.ReadKey();
        }
    }    
}
