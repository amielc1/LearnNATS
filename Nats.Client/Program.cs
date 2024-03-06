using NATS.Client;
using System;
using System.Text;
using System.Threading;

namespace NatsTest1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //conexão
            var cf = new ConnectionFactory();
            var c = cf.CreateConnection();

            //Subscrição
            EventHandler<MsgHandlerEventArgs> h = (sender, args) =>
            {
                Console.WriteLine($"{DateTime.Now:F} - Received: {args.Message}");
            };

            var sAsync = c.SubscribeAsync("foo");

            sAsync.MessageHandler += h;
            sAsync.Start();

            // Informação / mensagem / evento
            var message = "Hello World!";
            Console.WriteLine($"{DateTime.Now:F} - Send: {message}");

            //Publicação
            c.Publish("foo", Encoding.UTF8.GetBytes(message));

            Thread.Sleep(1000);

            // Desconectando
            sAsync.Unsubscribe();
            c.Drain();
            c.Close();


            Console.ReadLine();
        }
    }
}