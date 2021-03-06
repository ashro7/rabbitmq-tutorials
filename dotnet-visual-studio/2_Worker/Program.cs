﻿using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;

class Program
{
    public static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using( var connection = factory.CreateConnection() )
        using( var channel = connection.CreateModel() )
        {
            channel.QueueDeclare( queue: "task_queue", durable: true, exclusive: false, autoDelete: false, arguments: null );

            channel.BasicQos( prefetchSize: 0, prefetchCount: 1, global: false );
            var consumer = new QueueingBasicConsumer( channel );
            channel.BasicConsume( queue: "task_queue", noAck: false, consumer: consumer );

            Console.WriteLine( " [*] Waiting for messages. To exit press CTRL+C" );
            while( true )
            {
                var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                var body = ea.Body;
                var message = Encoding.UTF8.GetString( body );
                Console.WriteLine( " [x] Received {0}", message );

                int dots = message.Split( '.' ).Length - 1;
                Thread.Sleep( dots * 1000 );

                Console.WriteLine( " [x] Done" );

                channel.BasicAck( deliveryTag: ea.DeliveryTag, multiple: false );
            }
        }
    }
}
