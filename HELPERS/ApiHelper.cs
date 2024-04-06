using COMMON;
using COMMON.CITIZEN;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HYDSWMAPI.HELPERS
{
    public static class ApiHelper
    {
        private const string userService_Post1 = "https://cover.quantela.com/ds/1.0.0/public/token";
        private const string datapost_Post1 = "https://cover.quantela.com/abstraction/1.0.0/adapters/inbound/8nkBXXcBfK9boYyh4eYL";
        public static void GetAuthenticateQT(string SBody,string filePath)
        {
            string VToken = string.Empty;
            try
            {
                var requestContent = new
                {
                    username = "bengalurupocoperator@bengalurupoc.com",
                    password = "Demo@12345",
                    grant_type = "password"
                };

                string output = JsonConvert.SerializeObject(requestContent);

                using (HttpClient client = new HttpClient())
                {

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(output, Encoding.UTF8, "application/json");
                    content.Headers.ContentType.CharSet = "";
                    using (var Response = client.PostAsync(userService_Post1, content))
                    {
                        Response.Wait();
                        var result = Response.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsStringAsync();
                            readTask.Wait();

                            dynamic data1 = JObject.Parse(readTask.Result);
                            VToken = data1.access_token;
                        }

                    }
                }


                if (!string.IsNullOrEmpty(VToken))
                    SendDataOnserverQT(VToken, SBody, filePath);

            }
            catch (Exception ex)
            {
                // CommonHelper.WriteToFile("Trinity GPS Auth: {0} " + ex.Message + ex.StackTrace);
            }
        }
        public static void SendDataOnserverQT(string Token, string SBody,string filePath)
        {
            try
            {
                

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(SBody, Encoding.UTF8, "application/json");
                    content.Headers.ContentType.CharSet = "";
                    using (var Response = client.PostAsync(datapost_Post1, content))
                    {
                        Response.Wait();
                        var result = Response.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsStringAsync();
                            readTask.Wait();
                            dynamic data1 = JObject.Parse(readTask.Result);
                        }

                    }
                }
                CommonHelper.WriteToJsonFile("LOG", "Recieved At-" + CommonHelper.IndianStandard(DateTime.UtcNow), filePath);
            }
            catch (Exception ex)
            {
                // CommonHelper.WriteToFile("Trinity GPS: {0} " + ex.Message + ex.StackTrace);
            }
        }
    }
}
