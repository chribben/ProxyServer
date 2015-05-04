using System.Net.Http;
using System.Net.Http.Headers;

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Accept.Clear();
httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
var byteArray = Encoding.ASCII.GetBytes("jayway:jayway");
var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
httpClient.DefaultRequestHeaders.Authorization = header;
Console.WriteLine("Hello");
