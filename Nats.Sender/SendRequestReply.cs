﻿using NATS.Client;
using System.Text;

namespace Nats.Sender;

//https://natsbyexample.com/examples/messaging/request-reply/dotnet

public class SendRequestReply
{ 
    public void Start()
    {


        string natsUrl = Environment.GetEnvironmentVariable("NATS_URL");
        if (natsUrl == null)
        {
            natsUrl = "nats://127.0.0.1:4222";
        }

        // Create a new connection factory to create a connection.
        Options opts = ConnectionFactory.GetDefaultOptions();
        opts.Url = natsUrl;

        // Creates a connection to nats server at the `natsUrl`
        // An `IConnection` is `IDisposable` so it can be used
        // within a `using` statement.
        ConnectionFactory cf = new ConnectionFactory();
        IConnection c = cf.CreateConnection(opts);

        // ### Reply
        // Create a message event handler and then subscribe to the target
        // subject which leverages a wildcard `greet.*`.
        // When a user makes a "request", the client populates
        // the reply-to field and then listens (subscribes) to that
        // as a subject.
        // The replier simply publishes a message to that reply-to.
        EventHandler<MsgHandlerEventArgs> handler = (sender, args) =>
        {
            string name = args.Message.Subject.Substring(6);
            string response = $"hello {name}";
            c.Publish(args.Message.Reply, Encoding.UTF8.GetBytes(response));
        };
        IAsyncSubscription sub = c.SubscribeAsync("greet.*", handler);

        // ### Request
        // Make a request and wait a most 1 second for a response.
        try
        {
            Msg m0 = c.Request("greet.bob", null, 1000); 
            Console.WriteLine("Response received: " + Encoding.UTF8.GetString(m0.Data));
        }
        catch (NATSTimeoutException)
        {
            Console.WriteLine($"NATSTimeoutException: The request did not complete in time.");
        }

        // A request can also be made asynchronously
        Task<Msg> task1 = c.RequestAsync("greet.pam", null);
        task1.Wait(1000);
        Msg m1 = task1.Result;
        Console.WriteLine("Response received: " + Encoding.UTF8.GetString(m1.Data));

        // Once we unsubscribe there will be no subscriptions to reply.
        sub.Unsubscribe();

        // If there are no-responders to a synchronous request
        // we get a `NATSNoRespondersException`.
        try
        {
            c.Request("greet.fred", null, 1000);
        }
        catch (NATSNoRespondersException)
        {
            Console.WriteLine($"NATSNoRespondersException: There were no responders listening for the subject.");
        }
        catch (NATSTimeoutException)
        {
            Console.WriteLine($"NATSTimeoutException: The request did not complete in time.");
        }

        // If there are no-responders to an asynchronous request
        // we get a `NATSNoRespondersException` wrapped inside the `AggregateException`
        try
        {
            Task<Msg> task2 = c.RequestAsync("greet.sue", null);
            task2.Wait(1000);
        }
        catch (AggregateException ae)
        {
            if (ae.InnerExceptions[0] is NATSNoRespondersException)
            {
                Console.WriteLine($"NATSNoRespondersException: There were no responders listening for the subject.");
            }
            else
            {
                Console.WriteLine($"Some exception: " + ae.Message);
            }
        }

    }

     

}
