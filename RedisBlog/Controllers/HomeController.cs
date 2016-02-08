using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RedisBlog.Models;
using ServiceStack.Redis.Generic;
using Newtonsoft.Json;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;

namespace RedisBlog.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddProduct()
        {
            return View();
        }
        public void SaveProduct(Product product)
        {
            var conf = new RedisEndpoint() { Host = "xxxxxxxxxxxxxx.redis.cache.windows.net", Password = "yyyyyyyyyyyyyyyyyy", Ssl = true, Port = 6380 };
            using (IRedisClient client = new RedisClient(conf))
            {
                var userClient = client.As<Product>();
                product.Id = userClient.GetNextSequence();
                userClient.Store(product);
            }
        }
        public void SaveItem(Item item)
        {
            var conf = new RedisEndpoint() { Host = "xxxxxxxxxxxxxx.redis.cache.windows.net", Password = "yyyyyyyyyyyyyyyyyy", Ssl = true, Port = 6380 };
            using (IRedisClient client = new RedisClient(conf))
            {
                var itemClient = client.As<Item>();
                var itemList = itemClient.Lists["urn:item:" + item.ProductID];
                item.Id = itemClient.GetNextSequence();
                itemList.Add(item);

                client.AddItemToSortedSet("urn:Rank", item.Name, item.Price);

                //Publis top 5 Ranked Items
                IDictionary<string, double> Data = client.GetRangeWithScoresFromSortedSet("urn:Rank", 0, 4);
                List<Item> RankList = new List<Item>();
                int counter = 0;
                foreach (var itm in Data)
                {
                    counter++;
                    RankList.Add(new Item() { Name = itm.Key, Price = (int)itm.Value, Id = counter });
                }

                var itemJson = JsonConvert.SerializeObject(RankList);
                client.PublishMessage("Rank", itemJson);
                //---------------------------------------------
            }
        }
        public void UpdateItem(Item item)
        {
            var conf = new RedisEndpoint() { Host = "xxxxxxxxxxxxxx.redis.cache.windows.net", Password = "yyyyyyyyyyyyyyyyyy", Ssl = true, Port = 6380 };
            using (IRedisClient client = new RedisClient(conf))
            {
                IRedisTypedClient<Item> itemClient = client.As<Item>();
                IRedisList<Item> itemList = itemClient.Lists["urn:item:" + item.ProductID];

                var index = itemList.Select((Value, Index) => new { Value, Index })
                 .Single(p => p.Value.Id == item.Id).Index;

                var toUpdateItem = itemList.First(x => x.Id == item.Id);

                //var index = itemList.IndexOf(toUpdateItem);

                toUpdateItem.Name = item.Name;
                toUpdateItem.Price = item.Price;

                itemList.RemoveAt(index);
                if (itemList.Count - 1 < index)
                    itemList.Add(toUpdateItem);
                else itemList.Insert(index, toUpdateItem);

                client.RemoveItemFromSortedSet("urn:Rank", item.Name);
                client.AddItemToSortedSet("urn:Rank", item.Name, item.Price);

                //Publis top 5 Ranked Items
                IDictionary<string, double> Data = client.GetRangeWithScoresFromSortedSet("urn:Rank", 0, 4);
                List<Item> RankList = new List<Item>();
                int counter = 0;
                foreach (var itm in Data)
                {
                    counter++;
                    RankList.Add(new Item() { Name = itm.Key, Price = (int)itm.Value, Id = counter });
                }

                var itemJson = JsonConvert.SerializeObject(RankList);
                client.PublishMessage("Rank", itemJson);
                //---------------------------------------------
            }
        }
        public JsonResult GetProducts()
        {
            var conf = new RedisEndpoint() { Host = "xxxxxxxxxxxxxx.redis.cache.windows.net", Password = "yyyyyyyyyyyyyyyyyy", Ssl = true, Port = 6380 };
            using (IRedisClient client = new RedisClient(conf))
            {
                var productClient = client.As<Product>();
                var products = productClient.GetAll();
                return Json(products, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetRanks()
        {
            var conf = new RedisEndpoint() { Host = "xxxxxxxxxxxxxx.redis.cache.windows.net", Password = "yyyyyyyyyyyyyyyyyy", Ssl = true, Port = 6380 };
            using (IRedisClient client = new RedisClient(conf))
            {
                IDictionary<string, double> Data = client.GetRangeWithScoresFromSortedSet("urn:Rank",0,4);                
                List<Item> RankList = new List<Item>();
                int counter = 0;
                foreach(var item in Data)
                {
                    counter++;
                    RankList.Add(new Item() { Name = item.Key, Price = (int)item.Value, Id=counter });
                }                
                return Json(RankList, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetItems(int? productID)
        {
            var conf = new RedisEndpoint() { Host = "xxxxxxxxxxxxxx.redis.cache.windows.net", Password = "yyyyyyyyyyyyyyyyyy", Ssl = true, Port = 6380 };
            using (IRedisClient client = new RedisClient(conf))
            {
                var itemClient = client.As<Item>();
                var itemList = itemClient.Lists["urn:item:" + productID];
                var items = itemList.GetAll();
                return Json(items, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AddItem(int? selectedProduct)
        {
            ViewBag.ProductID = selectedProduct;
            return View();
        }
        public ActionResult EditItem(int ProductID, int Id)
        {
            ViewBag.ProductID = ProductID;
            ViewBag.Id = Id;

            return View();
        }
        public JsonResult GetEditItem(int ProductID, int Id)
        {
            var conf = new RedisEndpoint() { Host = "xxxxxxxxxxxxxx.redis.cache.windows.net", Password = "yyyyyyyyyyyyyyyyyy", Ssl = true, Port = 6380 };
            using (IRedisClient client = new RedisClient(conf))
            {
                var itemClient = client.As<Item>();
                var itemList = itemClient.Lists["urn:item:" + ProductID];
                var toUpdateItem = itemList.First(x => x.Id == Id);
                return Json(toUpdateItem, JsonRequestBehavior.AllowGet);
            }
        }        
    }
    public class Rank : Hub
    {
        public async Task SendRank(List<Item> rankList)
        {
            await Clients.All.getRank(rankList);
        }
    }
}