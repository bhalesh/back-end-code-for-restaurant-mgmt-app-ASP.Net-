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
    public class itemController : ApiController
    {
        private DBModel db = new DBModel();

        // GET: api/item
        public IQueryable<item> Getitems()
        {
            return db.items;
        }

        // GET: api/item/5
        [ResponseType(typeof(item))]
       
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

       
    }
}