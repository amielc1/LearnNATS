using System;
using NATS.Client;

class Program
{
    static void Main(string[] args)
    {
        var url = "nats://localhost:4222";

        // Create a connection factory
        var connectionFactory = new ConnectionFactory();

        // Create a synchronous connection to the NATS server
        using (var connection = connectionFactory.CreateConnection(url))
        {
            // Subscribe to a subject
            var subscription = connection.SubscribeAsync("test.subject");
            subscription.MessageHandler += (sender, e) =>
            {
                Console.WriteLine($"Received message: {System.Text.Encoding.UTF8.GetString(e.Message.Data)}");
            };
            subscription.Start();

            // Publish a message to the subject
            string message = "Hello, NATS!";
            connection.Publish("test.subject", System.Text.Encoding.UTF8.GetBytes(message));
            Console.WriteLine("Message published.");

            // Keep the console window open
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
