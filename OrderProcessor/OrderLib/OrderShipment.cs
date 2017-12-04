using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OrderLib
{
    public class OrderShipmentsRoot
    {
        [JsonProperty("orders")]
        public List<OrderShipment> OrderShipments { get; set; }
    }

    public class OrderShipment
    {
        [JsonProperty("marketplace")]
        public string Marketplace { get; set; }

        [JsonProperty("order reference")]
        public string OrderReference { get; set; }

        [JsonProperty("postal service")]
        public string PostalService { get; set; }

        [JsonProperty("postcode")]
        public string Postcode { get; set; }
    }
}
