using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ServiceStack.Redis;
using StackExchange.Redis;
using System;
using System.Text;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };
           

            using (IRedisClient client = new RedisClient())
            {
                var accountNames = client.Lists["urn:accountnames"];

                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: "hello", durable: true, exclusive: false, autoDelete: false, arguments: null);

                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);

                            //AccountManager manager = new AccountManager();
                            //manager.CreateAccount(new Account
                            //{
                            //    Name = message,
                            //});

                            //ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");
                            //IDatabase db = redis.GetDatabase();
                            //db.StringSet($"{message} key", message);

                            if (!accountNames.Contains(message))
                            {
                                accountNames.Add(message);
                                Console.WriteLine($"{message} kaydedildi");
                            }
                            else
                            {
                                Console.WriteLine($"{message} kaydı zaten var");
                            }


                        };

                        channel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);
                        Console.WriteLine("Çıkmak için [Enter]");
                        Console.ReadLine();
                    }
                }
            }



        }
    }
}
