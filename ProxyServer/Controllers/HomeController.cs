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
            var items = await GetItems();


            return View(items);
        }

        private static async Task<IEnumerable<Item>> GetItems()
        {
            IEnumerable<Item> items = new List<Item>();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes("jayway:jayway")));
                var httpResponseMessages = await Task.WhenAll(
                    httpClient.GetAsync("http://kanban-api-lab.herokuapp.com/items/backlog"), 
                    httpClient.GetAsync("http://kanban-api-lab.herokuapp.com/items/working"),
                    httpClient.GetAsync("http://kanban-api-lab.herokuapp.com/items/verify"), 
                    httpClient.GetAsync("http://kanban-api-lab.herokuapp.com/items/done"));
                string working = await httpResponseMessages[0].Content.ReadAsStringAsync();
                string backlog = await httpResponseMessages[1].Content.ReadAsStringAsync();
                string verify = await httpResponseMessages[2].Content.ReadAsStringAsync();
                string done = await httpResponseMessages[3].Content.ReadAsStringAsync();

                var backlogItems = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Item>>(backlog);
                var workingItems = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Item>>(working);
                var verifyItems = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Item>>(verify);
                var doneItems = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Item>>(done);
                items = items.Concat(backlogItems).Concat(workingItems).Concat(verifyItems).Concat(doneItems);
            }
            return items;
        }

        public Task<ViewResult> ToBacklog(int id)
        {
            return GetUpdate(id, "http://kanban-api-lab.herokuapp.com/items/backlog");
        }
        public Task<ViewResult> ToWorking(int id)
        {
            return GetUpdate(id, "http://kanban-api-lab.herokuapp.com/items/working");
        }
        public Task<ViewResult> ToVerify(int id)
        {
            return GetUpdate(id, "http://kanban-api-lab.herokuapp.com/items/verify");
        }
        public Task<ViewResult> ToDone(int id)
        {
            return GetUpdate(id, "http://kanban-api-lab.herokuapp.com/items/done");
        }

        public async Task<ViewResult> GetUpdate(int id, string url)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var byteArray = Encoding.ASCII.GetBytes("jayway:jayway");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                var content = new FormUrlEncodedContent(new[] 
                    {
                        new KeyValuePair<string, string>("id", id.ToString())
                    }); ;
                await httpClient.PostAsync(url, content);

            }
            var items = await GetItems();
            return View("Index", items);
            
        }
    }
}