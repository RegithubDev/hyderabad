using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HYDSWMAPI.HELPERS
{
    public class NotificationHelper
    {
        public static String SendNotificationFromFirebaseCloud()
        {
            var result = "-1";
            var webAddr = "https://fcm.googleapis.com/fcm/send";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "key=AAAAHa-7FmU:APA91bFHMIlBPcHasIWMIHHMhM5Xevpr9U_OLspe_A-xpT6lsPgb2ayGpY3NVXVPXyUJlWf-2Cq-RqE7kqYgeg_XOUDcFdomJ9_D2oErA6Az_cZgeMd90JD3W2MYDtYSoXkzRZsQe5tv");
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string strNJson = @"{
                    ""to"": ""/topics/ServiceNow"",
                    ""data"": {
                        ""ShortDesc"": ""Some short desc"",
                        ""IncidentNo"": ""any number"",
                        ""Description"": ""detail desc""
  },
  ""notification"": {
                ""title"": ""ServiceNow: Incident No. number"",
    ""text"": ""This is Notification"",
""sound"":""default""
  }
        }";
                streamWriter.Write(strNJson);
                streamWriter.Flush();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            return result;
        }
    }
}
