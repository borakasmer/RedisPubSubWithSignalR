using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedisBlog.Models
{
    public class Product
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Detail { get; set; }
    }
    public class Item
    {
        public long Id { get; set; }        
        public string Name { get; set; }
        public int Price { get; set; }

        public int ProductID { get; set; }
    }
}