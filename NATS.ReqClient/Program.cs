using Common;
using System.Text;
using System.Text.Json;
using NATS.Client;

var opts = ConnectionFactory.GetDefaultOptions();
opts.Url = "nats://localhost:4222"; // Adjust as necessary
string topic = "missionRequest";
var conn = new ConnectionFactory().CreateConnection(opts);

var request = new MissionRequest
{
    MissionName = "Retrieve Data" // Example request, adjust as necessary
};

var requestJson = JsonSerializer.Serialize(request);
var responseMsg = await conn.RequestAsync(topic, Encoding.UTF8.GetBytes(requestJson), 10000); // 5000ms timeout

var response = JsonSerializer.Deserialize<MissionParameters>(Encoding.UTF8.GetString(responseMsg.Data));
Console.WriteLine($"Response received: MissionName = {response.MissionName}, MissionStatus = {response.MissionStatus}");


Console.ReadLine();

