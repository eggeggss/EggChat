using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public enum PizzaType
    {
        夏威夷 = 0, 哈辣墨西哥 = 1
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            // Data();

            //Int32 test = (Int32)PizzaType.夏威夷;

            // Console.WriteLine(test);

            String cost = "2.55";


            double dcost = Convert.ToDouble(cost);
            
            Console.WriteLine(Math.Round(dcost,0,MidpointRounding.AwayFromZero));

            Console.ReadKey();
        }

        private static void Data()
        {
            var chatConnection = new HubConnection("http://eggeggss.ddns.net/chat/");
            IHubProxy stockTickerHubProxy = chatConnection.CreateHubProxy("Chathub");
            //client
            /*
            stockTickerHubProxy.On("hello", msg =>
            Console.WriteLine(msg)
            );
            */
            chatConnection.DeadlockErrorTimeout = new TimeSpan(0, 0, 3);

            stockTickerHubProxy.On<String>("hello", (e) =>
            {
                Console.WriteLine(e);
            });

            //stockTickerHubProxy.On<Stock>("UpdateStockPrice", stock => Console.WriteLine("Stock update for {0} new price {1}", stock.Symbol, stock.Price));

            //await hubConnection.Start();
            chatConnection.Start().ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    Console.WriteLine("Connection is ok");
                    //server
                    stockTickerHubProxy.Invoke("Hello", "Roger");
                }
                //連線成功時呼叫Server端方法register()
                //commHub.Invoke("register", clientName);
                else
                    Console.WriteLine("Fail");
                //done = true;
            });

            //chatConnection.Stop();
            //Console.WriteLine("wait");
        }
    }
}