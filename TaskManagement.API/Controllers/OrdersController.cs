using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TaskManagement.Controllers
{
    [RoutePrefix("api/Orders")]
    public class OrdersController : ApiController
    {
        [Authorize]
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(Order.CreateOrders());
        }
    }

    #region Helpers

    public class Order
    {
        public int OrderID { get; set; }
        public string CustomerName { get; set; }
        public string Product { get; set; }
        public int Price { get; set; }

        public static List<Order> CreateOrders()
        {
            var orders = new List<Order>();
            orders.Add(new Order { OrderID = 1, CustomerName = "Goce", Product = "Puma560", Price = 500 });
            orders.Add(new Order { OrderID = 2, CustomerName = "Goce", Product = "Beats", Price = 100 });
            orders.Add(new Order { OrderID = 3, CustomerName = "Goce", Product = "IPhone 5", Price = 300 });
            return orders;
        }
    }

    #endregion
}
