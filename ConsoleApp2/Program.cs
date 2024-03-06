using System;
using NATS.Client;

class PublisherProgram
{
    static void Main(string[] args)
    {
        var url = "nats://localhost:4222";
        var subject = "test.subject";

        // Create a connection factory
        var connectionFactory = new ConnectionFactory();

        // Create a connection to the NATS server
        using (var connection = connectionFactory.CreateConnection(url))
        {
            // Function to publish a message
            void PublishMessage(string message)
            {
                connection.Publish(subject, System.Text.Encoding.UTF8.GetBytes(message));
                Console.WriteLine($"Message published to '{subject}': {message}");
            }

            // Example messages
            PublishMessage("Hello, NATS! This is message 1.");
            PublishMessage("Hello, NATS! This is message 2.");

            // Keep the console window open
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
