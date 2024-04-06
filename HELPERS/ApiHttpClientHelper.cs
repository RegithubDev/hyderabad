using System;
using System.Net.Http;

namespace HYDSWMAPI.HELPERS
{
    public class ApiHttpClientHelper<T>
    {
        public string GetRequest(string apiUrl)
        {
            string endpoint = apiUrl;
            string result1 = string.Empty;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (var Response = client.GetAsync(endpoint))
                    {
                        Response.Wait();
                        var result = Response.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsStringAsync();
                            readTask.Wait();
                            result1 = readTask.Result;

                        }


                    }
                }
            }
            catch(Exception ex)
            {
                return string.Empty;
            }
            return result1;
        }
    }
}
