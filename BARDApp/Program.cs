using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BARDApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string last_msg = "Hi, what do you think about AI?";
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n");
                last_msg = Console.ReadLine();
                Console.WriteLine("\n");
                Console.ForegroundColor = ConsoleColor.Red;
                last_msg = ParseResponse(GetRawResponse(last_msg.Replace("\n", string.Empty) + " Respond in a chatty shortened form."));
                Console.WriteLine(last_msg);
            }
        }

        public static string GetRawResponse(string input)
        {
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };

            var client = new HttpClient(handler);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://bard.google.com/_/BardChatUi/data/assistant.lamda.BardFrontendService/StreamGenerate?bl=boq_assistant-bard-web-server_20230404.15_p0&_reqid=1440589");

            var cookie = new Cookie("__Secure-1PSID", "VAjYyF80atumrTEolrEYFD3lHhd59-oVoHXRGUYzuwyxu0dBZ-Iy16aEG74KRaF7k0-ETw.")
            {
                Secure = true,
                HttpOnly = true,
                Domain = "bard.google.com"
            };

            handler.CookieContainer.Add(cookie);
            request.Headers.TryAddWithoutValidation("Origin", "https://bard.google.com");

            var content = new StringContent("f.req=%5Bnull%2C%22%5B%5B%5C%22"+input+"%5C%22%5D%2Cnull%2C%5B%5C%22c_252cbf18c4995337%5C%22%2C%5C%22%5C%22%2C%5C%22%5C%22%5D%5D%22%5D&at=ABi_lZh4X1Gg449HStgiwwXbp4bo%3A1680948998927&", Encoding.UTF8, "application/x-www-form-urlencoded");

            request.Content = content;

            var response = client.SendAsync(request).GetAwaiter().GetResult();

            var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            return responseContent;
        }
        public static string ParseResponse(string input)
        {
            int offset = 6;
            string offsetted  = input.Substring(offset, input.Length - offset);
            string json = offsetted; 

            var substr = JsonNode.Parse(json);

            //Console.WriteLine(substr[0].ToString());
            if(substr[0][2] == null)
            {
                return "Rate limit is reached!";
            }
            json = substr[0][2].ToString();
            substr = JsonNode.Parse(json);
            return substr[0][0].ToString();
        }
    }
}
