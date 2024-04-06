using HYDSWMAPI.INTERFACE;
using COMMON;
using COMMON.ASSET;
using COMMON.CITIZEN;
using COMMON.SWMENTITY;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HYDSWMAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AssetController : ControllerBase
    {
        private IAsset<CLoginResponseInfo> _datarepository;
        private readonly IWebHostEnvironment HostingEnvironment;

        public AssetController(IAsset<CLoginResponseInfo> datarepository, IWebHostEnvironment hostingEnvironment)
        {
            this._datarepository = datarepository;
            this.HostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [Route("GetAllContainer")]
        public IActionResult GetAllContainer(DataTableAjaxPostModel requestModel)
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
                  new SqlParameter("@Status",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"0"),
                  new SqlParameter("@FPath",baseUrl),
                  new SqlParameter("@CollectionTypeId",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"0"),
             };

            string Result = _datarepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllContainer_Paging, parameters);

            return Ok(Result);
        }


        [HttpPost]
        [Route("GetAllContainerB64")]
        public IActionResult GetAllContainerB64(DataTableAjaxPostModel requestModel)
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
                  new SqlParameter("@Status",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"0"),
                  new SqlParameter("@FPath",baseUrl),
                  new SqlParameter("@CollectionTypeId",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"0"),
             };

            string Result = _datarepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllContainer_Paging, parameters);

            List<ContainInfo> Aglst = JsonConvert.DeserializeObject<List<ContainInfo>>(Result);
            string strJson = JsonConvert.SerializeObject(Aglst);
            return Ok(strJson);
        }


        [HttpPost]
        [Route("GetAllMContainer")]
        public IActionResult GetAllMContainer(JObject obj)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            string PageNumber = obj.GetValue("PageNumber").Value<string>();
            string PageSize = obj.GetValue("PageSize").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();

            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",""),
                  new SqlParameter("@SortColumn",""),
                  new SqlParameter("@SortOrder",""),
                  new SqlParameter("@PageNumber",PageNumber),
                  new SqlParameter("@PageSize",PageSize),
                  new SqlParameter("@LoginId",UserId),
                  new SqlParameter("@Status","0"),
                  new SqlParameter("@FPath",baseUrl),
                  new SqlParameter("@CollectionTypeId","0"),
             };

            string Result = _datarepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllContainer_Paging, parameters);

            return Ok(Result);
        }
        [HttpGet]
        [Route("GetContainerActionStatusById")]
        public IActionResult GetContainerActionStatusById(int CMId)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@CMId",CMId)
              };

            string Result = _datarepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetContainerActionStatusById, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("SaveAndUpdateContainer")]
        public async Task<IActionResult> SaveAndUpdateContainer([FromForm] IFormCollection value)
        {
            string FName = string.Empty;
            AContainerInfo obj = JsonConvert.DeserializeObject<AContainerInfo>(value["Input"]);
            string FolderName = "/content/Container/";
            if (value.Files.Count > 0)
                if (value.Files[0].Length > 0)

                    if (CommonHelper.ExtensionType(Path.GetExtension((value.Files[0].FileName))))
                    {
                        FName = CommonHelper.generateID() + Path.GetExtension((value.Files[0].FileName));
                    }
                    else
                    {
                        var Response = new { Result = 0, Msg = "Invalid File Extension" };
                        var EResult = JsonConvert.SerializeObject(Response);
                        return Ok(EResult);
                    }

            // FName = CommonHelper.generateID() + Path.GetExtension(value.Files[0].FileName);

            string filePath = Path.Combine(HostingEnvironment.WebRootPath + FolderName, FName);


            if (obj.CMId == 0)
            {
                obj.StatusId = 2;
                obj.ARemarks = "";
                //if (!obj.IsManual)
                //    obj.UId = "CNT_QRC_" + CommonHelper.GenerateRandomNumber(10);
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@CMId",obj.CMId),
                  new SqlParameter("@ContainerCode",obj.Containercode),
                  new SqlParameter("@ContainerName",obj.ContainerName),
                  new SqlParameter("@Capacity",obj.ContainerCapacity),
                  new SqlParameter("@ContainerTypeId",obj.ContainerTypeId),
                  new SqlParameter("@IsActive",true),
                  new SqlParameter("@CreatedDate",CommonHelper.IndianStandard(DateTime.UtcNow)),
                  new SqlParameter("@CreatedBy",obj.UserId),
                  new SqlParameter("@StatusId",obj.StatusId),
                  new SqlParameter("@ExStatusVal",!string.IsNullOrEmpty(obj.ExStatusVal)?obj.ExStatusVal:"0"),
                  new SqlParameter("@Remarks",!string.IsNullOrEmpty(obj.ARemarks)?obj.ARemarks:""),
                  new SqlParameter("@UId",obj.UId),
                  new SqlParameter("@IsManualUId",obj.IsManual),
                  new SqlParameter("@ImgUrl",FName),
                  new SqlParameter("@FolderName",FolderName),
              };

            string Result = _datarepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spSaveAndUpdateContainerInfo, parameters);
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
        [HttpGet]
        [Route("GetContainerInfoById")]
        public IActionResult GetContainerInfoById(int CMId)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@CMId",CMId)
              };

            string Result = _datarepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetContainerInfoById, parameters);
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetContainerInfoByText")]
        public IActionResult GetContainerInfoByText(string SearchTxt)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@SearchTxt",SearchTxt)
              };

            string Result = _datarepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetContainerInfoByText, parameters);
            if (string.IsNullOrEmpty(Result))
                Result = "{}";
            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllVehicleInfo")]
        public IActionResult GetAllVehicleInfo(DataTableAjaxPostModel requestModel)
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
                    SortColumn = "VehicleNo";
                    break;
                case 5:
                    SortColumn = "ChassisNo";
                    break;
                case 6:
                    SortColumn = "VehicleType";
                    break;
                case 7:
                    SortColumn = "OwnerType";
                    break;
                case 8:
                    SortColumn = "OperationType";
                    break;
                case 9:
                    SortColumn = "ZoneNo";
                    break;
                case 10:
                    SortColumn = "CircleName";
                    break;
                case 11:
                    SortColumn = "WardNo";
                    break;
                case 12:
                    SortColumn = "GrossWt";
                    break;
                case 13:
                    SortColumn = "TareWt";
                    break;
                case 14:
                    SortColumn = "NetWt";
                    break;
                case 15:
                    SortColumn = "VStatus";
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
                  new SqlParameter("@Status",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"0"),
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),
                  new SqlParameter("@FPath",baseUrl),
                  new SqlParameter("@CollectionTypeId",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"0"),
                  new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(requestModel.VehicleTypeId)?requestModel.VehicleTypeId:"0"),

             };

            string Result = _datarepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllVehicle_Paging, parameters);

            return Ok(Result);
        }


        [HttpPost]
        [Route("GetVehicleDepoymentReport")]
        public IActionResult GetVehicleDepoymentReport(DataTableAjaxPostModel requestModel)
        {


            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),
                  new SqlParameter("@UserId",requestModel.UserId),
                  new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(requestModel.VehicleTypeId)?requestModel.VehicleTypeId:"0"),
                  new SqlParameter("@SearchDate",CommonHelper.IndianStandard(DateTime.UtcNow))

             };

            string Result = _datarepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spVehicleDeploymentData, parameters);

            return Ok(Result);
        }



        [HttpPost]
        [Route("GetAllVehicleInfoB64")]
        public IActionResult GetAllVehicleInfoB64(DataTableAjaxPostModel requestModel)
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
                    SortColumn = "VehicleNo";
                    break;
                case 5:
                    SortColumn = "ChassisNo";
                    break;
                case 6:
                    SortColumn = "VehicleType";
                    break;
                case 7:
                    SortColumn = "OwnerType";
                    break;
                case 8:
                    SortColumn = "OperationType";
                    break;
                case 9:
                    SortColumn = "ZoneNo";
                    break;
                case 10:
                    SortColumn = "CircleName";
                    break;
                case 11:
                    SortColumn = "WardNo";
                    break;
                case 12:
                    SortColumn = "GrossWt";
                    break;
                case 13:
                    SortColumn = "TareWt";
                    break;
                case 14:
                    SortColumn = "NetWt";
                    break;
                case 15:
                    SortColumn = "VStatus";
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
                  new SqlParameter("@Status",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"0"),
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),
                  new SqlParameter("@FPath",baseUrl),
                  new SqlParameter("@CollectionTypeId",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"0"),
                  new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(requestModel.VehicleTypeId)?requestModel.VehicleTypeId:"0"),

             };

            string Result = _datarepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllVehicle_Paging, parameters);

            List<AssetB64Info> Aglst = JsonConvert.DeserializeObject<List<AssetB64Info>>(Result);
            string strJson = JsonConvert.SerializeObject(Aglst);
            return Ok(strJson);

            // return Ok(Result);
        }


        [HttpPost]
        [Route("GetAllMVehicleInfo")]
        public IActionResult GetAllMVehicleInfo(JObject obj)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            string PageNumber = obj.GetValue("PageNumber").Value<string>();
            string PageSize = obj.GetValue("PageSize").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",""),
                  new SqlParameter("@SortColumn",""),
                  new SqlParameter("@SortOrder",""),
                  new SqlParameter("@PageNumber",PageNumber),
                  new SqlParameter("@PageSize",PageSize),
                  new SqlParameter("@LoginId",UserId),
                  new SqlParameter("@Status","0"),
                  new SqlParameter("@ZoneId","0"),
                  new SqlParameter("@CircleId","0"),
                  new SqlParameter("@WardId","0"),
                  new SqlParameter("@FPath",baseUrl),
                  new SqlParameter("@CollectionTypeId","0"),
             };

            string Result = _datarepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllVehicle_Paging, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("SaveAndUpdateVehicleInfo")]
        public async Task<IActionResult> SaveAndUpdateVehicleInfo([FromForm] IFormCollection value)
        {
            try
            {

                string FolderName = "/content/Vehicle/";
                string FName = string.Empty;
                // var Input = value["Input"];

                // dynamic obj = JObject.Parse(Input);
                AVehicleInfo obj = JsonConvert.DeserializeObject<AVehicleInfo>(value["Input"]);
                if (value.Files.Count > 0)
                    if (value.Files[0].Length > 0)
                        if (CommonHelper.ExtensionType(Path.GetExtension((value.Files[0].FileName))))
                        {
                            FName = CommonHelper.generateID() + Path.GetExtension((value.Files[0].FileName));
                        }
                        else
                        {
                            var Response = new { Result = 0, Msg = "Invalid File Extension" };
                            var EResult = JsonConvert.SerializeObject(Response);
                            return Ok(EResult);
                        }



                var filePath = Path.Combine(HostingEnvironment.WebRootPath + FolderName, FName);

                bool IsActive = true;

                if (obj.VehicleId == 0)
                {
                    obj.StatusId = 2;
                    obj.ARemarks = "";
                    //if (!obj.IsManual)
                    //    obj.UId = "VH_QRC_" + CommonHelper.GenerateRandomNumber(10);
                }
                SqlParameter[] parameters = new SqlParameter[]
                  {
                  new SqlParameter("@VehicleId",obj.VehicleId),
                  new SqlParameter("@VehicleNo",obj.VehicleNo),
                  new SqlParameter("@ChassisNo",obj.ChassisNo),
                  new SqlParameter("@VehicleTypeId",obj.VehicleTypeId),
                  new SqlParameter("@ZoneId",obj.ZoneId),
                  new SqlParameter("@CircleId",obj.CircleId),
                  new SqlParameter("@WardId",obj.WardId),
                  new SqlParameter("@GrossWt",obj.GrossWt),
                  new SqlParameter("@TareWt",obj.TareWt),
                  new SqlParameter("@NetWt",obj.NetWt),
                  new SqlParameter("@IsActive",IsActive),
                  new SqlParameter("@UId",obj.UId),
                  new SqlParameter("@IsManualUId",obj.IsManual),
                  new SqlParameter("@StatusId",obj.StatusId),
                  new SqlParameter("@ExStatusVal",!string.IsNullOrEmpty(obj.ExStatusVal)?obj.ExStatusVal:"0"),
                  new SqlParameter("@Remarks",!string.IsNullOrEmpty(obj.ARemarks)?obj.ARemarks:""),
                  new SqlParameter("@CreatedDate",CommonHelper.IndianStandard(DateTime.UtcNow)),
                  new SqlParameter("@CreatedBy",obj.UserId),
                  new SqlParameter("@OwnerTypeId",obj.OwnerTypeId),
                  new SqlParameter("@ImgUrl",FName),
                  new SqlParameter("@FolderName",FolderName),
                  new SqlParameter("@DriverName",!string.IsNullOrEmpty(obj.DriverName)?obj.DriverName:string.Empty),
                  new SqlParameter("@ContactNo",!string.IsNullOrEmpty(obj.ContactNo)?obj.ContactNo:string.Empty),
                  new SqlParameter("@DInchargeId",obj.DInchargeId),
                  new SqlParameter("@DLocationId",obj.DLocationId),
                  new SqlParameter("@DTsId",obj.DTsId),
                  };

                string Result = _datarepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spSaveAndUpdateVehicleInfo, parameters);
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
            catch (Exception ex)
            {

            }
            return Ok("");
        }
        [HttpGet]
        [Route("GetVehicleInfoById")]
        public IActionResult GetVehicleInfoById(int VehicleId)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@VehicleId",VehicleId)
              };

            string Result = _datarepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetVehicleInfoById, parameters);
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetVehicleInfoByText")]
        public IActionResult GetVehicleInfoByText(string SearchTxt)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@SearchTxt",SearchTxt)
              };

            string Result = _datarepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetVehicleInfoByText, parameters);
            if (string.IsNullOrEmpty(Result))
                Result = "{}";
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetVehicleActionStatusById")]
        public IActionResult GetVehicleActionStatusById(int VehicleId)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@VehicleId",VehicleId)
              };

            string Result = _datarepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetVehicleActionStatusById, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllAssetNotification")]
        public IActionResult GetAllAssetNotification(JObject obj)
        {
            string ZoneId = obj.GetValue("ZoneId").Value<string>();
            string CircleId = obj.GetValue("CircleId").Value<string>();
            string WardId = obj.GetValue("WardId").Value<string>();
            string VehicleTypeId = obj.GetValue("VehicleTypeId").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(ZoneId)?ZoneId:"0"),
                  new SqlParameter("@CircleId",!string.IsNullOrEmpty(CircleId)?CircleId:"0"),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(WardId)?WardId:"0"),
                  new SqlParameter("@UserId",UserId),
                  new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(VehicleTypeId)?VehicleTypeId:"0"),
              };

            string Result = _datarepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetAllAssetNotification, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("GetZoneWiseVehicle")]
        public IActionResult GetZoneWiseVehicle(JObject obj)
        {
            string ZoneId = obj.GetValue("ZoneId").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(ZoneId)?ZoneId:"0"),
                  new SqlParameter("@UserId",UserId),
              };

            string Result = _datarepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetZoneWiseVehicle, parameters);
            return Ok(Result);
        }


        [HttpGet]
        [Route("GetAllVehicleTypeByLogin")]
        public IActionResult GetAllVehicleTypeByLogin(string LoginId, string AppType)
        {
            SqlParameter[] parameters = new SqlParameter[]
             {
                             new SqlParameter("@LoginId",LoginId),
                  new SqlParameter("@AppType",AppType),
             };

            string Result = _datarepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllVehicleTypeByLogin, parameters);

            return Ok(Result);
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

           // string Result = _datarepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetVehicleMasterSummary, parameters);
            string Result = _datarepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetVehicleMasterSummary_New, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("GetVehicleDeployedSummary")]
        public IActionResult GetVehicleDeployedSummary(JObject obj)
        {
            string ZoneId = obj.GetValue("ZoneId").Value<string>();
            string CircleId = obj.GetValue("CircleId").Value<string>();
            string WardId = obj.GetValue("WardId").Value<string>();
            string VehicleTypeId = obj.GetValue("VehicleTypeId").Value<string>();
            string SDate = obj.GetValue("SDate").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(ZoneId)?ZoneId:"0"),
                  new SqlParameter("@CircleId1",!string.IsNullOrEmpty(CircleId)?CircleId:"0"),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(WardId)?WardId:"0"),
                  new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(VehicleTypeId)?VehicleTypeId:"0"),
                  new SqlParameter("@SDate",!string.IsNullOrEmpty(SDate)?SDate:"0"),
                  new SqlParameter("@UserId",UserId),
              };
            string Result = _datarepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetVehicleDeployedNotDepSummary_New, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("GetVehicleDeployedSummaryExcel")]
        public IActionResult GetVehicleDeployedSummaryExcel(DataTableAjaxPostModel requestModel)

        {
            //string ZoneId = obj.GetValue("ZoneId").Value<string>();
            //string CircleId = obj.GetValue("CircleId").Value<string>();
            //string WardId = obj.GetValue("WardId").Value<string>();
            //string VehicleTypeId = obj.GetValue("VehicleTypeId").Value<string>();
            //string SDate = obj.GetValue("SDate").Value<string>();
            //string UserId = obj.GetValue("UserId").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId1",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),
                  new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(requestModel.VehicleTypeId)?requestModel.VehicleTypeId:"0"),
                  new SqlParameter("@SDate",requestModel.FromDate),
                  new SqlParameter("@UserId",requestModel.UserId),
              };
            string Result = _datarepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetVehicleDeployedNotDepSummary, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllVMasterSummary_Paging")]
        public IActionResult GetAllVMasterSummary_Paging(DataTableAjaxPostModel requestModel)
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
                case 0:
                    SortColumn = "Zonecode";
                    break;
                case 1:
                    SortColumn = "CircleCode";
                    break;
                case 2:
                    SortColumn = "WardName";
                    break;
                case 3:
                    SortColumn = "VehicleType";
                    break;
                case 4:
                    SortColumn = "Active";
                    break;
                case 5:
                    SortColumn = "InActive";
                    break;
                case 6:
                    SortColumn = "InRepair";
                    break;
                case 7:
                    SortColumn = "Condemed";
                    break;
                case 8:
                    SortColumn = "TotalAsset";
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
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),
                  new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(requestModel.VehicleTypeId)?requestModel.VehicleTypeId:"0"),

             };

            string Result = _datarepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllVMasterSummary_Paging, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllDepVsNotDepInfo")]
        public IActionResult GetAllDepVsNotDepInfo(DataTableAjaxPostModel requestModel)
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
                    SortColumn = "VehicleNo";
                    break;
              
                case 3:
                    SortColumn = "VehicleType";
                    break;
               
                case 4:
                    SortColumn = "ZoneNo";
                    break;
                case 5:
                    SortColumn = "CircleName";
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
                  new SqlParameter("@Status",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"0"),
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),
                  new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(requestModel.VehicleTypeId)?requestModel.VehicleTypeId:"0"),
                  new SqlParameter("@SDate",requestModel.FromDate),
                  new SqlParameter("@Typeid",requestModel.CategoryId),

             };

            string Result = _datarepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllDeployedNotDepVehicle_Paging, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetVehicleDeployedVsReportedSummary")]
        public IActionResult GetVehicleDeployedVsReportedSummary(JObject obj)

        {
            string ZoneId = obj.GetValue("ZoneId").Value<string>();
            string CircleId = obj.GetValue("CircleId").Value<string>();
            string WardId = obj.GetValue("WardId").Value<string>();
            string VehicleTypeId = obj.GetValue("VehicleTypeId").Value<string>();
            string SDate = obj.GetValue("SDate").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(ZoneId)?ZoneId:"0"),
                  new SqlParameter("@CircleId1",!string.IsNullOrEmpty(CircleId)?CircleId:"0"),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(WardId)?WardId:"0"),
                  new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(VehicleTypeId)?VehicleTypeId:"0"),
                  new SqlParameter("@SDate",!string.IsNullOrEmpty(SDate)?SDate:"0"),
                  new SqlParameter("@UserId",UserId),
              };
            string Result = _datarepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetVehicleDeployedReportedSummary_New, parameters);
            return Ok(Result);
        }

        [HttpPost]
        [Route("GetVehicleDeployedVsReportedPaging")]
        public IActionResult GetVehicleDeployedVsReportedPaging(DataTableAjaxPostModel requestModel)
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
                    SortColumn = "VehicleNo";
                    break;

                case 3:
                    SortColumn = "VehicleType";
                    break;

                case 4:
                    SortColumn = "ZoneNo";
                    break;
                case 5:
                    SortColumn = "CircleName";
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
                  new SqlParameter("@Status",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"0"),
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),
                  new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(requestModel.VehicleTypeId)?requestModel.VehicleTypeId:"0"),
                  new SqlParameter("@SDate",requestModel.FromDate),
                  new SqlParameter("@Typeid",requestModel.CategoryId),

             };
            string Result  = "";
            if (requestModel.Status == "1")
            {
                 Result = _datarepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllDeployedReported_Paging, parameters);
            }
           else if (requestModel.Status == "2")
            {
                Result = _datarepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllReportedUnique_Paging, parameters);
            }
            else
            {
                 Result = _datarepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllDeployedVsReported_Paging, parameters);
            }

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetDeployedVsReportedExcel")]
        public IActionResult GetDeployedVsReportedExcel(DataTableAjaxPostModel requestModel)

        {
            //string ZoneId = obj.GetValue("ZoneId").Value<string>();
            //string CircleId = obj.GetValue("CircleId").Value<string>();
            //string WardId = obj.GetValue("WardId").Value<string>();
            //string VehicleTypeId = obj.GetValue("VehicleTypeId").Value<string>();
            //string SDate = obj.GetValue("SDate").Value<string>();
            //string UserId = obj.GetValue("UserId").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId1",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),
                  new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(requestModel.VehicleTypeId)?requestModel.VehicleTypeId:"0"),
                  new SqlParameter("@SDate",requestModel.FromDate),
                  new SqlParameter("@UserId",requestModel.UserId),
              };
            string Result = _datarepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetVehicleDeployedReportedSummary, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("GetDeployedVsReportedSummaryExcel")]
        public IActionResult GetDeployedVsReportedSummaryExcel(DataTableAjaxPostModel requestModel)

        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId1",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),
                  new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(requestModel.VehicleTypeId)?requestModel.VehicleTypeId:"0"),
                  new SqlParameter("@SDate",requestModel.FromDate),
                  new SqlParameter("@UserId",requestModel.UserId),
              };
            string Result = _datarepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetVehicleDeployedReportedSummary, parameters);
            return Ok(Result);
        }

        //Deployed Vs Not Reported Section
        [HttpPost]
        [Route("GetDeployedVsNotReportedSummary")]
        public IActionResult GetDeployedVsNotReportedSummary(JObject obj)

        {
            string ZoneId = obj.GetValue("ZoneId").Value<string>();
            string CircleId = obj.GetValue("CircleId").Value<string>();
            string WardId = obj.GetValue("WardId").Value<string>();
            string VehicleTypeId = obj.GetValue("VehicleTypeId").Value<string>();
            string SDate = obj.GetValue("SDate").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(ZoneId)?ZoneId:"0"),
                  new SqlParameter("@CircleId1",!string.IsNullOrEmpty(CircleId)?CircleId:"0"),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(WardId)?WardId:"0"),
                  new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(VehicleTypeId)?VehicleTypeId:"0"),
                  new SqlParameter("@SDate",!string.IsNullOrEmpty(SDate)?SDate:"0"),
                  new SqlParameter("@UserId",UserId),
              };
            string Result = _datarepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetDeployedVsNotReportedSummary_New, parameters);
            return Ok(Result);
        }

        [HttpPost]
        [Route("GetDeployedVsNotReported_Paging")]
        public IActionResult GetDeployedVsNotReported_Paging(DataTableAjaxPostModel requestModel)
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
                    SortColumn = "VehicleNo";
                    break;

                case 3:
                    SortColumn = "VehicleType";
                    break;

                case 4:
                    SortColumn = "ZoneNo";
                    break;
                case 5:
                    SortColumn = "CircleName";
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
                  new SqlParameter("@Status",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"0"),
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),
                  new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(requestModel.VehicleTypeId)?requestModel.VehicleTypeId:"0"),
                  new SqlParameter("@SDate",requestModel.FromDate),
                  new SqlParameter("@Typeid",requestModel.CategoryId),

             };

            string Result = _datarepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetDeployedVsNotReported_Paging, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetDeployedVsNotReportedExcel")]
        public IActionResult GetDeployedVsNotReportedExcel(DataTableAjaxPostModel requestModel)

        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId1",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),
                  new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(requestModel.VehicleTypeId)?requestModel.VehicleTypeId:"0"),
                  new SqlParameter("@SDate",requestModel.FromDate),
                  new SqlParameter("@UserId",requestModel.UserId),
              };
            string Result = _datarepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetDeployedVsNotReportedSummary, parameters);
            return Ok(Result);
        }
        //Deployed Vs Not Reported Section

        [HttpPost]
        [Route("GetNotDeployedVsReportedSummary")]
        public IActionResult GetNotDeployedVsReportedSummary(JObject obj)

        {
            string ZoneId = obj.GetValue("ZoneId").Value<string>();
            string CircleId = obj.GetValue("CircleId").Value<string>();
            string WardId = obj.GetValue("WardId").Value<string>();
            string VehicleTypeId = obj.GetValue("VehicleTypeId").Value<string>();
            string SDate = obj.GetValue("SDate").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(ZoneId)?ZoneId:"0"),
                  new SqlParameter("@CircleId1",!string.IsNullOrEmpty(CircleId)?CircleId:"0"),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(WardId)?WardId:"0"),
                  new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(VehicleTypeId)?VehicleTypeId:"0"),
                  new SqlParameter("@SDate",!string.IsNullOrEmpty(SDate)?SDate:"0"),
                  new SqlParameter("@UserId",UserId),
              };
            string Result = _datarepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetNotDeployedVsReportedSummary, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("GetNotDeployedVsReportedPaging")]
        public IActionResult GetNotDeployedVsReportedPaging(DataTableAjaxPostModel requestModel)
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
                    SortColumn = "VehicleNo";
                    break;

                case 3:
                    SortColumn = "VehicleType";
                    break;

                case 4:
                    SortColumn = "ZoneNo";
                    break;
                case 5:
                    SortColumn = "CircleName";
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
                  new SqlParameter("@Status",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"0"),
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),
                  new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(requestModel.VehicleTypeId)?requestModel.VehicleTypeId:"0"),
                  new SqlParameter("@SDate",requestModel.FromDate),
                  new SqlParameter("@Typeid",requestModel.CategoryId),

             };
            string Result = "";
            if (requestModel.Status == "1")
            {
                Result = _datarepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetNotDeployedReported_Paging, parameters);
            }
            else if (requestModel.Status == "2")
            {
                Result = _datarepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllReportedUnique_Paging, parameters);
            }
            else
            {
                Result = _datarepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllDeployedVsReported_Paging, parameters);
            }

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetNotDeployedVsReportedSummaryExcel")]
        public IActionResult GetNotDeployedVsReportedSummaryExcel(DataTableAjaxPostModel requestModel)

        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId1",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),
                  new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(requestModel.VehicleTypeId)?requestModel.VehicleTypeId:"0"),
                  new SqlParameter("@SDate",requestModel.FromDate),
                  new SqlParameter("@UserId",requestModel.UserId),
              };
            string Result = _datarepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetNotDeployedVsReportedSummary, parameters);
            return Ok(Result);
        }

    }
}
