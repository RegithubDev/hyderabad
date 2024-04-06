using COMMON;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace HYDSWMAPI.HELPERS
{
    public class MailSenderHelper
    {
        static string Smtp = Startup.StaticConfig.GetValue<string>("EmailSetting:Smtp");
        static bool SSL = Startup.StaticConfig.GetValue<bool>("EmailSetting:SSL");
        static int Port = Startup.StaticConfig.GetValue<int>("EmailSetting:Port");
        static string FromEmailId = Startup.StaticConfig.GetValue<string>("EmailSetting:EmailId");
        static string Password = Startup.StaticConfig.GetValue<string>("EmailSetting:Password");

        public static void GeofenceAlertEmail(string FPath, List<string> ToMail, List<string> CCMail, string MailDisplayDesc, string Subject, JObject item)
        {

            string readFile = string.Empty;
            using (StreamReader reader = new StreamReader(FPath))
            {
                readFile = reader.ReadToEnd();
            }
            string StrContent = "";
            StrContent = readFile;
            StrContent = StrContent.Replace("[content]", "Geofence Alert");
            StrContent = StrContent.Replace("[VEHICLE]",  item.GetValue("VehicleNo").ToString());
            StrContent = StrContent.Replace("[imei]", item.GetValue("ImeiNo").ToString());
            StrContent = StrContent.Replace("[GeoName]", item.GetValue("GeofenceName").ToString());

            MailHelper.SendEmail(ToMail, CCMail, FromEmailId, Password, MailDisplayDesc, Smtp, SSL, Port, Subject, StrContent);
        }
    }
}
