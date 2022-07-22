using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyLand.Services
{
    public class NotificationService
    {
        const string Url = "https://blhirjn5a3.execute-api.us-east-1.amazonaws.com/default/Email-SendPropertyInterestEmail";

        public async Task<bool> CallLambda(string propertyName, string customerName, string arn)
        {
            var client = new HttpClient();
            var postData = new
            {
                customerName,
                propertyName,
                arn
            };
            var content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(Url, content);
            return response.IsSuccessStatusCode;
        }
    }
}
