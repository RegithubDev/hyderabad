using COMMON;
using COMMON.ASSET;
using COMMON.CITIZEN;
using COMMON.GENERIC;
using COMMON.OPERATION;
using HYDSWMAPI.INTERFACE;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HYDSWMAPI.HELPERS;


namespace HYDSWMAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OperationController : ControllerBase
    {
        private TReport _report;
        private IOperation<CLoginResponseInfo> _dataRepository;
        private readonly IWebHostEnvironment HostingEnvironment;
        public OperationController(IOperation<CLoginResponseInfo> dataRepository, IWebHostEnvironment hostingEnvironment, TReport report)
        {
            this._dataRepository = dataRepository;
            this.HostingEnvironment = hostingEnvironment;
            this._report = report;
        }

        [HttpPost]
        [Route("GetUIDByCode")]
        public IActionResult GetUIDByCode(JObject obj)
        {
            string EntityType = obj.GetValue("EntityType").Value<string>();
            string Code = obj.GetValue("Code").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@EntityType",EntityType),
                  new SqlParameter("@Code",Code),
             };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetUIDByCode, parameters);
            if (string.IsNullOrEmpty(Result))
                Result = "{}";
            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAssetInfoByQrScan")]
        public IActionResult GetAssetInfoByQrScan(JObject obj)
        {
            ContainerInfo containerinfo = new ContainerInfo();
            QrHelperInfo datainfo = new QrHelperInfo();
            int OperationTypeId = obj.GetValue("OperationTypeId").Value<int>();
            int Step = obj.GetValue("Step").Value<int>();
            string UId = obj.GetValue("UId").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@OperationTypeId",OperationTypeId),
                  new SqlParameter("@Step",Step),
                  new SqlParameter("@UId",UId),
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetAssetInfoByQrScan, parameters);
            if (!string.IsNullOrEmpty(Result))
            {
                dynamic FResult = JObject.Parse(Result);
                JArray _lst = FResult.Table;
                JArray _lst1 = FResult.Table1;
                List<ContainerInfo> containerlst = _lst1.ToObject<List<ContainerInfo>>();
                List<QrHelperInfo> datalst = _lst.ToObject<List<QrHelperInfo>>();
                datainfo = datalst.FirstOrDefault();
                containerinfo = containerlst.FirstOrDefault();
                if (containerinfo == null)
                {
                    containerinfo = new ContainerInfo();
                    containerinfo.ContainerCode = "";
                    containerinfo.ContainerName = "";
                    containerinfo.ContainerType = "";
                }
            }
            var response = new
            {
                data = datainfo,
                containerinfo = containerinfo
            };
            return Ok(response);
        }
        [HttpPost]
        [Route("QrCodeScanAtSCTP")]
        public IActionResult QrCodeScanAtSCTP(JObject obj)
        {
            try
            {
                int OperationTypeId = obj.GetValue("OperationTypeId").Value<int>();
                string EntityType = obj.GetValue("EntityType").Value<string>();
                int PTDId = obj.GetValue("PTDId").Value<int>();
                SqlParameter[] parameters = new SqlParameter[]
                  {
                  new SqlParameter("@OperationTypeId",OperationTypeId),
                  new SqlParameter("@EntityType",EntityType),
                  new SqlParameter("@PTDId",PTDId),
                  };

                string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spQrCodeScanAtSCTP, parameters);

                dynamic dResult = JObject.Parse(Result);

                string VehicleNo = dResult.VehicleNo;
                string DriverName = dResult.DriverName;
                string ContactNo = dResult.ContactNo;
                string NameOfContractor = dResult.NameOfContractor;
                string DateAndTimeofScanned = dResult.DateAndTimeofScanned;
                int TripNoForday = dResult.TripNoForday;
                string QRCodeLabel = dResult.QRCodeLabel;
                string PTDIdForQrCode = "This is authenticated reciept";
                string TStationName = dResult.TStationName;

                byte[] fileBytes = GetQrImage(VehicleNo, DriverName, ContactNo, NameOfContractor, DateAndTimeofScanned, TripNoForday, QRCodeLabel, PTDIdForQrCode, TStationName);
                return File(fileBytes, "application/pdf", VehicleNo + ".pdf");

            }


            catch (Exception ex)
            {
                CommonHelper.WriteToJsonFile("Pdffile", " Error-" + ex.Message, "");

                return Ok(ex.Message);

            }

        }
        private byte[] GetQrImage(string VehicleNo, string DriverName, string ContactNo, string NameOfContractor, string DateAndTimeofScanned, int TripNoForday, string QRCodeLabel, string PTDIdForQrCode, string TStationName)
        {
            Bitmap QRCode = QRHelper.VehicleQrCodeTest(PTDIdForQrCode);
            var finalResult = _report.GenerateQRCode(VehicleNo, DriverName, ContactNo, NameOfContractor, DateAndTimeofScanned, TripNoForday, QRCodeLabel, QRCode, TStationName);
            return finalResult;
        }

        [HttpPost]
        [Route("AddNewQrTransaction")]
        public async Task<IActionResult> AddNewQrTransaction([FromForm] IFormCollection value)
        {

            string FName = string.Empty;
            QRTransactionInfo obj = JsonConvert.DeserializeObject<QRTransactionInfo>(value["Input"]);

            string FolderName = "/content/qrtransaction/";
            if (value.Files.Count > 0)
                if (value.Files[0].Length > 0)
                    FName = CommonHelper.generateID() + Path.GetExtension(value.Files[0].FileName);

            string filePath = Path.Combine(HostingEnvironment.WebRootPath + FolderName, FName);

            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@OperationTypeId",obj.OperationTypeId),
                  new SqlParameter("@Step1UId",obj.Step1UId),
                  new SqlParameter("@Step1Lat",obj.Step1Lat),
                  new SqlParameter("@Step1Lng",obj.Step1Lng),
                  new SqlParameter("@Step1SyncOn",obj.Step1SyncOn),
                  new SqlParameter("@Step2UId",obj.OperationTypeId==(int)Enums.OperationType.TERTIARY_COLLECTION?"":obj.Step2UId),
                  new SqlParameter("@Step2Lat",obj.OperationTypeId==(int)Enums.OperationType.TERTIARY_COLLECTION?"":obj.Step2Lat),
                  new SqlParameter("@Step2Lng",obj.OperationTypeId==(int)Enums.OperationType.TERTIARY_COLLECTION?"":obj.Step2Lng),
                 // new SqlParameter("@Step2SyncOn",obj.OperationTypeId==(int)Enums.OperationType.TERTIARY_COLLECTION?CommonHelper.IndianStandard(DateTime.UtcNow):obj.Step2SyncOn),
                  new SqlParameter("@Step2SyncOn",CommonHelper.IndianStandard(DateTime.UtcNow)),
                  new SqlParameter("@Is90Perc",obj.OperationTypeId==(int)Enums.OperationType.PRIMARY_COLLECTION?obj.Is90Perc:false),
                  new SqlParameter("@IsClosed",obj.IsClosed),//obj.OperationTypeId==(int)Enums.OperationType.PRIMARY_COLLECTION?obj.IsClosed:true),
                  new SqlParameter("@Step2Filled",obj.OperationTypeId==(int)Enums.OperationType.PRIMARY_COLLECTION?obj.Step2Filled:0),
                  new SqlParameter("@TSId",obj.TSId),
                  new SqlParameter("@IsDeviated",obj.IsDeviated),
                  new SqlParameter("@DistanceFromTS",obj.DistanceFromTS),
                  new SqlParameter("@CreatedDate",CommonHelper.IndianStandard(DateTime.UtcNow)),
                  new SqlParameter("@CreatedBy",obj.CreatedBy),
                  new SqlParameter("@ImgUrl",FName),
                  new SqlParameter("@FolderName",FolderName),
                  new SqlParameter("@Remarks",obj.Remarks),
                  new SqlParameter("@ParentId",obj.ParentId),
                  new SqlParameter("@Step2IsDeviated",obj.Step2IsDeviated),
                  new SqlParameter("@Step2DistanceFromTS",obj.Step2DistanceFromTS),
                  new SqlParameter("@DHLTId",obj.DHLTId),
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spAddNewQrTransaction, parameters);
            dynamic dresult = JObject.Parse(Result);
            if (dresult.Result == "1" || dresult.Result == "2")
            {
                if (value.Files.Count > 0)
                {
                    if (value.Files[0].Length > 0)
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await value.Files[0].CopyToAsync(fileStream);
                        }
                }
            }
            return Ok(Result);

        }

        [HttpPost]
        [Route("GetAllMQRTransactionInfo")]
        public IActionResult GetAllMQRTransactionInfo(JObject obj)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            string PageNumber = obj.GetValue("PageNumber").Value<string>();
            string PageSize = obj.GetValue("PageSize").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            string SearchTerm = obj.GetValue("SearchTerm").Value<string>();
            int OperationTypeId = obj.GetValue("OperationTypeId").Value<int>();
            DateTime FromDate = obj.GetValue("FromDate").Value<DateTime>();
            DateTime ToDate = obj.GetValue("ToDate").Value<DateTime>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",SearchTerm),
                  new SqlParameter("@SortColumn",""),
                  new SqlParameter("@SortOrder",""),
                  new SqlParameter("@PageNumber",PageNumber),
                  new SqlParameter("@PageSize",PageSize),
                  new SqlParameter("@LoginId",UserId),
                  new SqlParameter("@OperationTypeId",OperationTypeId),
                  new SqlParameter("@FPath",baseUrl),
                  new SqlParameter("@FromDate",FromDate),
                  new SqlParameter("@ToDate",ToDate)
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllQRTransaction_Paging, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllCollectionNotification")]
        public IActionResult GetAllCollectionNotification(JObject obj)
        {
            string ZoneId = obj.GetValue("ZoneId").Value<string>();
            string CircleId = obj.GetValue("CircleId").Value<string>();
            string WardId = obj.GetValue("WardId").Value<string>();
            string LoginId = obj.GetValue("LoginId").Value<string>();
            ZoneId = !string.IsNullOrEmpty(ZoneId) ? ZoneId : "0";
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@ZoneId",ZoneId),
                  new SqlParameter("@CircleId",CircleId),
                  new SqlParameter("@WardId",WardId),
                  new SqlParameter("@LoginId",LoginId)
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetAllCollectionNotification, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllCollectionNoti")]
        public IActionResult GetAllCollectionNoti(DataTableAjaxPostModel requestModel)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            Order _order = new Order();

            string SearchTXT = requestModel.search.value;
            int draw = requestModel.draw;
            int start = requestModel.start;
            int length = requestModel.length;

            string str = string.Empty;
            if (SearchTXT != null)
                str = SearchTXT.Trim();

            string SortColumn = string.Empty;
            string SortDir = requestModel.order[0].dir;
            switch (requestModel.order[0].column)
            {
                case 2:
                    SortColumn = "AssetStatus";
                    break;
                case 3:
                    SortColumn = "UId";
                    break;
                case 4:
                    SortColumn = "ContainerCode";
                    break;
                case 5:
                    SortColumn = "ContainerName";
                    break;
                case 6:
                    SortColumn = "Capacity";
                    break;
                case 7:
                    SortColumn = "ContainerType";
                    break;
                case 8:
                    SortColumn = "CStatus";
                    break;

                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",str),
                  new SqlParameter("@SortColumn",SortColumn),
                  new SqlParameter("@SortOrder",SortDir),
                  new SqlParameter("@PageNumber",start),
                  new SqlParameter("@PageSize",length),
                  new SqlParameter("@LoginId",requestModel.UserId),
                  new SqlParameter("@OperationTypeId",requestModel.NotiId),
                  new SqlParameter("@FPath",baseUrl),
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@ToDate",requestModel.ToDate),
                  new SqlParameter("@IsVehicle",requestModel.VehicleTypeId),
                  new SqlParameter("@ZoneId",requestModel.ZoneId),
                  new SqlParameter("@CircleId",requestModel.CircleId),
                  new SqlParameter("@WardId",requestModel.WardId),
                  new SqlParameter("@IsAll",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"0"),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllQRTransactionNoti_Paging, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetTransactionParentId")]
        public IActionResult GetTransactionParentId(JObject obj)
        {
            string UId = obj.GetValue("UId").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@OperationTypeId",(int)Enums.OperationType.PRIMARY_COLLECTION),
                  new SqlParameter("@UId",UId),
                  new SqlParameter("@TDate",CommonHelper.IndianStandard(DateTime.UtcNow)),
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetTransactionParentId, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllMPrimaryTransaction")]
        public IActionResult GetAllMPrimaryTransaction(JObject obj)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            string PageNumber = obj.GetValue("PageNumber").Value<string>();
            string PageSize = obj.GetValue("PageSize").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            string SearchTerm = obj.GetValue("SearchTerm").Value<string>();
            int OperationTypeId = obj.GetValue("OperationTypeId").Value<int>();
            DateTime FromDate = obj.GetValue("FromDate").Value<DateTime>();
            DateTime ToDate = obj.GetValue("ToDate").Value<DateTime>();
            int IsClosed = obj.GetValue("IsClosed").Value<int>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",SearchTerm),
                  new SqlParameter("@SortColumn",""),
                  new SqlParameter("@SortOrder",""),
                  new SqlParameter("@PageNumber",PageNumber),
                  new SqlParameter("@PageSize",PageSize),
                  new SqlParameter("@LoginId",UserId),
                  new SqlParameter("@OperationTypeId",OperationTypeId),
                  new SqlParameter("@FPath",baseUrl),
                  new SqlParameter("@FromDate",FromDate),
                  new SqlParameter("@ToDate",ToDate),
                  new SqlParameter("@IsWeb","0"),
                  new SqlParameter("@IsClosed",IsClosed)
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllPrimaryTransaction_Paging, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllMPrimaryVehicleTransactionByContainer")]
        public IActionResult GetAllMPrimaryVehicleTransactionByContainer(JObject obj)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            int CTDId = obj.GetValue("CTDId").Value<int>();
            DateTime CreatedDate = obj.GetValue("CreatedDate").Value<DateTime>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@CTDId",CTDId),
                  new SqlParameter("@CreatedDate",CreatedDate),
                  new SqlParameter("@FPath",baseUrl),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllPrimaryVehicleTransactionByContainer, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("ChangeStatusOfTransaction")]
        public IActionResult ChangeStatusOfTransaction(JObject obj)
        {
            int CTDId = obj.GetValue("CTDId").Value<int>();
            string OperationTypeId = obj.GetValue("OperationTypeId").Value<string>();
            string TId = obj.GetValue("TId").Value<string>();
            string StatusId = obj.GetValue("StatusId").Value<string>();
            string CreatedBy = obj.GetValue("CreatedBy").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@CTDId",CTDId),
                  new SqlParameter("@StatusId",StatusId),
                  new SqlParameter("@OperationTypeId",OperationTypeId),
                  new SqlParameter("@TId",TId),
                  new SqlParameter("@CreatedDate",CommonHelper.IndianStandard(DateTime.UtcNow)),
                  new SqlParameter("@CreatedBy",CreatedBy)
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spChangeStatusOfTransaction, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllPrimaryTransaction")]
        public IActionResult GetAllPrimaryTransaction(DataTableAjaxPostModel requestModel)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            Order _order = new Order();

            string SearchTXT = requestModel.search.value;
            int draw = requestModel.draw;
            int start = requestModel.start;
            int length = requestModel.length;

            string str = string.Empty;
            if (SearchTXT != null)
                str = SearchTXT.Trim();

            string SortColumn = string.Empty;
            string SortDir = requestModel.order[0].dir;
            switch (requestModel.order[0].column)
            {
                case 2:
                    SortColumn = "AssetStatus";
                    break;
                case 3:
                    SortColumn = "UId";
                    break;
                case 4:
                    SortColumn = "ContainerCode";
                    break;
                case 5:
                    SortColumn = "ContainerName";
                    break;
                case 6:
                    SortColumn = "Capacity";
                    break;
                case 7:
                    SortColumn = "ContainerType";
                    break;
                case 8:
                    SortColumn = "CStatus";
                    break;

                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",str),
                  new SqlParameter("@SortColumn",SortColumn),
                  new SqlParameter("@SortOrder",SortDir),
                  new SqlParameter("@PageNumber",start),
                  new SqlParameter("@PageSize",length),
                  new SqlParameter("@LoginId",requestModel.UserId),
                  new SqlParameter("@OperationTypeId",requestModel.NotiId),
                  new SqlParameter("@FPath",baseUrl),
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@ToDate",requestModel.ToDate),
                  new SqlParameter("@IsWeb","1"),
                  new SqlParameter("@IsClosed",!string.IsNullOrEmpty(requestModel.EmpCode)?requestModel.EmpCode:"-1"),
             };
            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllPrimaryTransaction_Paging, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("AddPrimaryTransaction")]
        public async Task<IActionResult> AddPrimaryTransaction([FromForm] IFormCollection value)
        {
            DateTime TDate = CommonHelper.IndianStandard(DateTime.UtcNow);
            string FName = string.Empty;
            PTransactionInfo obj = JsonConvert.DeserializeObject<PTransactionInfo>(value["Input"]);
            string FolderName = "/content/qrtransaction/";
            if (value.Files.Count > 0)
                if (value.Files[0].Length > 0)
                    FName = CommonHelper.generateID() + Path.GetExtension(value.Files[0].FileName);

            string filePath = Path.Combine(HostingEnvironment.WebRootPath + FolderName, FName);

            #region Same vehicle within 25 min validation
            if (obj.ParentId > 0 && obj.UIdType == "VEHICLE")
            {
                SqlParameter[] vparameters = new SqlParameter[]
                  {
                  new SqlParameter("@OperationTypeId",obj.OperationTypeId),
                  new SqlParameter("@UId",obj.UId),
                  new SqlParameter("@UIdType",obj.UIdType),
                  new SqlParameter("@TDate",TDate),
                  new SqlParameter("@ParentId",obj.ParentId),
                  };

                string VResult = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetValidatePrimaryVehicleEntry, vparameters);
                dynamic Vdresult = JObject.Parse(VResult);
                if (Vdresult.Result == "0")
                {
                    return Ok(VResult);
                }
            }
            #endregion


            SqlParameter[] parameters = new SqlParameter[]
          {
                  new SqlParameter("@OperationTypeId",obj.OperationTypeId),
                  new SqlParameter("@UId",obj.UId),
                  new SqlParameter("@UIdType",obj.UIdType),
                  new SqlParameter("@Lat",obj.Lat),
                  new SqlParameter("@Lng",obj.Lng),
                  new SqlParameter("@IsClosed",obj.IsClosed),
                  new SqlParameter("@TSId",obj.TSId),
                  new SqlParameter("@DistanceFromTS",obj.DistanceFromTS),
                  new SqlParameter("@CreatedDate",TDate),
                  new SqlParameter("@CreatedBy",obj.CreatedBy),
                  new SqlParameter("@IsDeviated",obj.IsDeviated),
                  new SqlParameter("@ImgUrl",FName),
                  new SqlParameter("@FolderName",FolderName),
                  new SqlParameter("@ParentId",obj.ParentId),
                  new SqlParameter("@FilledPerc",obj.FilledPerc),
                  new SqlParameter("@Remarks",obj.Remarks),
                  new SqlParameter("@Address",obj.Address),
                  new SqlParameter("@DHLTId",obj.DHLTId),
                  new SqlParameter("@WasteType",obj.WasteType),
          };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spAddPrimaryTransaction, parameters);
            dynamic dresult = JObject.Parse(Result);
            if (dresult.Result == "1" || dresult.Result == "2")
                if (value.Files.Count > 0)
                {
                    if (value.Files[0].Length > 0)
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await value.Files[0].CopyToAsync(fileStream);
                        }
                }
            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAssetInfoByUID")]
        public IActionResult GetAssetInfoByUID(JObject obj)
        {
            ContainerInfo containerinfo = new ContainerInfo();
            QrHelperInfo datainfo = new QrHelperInfo();
            int CTDId = obj.GetValue("CTDId").Value<int>();
            int OperationTypeId = obj.GetValue("OperationTypeId").Value<int>();
            string UId = obj.GetValue("UId").Value<string>();
            string UIdType = obj.GetValue("UIdType").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@UId",UId),
                  new SqlParameter("@TDate",CommonHelper.IndianStandard(DateTime.UtcNow)),
                  new SqlParameter("@CTId",CTDId),
                  new SqlParameter("@UIdType",UIdType),
                  new SqlParameter("@OperationTypeId",OperationTypeId)
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetAssetInfoByUID, parameters);

            if (!string.IsNullOrEmpty(Result))
            {
                dynamic FResult = JObject.Parse(Result);
                JArray _lst = FResult.Table;
                JArray _lst1 = FResult.Table1;
                List<ContainerInfo> containerlst = _lst1.ToObject<List<ContainerInfo>>();
                List<QrHelperInfo> datalst = _lst.ToObject<List<QrHelperInfo>>();
                datainfo = datalst.FirstOrDefault();
                containerinfo = containerlst.FirstOrDefault();
                if (containerinfo == null)
                {
                    containerinfo = new ContainerInfo();
                    containerinfo.ContainerCode = "";
                    containerinfo.ContainerName = "";
                    containerinfo.ContainerType = "";
                }
            }
            var response = new
            {
                data = datainfo,
                containerinfo = containerinfo
            };
            return Ok(response);
        }
        [HttpGet]
        [Route("GetPCollectionById")]
        public IActionResult GetPCollectionById(int PTDId, int OperationTypeId)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            PContainerInfo containerinfo = new PContainerInfo();
            List<PVehicleInfo> vehiclelst = new List<PVehicleInfo>();
            // int PTDId = obj.GetValue("PTDId").Value<int>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@PTDId",PTDId),
                  new SqlParameter("@FPath",baseUrl),
                  new SqlParameter("@OperationTypeId",OperationTypeId),
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetPCollectionById, parameters);

            if (!string.IsNullOrEmpty(Result))
            {
                dynamic FResult = JObject.Parse(Result);
                JArray _lst = FResult.Table;
                JArray _lst1 = FResult.Table1;
                List<PContainerInfo> containerlst = _lst.ToObject<List<PContainerInfo>>();
                vehiclelst = _lst1.ToObject<List<PVehicleInfo>>();
                containerinfo = containerlst.FirstOrDefault();
                if (containerinfo == null)
                {
                    containerinfo = new PContainerInfo();
                    containerinfo.ContainerCode = "";
                    containerinfo.ContainerName = "";
                    containerinfo.ContainerType = "";
                    containerinfo.CreatedOn = "";
                    containerinfo.CreatedBy = "";
                    containerinfo.OperationType = "";
                }
            }
            var response = new
            {
                containerinfo = containerinfo,
                vehicleinfo = vehiclelst
            };
            return Ok(response);
        }
        [HttpPost]
        [Route("GetAllMNotification")]
        public IActionResult GetAllMNotification(JObject obj)
        {
            string LoginId = obj.GetValue("LoginId").Value<string>();
            string TSId = obj.GetValue("TSId").Value<string>();
            DateTime TDate = CommonHelper.IndianStandard(DateTime.UtcNow);
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@LoginId",LoginId),
                  new SqlParameter("@TSId",TSId),
                  new SqlParameter("@TDate",TDate)
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetAllMAppNotification, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllMCollectionNotification")]
        public IActionResult GetAllMCollectionNotification(JObject obj)
        {
            string PageNumber = obj.GetValue("PageNumber").Value<string>();
            string PageSize = obj.GetValue("PageSize").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            string SearchTerm = obj.GetValue("SearchTerm").Value<string>();
            int OperationTypeId = obj.GetValue("OperationTypeId").Value<int>();
            int TSId = obj.GetValue("TSId").Value<int>();
            int IsVehicle = obj.GetValue("IsVehicle").Value<int>();
            int IsAll = obj.GetValue("IsAll").Value<int>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@SearchTerm",SearchTerm),
                  new SqlParameter("@SortColumn",""),
                  new SqlParameter("@SortOrder",""),
                  new SqlParameter("@PageNumber",PageNumber),
                  new SqlParameter("@PageSize",PageSize),
                  new SqlParameter("@LoginId",UserId),
                  new SqlParameter("@OperationTypeId",OperationTypeId),
                  new SqlParameter("@TSId",TSId),
                  new SqlParameter("@TDate",CommonHelper.IndianStandard(DateTime.UtcNow)),
                  new SqlParameter("@IsVehicle",IsVehicle),
                  new SqlParameter("@IsAll",IsAll),
            };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllMTransactionNoti, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("spGetAllMForceTransactionInfo")]
        public IActionResult spGetAllMForceTransactionInfo(JObject obj)
        {
            string PageNumber = obj.GetValue("PageNumber").Value<string>();
            string PageSize = obj.GetValue("PageSize").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            string SearchTerm = obj.GetValue("SearchTerm").Value<string>();
            int TSId = obj.GetValue("TSId").Value<int>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@SearchTerm",SearchTerm),
                  new SqlParameter("@SortColumn",""),
                  new SqlParameter("@SortOrder",""),
                  new SqlParameter("@PageNumber",PageNumber),
                  new SqlParameter("@PageSize",PageSize),
                  new SqlParameter("@UserId",UserId),
                  new SqlParameter("@TSId",TSId),
                  new SqlParameter("@TDate",CommonHelper.IndianStandard(DateTime.UtcNow)),
                  new SqlParameter("@UTSId","0"),
                  new SqlParameter("@TId","0"),
            };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllForceTransaction_Paging, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllWeightBridgeInfo")]
        public IActionResult GetAllWeightBridgeInfo(DataTableAjaxPostModel requestModel)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            Order _order = new Order();

            string SearchTXT = requestModel.search.value;
            int draw = requestModel.draw;
            int start = requestModel.start;
            int length = requestModel.length;

            string str = string.Empty;
            if (SearchTXT != null)
                str = SearchTXT.Trim();

            string SortColumn = string.Empty;
            string SortDir = requestModel.order[0].dir;
            switch (requestModel.order[0].column)
            {
                case 1:
                    SortColumn = "WBId";
                    break;
                case 2:
                    SortColumn = "WTCode";
                    break;
                case 3:
                    SortColumn = "TDate";
                    break;
                case 4:
                    SortColumn = "Status";
                    break;
                case 5:
                    SortColumn = "TCode";
                    break;
                case 6:
                    SortColumn = "EntityNo";
                    break;
                case 8:
                    SortColumn = "GrossWt";
                    break;
                case 9:
                    SortColumn = "TareWt";
                    break;
                case 10:
                    SortColumn = "NetWt";
                    break;

                case 13:
                    SortColumn = "CreatedDate";
                    break;
                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",str),
                  new SqlParameter("@SortColumn",SortColumn),
                  new SqlParameter("@SortOrder",SortDir),
                  new SqlParameter("@PageNumber",start),
                  new SqlParameter("@PageSize",length),
                  new SqlParameter("@UserId",requestModel.UserId),
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@ToDate",requestModel.ToDate),
                  new SqlParameter("@IsAll",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"0"),
             };
            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllWeightBridgeInfo_Paging, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllMScannedVehicle_Paging")]
        public IActionResult GetAllMScannedVehicle_Paging(JObject obj)
        {
            string PageNumber = obj.GetValue("PageNumber").Value<string>();
            string PageSize = obj.GetValue("PageSize").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            string SearchTerm = obj.GetValue("SearchTerm").Value<string>();
            int TSId = obj.GetValue("TSId").Value<int>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@SearchTerm",SearchTerm),
                  new SqlParameter("@SortColumn",""),
                  new SqlParameter("@SortOrder",""),
                  new SqlParameter("@PageNumber",PageNumber),
                  new SqlParameter("@PageSize",PageSize),
                  new SqlParameter("@UserId",UserId),
                  new SqlParameter("@TSId",TSId),
                  new SqlParameter("@TDate",CommonHelper.IndianStandard(DateTime.UtcNow)),
            };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllMScannedVehicle_Paging, parameters);

            return Ok(Result);
        }

        [HttpGet]
        [Route("GetAllTransferStationByUser")]
        public IActionResult GetAllTransferStationByUser(string UserId)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@UserId",UserId),
              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllTransferStationByUser, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetMDashboardCount")]
        public IActionResult GetMDashboardCount(JObject obj)
        {
            string ZoneId = obj.GetValue("ZoneId").Value<string>();
            string CircleId = obj.GetValue("CircleId").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            string VehicleTypeId = obj.GetValue("VehicleTypeId").Value<string>();
            int TSId = obj.GetValue("TSId").Value<int>();
            int UTsId = obj.GetValue("UTsId").Value<int>();
            DateTime FromDate = obj.GetValue("FromDate").Value<DateTime>();
            DateTime ToDate = obj.GetValue("ToDate").Value<DateTime>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@UserId",UserId),
                  new SqlParameter("@ZoneId",ZoneId),
                  new SqlParameter("@CircleId",CircleId),
                  new SqlParameter("@VehicleTypeId",VehicleTypeId),
                  new SqlParameter("@TSId",TSId),
                  new SqlParameter("@UTsId",UTsId),
                  new SqlParameter("@FromDate",FromDate),
                  new SqlParameter("@ToDate",ToDate),
            };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetMDashboardCount, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("ExportMAppDataLog")]
        public FileResult ExportMAppDataLog(JObject obj)
        {
            string ZoneId = obj.GetValue("ZoneId").Value<string>();
            string CircleId = obj.GetValue("CircleId").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            string VehicleTypeId = obj.GetValue("VehicleTypeId").Value<string>();
            int TSId = obj.GetValue("TSId").Value<int>();
            int UTsId = obj.GetValue("UTsId").Value<int>();
            DateTime FromDate = obj.GetValue("FromDate").Value<DateTime>();
            DateTime ToDate = obj.GetValue("ToDate").Value<DateTime>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@UserId",UserId),
                  new SqlParameter("@ZoneId",ZoneId),
                  new SqlParameter("@CircleId",CircleId),
                  new SqlParameter("@VehicleTypeId",VehicleTypeId),
                  new SqlParameter("@TSId",TSId),
                  new SqlParameter("@UTsId",UTsId),
                  new SqlParameter("@FromDate",FromDate),
                  new SqlParameter("@ToDate",ToDate),
            };
            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetMAppLogFile, parameters);

            byte[] filearray = null;
            string ContentType = string.Empty;
            DateTime ReportTime = DateTime.Now;
            string Name = "Data Report From- " + FromDate.ToString("dd-MM-yyyy") + " To- " + ToDate.ToString("dd-MM-yyyy");

            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string filename = Name + ReportTime.ToShortDateString() + ".xlsx";
            filearray = _report.ExportMAppDataLog(Result, Name);

            return File(filearray, ContentType, filename);
        }

        [HttpPost]
        [Route("AddDeployHLInfo")]
        public IActionResult AddDeployHLInfo(JObject obj)
        {
            string UId = obj.GetValue("UId").Value<string>();
            string TSId = obj.GetValue("TSId").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            string UIdType = obj.GetValue("UIdType").Value<string>();
            string Address = obj.GetValue("Address").Value<string>();
            string Lat = obj.GetValue("Lat").Value<string>();
            string Lng = obj.GetValue("Lng").Value<string>();
            bool IsDeviated = obj.GetValue("IsDeviated").Value<bool>();
            decimal DistanceFromTS = obj.GetValue("DistanceFromTS").Value<decimal>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                 new SqlParameter("@UId",UId),
                  new SqlParameter("@TSId",TSId),
                  new SqlParameter("@CreatedDate",CommonHelper.IndianStandard(DateTime.UtcNow)),
                  new SqlParameter("@CreatedBy",UserId),
                  new SqlParameter("@UIdType",UIdType),
                  new SqlParameter("@Lat",Lat),
                  new SqlParameter("@Lng",Lng),
                  new SqlParameter("@Address",Address),
                  new SqlParameter("@IsDeviated",IsDeviated),
                  new SqlParameter("@DistanceFromTS",DistanceFromTS),
            };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spAddDeployHLInfo, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetValidateHLUId")]
        public IActionResult GetValidateHLUId(JObject obj)
        {
            ContainerInfo containerinfo = new ContainerInfo();
            QrHelperInfo datainfo = new QrHelperInfo();
            int OperationTypeId = (int)Enums.OperationType.SECONDARY_COLLECTION;
            string UId = obj.GetValue("UId").Value<string>();
            string UIdType = obj.GetValue("UIdType").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                   new SqlParameter("@UId",UId),
                  new SqlParameter("@OperationTypeId",OperationTypeId),
                  new SqlParameter("@UIdType",UIdType),

              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spValidateHLUId, parameters);
            if (!string.IsNullOrEmpty(Result))
            {
                dynamic FResult = JObject.Parse(Result);
                JArray _lst = FResult.Table;
                JArray _lst1 = FResult.Table1;
                List<ContainerInfo> containerlst = _lst1.ToObject<List<ContainerInfo>>();
                List<QrHelperInfo> datalst = _lst.ToObject<List<QrHelperInfo>>();
                datainfo = datalst.FirstOrDefault();
                containerinfo = containerlst.FirstOrDefault();
            }
            var response = new
            {
                data = datainfo,
                containerinfo = containerinfo
            };
            return Ok(response);
        }

        [HttpPost]
        [Route("GetAllAvailContainer")]
        public IActionResult GetAllAvailContainer(JObject obj)
        {
            string TSId = obj.GetValue("TSId").Value<string>();
            string LoginId = obj.GetValue("LoginId").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                   new SqlParameter("@TsId",TSId),
                  new SqlParameter("@LoginId",LoginId),

              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllAvailContainer, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetMDashboardLst")]
        public IActionResult GetMDashboardLst(SearchParamInfo obj)
        {
            //string ZoneId = obj.GetValue("ZoneId").Value<string>();
            //string CircleId = obj.GetValue("CircleId").Value<string>();
            //string UserId = obj.GetValue("UserId").Value<string>();
            //string VehicleTypeId = obj.GetValue("VehicleTypeId").Value<string>();
            //int TSId = obj.GetValue("TSId").Value<int>();
            //int UTsId = obj.GetValue("UTsId").Value<int>();
            //DateTime FromDate = obj.GetValue("FromDate").Value<DateTime>();
            //DateTime ToDate = obj.GetValue("ToDate").Value<DateTime>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@UserId",obj.UserId),
                  new SqlParameter("@ZoneId",obj.ZoneId),
                  new SqlParameter("@CircleId",obj.CircleId),
                  new SqlParameter("@VehicleTypeId",obj.VehicleTypeId),
                  new SqlParameter("@TSId",obj.TSId),
                  new SqlParameter("@UTsId",obj.UTsId),
                  new SqlParameter("@FromDate",obj.FromDate),
                  new SqlParameter("@ToDate",obj.ToDate),
                  new SqlParameter("@VStatus",!string.IsNullOrEmpty(obj.TStatus)?obj.TStatus:"0"),
            };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetMDashboardLst, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetOpMapDashboardLst")]
        public IActionResult GetOpMapDashboardLst(SearchParamInfo obj)
        {
            //string ZoneId = obj.GetValue("ZoneId").Value<string>();
            //string CircleId = obj.GetValue("CircleId").Value<string>();
            //string UserId = obj.GetValue("UserId").Value<string>();
            //string VehicleTypeId = obj.GetValue("VehicleTypeId").Value<string>();
            //int TSId = obj.GetValue("TSId").Value<int>();
            //int UTsId = obj.GetValue("UTsId").Value<int>();
            //DateTime FromDate = obj.GetValue("FromDate").Value<DateTime>();
            //DateTime ToDate = obj.GetValue("ToDate").Value<DateTime>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@UserId",obj.UserId),
                  new SqlParameter("@ZoneId",obj.ZoneId),
                  new SqlParameter("@CircleId",obj.CircleId),
                  new SqlParameter("@VehicleTypeId",obj.VehicleTypeId),
                  new SqlParameter("@TSId",obj.TSId),
                  new SqlParameter("@UTsId",obj.UTsId),
                  new SqlParameter("@FromDate",obj.FromDate),
                  new SqlParameter("@ToDate",obj.ToDate),
                  new SqlParameter("@VStatus",!string.IsNullOrEmpty(obj.TStatus)?obj.TStatus:"0"),
            };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetOpMapDashboardLst, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("ExportDataLogByTS")]
        public FileResult ExportDataLogByTS(JObject obj)
        {
            string ZoneId = obj.GetValue("ZoneId").Value<string>();
            string CircleId = obj.GetValue("CircleId").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            string VehicleTypeId = obj.GetValue("VehicleTypeId").Value<string>();
            int TSId = obj.GetValue("TSId").Value<int>();
            DateTime FromDate = obj.GetValue("FromDate").Value<DateTime>();
            DateTime ToDate = obj.GetValue("ToDate").Value<DateTime>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@UserId",UserId),
                  new SqlParameter("@ZoneId",ZoneId),
                  new SqlParameter("@CircleId",CircleId),
                  new SqlParameter("@VehicleTypeId",VehicleTypeId),
                  new SqlParameter("@TSId",TSId),
                  new SqlParameter("@FromDate",FromDate),
                  new SqlParameter("@ToDate",ToDate),
            };
            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetDataLogByTS, parameters);

            byte[] filearray = null;
            string ContentType = string.Empty;
            DateTime ReportTime = DateTime.Now;
            string Name = "Data Report DatedOn- " + FromDate.ToString("dd-MM-yyyy") + " To- " + ToDate.ToString("dd-MM-yyyy");

            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string filename = Name + ReportTime.ToShortDateString() + ".xlsx";
            filearray = _report.ExportDataLogByTS(Result, Name);

            return File(filearray, ContentType, filename);
        }
        [HttpGet]
        [Route("GetFile")]
        public IActionResult GetFile()
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            string FPtah = baseUrl + "/content/vehicle/logfile.xlsx";
            return Ok(FPtah);
        }
        [HttpPost]
        [Route("rptGetOperationSummary")]
        public IActionResult rptGetOperationSummary(DataTableAjaxPostModel requestModel)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            Order _order = new Order();

            string SearchTXT = requestModel.search.value;
            int draw = requestModel.draw;
            int start = requestModel.start;
            int length = requestModel.length;

            string str = string.Empty;
            if (SearchTXT != null)
                str = SearchTXT.Trim();

            string SortColumn = string.Empty;
            string SortDir = requestModel.order[0].dir;
            switch (requestModel.order[0].column)
            {
                case 1:
                    SortColumn = "DatedOn";
                    break;
                case 2:
                    SortColumn = "OperationType";
                    break;
                case 3:
                    SortColumn = "VehicleNo";
                    break;
                case 4:
                    SortColumn = "ZoneNo";
                    break;
                case 5:
                    SortColumn = "TStationName";
                    break;
                case 6:
                    SortColumn = "OwnerType";
                    break;
                case 7:
                    SortColumn = "VehicleType";
                    break;
                case 8:
                    SortColumn = "ContainerCode";
                    break;

                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",str),
                  new SqlParameter("@SortColumn",SortColumn),
                  new SqlParameter("@SortOrder",SortDir),
                  new SqlParameter("@PageNumber",start),
                  new SqlParameter("@PageSize",length),
                  new SqlParameter("@UserId",requestModel.UserId),
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(requestModel.VehicleTypeId)?requestModel.VehicleTypeId:"0"),
                  new SqlParameter("@TsId",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"0"),
                  new SqlParameter("@UTsId",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"0"),
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@ToDate",requestModel.ToDate),
             };
            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.rptGetOperationSummary_Paging, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetPendingContainerforHKL")]
        public IActionResult GetPendingContainerforHKL(DataTableAjaxPostModel requestModel)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            Order _order = new Order();

            string SearchTXT = requestModel.search.value;
            int draw = requestModel.draw;
            int start = requestModel.start;
            int length = requestModel.length;

            string str = string.Empty;
            if (SearchTXT != null)
                str = SearchTXT.Trim();

            string SortColumn = string.Empty;
            string SortDir = requestModel.order[0].dir;
            switch (requestModel.order[0].column)
            {
                case 1:
                    SortColumn = "CreatedDate";
                    break;
                case 2:
                    SortColumn = "ContainerCode";
                    break;
                case 3:
                    SortColumn = "ZoneNo";
                    break;
                case 4:
                    SortColumn = "TStationName";
                    break;


                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",str),
                  new SqlParameter("@SortColumn",SortColumn),
                  new SqlParameter("@SortOrder",SortDir),
                  new SqlParameter("@PageNumber",start),
                  new SqlParameter("@PageSize",length),
                  new SqlParameter("@UserId",requestModel.UserId),
                  //new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@TsId",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"0"),
                  new SqlParameter("@UTsId",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"0"),
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@ToDate",requestModel.ToDate),
             };
            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllContPendingForHKL, parameters);

            return Ok(Result);
        }


        [HttpPost]
        [Route("GetrptContainerforHKL")]
        public IActionResult GetrptContainerforHKL(DataTableAjaxPostModel requestModel)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            Order _order = new Order();

            string SearchTXT = requestModel.search.value;
            int draw = requestModel.draw;
            int start = requestModel.start;
            int length = requestModel.length;

            string str = string.Empty;
            if (SearchTXT != null)
                str = SearchTXT.Trim();

            string SortColumn = string.Empty;
            string SortDir = requestModel.order[0].dir;
            switch (requestModel.order[0].column)
            {
                case 1:
                    SortColumn = "VehicleNo";
                    break;
                case 2:
                    SortColumn = "ContainerCode";
                    break;
                case 3:
                    SortColumn = "TStationName";
                    break;
                case 4:
                    SortColumn = "ZoneNo";
                    break;
                case 5:
                    SortColumn = "CreatedDate";
                    break;


                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",str),
                  new SqlParameter("@SortColumn",SortColumn),
                  new SqlParameter("@SortOrder",SortDir),
                  new SqlParameter("@PageNumber",start),
                  new SqlParameter("@PageSize",length),
                  new SqlParameter("@UserId",requestModel.UserId),
                  //new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@TsId",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"0"),
                  new SqlParameter("@UTsId",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"0"),
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@ToDate",requestModel.ToDate),
             };
            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.sprptGetAllLHKL, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllTotalVehicle")]
        public IActionResult GetAllTotalVehicle(DataTableAjaxPostModel requestModel)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            Order _order = new Order();

            string SearchTXT = requestModel.search.value;
            int draw = requestModel.draw;
            int start = requestModel.start;
            int length = requestModel.length;

            string str = string.Empty;
            if (SearchTXT != null)
                str = SearchTXT.Trim();

            string SortColumn = string.Empty;
            string SortDir = requestModel.order[0].dir;
            switch (requestModel.order[0].column)
            {
                case 1:
                    SortColumn = "VehicleType";
                    break;
                case 2:
                    SortColumn = "TotalCount";
                    break;
                case 3:
                    SortColumn = "TotalArrivalCount";
                    break;
                case 4:
                    SortColumn = "TotalTripCount";
                    break;



                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            SqlParameter[] parameters = new SqlParameter[]
             {
                  //new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@TsId",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"0"),
                   new SqlParameter("@LoginId",requestModel.UserId),

                  //new SqlParameter("@UTsId",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"0"),
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@ToDate",requestModel.ToDate),
             };
            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.sp_GetRptVehicleInfo, parameters);

            return Ok(Result);
        }



        [HttpPost]
        [Route("GetExportDataLogByTS")]
        public IActionResult GetExportDataLogByTS(JObject obj)
        {
            string ZoneId = obj.GetValue("ZoneId").Value<string>();
            string CircleId = obj.GetValue("CircleId").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            string VehicleTypeId = obj.GetValue("VehicleTypeId").Value<string>();
            int TSId = obj.GetValue("TSId").Value<int>();
            DateTime FromDate = obj.GetValue("FromDate").Value<DateTime>();
            DateTime ToDate = obj.GetValue("ToDate").Value<DateTime>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@UserId",UserId),
                  new SqlParameter("@ZoneId",ZoneId),
                  new SqlParameter("@CircleId",CircleId),
                  new SqlParameter("@VehicleTypeId",VehicleTypeId),
                  new SqlParameter("@TSId",TSId),
                  new SqlParameter("@FromDate",FromDate),
                  new SqlParameter("@ToDate",ToDate),
            };
            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetDataLogByTS, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetValidateHLTSId")]
        public IActionResult GetValidateHLTSId(JObject obj)
        {
            string UId = obj.GetValue("UId").Value<string>();
            string UIdType = obj.GetValue("UIdType").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            int TSId = obj.GetValue("TSId").Value<int>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@UId",UId),
                  new SqlParameter("@UIdType",UIdType),
                  new SqlParameter("@TSId",TSId),
            };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetValidateHLUId, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllDHLCInfo")]
        public IActionResult GetAllDHLCInfo(JObject obj)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            string PageNumber = obj.GetValue("PageNumber").Value<string>();
            string PageSize = obj.GetValue("PageSize").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            string SearchTerm = obj.GetValue("SearchTerm").Value<string>();
            string UIDType = obj.GetValue("UIDType").Value<string>();
            int TSId = obj.GetValue("TSId").Value<int>();
            DateTime FromDate = obj.GetValue("FromDate").Value<DateTime>();
            DateTime ToDate = obj.GetValue("ToDate").Value<DateTime>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",SearchTerm),
                  new SqlParameter("@SortColumn",""),
                  new SqlParameter("@SortOrder",""),
                  new SqlParameter("@PageNumber",PageNumber),
                  new SqlParameter("@PageSize",PageSize),
                  new SqlParameter("@UserId",UserId),
                  new SqlParameter("@FromDate",FromDate),
                  new SqlParameter("@ToDate",ToDate),
                  new SqlParameter("@TSId",TSId),
                  new SqlParameter("@UIDType",UIDType),
                  new SqlParameter("@UTSId","0"),
                  new SqlParameter("@IsCompleted","-1"),
                  new SqlParameter("@Status","0"),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllDHLCInfo_Paging, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllActiveDHLCInfo")]
        public IActionResult GetAllActiveDHLCInfo(JObject obj)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            string PageNumber = obj.GetValue("PageNumber").Value<string>();
            string PageSize = obj.GetValue("PageSize").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            string SearchTerm = obj.GetValue("SearchTerm").Value<string>();
            string UIDType = obj.GetValue("UIDType").Value<string>();
            int TSId = obj.GetValue("TSId").Value<int>();
            int UTSId = obj.GetValue("UTSId").Value<int>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",SearchTerm),
                  new SqlParameter("@SortColumn",""),
                  new SqlParameter("@SortOrder",""),
                  new SqlParameter("@PageNumber",PageNumber),
                  new SqlParameter("@PageSize",PageSize),
                  new SqlParameter("@UserId",UserId),
                  new SqlParameter("@TSId",TSId),
                  new SqlParameter("@UTSId",UTSId),
                  new SqlParameter("@UIDType",UIDType),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllActiveDHLCInfo, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllPendingOperation")]
        public IActionResult GetAllPendingOperation(DataTableAjaxPostModel requestModel)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            Order _order = new Order();

            string SearchTXT = requestModel.search.value;
            int draw = requestModel.draw;
            int start = requestModel.start;
            int length = requestModel.length;

            string str = string.Empty;
            if (SearchTXT != null)
                str = SearchTXT.Trim();

            string SortColumn = string.Empty;
            string SortDir = requestModel.order[0].dir;
            switch (requestModel.order[0].column)
            {
                case 1:
                    SortColumn = "OperationType";
                    break;
                case 2:
                    SortColumn = "AssetType";
                    break;
                case 3:
                    SortColumn = "EnType";
                    break;
                case 4:
                    SortColumn = "UId";
                    break;
                case 5:
                    SortColumn = "TStationName";
                    break;
                case 6:
                    SortColumn = "CreatedOn";
                    break;
                case 7:
                    SortColumn = "CreatedBy";
                    break;


                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",str),
                  new SqlParameter("@SortColumn",SortColumn),
                  new SqlParameter("@SortOrder",SortDir),
                  new SqlParameter("@PageNumber",start),
                  new SqlParameter("@PageSize",length),
                  new SqlParameter("@UserId",requestModel.UserId),
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@Todate",requestModel.ToDate),
                  new SqlParameter("@TSId",requestModel.Shift),
                  new SqlParameter("@UTSId",requestModel.Status),
                  new SqlParameter("@Status",requestModel.NotiId),
                  new SqlParameter("@UUserId",requestModel.VehicleTypeId),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllPendingOperation, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllOperation1Notification")]
        public IActionResult GetAllOperation1Notification(JObject obj)
        {
            string LoginId = obj.GetValue("LoginId").Value<string>();
            int TSId = obj.GetValue("TSId").Value<int>();
            int UTSId = obj.GetValue("UTSId").Value<int>();
            DateTime TDate = CommonHelper.IndianStandard(DateTime.UtcNow);
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@UserId",LoginId),
                  new SqlParameter("@TSId",TSId),
                  new SqlParameter("@UTSId",UTSId),
                  new SqlParameter("@TDate",TDate)
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetAllOperation1Noti, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllScannedVehicleForBarChart")]
        public IActionResult GetAllScannedVehicleForBarChart(JObject obj)
        {
            string LoginId = obj.GetValue("LoginId").Value<string>();
            string SearchType = obj.GetValue("SearchType").Value<string>();
            int TSId = obj.GetValue("TSId").Value<int>();
            int UTSId = obj.GetValue("UTSId").Value<int>();
            int OperationTypeId = obj.GetValue("OperationTypeId").Value<int>();
            DateTime TDate = CommonHelper.IndianStandard(DateTime.UtcNow);
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@UserId",LoginId),
                  new SqlParameter("@TSId",TSId),
                  new SqlParameter("@UTSId",UTSId),
                  new SqlParameter("@TDate",TDate),
                  new SqlParameter("@SearchType",SearchType),
                  new SqlParameter("@OperationTypeId",OperationTypeId),
              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllScannedVehicleForBarChart, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllVehicleOpt1Noti")]
        public IActionResult GetAllVehicleOpt1Noti(DataTableAjaxPostModel requestModel)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            Order _order = new Order();

            string SearchTXT = requestModel.search.value;
            int draw = requestModel.draw;
            int start = requestModel.start;
            int length = requestModel.length;

            string str = string.Empty;
            if (SearchTXT != null)
                str = SearchTXT.Trim();

            string SortColumn = string.Empty;
            string SortDir = requestModel.order[0].dir;
            switch (requestModel.order[0].column)
            {
                case 2:
                    SortColumn = "OperationType";
                    break;
                case 3:
                    SortColumn = "ContainerCode";
                    break;
                case 4:
                    SortColumn = "ContainerName";
                    break;
                case 5:
                    SortColumn = "Step1UId";
                    break;
                case 6:
                    SortColumn = "VehicleType";
                    break;
                case 7:
                    SortColumn = "VehicleNo";
                    break;
                case 8:
                    SortColumn = "Step2UId";
                    break;
                case 9:
                    SortColumn = "TStationName";
                    break;
                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",str),
                  new SqlParameter("@SortColumn",SortColumn),
                  new SqlParameter("@SortOrder",SortDir),
                  new SqlParameter("@PageNumber",start),
                  new SqlParameter("@PageSize",length),
                  new SqlParameter("@LoginId",requestModel.UserId),
                  new SqlParameter("@OperationTypeId",requestModel.NotiId),
                  new SqlParameter("@FPath",baseUrl),
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@ToDate",requestModel.ToDate),
                  new SqlParameter("@VehicleTypeId",requestModel.VehicleTypeId),
                  new SqlParameter("@TSId",requestModel.Shift),
                  new SqlParameter("@UTSId",requestModel.Status),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllOpt1VehicleNoti, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllVehicleOpt1NotiB64")]
        public IActionResult GetAllVehicleOpt1NotiB64(DataTableAjaxPostModel requestModel)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            Order _order = new Order();

            string SearchTXT = requestModel.search.value;
            int draw = requestModel.draw;
            int start = requestModel.start;
            int length = requestModel.length;

            string str = string.Empty;
            if (SearchTXT != null)
                str = SearchTXT.Trim();

            string SortColumn = string.Empty;
            string SortDir = requestModel.order[0].dir;
            switch (requestModel.order[0].column)
            {
                case 2:
                    SortColumn = "OperationType";
                    break;
                case 3:
                    SortColumn = "ContainerCode";
                    break;
                case 4:
                    SortColumn = "ContainerName";
                    break;
                case 5:
                    SortColumn = "Step1UId";
                    break;
                case 6:
                    SortColumn = "VehicleType";
                    break;
                case 7:
                    SortColumn = "VehicleNo";
                    break;
                case 8:
                    SortColumn = "Step2UId";
                    break;
                case 9:
                    SortColumn = "TStationName";
                    break;
                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",str),
                  new SqlParameter("@SortColumn",SortColumn),
                  new SqlParameter("@SortOrder",SortDir),
                  new SqlParameter("@PageNumber",start),
                  new SqlParameter("@PageSize",length),
                  new SqlParameter("@LoginId",requestModel.UserId),
                  new SqlParameter("@OperationTypeId",requestModel.NotiId),
                  new SqlParameter("@FPath",baseUrl),
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@ToDate",requestModel.ToDate),
                  new SqlParameter("@VehicleTypeId",requestModel.VehicleTypeId),
                  new SqlParameter("@TSId",requestModel.Shift),
                  new SqlParameter("@UTSId",requestModel.Status),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllOpt1VehicleNoti, parameters);

            List<VehicleInfo> Vhlst = JsonConvert.DeserializeObject<List<VehicleInfo>>(Result);
            string strJson = JsonConvert.SerializeObject(Vhlst);
            return Ok(strJson);
        }


        [HttpPost]
        [Route("GetAllArvlOfEntityOpt1Noti")]
        public IActionResult GetAllArvlOfEntityOpt1Noti(DataTableAjaxPostModel requestModel)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            Order _order = new Order();

            string SearchTXT = requestModel.search.value;
            int draw = requestModel.draw;
            int start = requestModel.start;
            int length = requestModel.length;

            string str = string.Empty;
            if (SearchTXT != null)
                str = SearchTXT.Trim();

            string SortColumn = string.Empty;
            string SortDir = requestModel.order[0].dir;
            switch (requestModel.order[0].column)
            {
                case 1:
                    SortColumn = "UId";
                    break;
                case 2:
                    SortColumn = "EntityType";
                    break;
                case 3:
                    SortColumn = "EntityCode";
                    break;
                case 4:
                    SortColumn = "TStationName";
                    break;
                case 5:
                    SortColumn = "Status";
                    break;
                case 6:
                    SortColumn = "DeviationStatus";
                    break;
                case 7:
                    SortColumn = "CreatedOn";
                    break;

                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",str),
                  new SqlParameter("@SortColumn",SortColumn),
                  new SqlParameter("@SortOrder",SortDir),
                  new SqlParameter("@PageNumber",start),
                  new SqlParameter("@PageSize",length),
                  new SqlParameter("@UserId",requestModel.UserId),
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@ToDate",requestModel.ToDate),
                  new SqlParameter("@TSId",requestModel.Shift),
                  new SqlParameter("@UIDType",requestModel.NotiId),
                  new SqlParameter("@UTSId",requestModel.Status),
                   new SqlParameter("@IsCompleted",requestModel.Route),
                   new SqlParameter("@Status",requestModel.VehicleTypeId),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllDHLCOpt1Info_Paging, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllOpt1ContainerNoti")]
        public IActionResult GetAllOpt1ContainerNoti(DataTableAjaxPostModel requestModel)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            Order _order = new Order();

            string SearchTXT = requestModel.search.value;
            int draw = requestModel.draw;
            int start = requestModel.start;
            int length = requestModel.length;

            string str = string.Empty;
            if (SearchTXT != null)
                str = SearchTXT.Trim();

            string SortColumn = string.Empty;
            string SortDir = requestModel.order[0].dir;
            switch (requestModel.order[0].column)
            {
                case 2:
                    SortColumn = "OperationType";
                    break;
                case 3:
                    SortColumn = "ContainerCode";
                    break;
                case 4:
                    SortColumn = "TStationName";
                    break;
                case 5:
                    SortColumn = "Step1UId";
                    break;
                case 6:
                    SortColumn = "CreatedOn";
                    break;
                case 7:
                    SortColumn = "LastActivityOn";
                    break;
                case 8:
                    SortColumn = "Status";
                    break;

                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",str),
                  new SqlParameter("@SortColumn",SortColumn),
                  new SqlParameter("@SortOrder",SortDir),
                  new SqlParameter("@PageNumber",start),
                  new SqlParameter("@PageSize",length),
                  new SqlParameter("@LoginId",requestModel.UserId),
                  new SqlParameter("@OperationTypeId",requestModel.NotiId),
                  new SqlParameter("@FPath",baseUrl),
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@ToDate",requestModel.ToDate),
                  new SqlParameter("@TSId",requestModel.Shift),
                  new SqlParameter("@UTSId",requestModel.Status),
                  new SqlParameter("@Status",requestModel.Route),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllOpt1ContainerNoti, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllOpt1HKLNoti")]
        public IActionResult GetAllOpt1HKLNoti(DataTableAjaxPostModel requestModel)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            Order _order = new Order();

            string SearchTXT = requestModel.search.value;
            int draw = requestModel.draw;
            int start = requestModel.start;
            int length = requestModel.length;

            string str = string.Empty;
            if (SearchTXT != null)
                str = SearchTXT.Trim();

            string SortColumn = string.Empty;
            string SortDir = requestModel.order[0].dir;
            switch (requestModel.order[0].column)
            {
                case 2:
                    SortColumn = "OperationType";
                    break;
                case 3:
                    SortColumn = "VehicleType";
                    break;
                case 4:
                    SortColumn = "VehicleNo";
                    break;
                case 5:
                    SortColumn = "Step1UId";
                    break;
                case 6:
                    SortColumn = "ContainerCode";
                    break;
                case 7:
                    SortColumn = "ContainerName";
                    break;
                case 8:
                    SortColumn = "Step2UId";
                    break;

                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",str),
                  new SqlParameter("@SortColumn",SortColumn),
                  new SqlParameter("@SortOrder",SortDir),
                  new SqlParameter("@PageNumber",start),
                  new SqlParameter("@PageSize",length),
                  new SqlParameter("@LoginId",requestModel.UserId),
                  new SqlParameter("@OperationTypeId",requestModel.NotiId),
                  new SqlParameter("@FPath",baseUrl),
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@ToDate",requestModel.ToDate),

                  new SqlParameter("@TSId",requestModel.Shift),
                  new SqlParameter("@UTSId",requestModel.Status),
                  new SqlParameter("@Status",requestModel.VehicleTypeId),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllOpt1HKLNoti, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllOpt1HKLNotiB64")]
        public IActionResult GetAllOpt1HKLNotiB64(DataTableAjaxPostModel requestModel)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            Order _order = new Order();

            string SearchTXT = requestModel.search.value;
            int draw = requestModel.draw;
            int start = requestModel.start;
            int length = requestModel.length;

            string str = string.Empty;
            if (SearchTXT != null)
                str = SearchTXT.Trim();

            string SortColumn = string.Empty;
            string SortDir = requestModel.order[0].dir;
            switch (requestModel.order[0].column)
            {
                case 2:
                    SortColumn = "OperationType";
                    break;
                case 3:
                    SortColumn = "VehicleType";
                    break;
                case 4:
                    SortColumn = "VehicleNo";
                    break;
                case 5:
                    SortColumn = "Step1UId";
                    break;
                case 6:
                    SortColumn = "ContainerCode";
                    break;
                case 7:
                    SortColumn = "ContainerName";
                    break;
                case 8:
                    SortColumn = "Step2UId";
                    break;

                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",str),
                  new SqlParameter("@SortColumn",SortColumn),
                  new SqlParameter("@SortOrder",SortDir),
                  new SqlParameter("@PageNumber",start),
                  new SqlParameter("@PageSize",length),
                  new SqlParameter("@LoginId",requestModel.UserId),
                  new SqlParameter("@OperationTypeId",requestModel.NotiId),
                  new SqlParameter("@FPath",baseUrl),
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@ToDate",requestModel.ToDate),

                  new SqlParameter("@TSId",requestModel.Shift),
                  new SqlParameter("@UTSId",requestModel.Status),
                  new SqlParameter("@Status",requestModel.VehicleTypeId),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllOpt1HKLNoti, parameters);


            List<VehicleInfoB64> Vhlst = JsonConvert.DeserializeObject<List<VehicleInfoB64>>(Result);
            string strJson = JsonConvert.SerializeObject(Vhlst);
            return Ok(strJson);
            // return Ok(Result);
        }


        [HttpPost]
        [Route("GetAllOpt1RCVNoti")]
        public IActionResult GetAllOpt1RCVNoti(DataTableAjaxPostModel requestModel)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            Order _order = new Order();

            string SearchTXT = requestModel.search.value;
            int draw = requestModel.draw;
            int start = requestModel.start;
            int length = requestModel.length;

            string str = string.Empty;
            if (SearchTXT != null)
                str = SearchTXT.Trim();

            string SortColumn = string.Empty;
            string SortDir = requestModel.order[0].dir;
            switch (requestModel.order[0].column)
            {
                case 2:
                    SortColumn = "OperationType";
                    break;
                case 3:
                    SortColumn = "ContainerCode";
                    break;
                case 4:
                    SortColumn = "TStationName";
                    break;
                case 5:
                    SortColumn = "Step1UId";
                    break;
                case 6:
                    SortColumn = "CreatedOn";
                    break;
                case 7:
                    SortColumn = "LastActivityOn";
                    break;
                case 8:
                    SortColumn = "Status";
                    break;

                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",str),
                  new SqlParameter("@SortColumn",SortColumn),
                  new SqlParameter("@SortOrder",SortDir),
                  new SqlParameter("@PageNumber",start),
                  new SqlParameter("@PageSize",length),
                  new SqlParameter("@LoginId",requestModel.UserId),
                  new SqlParameter("@OperationTypeId",requestModel.NotiId),
                  new SqlParameter("@FPath",baseUrl),
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@ToDate",requestModel.ToDate),
                  new SqlParameter("@TSId",requestModel.Shift),
                  new SqlParameter("@UTSId",requestModel.Status),
                  new SqlParameter("@Status",requestModel.Route),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllOpt1RCVNoti, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllOpt1UNQContainerNoti")]
        public IActionResult GetAllOpt1UNQContainerNoti(DataTableAjaxPostModel requestModel)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            Order _order = new Order();

            string SearchTXT = requestModel.search.value;
            int draw = requestModel.draw;
            int start = requestModel.start;
            int length = requestModel.length;

            string str = string.Empty;
            if (SearchTXT != null)
                str = SearchTXT.Trim();

            string SortColumn = string.Empty;
            string SortDir = requestModel.order[0].dir;
            switch (requestModel.order[0].column)
            {
                case 1:
                    SortColumn = "ContainerCode";
                    break;
                case 2:
                    SortColumn = "ContainerName";
                    break;
                case 3:
                    SortColumn = "UId";
                    break;
                case 4:
                    SortColumn = "ContainerType";
                    break;


                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",str),
                  new SqlParameter("@SortColumn",SortColumn),
                  new SqlParameter("@SortOrder",SortDir),
                  new SqlParameter("@PageNumber",start),
                  new SqlParameter("@PageSize",length),
                  new SqlParameter("@LoginId",requestModel.UserId),
                  new SqlParameter("@TSId",requestModel.Shift),
                  new SqlParameter("@UTSId",requestModel.Status),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllOpt1UNQContainerNoti, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllOpt1UNQHKLNoti")]
        public IActionResult GetAllOpt1UNQHKLNoti(DataTableAjaxPostModel requestModel)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            Order _order = new Order();

            string SearchTXT = requestModel.search.value;
            int draw = requestModel.draw;
            int start = requestModel.start;
            int length = requestModel.length;

            string str = string.Empty;
            if (SearchTXT != null)
                str = SearchTXT.Trim();

            string SortColumn = string.Empty;
            string SortDir = requestModel.order[0].dir;
            switch (requestModel.order[0].column)
            {
                case 1:
                    SortColumn = "VehicleNo";
                    break;
                case 2:
                    SortColumn = "VehicleType";
                    break;
                case 3:
                    SortColumn = "UId";
                    break;
                case 4:
                    SortColumn = "CircleName";
                    break;
                case 5:
                    SortColumn = "ZoneNo";
                    break;
                case 6:
                    SortColumn = "WardNo";
                    break;

                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",str),
                  new SqlParameter("@SortColumn",SortColumn),
                  new SqlParameter("@SortOrder",SortDir),
                  new SqlParameter("@PageNumber",start),
                  new SqlParameter("@PageSize",length),
                  new SqlParameter("@LoginId",requestModel.UserId),
                  new SqlParameter("@TSId",requestModel.Shift),
                  new SqlParameter("@UTSId",requestModel.Status),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllOpt1UNQHKLNoti, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllForceTransaction_Paging")]
        public IActionResult GetAllForceTransaction_Paging(DataTableAjaxPostModel requestModel)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            Order _order = new Order();

            string SearchTXT = requestModel.search.value;
            int draw = requestModel.draw;
            int start = requestModel.start;
            int length = requestModel.length;

            string str = string.Empty;
            if (SearchTXT != null)
                str = SearchTXT.Trim();

            string SortColumn = string.Empty;
            string SortDir = requestModel.order[0].dir;
            switch (requestModel.order[0].column)
            {
                case 1:
                    SortColumn = "OperationType";
                    break;
                case 2:
                    SortColumn = "AssetType";
                    break;
                case 3:
                    SortColumn = "EnType";
                    break;
                case 4:
                    SortColumn = "UId";
                    break;
                case 5:
                    SortColumn = "TStationName";
                    break;
                case 6:
                    SortColumn = "CreatedOn";
                    break;
                case 7:
                    SortColumn = "CreatedBy";
                    break;


                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",str),
                  new SqlParameter("@SortColumn",SortColumn),
                  new SqlParameter("@SortOrder",SortDir),
                  new SqlParameter("@PageNumber",start),
                  new SqlParameter("@PageSize",length),
                  new SqlParameter("@UserId",requestModel.UserId),
                  new SqlParameter("@TSId",requestModel.Shift),
                  new SqlParameter("@TDate",CommonHelper.IndianStandard(DateTime.UtcNow)),
                  new SqlParameter("@UTSId",requestModel.Status),
                  new SqlParameter("@TId",requestModel.NotiId),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllForceTransaction_Paging, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllPendingContainer")]
        public IActionResult GetAllPendingContainer(DataTableAjaxPostModel requestModel)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            Order _order = new Order();

            string SearchTXT = requestModel.search.value;
            int draw = requestModel.draw;
            int start = requestModel.start;
            int length = requestModel.length;

            string str = string.Empty;
            if (SearchTXT != null)
                str = SearchTXT.Trim();

            string SortColumn = string.Empty;
            string SortDir = requestModel.order[0].dir;
            switch (requestModel.order[0].column)
            {
                case 1:
                    SortColumn = "CreatedDate";
                    break;
                case 2:
                    SortColumn = "ContainerCode";
                    break;
                case 3:
                    SortColumn = "ZoneNo";
                    break;
                case 4:
                    SortColumn = "TStationName";
                    break;


                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",str),
                  new SqlParameter("@SortColumn",SortColumn),
                  new SqlParameter("@SortOrder",SortDir),
                  new SqlParameter("@PageNumber",start),
                  new SqlParameter("@PageSize",length),
                  new SqlParameter("@UserId",requestModel.UserId),
                  //new SqlParameter("@TSId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  //new SqlParameter("@UTSId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  //new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(requestModel.VehicleTypeId)?requestModel.VehicleTypeId:"0"),
                  new SqlParameter("@TsId",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"0"),
                  new SqlParameter("@UTsId",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"0"),
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@ToDate",requestModel.ToDate),
             };
            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllContPendingForHKL, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllRptContainerForHkl")]
        public IActionResult GetAllRptContainerForHkl(DataTableAjaxPostModel requestModel)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            Order _order = new Order();

            string SearchTXT = requestModel.search.value;
            int draw = requestModel.draw;
            int start = requestModel.start;
            int length = requestModel.length;

            string str = string.Empty;
            if (SearchTXT != null)
                str = SearchTXT.Trim();

            string SortColumn = string.Empty;
            string SortDir = requestModel.order[0].dir;
            switch (requestModel.order[0].column)
            {
                case 1:
                    SortColumn = "Step2UId";
                    break;
                case 2:
                    SortColumn = "ContainerCode";
                    break;
                case 3:
                    SortColumn = "TStationName";
                    break;
                case 4:
                    SortColumn = "ZoneNo";
                    break;
                case 5:
                    SortColumn = "CreatedDate";
                    break;


                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",str),
                  new SqlParameter("@SortColumn",SortColumn),
                  new SqlParameter("@SortOrder",SortDir),
                  new SqlParameter("@PageNumber",start),
                  new SqlParameter("@PageSize",length),
                  new SqlParameter("@UserId",requestModel.UserId),
                  //new SqlParameter("@TSId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  //new SqlParameter("@UTSId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  //new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(requestModel.VehicleTypeId)?requestModel.VehicleTypeId:"0"),
                  new SqlParameter("@TsId",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"0"),
                  new SqlParameter("@UTsId",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"0"),
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@ToDate",requestModel.ToDate),
             };
            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.sprptGetAllLHKL, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllRptVehicle")]
        public IActionResult GetAllRptVehicle(DataTableAjaxPostModel requestModel)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            Order _order = new Order();

            string SearchTXT = requestModel.search.value;
            int draw = requestModel.draw;
            int start = requestModel.start;
            int length = requestModel.length;

            string str = string.Empty;
            if (SearchTXT != null)
                str = SearchTXT.Trim();

            string SortColumn = string.Empty;
            string SortDir = requestModel.order[0].dir;
            switch (requestModel.order[0].column)
            {
                case 1:
                    SortColumn = "VehicleType";
                    break;
                case 2:
                    SortColumn = "TotalCount";
                    break;
                case 3:
                    SortColumn = "TotalArrivalCount";
                    break;
                case 4:
                    SortColumn = "TotalTripCount";
                    break;
                //case 5:
                //    SortColumn = "CreatedDate";
                //    break;


                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            SqlParameter[] parameters = new SqlParameter[]
             {
                  //new SqlParameter("@SearchTerm",str),
                  //new SqlParameter("@SortColumn",SortColumn),
                  //new SqlParameter("@SortOrder",SortDir),
                  //new SqlParameter("@PageNumber",start),
                  //new SqlParameter("@PageSize",length),
                  //new SqlParameter("@UserId",requestModel.UserId),
                  //new SqlParameter("@TSId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  //new SqlParameter("@UTSId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  //new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(requestModel.VehicleTypeId)?requestModel.VehicleTypeId:"0"),
                  new SqlParameter("@TsId",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"0"),
                  new SqlParameter("@LoginId",!string.IsNullOrEmpty(requestModel.UserId)?requestModel.UserId:"0"),
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@ToDate",requestModel.ToDate),
             };
            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.sp_GetRptVehicleInfo, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllDeployLocation_Paging")]
        public IActionResult GetAllDeployLocation_Paging(DataTableAjaxPostModel requestModel)
        {

            Order _order = new Order();

            string SearchTXT = requestModel.search.value;
            int draw = requestModel.draw;
            int start = requestModel.start;
            int length = requestModel.length;

            string str = string.Empty;
            if (SearchTXT != null)
                str = SearchTXT.Trim();

            string SortColumn = string.Empty;
            string SortDir = requestModel.order[0].dir;

            switch (requestModel.order[0].column)
            {

                case 1:
                    SortColumn = "A.LandMark";
                    break;


                case 2:
                    SortColumn = "C.ZoneNo";
                    break;
                case 3:
                    SortColumn = "D.CircleName";
                    break;
                case 4:
                    SortColumn = "E.WardNo";
                    break;

                case 5:
                    SortColumn = "A.Radius";
                    break;
                case 6:
                    SortColumn = "A.CreatedBy";
                    break;
                case 7:
                    SortColumn = "A.ModifiedDate";
                    break;

                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;

            }
            str = str.Replace("\"", "");
            str = str.Replace("'", "");
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",str),
                  new SqlParameter("@SortColumn",SortColumn),
                  new SqlParameter("@SortOrder",SortDir),
                  new SqlParameter("@PageNumber",start),
                  new SqlParameter("@PageSize",length),
                  new SqlParameter("@LoginId",requestModel.UserId),
                   new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                   new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                   new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),

                   new SqlParameter("@StatusId",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"-1"),

             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllDeployLocation_Paging, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("AddDeployLocation")]
        public IActionResult AddDeployLocation(JObject obj)
        {


            int DLId = obj.GetValue("DLId").Value<int>();
            // string RouteName = obj.GetValue("RouteName").Value<string>();
            string DeployLocation = obj.GetValue("DeployLocation").Value<string>();
            string Radius = obj.GetValue("Radius").Value<string>();
            string Lat = obj.GetValue("Lat").Value<string>();
            string Lng = obj.GetValue("Lng").Value<string>();
            string IsActive = obj.GetValue("IsActive").Value<string>();
            string CreatedBy = obj.GetValue("CreatedBy").Value<string>();
            string ZoneId = obj.GetValue("ZoneId").Value<string>();
            string CircleId = obj.GetValue("CircleId").Value<string>();
            string WardId = obj.GetValue("WardId").Value<string>();

            //int dlid = 0;
            //if(DLId==0)
            //{
            //    dlid = 0;
            //}
            //else
            //{
            //    dlid = DLId;
            //}
            SqlParameter[] parameters = new SqlParameter[]
                 {
                  new SqlParameter("@DLId",DLId),
                  new SqlParameter("@LandMark",DeployLocation),

                  new SqlParameter("@Lat",Lat),
                  new SqlParameter("@Lng",Lng),

                 new SqlParameter("@Radius",Radius),
                 new SqlParameter("@IsActive",IsActive),
                  new SqlParameter("@CreatedDate",CommonHelper.IndianStandard(DateTime.UtcNow)),
                  new SqlParameter("@CreatedBy",CreatedBy),

                  new SqlParameter("@ZoneId",ZoneId),
                  new SqlParameter("@CircleId",CircleId),
                  new SqlParameter("@WardId",WardId),
                 };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spAddAndUpdateDeployLocation, parameters);

            return Ok(Result);



        }
        [HttpGet]
        [Route("GetDeployLocationInfoById")]
        public IActionResult GetDeployLocationInfoById(int DLId)
        {


            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@DLId",DLId)

              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetDeployLocationInfoById, parameters);
            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllDeployIncharge")]
        public IActionResult GetAllDeployIncharge(DataTableAjaxPostModel requestModel)
        {
            Order _order = new Order();
            string SearchTXT = requestModel.search.value;
            int draw = requestModel.draw;
            int start = requestModel.start;
            int length = requestModel.length;
            string str = string.Empty;
            if (SearchTXT != null)
                str = SearchTXT.Trim();

            string SortColumn = string.Empty;
            string SortDir = requestModel.order[0].dir;
            switch (requestModel.order[0].column)
            {

                case 1:
                    SortColumn = "Name";
                    break;
                case 2:
                    SortColumn = "MobileNo";
                    break;
                case 3:
                    SortColumn = "LoginId";
                    break;


                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@SearchTerm",str),
                  new SqlParameter("@SortColumn",SortColumn),
                  new SqlParameter("@SortOrder",SortDir),
                  new SqlParameter("@PageNumber",start),
                  new SqlParameter("@PageSize",length),
                  new SqlParameter("@LoginId",requestModel.UserId),


            };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllDeployIncharge_Paging, parameters);
            return Ok(Result);

        }
        [HttpPost]
        [Route("GetAllDeployLocationAndWardMaster")]
        public IActionResult GetAllDeployLocationAndWardMaster(JObject obj)
        {
            object[] mparameters1 = { obj.GetValue("Ccode").Value<string>(), };
            //object[] mparameters = { obj.GetValue("LoginId").Value<string>() };
            string loginid = "";
            if (obj.GetValue("LoginId").Value<string>() == null)
            {
                loginid = "";
            }
            else
            {
                loginid = obj.GetValue("LoginId").Value<string>();
            }
            SqlParameter[] mparameters = new SqlParameter[]
                  {
                    new SqlParameter("@LoginId",loginid)
                  };
            // string _lst = _dataRepository.ExcuteRowSqlCommand(StoredProcedureHelper.spGetALLCircleMaster, mparameters1);
            string _lst = _dataRepository.ExcuteRowSqlCommand(StoredProcedureHelper.spGetAllUZone, mparameters1);
            string _lstSubMenu = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllDeployLocationMaster, mparameters);
            var response = new { data1 = _lst, data2 = _lstSubMenu };
            return Ok(response);
        }
        [HttpPost]
        [Route("SaveandupdateDeployIncharge")]
        public IActionResult SaveandupdateDeployIncharge(JObject obj)
        {
            string JArrayval = obj.GetValue("JArrayval").Value<string>();

            string UserId = obj.GetValue("UserId").Value<string>();
            bool IsActive = Convert.ToBoolean(obj.GetValue("IsActive").Value<string>());
            string FullName = obj.GetValue("Name").Value<string>();
            string EmailId = obj.GetValue("LoginId").Value<string>();
            string Pwd = obj.GetValue("Pwd").Value<string>();
            string Mobile = obj.GetValue("ContactNo").Value<string>();

            string CreatedBy = obj.GetValue("CreatedBy").Value<string>();

            var dattable = CommonHelper.toDataTable(JArrayval);
            SqlParameter[] mparameters = new SqlParameter[]
                {
                    new SqlParameter("@DIId",UserId),
                    new SqlParameter("@Name",FullName),
                    new SqlParameter("@LoginId",EmailId),
                    new SqlParameter("@Pwd",Pwd),
                    new SqlParameter("@ContactNo",Mobile),
                    new SqlParameter("@IsActive",IsActive),
                    new SqlParameter("@CreatedBy",CreatedBy),

                    new SqlParameter("@AssinedCircle",dattable)
                };
            string _lst = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.sp_InsertOrUpdateDeployeIncharge, mparameters);

            return Ok(_lst);
        }
        [HttpGet]
        [Route("GetDeployInchargeDataById")]
        public IActionResult GetDeployInchargeDataById(int UserId)
        {
            SqlParameter[] mparameters = new SqlParameter[]
                   {
                    new SqlParameter("@UserId",UserId)
                   };
            //object[] mparameters = { UserId };
            string _lst = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetDeployInchargeDataById, mparameters);

            return Ok(_lst);
        }
        [HttpPost]
        [Route("GetVehicleMasterSummary")]
        public IActionResult GetVehicleMasterSummary(JObject obj)

        {
            string ZoneId = obj.GetValue("ZoneId").Value<string>();
            string CircleId = obj.GetValue("CircleId").Value<string>();
            string WardId = obj.GetValue("WardId").Value<string>();
            string VehicleTypeId = obj.GetValue("VehicleTypeId").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(ZoneId)?ZoneId:"0"),
                  new SqlParameter("@CircleId1",!string.IsNullOrEmpty(CircleId)?CircleId:"0"),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(WardId)?WardId:"0"),
                  new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(VehicleTypeId)?VehicleTypeId:"0"),
                  new SqlParameter("@UserId",UserId),
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamicDataset("spGetVehicleMasterSummary_N1", parameters);
            return Ok(Result);
        }
    }
}
