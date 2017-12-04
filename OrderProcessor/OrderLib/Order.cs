using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace OrderLib
{
    public class OrdersRoot
    {
        [JsonProperty("orders")]
        public List<Order> Orders { get; set; }
    }

    public class Order
    {
        [JsonProperty("marketplace")]
        public string Marketplace { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("order reference")]
        public string OrderReference { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }

        public OrderShipment OrderShipment { get; set; }
        public List<OrderItem> OrderItems { get; set; }

        public Order()
        {
            OrderItems = new List<OrderItem>();
        }

        public string ToCsv(string separator = ",", bool header = true)
        {
            if (IsValid())
            {
                string csvString = "";

                if(header)
                    csvString = $"Order Reference{separator} " +
                                $"Marketplace{separator} " +
                                $"Name{separator} " +
                                $"Surname{separator} " +
                                $"Order Item Number{separator} " +
                                $"SKU{separator} " +
                                $"Price Per Unit Quantity{separator} " +
                                $"Postal Service{separator} " +
                                $"Post Code";
                
                foreach (var orderItem in OrderItems)
                {
                    csvString += Environment.NewLine + 
                        $"{EscapeCSV(OrderReference,separator)}{separator} " +
                        $"{EscapeCSV(Marketplace,separator)}{separator} " +
                        $"{EscapeCSV(Name,separator)}{separator} " +
                        $"{EscapeCSV(Surname,separator)}{separator} " +
                        $"{EscapeCSV(orderItem.OrderItemNumber,separator)}{separator} " +
                        $"{EscapeCSV(orderItem.Sku,separator)}{separator} " +
                        $"{EscapeCSV(orderItem.PricePerUnit.ToString(),separator)}{separator} " +
                        $"{EscapeCSV(OrderShipment.PostalService,separator)}{separator} " +
                        $"{EscapeCSV(OrderShipment.Postcode, separator)}";
                }

                return csvString;
            }
            else
            {
                return null;
            }
        }

        private string EscapeCSV(string value, string seperator)
        {
            if (value.Contains("\"")) // Escape Quotes
                value = value.Replace("\"", "\"\"");
            if (value.Contains(seperator)) // Escape Seperators
                value = "\"" + value + "\"";
            return value;
        }
        private static readonly Regex[] _uk_postcode = new Regex[] {
            new Regex("(^[A-PR-UWYZa-pr-uwyz][0-9][ ]*[0-9][ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2}$)", RegexOptions.Compiled),
            new Regex("(^[A-PR-UWYZa-pr-uwyz][0-9][0-9][ ]*[0-9][ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2}$)", RegexOptions.Compiled),
            new Regex("(^[A-PR-UWYZa-pr-uwyz][A-HK-Ya-hk-y][0-9][ ]*[0-9][ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2}$)", RegexOptions.Compiled),
            new Regex("(^[A-PR-UWYZa-pr-uwyz][A-HK-Ya-hk-y][0-9][0-9][ ]*[0-9][ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2}$)", RegexOptions.Compiled),
            new Regex("(^[A-PR-UWYZa-pr-uwyz][0-9][A-HJKS-UWa-hjks-uw][ ]*[0-9][ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2}$)", RegexOptions.Compiled),
            new Regex("(^[A-PR-UWYZa-pr-uwyz][A-HK-Ya-hk-y][0-9][A-Za-z][ ]*[0-9][ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2}$)", RegexOptions.Compiled),
            new Regex("(^[Gg][Ii][Rr][]*0[Aa][Aa])") };

        public bool ValidatePostCode(string text)
        {
            return (_uk_postcode[0].IsMatch(text) ||
                    _uk_postcode[1].IsMatch(text) ||
                    _uk_postcode[2].IsMatch(text) ||
                    _uk_postcode[3].IsMatch(text) ||
                    _uk_postcode[4].IsMatch(text) ||
                    _uk_postcode[5].IsMatch(text) ||
                    _uk_postcode[6].IsMatch(text));
        }
        private bool IsValid()
        {
            if (!string.IsNullOrWhiteSpace(OrderReference) && !string.IsNullOrWhiteSpace(Marketplace) &&
                !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Surname) && OrderShipment != null &&
                OrderItems.Count > 0)
            {
                if (OrderReference == OrderShipment.OrderReference && Marketplace == OrderShipment.Marketplace)
                {
                    //shipment linked to wrong order - some ones manually changing references!
                    if (!string.IsNullOrWhiteSpace(OrderShipment.PostalService) && !string.IsNullOrWhiteSpace(OrderShipment.Postcode))
                    {
                        if (!ValidatePostCode(OrderShipment.Postcode)) //Is this a uk postcode only database if not this check isn't required
                        {
                            //invalid postcode
                            return false; 
                        }
                        foreach (var item in OrderItems)
                        {
                            if (item.OrderReference != OrderReference || item.Marketplace != Marketplace)
                            {
                                //item linked to wrong order - some ones manually changing references!
                                return false;
                            }
                            if (string.IsNullOrWhiteSpace(item.OrderItemNumber) ||
                                string.IsNullOrWhiteSpace(item.Sku) ||
                                item.PricePerUnit == null || item.Quantity == null)
                            {
                                //Invalid data in Item.
                                return false;
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }
        
    }
}
