using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApiTokenAuth.Model
{
    public class Order
    {
         static Order()
        {
            OrderList = new List<Order>
            {
                new Order {OrderID = 10248, CustomerName = "Taiseer Joudeh", ShipperCity = "Amman", IsShipped = true },
                new Order {OrderID = 10249, CustomerName = "Ahmad Hasan", ShipperCity = "Dubai", IsShipped = false},
                new Order {OrderID = 10250,CustomerName = "Tamer Yaser", ShipperCity = "Jeddah", IsShipped = false },
                new Order {OrderID = 10251,CustomerName = "Lina Majed", ShipperCity = "Abu Dhabi", IsShipped = false},
                new Order {OrderID = 10252,CustomerName = "Yasmeen Rami", ShipperCity = "Kuwait", IsShipped = true}
            };
        }

        public int OrderID { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string ShipperCity { get; set; }
        public Boolean IsShipped { get; set; }

        public static List<Order> OrderList = new List<Order>();
        public static List<Order> CreateOrders()
        {
            return OrderList;
        }
    }
}