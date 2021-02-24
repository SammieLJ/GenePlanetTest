using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyWebApi.Models;
using Ryadel.Components.Util;

namespace MyWebApi.Controllers
{
    public class CountingApiController:Controller
    {
        private readonly ILogger<CountingApiController> _logger;

        public CountingApiController(ILogger<CountingApiController> logger)
        {
            _logger = logger;
        }

        // POST api/blog
        [HttpGet("api/count")]
        public object Get()
        {
            using (var db = new AppDb())
            {
                db.Connection.Open();

                var logAccessCount = new LogAccessCounts
                {
                    Db = db,
                    IP = NetworkUtil.GetIPAddress().MapToIPv4().ToString(),
                    HostName = System.Net.Dns.GetHostName()
                };

                logAccessCount.InsertSync();
                
                // now we insert our call as count into the table
                return new
                {
                    message = $"API metoda count was called {logAccessCount.Id} on server {System.Net.Dns.GetHostName()}",
                    time = DateTime.Now
                };
            }
        }
        /*public object Get()
        {
            using (var db = new AppDb())
            {
                db.Connection.OpenAsync();

                var logAccessCount = new LogAccessCounts
                {
                    Db = db,
                    IP = NetworkUtil.GetIPAddress().MapToIPv4().ToString(),
                    HostName = System.Net.Dns.GetHostName()
                };

                // now we insert our call as count into the table
                return await (logAccessCount.InsertAsync());
            }
        }*/

        [HttpGet("api/AllCounts")]
        public object GetAllCounts()
        {
            using (var db = new AppDb())
            {
                // get all records in table
                db.Connection.Open();

                var logAccessCount = new LogAccessCounts(db);
                
                var reader = logAccessCount.LatestCountCmd().ExecuteReader();
                reader.Read();
                logAccessCount.allCounts = reader.GetFieldValue<int>(0);
                //db.Connection.Close();

                // now we insert our call as count into the table
                return new
                {
                    message = $"API method for summary of all call counts {logAccessCount.allCounts}, served from {System.Net.Dns.GetHostName()}",
                    time = DateTime.Now
                };
            }
        }
        private object await(Task task)
        {
            //throw new NotImplementedException();            
            return new
            {
                message = $"API metoda count je bila klicana {task.Id}",
                time = DateTime.Now
            };
        }

        private static Task NewMethod(LogAccessCounts logAccessCount)
        {
            return logAccessCount.InsertAsync();
        }

        [HttpGet("api/helloworld/{name}")]
        public object Get(string name)
        {
            return new
            {
                message = $"Hello world, {name}.",
                time = DateTime.Now
            };
        }
    }
}