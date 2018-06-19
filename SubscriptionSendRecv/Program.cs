using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionSendRecv
{
    class Program
    {
        //static string connectionString = "Endpoint=sb://sblab1-ns-3dbkbvgazmng2.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=v8Yx+P2y4RqvBpMKyPfVvE3gz69CAOg7+pE0f9Npojs=";
        static string connectionString = "[REPLACE-WITH-CONNECTION-STRING]";
        static string topicName = "SBLab1Topic";
        static string subscriptionName = "SBLab1Sub";

        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var factory = MessagingFactory.CreateFromConnectionString(connectionString);
            //create a receiver on the topic subscription
            var receiver = await factory.CreateMessageReceiverAsync(topicName + "/Subscriptions/" + subscriptionName);
            //create a sender on the topic
            var sender = await factory.CreateMessageSenderAsync(topicName);

            //start message pump to listen for message on the topic subscription
            receiver.OnMessageAsync( async receivedMessage =>
            {
                //trace out the MessageId property of received message
                Console.WriteLine("Receiving message - {0}", receivedMessage.MessageId);
                await receivedMessage.CompleteAsync();
            }, new OnMessageOptions() { AutoComplete = false });
            
            //send message to the topic
            await sender.SendAsync(new BrokeredMessage("Hello World!") { MessageId = "deadbeef-dead-beef-dead-beef00000075" });

            await Task.WhenAny(
                Task.Run(() => Console.ReadKey()),
                Task.Delay(TimeSpan.FromSeconds(10))
            );
        }
    }
}
