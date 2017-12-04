using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace OrderLib
{

    public static class JsonProcessor
    {
        public static bool ProcessJsonOrderFile(string path)
        {
            try
            {
                string jsonString = "";
                jsonString = File.ReadAllText(path);
                return ProcessJsonStringOrderDetails(jsonString);
            }
            catch (IOException)
            {
                Console.WriteLine($"File in use will try to process later: {path}");
                //file in use
                return false;
            }
        }
        // try to process the file into our 3 types
        public static bool ProcessJsonStringOrderDetails(string jsonString)
        {
            object orderDetails = null;
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                MissingMemberHandling = MissingMemberHandling.Error
            };
            //Try to see if its a list of Shipment information
            try
            {
                orderDetails = JsonConvert.DeserializeObject<OrderShipmentsRoot>(jsonString, settings);
            }
            catch
            {
                orderDetails = null;
            }
            if (orderDetails != null)
            {
                foreach (var orderShipment in ((OrderShipmentsRoot)orderDetails).OrderShipments)
                {
                    orderShipment.Postcode = orderShipment.Postcode.Replace(" ", "");//Fix up postcode dodgy spaces
                    if (OrderManager.Orders.TryGetValue(new Tuple<string, string>(orderShipment.OrderReference, orderShipment.Marketplace), out var existingorder))
                    {
                        existingorder.OrderShipment = orderShipment;
                    }
                    else
                    {
                        var order = new Order
                        {
                            OrderReference = orderShipment.OrderReference,
                            Marketplace = orderShipment.Marketplace,
                            OrderShipment = orderShipment
                        };
                        OrderManager.Orders.Add(order.OrderReference, order.Marketplace, order);
                    }
                    return true;
                } 
            }
            //try to see if its a list of Order Items
            try
            {
                orderDetails = JsonConvert.DeserializeObject<OrderItemsRoot>(jsonString, settings);
            }
            catch
            {
                orderDetails = null;
            }
            if (orderDetails != null)
            {
                foreach (var orderItem in ((OrderItemsRoot)orderDetails).OrderItems)
                {
                    if (OrderManager.Orders.TryGetValue(new Tuple<string, string>(orderItem.OrderReference, orderItem.Marketplace), out var existingorder))
                    {
                        existingorder.OrderItems.Add(orderItem); //should probably validate this further if there is a chance of duplicates existing likely by seeing if an order item number exists in a linked order
                    }
                    else
                    {
                        var order = new Order
                        {
                            OrderReference = orderItem.OrderReference,
                            Marketplace = orderItem.Marketplace,
                        };
                        order.OrderItems.Add(orderItem);
                        OrderManager.Orders.Add(order.OrderReference, order.Marketplace, order);
                    }
                }
                return true;
            }
            // Try to see if its a list of Orders
            try
            {
                orderDetails = JsonConvert.DeserializeObject<OrdersRoot>(jsonString, settings);
            }
            catch
            {
                orderDetails = null;
            }
            if (orderDetails != null)
            {
                foreach (var order in ((OrdersRoot)orderDetails).Orders)
                {
                    if (OrderManager.Orders.TryGetValue(
                        new Tuple<string, string>(order.OrderReference, order.Marketplace), out var existingorder))
                    {
                        //update order details
                        existingorder.Name = order.Name;
                        existingorder.Surname = order.Surname;
                    }
                    else
                    {
                        OrderManager.Orders.Add(order.OrderReference, order.Marketplace, order);
                    }
                    return true;
                }
            }
            // could't process the file it must be invalid
            Console.WriteLine($"Warning - Couldn't process json string: {jsonString}");
            return false;
        }
    }

}
