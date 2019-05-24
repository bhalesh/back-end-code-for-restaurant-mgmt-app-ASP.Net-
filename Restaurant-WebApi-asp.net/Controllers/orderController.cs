using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Restaurant_WebApi_asp.net.Models;

namespace Restaurant_WebApi_asp.net.Controllers
{
    public class orderController : ApiController
    {
        private DBModel db = new DBModel();

        // GET: api/order
        public System.Object Getorders()
        {
            var result = (from a in db.orders
                           join b in db.Customers on a.customerID equals b.customerID

                           select new
                           {
                               a.orderID,
                               a.orderNo,
                               Customer = b.Name,
                               a.pMethod,
                               a.gTotal
                           }).ToList();
                            
                return result;
        }

        // GET: api/order/5
        [ResponseType(typeof(order))]
        public IHttpActionResult Getorder(long id)
        {
            var order =(from a in db.orders
                       where a.orderID == id

                       select new {
                        a.orderID,
                        a.orderNo,
                        a.customerID,
                        a.pMethod,
                        a.gTotal,
                        deletedOrderItemIDs=""
                       }).FirstOrDefault();

            var orderDetails = (from a in db.orderItems
                               join b in db.items on a.itemID equals b.itemID
                               where a.orderID == id

                               select new{
                               a.orderID,
                               a.orderItemID,
                               a.itemID,
                               itemName = b.Name,
                               b.price,
                               a.quantity,
                               total=a.quantity * b.price
                               }).ToList();

            return Ok(new { order, orderDetails});
        }

       

        // POST: api/order
        [ResponseType(typeof(order))]
        public IHttpActionResult Postorder(order order)
        {
            try
            {
                //order.table
                if (order.orderID == 0)
                {
                    db.orders.Add(order);
                }
                else
                {
                    db.Entry(order).State = EntityState.Modified;
                }
                

                //order.items table
                foreach (var item in order.orderItems)
                {
                    if (item.orderItemID == 0)
                    {
                        db.orderItems.Add(item);
                    }
                    else
                    {
                        db.Entry(item).State = EntityState.Modified;
                    }
                    
                        
                }

                //Delete for OrderItems
                foreach(var id in order.deletedOrderItemIDs.Split(',').Where(x => x != ""))
                {
                    orderItem x = db.orderItems.Find(Convert.ToInt64(id));
                    db.orderItems.Remove(x);
                }
                db.SaveChanges();

                return Ok();
                //return CreatedAtRoute("DefaultApi", new { id = order.orderID }, order);

            }
            catch (Exception ex)
            {
                throw ex;
            }

           
        }

        // DELETE: api/order/5
        [ResponseType(typeof(order))]
        public IHttpActionResult Deleteorder(long id)
        {
            order order = db.orders.Include(y => y.orderItems)
                .SingleOrDefault(x => x.orderID == id);

            foreach(var item in order.orderItems.ToList())
            {
                db.orderItems.Remove(item);
            }
            db.orders.Remove(order);
            db.SaveChanges();

            return Ok(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool orderExists(long id)
        {
            return db.orders.Count(e => e.orderID == id) > 0;
        }
    }
}