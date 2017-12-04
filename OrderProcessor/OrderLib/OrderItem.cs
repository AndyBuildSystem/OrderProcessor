using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OrderLib
{
    public class OrderItemsRoot
    {
        [JsonProperty("order items")]
        public List<OrderItem> OrderItems { get; set; }
    }

    public class OrderItem
    {
        [JsonProperty("marketplace")]
        public string Marketplace { get; set; }

        [JsonProperty("order item number")]
        public string OrderItemNumber { get; set; }

        [JsonProperty("order reference")]
        public string OrderReference { get; set; }

        [JsonProperty("price per unit")]
        public double? PricePerUnit { get; set; }

        [JsonProperty("quantity")]
        public long? Quantity { get; set; }

        [JsonProperty("sku")]
        public string Sku { get; set; }
    }
}