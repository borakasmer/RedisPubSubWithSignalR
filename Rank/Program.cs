using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rank
{
    class Program
    {
        static void Main(string[] args)
        {
            var conf = new RedisEndpoint() { Host = "xxxxxxxxxxxxxx.redis.cache.windows.net", Password = "yyyyyyyyyyyyyyyyyy", Ssl = true, Port = 6380 };
            using (IRedisClient client = new RedisClient(conf))
            {
                IRedisSubscription sub = null;
                using (sub = client.CreateSubscription())
                {
                    sub.OnMessage += (channel, message) =>
                    {
                        try
                        {
                            List<Item> items = JsonConvert.DeserializeObject<List<Item>>(message);
                            Console.WriteLine((string)message);
                            SignalRClass sc = new SignalRClass();
                            sc.SendRank(items);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    };
                }
                sub.SubscribeToChannels(new string[] { "Rank" });
            }

            Console.ReadLine();
        }
    }

    public class SignalRClass
    {
        public async Task SendRank(List<Item> rankList)
        {
            HubConnection hubConnection = new HubConnection("http://localhost:34511/");
            IHubProxy hubProxy = hubConnection.CreateHubProxy("Rank");
            await hubConnection.Start(new LongPollingTransport());
            hubProxy.Invoke("SendRank", rankList);
        }
    }
    public class Item
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }

        public int ProductID { get; set; }
    }
}
