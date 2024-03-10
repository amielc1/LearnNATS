using Common;
using NATS.Client;
using System.Text;
using System.Text.Json;

var opts = ConnectionFactory.GetDefaultOptions();
opts.Url = "nats://localhost:4222"; // Adjust as necessary

string topic = "missionRequest";
using (var conn = new ConnectionFactory().CreateConnection(opts))
{
    var subscription = conn.SubscribeAsync(topic, (sender, args) =>
    {
        var receivedRequest = JsonSerializer.Deserialize<MissionRequest>(Encoding.UTF8.GetString(args.Message.Data));
        Console.WriteLine($"Received mission request: {receivedRequest.MissionName}");

        var response = new MissionParameters
        {
            MissionName = receivedRequest.MissionName,
            MissionStatus = "Completed" // Example status, adjust logic as needed
        };

        var responseJson = JsonSerializer.Serialize(response);
        
        conn.Publish(args.Message.Reply, Encoding.UTF8.GetBytes(responseJson));
 
    });
    Console.WriteLine("Server is running. Press Enter to exit.");
    Console.ReadLine();
}

