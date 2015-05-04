using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ProxyServer.Controllers
{
    public class Item
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
    public class HomeController : Controller
    {
        public async Task<ViewResult> Index()
        {
            IEnumerable<Item> items = new List<Item>();
            //Get all items
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var byteArray = Encoding.ASCII.GetBytes("jayway:jayway");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                var response = await httpClient.GetAsync("http://kanban-api-lab.herokuapp.com/items/backlog");
                string backlog = await response.Content.ReadAsStringAsync();
                response = await httpClient.GetAsync("http://kanban-api-lab.herokuapp.com/items/working");
                string working = await response.Content.ReadAsStringAsync();
                response = await httpClient.GetAsync("http://kanban-api-lab.herokuapp.com/items/verify");
                string verify = await response.Content.ReadAsStringAsync();
                response = await httpClient.GetAsync("http://kanban-api-lab.herokuapp.com/items/done");
                string done = await response.Content.ReadAsStringAsync();

                var backlogItems = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Item>>(backlog);
                var workingItems = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Item>>(working);
                var verifyItems = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Item>>(verify);
                var doneItems = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Item>>(done);
                items = items.Concat(backlogItems).Concat(workingItems).Concat(verifyItems).Concat(doneItems);
            }
            
            return View(items);
        }
    }
}