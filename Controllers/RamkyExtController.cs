using COMMON;
using COMMON.OPERATION;
using COMMON.SWMENTITY;
using HYDSWMAPI.HELPERS;
using HYDSWMAPI.INTERFACE;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HYDSWMAPI.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RamkyExtController : ControllerBase
    {
        private IRamky<RamkyResposnse> _dataRepository;
        private readonly IWebHostEnvironment HostingEnvironment;
        public RamkyExtController(IRamky<RamkyResposnse> dataRepository, IWebHostEnvironment hostingEnvironment)
        {
            this._dataRepository = dataRepository;
            this.HostingEnvironment = hostingEnvironment;
        }
        [HttpPost]
        [Route("AddWBridgeData")]
        public IActionResult AddWBridgeData(JObject obj)
        {
            RamkyResposnse Response = new RamkyResposnse();
            string WTCode = obj.GetValue("WTCode").Value<string>();
            string WBId = obj.GetValue("WBId").Value<string>();
            string TCode = obj.GetValue("TCode").Value<string>();
            string Status = obj.GetValue("Status").Value<string>();
            string VehicleNo = obj.GetValue("VehicleNo").Value<string>();
            decimal GrossWt = obj.GetValue("GrossWt").Value<decimal>();
            decimal TareWt = obj.GetValue("TareWt").Value<decimal>();
            decimal NetWt = obj.GetValue("NetWt").Value<decimal>();
            string TSId = obj.GetValue("TSId").Value<String>();
            string SCId = obj.GetValue("SCId").Value<String>();
            DateTime TDate = obj.GetValue("TDate").Value<DateTime>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@EntityNo",VehicleNo),
                   new SqlParameter("@GrossWt",GrossWt),
                   new SqlParameter("@TareWt",TareWt),
                   new SqlParameter("@NetWt",NetWt),
                   new SqlParameter("@TDate",TDate),
                  new SqlParameter("@CreatedDate",CommonHelper.IndianStandard(DateTime.UtcNow)),
                  new SqlParameter("@CreatedBy","RAMKY SERVICE"),
                  new SqlParameter("@WTCode",WTCode),
                  new SqlParameter("@WBId",WBId),
                  new SqlParameter("@TCode",TCode),
                  new SqlParameter("@Status",Status),
                  new SqlParameter("@TSId",TSId),
                  new SqlParameter("@SCId",SCId),
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spAddWBTransaction, parameters);
            Response = JsonConvert.DeserializeObject<RamkyResposnse>(Result);

            return Ok(Response);
        }
        [HttpPost]
        [Route("PushWBBulkInfo")]
        public IActionResult PushWBBulkInfo(List<PendingWBInfo> obj)
        {
            RamkyResposnse Response = new RamkyResposnse();
            Response.Result = 1;
            Response.Msg = "Data Pushed Successfully";
            Task.Run(() => PushBulkInsert(obj));

            return Ok(Response);
        }
        public void PushBulkInsert(List<PendingWBInfo> obj)
        {
            try
            {
                DataTable dt = CommonHelper.ListToDataTable(obj);
                SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@WeightBridgeTType",dt),
                  new SqlParameter("@Syncon",CommonHelper.IndianStandard(DateTime.UtcNow)),
              };

                string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spAddBulkWBInfo, parameters);
            }
            catch (Exception ex)
            {

            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("PushExternalReaderData")]
        public IActionResult PushExternalReaderData()
        {
            string InputData = Request.QueryString.ToString();
            string filePath = Path.Combine(HostingEnvironment.WebRootPath + "/content/Logs/");
            CommonHelper.WriteToJsonFile("SiliconLog", InputData, filePath);
            string Response = "Failed";
            try
            {
                // string[] querystring = new string[20];
                InputData = CommonHelper.RemoveSpecialChars(InputData);
                string[] querystring = InputData.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (querystring.Count() > 0)
                {
                    Response = "$RFID=0#";
                    Task.Run(() => SendDataOnChennaiServer(InputData));
                }

            }
            catch (Exception ex)
            {
                var inputData = new
                {
                    Msg = InputData,
                    Error = ex.ToString()
                };
                var input = JsonConvert.SerializeObject(inputData);
                CommonHelper.WriteToJsonFile("SiliconERRLog1", input, filePath);
                return Ok(Response);
            }
            return Ok(Response);
        }
        public void SendDataOnChennaiServer(string InputData)
        {
            try
            {
                string endpoint = "https://appapi.chennaienviro.com/api/RamkyExternal/PushExternalReaderDataFromHyd?" + InputData;
                ApiHttpClientHelper<string> apiobj = new ApiHttpClientHelper<string>();

                string Result = apiobj.GetRequest(endpoint);
            }
            catch(Exception ex)
            {

            }
        }
    }
}
