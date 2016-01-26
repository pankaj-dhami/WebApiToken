using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiTokenAuth.Model;

namespace WebApiTokenAuth.Controller{
    [RoutePrefix("api/Orders")]
    public class OrdersController : ApiController
    {
        [Authorize]
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(Order.CreateOrders());
        }

        [AllowAnonymous]
        [ActionName("GetOrders")]
        public IHttpActionResult Get_Orders()
        {
            return Ok(Order.CreateOrders());
        }

        [Authorize]
        [Route("Createnew"),HttpPost]
        public IHttpActionResult Post(Order order)
        {
            if (ModelState.IsValid)
            {
               Order.OrderList.Add(order);
                return Ok();
            }
            else
            {
                return BadRequest(ModelState);
              HttpResponseMessage
            }
           
        }

    }   
}
