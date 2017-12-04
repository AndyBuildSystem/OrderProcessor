using System;
using System.Collections.Generic;
using OrderLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OrderUnitTest
{
    [TestClass]
    public class OrderTest
    {
        [TestMethod]
        public void IsValidOrder()
        {
            //missing order data
            Order order = new Order
            {
                Name = "firstname"
            };
            Assert.AreEqual(null, order.ToCsv(), "Invalid order 1 processed");
            //missing order data
            Order order2 = new Order
            {
                Name = "firstname",
                Surname = "suranme"
            };
            Assert.AreEqual(null, order2.ToCsv(), "Invalid order 2 processed");
            //missing order shipment data
            Order order3 = new Order
            {
                OrderReference = "12345",
                Name = "firstname",
                Surname = "suranme",
                OrderShipment = new OrderShipment()
            };
            Assert.AreEqual(null, order3.ToCsv(), "Invalid order 3 processed");
            //missing order shipment data
            Order order4 = new Order
            {
                OrderReference = "12345",
                Marketplace = "Market",
                Name = "firstname",
                Surname = "suranme",
                OrderShipment = new OrderShipment() { Marketplace = "Market"}
            };
            Assert.AreEqual(null, order4.ToCsv(), "Invalid order 4 processed");
            //missing order shipment data
            Order order5 = new Order
            {
                OrderReference = "12345",
                Marketplace = "Market",
                Name = "firstname",
                Surname = "suranme",
                OrderShipment = new OrderShipment() { Marketplace = "Market", OrderReference = "12345"}
            };
            Assert.AreEqual(null, order5.ToCsv(), "Invalid order 5 processed");
            //missing order shipment data
            Order order6 = new Order
            {
                OrderReference = "12345",
                Marketplace = "Market",
                Name = "firstname",
                Surname = "suranme",
                OrderShipment = new OrderShipment() { Marketplace = "Market", OrderReference = "12345", PostalService = "Mail"}
            };
            Assert.AreEqual(null, order6.ToCsv(), "Invalid order 6 processed");
            //missing order item data
            Order order7 = new Order
            {
                OrderReference = "12345",
                Marketplace = "Market",
                Name = "firstname",
                Surname = "suranme",
                OrderShipment = new OrderShipment() { Marketplace = "Market", OrderReference = "12345", PostalService = "Mail", Postcode = "RG99ZGT"}
            };
            Assert.AreEqual(null, order7.ToCsv(), "Invalid order 7 processed");
            //missing order item data
            Order order8 = new Order
            {
                OrderReference = "12345",
                Marketplace = "Market",
                Name = "firstname",
                Surname = "suranme",
                OrderShipment = new OrderShipment() { Marketplace = "Market", OrderReference = "12345", PostalService = "Mail", Postcode = "RG99ZGT" },
                OrderItems = new List<OrderItem>() { new OrderItem() { Marketplace = "Market" } }
                
            };
            Assert.AreEqual(null, order8.ToCsv(), "Invalid order 8 processed");
            //missing order item data
            Order order9 = new Order
            {
                OrderReference = "12345",
                Marketplace = "Market",
                Name = "firstname",
                Surname = "suranme",
                OrderShipment = new OrderShipment() { Marketplace = "Market", OrderReference = "12345", PostalService = "Mail", Postcode = "RG99ZGT" },
                OrderItems = new List<OrderItem>() { new OrderItem() { Marketplace = "Market", OrderReference = "12345" } }

            };
            Assert.AreEqual(null, order9.ToCsv(), "Invalid order 9 processed");
            //missing order item data
            Order order10 = new Order
            {
                OrderReference = "12345",
                Marketplace = "Market",
                Name = "firstname",
                Surname = "suranme",
                OrderShipment = new OrderShipment() { Marketplace = "Market", OrderReference = "12345", PostalService = "Mail", Postcode = "RG99ZGT" },
                OrderItems = new List<OrderItem>() { new OrderItem() { Marketplace = "Market", OrderReference = "12345", OrderItemNumber = "ABC"} }

            };
            Assert.AreEqual(null, order10.ToCsv(), "Invalid order 10 processed");
            //missing order item data
            Order order11 = new Order
            {
                OrderReference = "12345",
                Marketplace = "Market",
                Name = "firstname",
                Surname = "suranme",
                OrderShipment = new OrderShipment() { Marketplace = "Market", OrderReference = "12345", PostalService = "Mail", Postcode = "RG99ZGT" },
                OrderItems = new List<OrderItem>() { new OrderItem() { Marketplace = "Market", OrderReference = "12345", OrderItemNumber = "ABC", PricePerUnit = 10.00} }

            };
            Assert.AreEqual(null, order11.ToCsv(), "Invalid order 11 processed");
            //missing order item data
            Order order12 = new Order
            {
                OrderReference = "12345",
                Marketplace = "Market",
                Name = "firstname",
                Surname = "suranme",
                OrderShipment = new OrderShipment() { Marketplace = "Market", OrderReference = "12345", PostalService = "Mail", Postcode = "RG99ZGT" },
                OrderItems = new List<OrderItem>() { new OrderItem() { Marketplace = "Market", OrderReference = "12345", OrderItemNumber = "ABC", PricePerUnit = 10.00 , Quantity = 1, } }

            };
            Assert.AreEqual(null, order12.ToCsv(), "Invalid order 12 processed");
            // Order is invalid due to missmatch reference
            Order order13 = new Order
            {
                OrderReference = "1234",
                Marketplace = "Market",
                Name = "firstname",
                Surname = "suranme",
                OrderShipment = new OrderShipment() { Marketplace = "Market", OrderReference = "12345", PostalService = "Mail", Postcode = "RG99ZGT" },
                OrderItems = new List<OrderItem>() { new OrderItem() { Marketplace = "Market", OrderReference = "12345", OrderItemNumber = "ABC", PricePerUnit = 10.00, Quantity = 1, Sku = "test"} }

            };
            Assert.AreEqual(null, order13.ToCsv(), "Invalid order 13 processed");
            //Invalid Order Item
            Order order14 = new Order
            {
                OrderReference = "12345",
                Marketplace = "Market",
                Name = "firstname",
                Surname = "suranme",
                OrderShipment = new OrderShipment() { Marketplace = "Market", OrderReference = "12345", PostalService = "Mail", Postcode = "RG99ZGT" },
                OrderItems = new List<OrderItem>() { new OrderItem() { Marketplace = "Market", OrderReference = "12345", OrderItemNumber = "ABC", PricePerUnit = 10.00, Quantity = 1, Sku = "test" }, new OrderItem() { Marketplace = "Market", OrderReference = "12345", OrderItemNumber = "ABC", PricePerUnit = 10.00, Quantity = 1, Sku = ""} }

            };
            Assert.AreEqual(null, order14.ToCsv(), "Invalid order 14 processed");
            //Valid Order
            Order order15 = new Order
            {
                OrderReference = "12345",
                Marketplace = "Market",
                Name = "firstname",
                Surname = "suranme",
                OrderShipment = new OrderShipment() { Marketplace = "Market", OrderReference = "12345", PostalService = "Mail", Postcode = "RG99ZGT" },
                OrderItems = new List<OrderItem>() { new OrderItem() { Marketplace = "Market", OrderReference = "12345", OrderItemNumber = "ABC", PricePerUnit = 10.00, Quantity = 1, Sku = "test" } }

            };
            Assert.AreEqual("Order Reference, Marketplace, Name, Surname, Order Item Number, SKU, Price Per Unit Quantity, Postal Service, Post Code" + Environment.NewLine
                            + "12345, Market, firstname, suranme, ABC, test, 10, Mail, RG999ZGT", order15.ToCsv(), "Valid order 15 not processed");
        }
    }
}
