using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace webExample.Controllers
{
    public class HomeController : ApiController
    {

        public static List<Task> TASKS = new List<Task>{
            new Task{ id=1, name="task1server", done= true},
            new Task{ id=2, name="task2server"},
            new Task{ id=3, name="task3server", done = true},
            new Task{ id=4, name="task4server"}
        };
        // GET: api/Home
        public List<Task> Get()
        {
            return TASKS;
        }

        // GET: api/Home/5
        public Task Get(int id)
        {
            return TASKS.Find(t=>t.id==id);
        }

        // POST: api/Home
      
        public void Post(Task t)
        {
        
            TASKS.Add(t);
        }

        // PUT: api/Home/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Home/5
        public void Delete(int id)
        {
        }
    }

    public class Task
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool done { get; set; }
        public int userId { get; set; }
    }
}
