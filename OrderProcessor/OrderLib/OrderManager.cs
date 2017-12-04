using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OrderLib
{
    //Dictionary definiton to link OrderReference and Marketplace into a Tuple Key
    public class Dictionary<TKey1, TKey2, TValue> : Dictionary<Tuple<TKey1, TKey2>, TValue>, IDictionary<Tuple<TKey1, TKey2>, TValue>
    {

        public TValue this[TKey1 key1, TKey2 key2]
        {
            get { return base[Tuple.Create(key1, key2)]; }
            set { base[Tuple.Create(key1, key2)] = value; }
        }

        public void Add(TKey1 key1, TKey2 key2, TValue value)
        {
            base.Add(Tuple.Create(key1, key2), value);
        }

        public bool ContainsKey(TKey1 key1, TKey2 key2)
        {
            return base.ContainsKey(Tuple.Create(key1, key2));
        }
    }

    public static class OrderManager
    {
        public static Dictionary<string,string,Order> Orders = new Dictionary<string, string, Order>();

        public static bool WriteOrdersToCsv(string path)
        {
            //convert orders into desired format and write to disk
            foreach (var order in Orders.Values)
            {
                try
                {
                    string orderCsv = order.ToCsv(); 
                    if (!string.IsNullOrWhiteSpace(orderCsv))
                    {
                        File.WriteAllText(Path.Combine(path, $"{order.OrderReference}-{order.Marketplace}.csv"),
                            orderCsv); 
                        Console.WriteLine("Writing CSV: " + Path.Combine(path, $"{order.OrderReference}-{order.Marketplace}.csv"));
                    }
                }
                catch (IOException e)
                {
                    Console.Error.WriteLine($"Failed to write CSV for order: {order.OrderReference}-{order.Marketplace}");
                    Console.Error.WriteLine(e);
                }
            }
            return false;
        }
    }



    
}
