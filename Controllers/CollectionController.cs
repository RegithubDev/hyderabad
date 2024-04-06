using COMMON;
using COMMON.COLLECTION;
using COMMON.SWMENTITY;
using HYDSWMAPI.INTERFACE;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using COMMON.COLLECTION;
using COMMON.OPERATION;

namespace HYDSWMAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionController : ControllerBase
    {
        public ISWMCollection<GResposnse> _dataRepository;
        private readonly IWebHostEnvironment HostingEnvironment;
        public CollectionController(ISWMCollection<GResposnse> dataRepository, IWebHostEnvironment hostingEnvironment)
        {
            this._dataRepository = dataRepository;
            this.HostingEnvironment = hostingEnvironment;
        }
        [HttpPost]
        [Route("AddGeoLocationSurvey")]
        public async Task<IActionResult> AddGeoLocationSurvey([FromForm] IFormCollection value)
        {

            string FName = string.Empty;
            GeoLocationSurveyInfo obj = JsonConvert.DeserializeObject<GeoLocationSurveyInfo>(value["Input"]);

            string FolderName = "/content/Survey/";
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
            //  FName = CommonHelper.generateID() + Path.GetExtension(value.Files[0].FileName);

            string filePath = Path.Combine(HostingEnvironment.WebRootPath + FolderName, FName);
            string userName = this.User.GetUserName(); ;
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@GeoPointId",obj.GeoPointId),
                  new SqlParameter("@GeoPointName",obj.GeoPointName.Trim()),
                  new SqlParameter("@PointCategoryId",obj.CategoryId),
                  new SqlParameter("@Lat",obj.Lat),
                  new SqlParameter("@Lng",obj.Lng),
                 new SqlParameter("@Location",obj.Location.Trim()),
                 new SqlParameter("@Remarks",!string.IsNullOrEmpty(obj.Remarks)? obj.Remarks.Trim():string.Empty),
                 new SqlParameter("@Radius",obj.Radius),
                 new SqlParameter("@IsActive",obj.IsActive),
                  new SqlParameter("@CreatedDate",CommonHelper.IndianStandard(DateTime.UtcNow)),
                  new SqlParameter("@CreatedBy",userName),
                  new SqlParameter("@FName",FName),
                  new SqlParameter("@FolderName",FolderName),
                  new SqlParameter("@ZoneId",obj.ZoneId),
                  new SqlParameter("@CircleId",obj.CircleId),
                  new SqlParameter("@WardId",obj.WardId),
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spAddAndUpdateGeoPoint, parameters);

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
        [Route("AddEmergencyLocationSurvey")]
        public async Task<IActionResult> AddEmergencyLocationSurvey([FromForm] IFormCollection value)
        {

            string FName = string.Empty;
            EmergencyLocationSurveyInfo obj = JsonConvert.DeserializeObject<EmergencyLocationSurveyInfo>(value["Input"]);

            string FolderName = "/content/Survey/";
            if (value.Files.Count > 0)
                if (value.Files[0].Length > 0)
                    FName = CommonHelper.generateID() + Path.GetExtension(value.Files[0].FileName);

            string filePath = Path.Combine(HostingEnvironment.WebRootPath + FolderName, FName);

            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@GeoPointId",obj.GeoPointId),
                  new SqlParameter("@GeoPointName",obj.GeoPointName),
                  new SqlParameter("@PointCategoryId",obj.CategoryId),
                  new SqlParameter("@Lat",obj.Lat),
                  new SqlParameter("@Lng",obj.Lng),
                 new SqlParameter("@Location",obj.Location),
                 new SqlParameter("@Remarks",obj.Remarks),
                 new SqlParameter("@Radius",obj.Radius),
                 new SqlParameter("@IsActive",obj.IsActive),
                  new SqlParameter("@CreatedDate",CommonHelper.IndianStandard(DateTime.UtcNow)),
                  new SqlParameter("@CreatedBy",obj.CreatedBy),
                  new SqlParameter("@FName",FName),
                  new SqlParameter("@FolderName",FolderName),
                  new SqlParameter("@ZoneId",obj.ZoneId),
                  new SqlParameter("@CircleId",obj.CircleId),
                  new SqlParameter("@WardId",obj.WardId),
                  new SqlParameter("@PointType",obj.PointType),
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spAddAndUpdateEmergencyPointMaster, parameters);

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

        [HttpGet]
        [Route("GetAllGeoPointCategory")]
        public IActionResult GetAllGeoPointCategory(string IsAll, int GPCId)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@IsAll",IsAll),
                  new SqlParameter("@GPCId",GPCId),
              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllGeoPointCategory, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllGeoPoint")]
        public IActionResult GetAllGeoPoint(JObject obj)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            string PageNumber = obj.GetValue("PageNumber").Value<string>();
            string PageSize = obj.GetValue("PageSize").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            string SearchTerm = obj.GetValue("SearchTerm").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",SearchTerm),
                  new SqlParameter("@SortColumn",""),
                  new SqlParameter("@SortOrder",""),
                  new SqlParameter("@PageNumber",PageNumber),
                  new SqlParameter("@PageSize",PageSize),
                  new SqlParameter("@LoginId",UserId),
                  new SqlParameter("@FPath",baseUrl),
                  new SqlParameter("@ZoneId","0"),
                   new SqlParameter("@CircleId","0"),
                   new SqlParameter("@WardId","0"),
                   new SqlParameter("@PointId","-1"),
                   new SqlParameter("@StatusId","-1"),
                   new SqlParameter("@IsSEPoint","0"),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllGeoPoint_Paging, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllGeoPoint_Paging")]
        public IActionResult GetAllGeoPoint_Paging(DataTableAjaxPostModel requestModel)
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
            if (requestModel.ContratorId == "1")
            {
                switch (requestModel.order[0].column)
                {

                    case 1:
                        SortColumn = "A.GeoPointName";
                        break;

                    case 2:
                        SortColumn = "A.GeoPointCategory";
                        break;
                    case 3:
                        SortColumn = "C.ZoneNo";
                        break;
                    case 4:
                        SortColumn = "D.CircleName";
                        break;

                    default:
                        SortDir = String.Empty;
                        SortColumn = string.Empty;
                        break;
                }
            }
            else
            {
                switch (requestModel.order[0].column)
                {

                    case 3:
                        SortColumn = "A.GeoPointName";
                        break;

                    case 4:
                        SortColumn = "A.GeoPointCategory";
                        break;
                    case 5:
                        SortColumn = "C.ZoneNo";
                        break;
                    case 6:
                        SortColumn = "D.CircleName";
                        break;
                    case 7:
                        SortColumn = "E.WardNo";
                        break;

                    case 9:
                        SortColumn = "A.Radius";
                        break;
                    case 10:
                        SortColumn = "A.Remarks";
                        break;
                    case 11:
                        SortColumn = "A.CreatedBy";
                        break;
                    case 12:
                        SortColumn = "A.ModifiedDate";
                        break;

                    default:
                        SortDir = String.Empty;
                        SortColumn = string.Empty;
                        break;
                }
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
                   new SqlParameter("@FPath",baseUrl),
                   new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                   new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                   new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),
                   new SqlParameter("@PointId",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"-1"),
                   new SqlParameter("@StatusId",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"-1"),
                   new SqlParameter("@IsSEPoint",!string.IsNullOrEmpty(requestModel.ContratorId)?requestModel.ContratorId:"0"),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllGeoPoint_Paging, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllEmergencyPoint_Paging")]
        public IActionResult GetAllEmergencyPoint_Paging(DataTableAjaxPostModel requestModel)
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
            if (requestModel.ContratorId == "1")
            {
                switch (requestModel.order[0].column)
                {

                    case 1:
                        SortColumn = "A.GeoPointName";
                        break;

                    case 2:
                        SortColumn = "A.GeoPointCategory";
                        break;
                    case 3:
                        SortColumn = "C.ZoneNo";
                        break;
                    case 4:
                        SortColumn = "D.CircleName";
                        break;

                    default:
                        SortDir = String.Empty;
                        SortColumn = string.Empty;
                        break;
                }
            }
            else
            {
                switch (requestModel.order[0].column)
                {

                    case 3:
                        SortColumn = "A.PointName";
                        break;

                    case 4:
                        SortColumn = "A.GeoPointCategory";
                        break;
                    case 5:
                        SortColumn = "C.ZoneNo";
                        break;
                    case 6:
                        SortColumn = "D.CircleName";
                        break;
                    case 7:
                        SortColumn = "E.WardNo";
                        break;

                    case 9:
                        SortColumn = "A.Radius";
                        break;
                    case 10:
                        SortColumn = "A.Remarks";
                        break;
                    case 11:
                        SortColumn = "A.CretedBy";
                        break;
                    case 12:
                        SortColumn = "A.ModifyDate";
                        break;

                    default:
                        SortDir = String.Empty;
                        SortColumn = string.Empty;
                        break;
                }
            }
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",str),
                  new SqlParameter("@SortColumn",SortColumn),
                  new SqlParameter("@SortOrder",SortDir),
                  new SqlParameter("@PageNumber",start),
                  new SqlParameter("@PageSize",length),
                  new SqlParameter("@LoginId",requestModel.UserId),
                   new SqlParameter("@FPath",baseUrl),
                   new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                   new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                   new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),
                   new SqlParameter("@PointId",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"-1"),
                   new SqlParameter("@StatusId",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"-1"),
                   new SqlParameter("@IsSEPoint",!string.IsNullOrEmpty(requestModel.ContratorId)?requestModel.ContratorId:"0"),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllEmergencyPoint_Paging, parameters);
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetGeoPointInfoById")]
        public IActionResult GetGeoPointInfoById(int GeoPointId)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@GeoPointId",GeoPointId),
                  new SqlParameter("@FPath",baseUrl)
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetGeoPointInfoById, parameters);
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetEmergencyPointMasterInfoById")]
        public IActionResult GetEmergencyPointMasterInfoById(int GeoPointId)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@GeoPointId",GeoPointId),
                  new SqlParameter("@FPath",baseUrl)
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetEmergencyPointMasterInfoById, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("AllNRouteInfo")]
        public IActionResult AllNRouteInfo(DataTableAjaxPostModel requestModel)
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



                case 2:
                    SortColumn = "RouteName";
                    break;
                case 3:
                    SortColumn = "RouteCode";
                    break;
                case 4:
                    SortColumn = "SourceStop";
                    break;
                case 5:
                    SortColumn = "DestinationStop";
                    break;
                case 6:
                    SortColumn = "SArrvlTime";
                    break;
                case 7:
                    SortColumn = "SDeptTime";
                    break;

                case 8:
                    SortColumn = "TotalStop";
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
                  new SqlParameter("@VUId",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:""),
            };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllNRoute_Paging, parameters);
            return Ok(Result);
        }
        //older version
        [HttpPost]
        //[Route("GetAllNRouteByVehicle")]
        [Route("GetAllNRouteByVehicle_V1")]
        public IActionResult GetAllNRouteByVehicle_V1(JObject obj)
        {
            List<RouteNTripInfo> _lst = new List<RouteNTripInfo>();
            try
            {
                string PageNumber = obj.GetValue("PageNumber").Value<string>();
                string PageSize = obj.GetValue("PageSize").Value<string>();
                string UserId = obj.GetValue("UserId").Value<string>();
                string SearchTerm = obj.GetValue("SearchTerm").Value<string>();
                string VUId = obj.GetValue("VUId").Value<string>();
                SqlParameter[] parameters = new SqlParameter[]
                 {
                  new SqlParameter("@SearchTerm",SearchTerm),
                  new SqlParameter("@SortColumn",""),
                  new SqlParameter("@SortOrder",""),
                  new SqlParameter("@PageNumber",PageNumber),
                  new SqlParameter("@PageSize",PageSize),
                  new SqlParameter("@LoginId",UserId),
                  new SqlParameter("@VUId",VUId),
                 };

                string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllNRouteByVehicle, parameters);
                if (!string.IsNullOrEmpty(Result))
                {
                    _lst = JsonConvert.DeserializeObject<List<RouteNTripInfo>>(Result);
                }
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
            return Ok(_lst);
        }
        [HttpPost]
        [Route("GetAllNRouteByVehicle_N")]
        public IActionResult GetAllNRouteByVehicle_N(JObject obj)
        {
            List<RouteNTripInfo> _lst = new List<RouteNTripInfo>();
            try
            {
                string PageNumber = obj.GetValue("PageNumber").Value<string>();
                string PageSize = obj.GetValue("PageSize").Value<string>();
                string UserId = obj.GetValue("UserId").Value<string>();
                string SearchTerm = obj.GetValue("SearchTerm").Value<string>();
                string VUId = obj.GetValue("VUId").Value<string>();
                SqlParameter[] parameters = new SqlParameter[]
                 {
                  new SqlParameter("@SearchTerm",SearchTerm),
                  new SqlParameter("@SortColumn",""),
                  new SqlParameter("@SortOrder",""),
                  new SqlParameter("@PageNumber",PageNumber),
                  new SqlParameter("@PageSize",PageSize),
                  new SqlParameter("@LoginId",UserId),
                  new SqlParameter("@VUId",VUId),
                 };

                string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllNRouteByVehicle_1, parameters);
                if (!string.IsNullOrEmpty(Result))
                {
                    _lst = JsonConvert.DeserializeObject<List<RouteNTripInfo>>(Result);
                }
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
            return Ok(_lst);
        }
        [HttpPost]
        [Route("AddNRouteInfo")]
        public IActionResult AddNRouteInfo(JObject obj)
        {
            string RouteId = obj.GetValue("RouteId").Value<string>();
            string RouteName = obj.GetValue("RouteName").Value<string>();
            string RouteCode = obj.GetValue("RouteCode").Value<string>();
            string VehicleUId = "";//obj.GetValue("VehicleUId").Value<string>();
            int BufferMin = 0; //obj.GetValue("BufferMin").Value<int>();
                               // string DeptTime = obj.GetValue("DeptTime").Value<string>();
            string IsActive = obj.GetValue("IsActive").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            string CCode = obj.GetValue("CCode").Value<string>();
            string JArrayval = obj.GetValue("JArrayval").Value<string>();

            List<NRouteStopInfo> _lst = JsonConvert.DeserializeObject<List<NRouteStopInfo>>(JArrayval);

            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@SourceId",_lst.Select(i=>i.StopId).FirstOrDefault()),
                  new SqlParameter("@DestinationId",_lst.Select(i=>i.StopId).LastOrDefault()),
                  new SqlParameter("@DeptTime",_lst.Select(i=>i.PickupTime).LastOrDefault()),
                  new SqlParameter("@ArrvlTime",_lst.Select(i=>i.PickupTime).FirstOrDefault()),
                  new SqlParameter("@RouteId",!string.IsNullOrEmpty(RouteId)?RouteId:"0"),
                 new SqlParameter("@RouteCode",!string.IsNullOrEmpty(RouteCode)?RouteCode:string.Empty),
                 new SqlParameter("@IsActive",IsActive),
                 new SqlParameter("@CreatedDate",CommonHelper.IndianStandard(DateTime.UtcNow)),
                 new SqlParameter("@CreatedBy",UserId),
                  new SqlParameter("@RouteName",RouteName),
                  new SqlParameter("@VehicleUId",VehicleUId),
                  new SqlParameter("@BufferMin",BufferMin),
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spAddAndUpdateNRoute, parameters);



            dynamic dresult = JObject.Parse(Result);
            if (dresult.Result == 1 || dresult.Result == 2)
            {
                string RCode = dresult.Code;
                DataTable dt = CommonHelper.ToDataTable(_lst.OrderBy(i => i.PickupTime).ToList());
                SqlParameter[] parameters1 = new SqlParameter[]
             {
                  new SqlParameter("@NRouteStopType",dt),
                  new SqlParameter("@RouteCode",RCode),
             };

                string Result1 = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spAddRouteStop, parameters1);
            }

            return Ok(Result);
        }
        [HttpGet]
        [Route("GetNRouteInfoById")]
        public IActionResult GetNRouteInfoById(int RouteId)
        {
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@RouteId",RouteId)
             };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetRouteInfoById, parameters);

            return Ok(Result);
        }
        [HttpGet]
        [Route("GetAllNStopByRouteCode")]
        public IActionResult GetAllNStopByRouteCode(string RouteCode)
        {
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@RouteCode",RouteCode)
             };
            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllNStopByRouteCode, parameters);

            return Ok(Result);
        }
        [HttpGet]
        [Route("GetAllNStopByTripId")]
        public IActionResult GetAllNStopByTripId(int RTDId)
        {
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@RTDId",RTDId)
             };
            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllNStopByTripId, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllTripByRoute")]
        public IActionResult GetAllTripByRoute(JObject obj)
        {
            List<RouteNTripInfo> _lst = new List<RouteNTripInfo>();

            string RouteCode = obj.GetValue("RouteCode").Value<string>();
            string RouteId = obj.GetValue("RouteId").Value<string>();
            string VUId = obj.GetValue("VUId").Value<string>();
            string LoginId = obj.GetValue("LoginId").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@VUID",VUId),
                  new SqlParameter("@RouteId",RouteId),
                  new SqlParameter("@RouteCode",RouteCode),
                  new SqlParameter("@LoginId",LoginId),

             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllTripByRoute, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllTransPointByTrip")]
        public IActionResult GetAllTransPointByTrip(JObject obj)
        {
            try
            {
                string RouteCode = obj.GetValue("RouteCode").Value<string>();
                string RouteId = obj.GetValue("RouteId").Value<string>();
                string VUId = obj.GetValue("VUId").Value<string>();
                string TripId = obj.GetValue("TripId").Value<string>();
                string LoginId = obj.GetValue("LoginId").Value<string>();
                SqlParameter[] parameters = new SqlParameter[]
                 {
                  new SqlParameter("@TRIPID",TripId),
                  new SqlParameter("@VUID",VUId),
                  new SqlParameter("@RouteCode",RouteCode),
                  new SqlParameter("@LoginId",LoginId),

                 };

                string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllTransPointByTrip, parameters);

                return Ok(Result);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetAllTransPointByTrip_N")]
        public IActionResult GetAllTransPointByTrip_N(JObject obj)
        {
            try
            {
                string RouteCode = obj.GetValue("RouteCode").Value<string>();
                string RouteId = obj.GetValue("RouteId").Value<string>();
                string VUId = obj.GetValue("VUId").Value<string>();
                string TripId = obj.GetValue("TripId").Value<string>();
                string LoginId = obj.GetValue("LoginId").Value<string>();
                string TRefId = obj.GetValue("TRefId").Value<string>();
                SqlParameter[] parameters = new SqlParameter[]
                 {
                  new SqlParameter("@TRIPID",TripId),
                  new SqlParameter("@VUID",VUId),
                  new SqlParameter("@RouteCode",RouteCode),
                  new SqlParameter("@LoginId",LoginId),
                  new SqlParameter("@TRefId",TRefId),

                 };

                string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllTransPointByTrip_1, parameters);

                return Ok(Result);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        //older version
        [HttpPost]
        //[Route("AddPointCollectTrans")]
        [Route("AddPointCollectTrans_V1")]
        public async Task<IActionResult> AddPointCollectTrans_V1([FromForm] IFormCollection value)
        {
            string LogfilePath = Path.Combine(HostingEnvironment.WebRootPath + "/content/Logs/");
            string ErrorMsg = string.Empty;
            string InputData = value["Input"];

            if (value.Files.Count < 2)
            {
                var EResult = new { Result = 0, Msg = "Please Attach Before And After Photo" };
                ErrorMsg = JsonConvert.SerializeObject(EResult);
                return Ok(ErrorMsg);
            }
            try
            {
                DateTime TDate = CommonHelper.IndianStandard(DateTime.UtcNow);
                string FName = string.Empty;
                string FName1 = string.Empty;
                PointCollectionInfo obj = JsonConvert.DeserializeObject<PointCollectionInfo>(value["Input"]);
                obj.Address = !string.IsNullOrEmpty(obj.Address) ? obj.Address : string.Empty;
                obj.BPhotoStamp = obj.BPhotoStamp != null ? obj.BPhotoStamp : TDate;
                string TripStatusMsg = GetValidateTripStart(obj, TDate);
                dynamic dTresult = JObject.Parse(TripStatusMsg);
                if (dTresult.Result == "0")
                {
                    var EResult = new { Result = 0, Msg = "Please Go Back On Home Screen Then Come Again For Collection" };
                    ErrorMsg = JsonConvert.SerializeObject(EResult);
                    return Ok(ErrorMsg);
                }
                string FolderName = "/content/Point_Collection/";
                if (value.Files.Count > 0)
                    if (value.Files[0].Length > 0)
                        FName = CommonHelper.generateID() + Path.GetExtension(value.Files[0].FileName);

                if (value.Files.Count > 1)
                    if (value.Files[1].Length > 0)
                        FName1 = CommonHelper.generateID() + Path.GetExtension(value.Files[1].FileName);

                string filePath = Path.Combine(HostingEnvironment.WebRootPath + FolderName, FName);
                string filePath1 = Path.Combine(HostingEnvironment.WebRootPath + FolderName, FName1);

                SqlParameter[] parameters = new SqlParameter[]
                  {
                  new SqlParameter("@PointId",obj.PointId),
                  new SqlParameter("@PointName",obj.PointName),
                  new SqlParameter("@PickUpTime",TDate),
                  new SqlParameter("@RouteCode",obj.RouteCode),
                  new SqlParameter("@RouteId",obj.RouteId),
                  new SqlParameter("@Lat",obj.Lat),
                  new SqlParameter("@Lng",obj.Lng),
                  new SqlParameter("@Address",obj.Address),
                  new SqlParameter("@Remarks",obj.Remarks),
                  new SqlParameter("@ImgUrl1",FName),
                  new SqlParameter("@ImgUrl2",FName1),
                  new SqlParameter("@CreatedDate",TDate),
                  new SqlParameter("@CreatedBy",obj.LoginId),
                  new SqlParameter("@TripId",obj.TripId),
                  new SqlParameter("@VehicleUId",obj.VehicleUId),
                  new SqlParameter("@FolderName",FolderName),
                  new SqlParameter("@IsDeviated",obj.IsDeviated),
                  new SqlParameter("@ActPointId",obj.ActPointId),
                  new SqlParameter("@AppVersion",!string.IsNullOrEmpty(obj.AppVersion)?obj.AppVersion:string.Empty),
                  new SqlParameter("@BPhotoStamp",obj.BPhotoStamp),
                  };


                string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spAddPointCollectTrans, parameters);

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
                    if (value.Files.Count > 1)
                    {
                        if (value.Files[1].Length > 0)
                            using (var fileStream = new FileStream(filePath1, FileMode.Create))
                            {
                                await value.Files[1].CopyToAsync(fileStream);
                            }
                    }
                }

                return Ok(Result);
            }
            catch (Exception ex)
            {
                CommonHelper.WriteToJsonFile("PointCltLog", InputData + " Error-" + ex.Message, LogfilePath);

                return Ok(ex.Message);
            }
        }
        [HttpPost]
        [Route("AddPointCollectTrans_N")]
        public async Task<IActionResult> AddPointCollectTrans_N([FromForm] IFormCollection value)
        {
            string LogfilePath = Path.Combine(HostingEnvironment.WebRootPath + "/content/Logs/");
            string ErrorMsg = string.Empty;
            string InputData = value["Input"];

            if (value.Files.Count < 2)
            {
                var EResult = new { Result = 0, Msg = "Please Attach Before And After Photo" };
                ErrorMsg = JsonConvert.SerializeObject(EResult);
                return Ok(ErrorMsg);
            }
            try
            {
                DateTime TDate = CommonHelper.IndianStandard(DateTime.UtcNow);
                string FName = string.Empty;
                string FName1 = string.Empty;
                PointCollectionInfo obj = JsonConvert.DeserializeObject<PointCollectionInfo>(value["Input"]);
                obj.Address = !string.IsNullOrEmpty(obj.Address) ? obj.Address : string.Empty;
                obj.BPhotoStamp = obj.BPhotoStamp != null ? obj.BPhotoStamp : TDate;
                string TripStatusMsg = GetValidateTripStart_N(obj, TDate);
                dynamic dTresult = JObject.Parse(TripStatusMsg);
                if (dTresult.Result == "0")
                {
                    var EResult = new { Result = 0, Msg = "Please Go Back On Home Screen Then Come Again For Collection" };
                    ErrorMsg = JsonConvert.SerializeObject(EResult);
                    return Ok(ErrorMsg);
                }
                string FolderName = "/content/Point_Collection/";
                if (value.Files.Count > 0)
                    if (value.Files[0].Length > 0)
                        FName = CommonHelper.generateID() + Path.GetExtension(value.Files[0].FileName);

                if (value.Files.Count > 1)
                    if (value.Files[1].Length > 0)
                        FName1 = CommonHelper.generateID() + Path.GetExtension(value.Files[1].FileName);

                string filePath = Path.Combine(HostingEnvironment.WebRootPath + FolderName, FName);
                string filePath1 = Path.Combine(HostingEnvironment.WebRootPath + FolderName, FName1);

                SqlParameter[] parameters = new SqlParameter[]
                  {
                  new SqlParameter("@PointId",obj.PointId),
                  new SqlParameter("@PointName",obj.PointName),
                  new SqlParameter("@PickUpTime",TDate),
                  new SqlParameter("@RouteCode",obj.RouteCode),
                  new SqlParameter("@RouteId",obj.RouteId),
                  new SqlParameter("@Lat",obj.Lat),
                  new SqlParameter("@Lng",obj.Lng),
                  new SqlParameter("@Address",obj.Address),
                  new SqlParameter("@Remarks",obj.Remarks),
                  new SqlParameter("@ImgUrl1",FName),
                  new SqlParameter("@ImgUrl2",FName1),
                  new SqlParameter("@CreatedDate",TDate),
                  new SqlParameter("@CreatedBy",obj.LoginId),
                  new SqlParameter("@TripId",obj.TripId),
                  new SqlParameter("@VehicleUId",obj.VehicleUId),
                  new SqlParameter("@FolderName",FolderName),
                  new SqlParameter("@IsDeviated",obj.IsDeviated),
                  new SqlParameter("@ActPointId",obj.ActPointId),
                  new SqlParameter("@AppVersion",!string.IsNullOrEmpty(obj.AppVersion)?obj.AppVersion:string.Empty),
                  new SqlParameter("@BPhotoStamp",obj.BPhotoStamp),
                  new SqlParameter("@TRefID",!string.IsNullOrEmpty(obj.TRefId)?obj.TRefId:string.Empty),
                  };


                string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spAddPointCollectTrans_1, parameters);

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
                    if (value.Files.Count > 1)
                    {
                        if (value.Files[1].Length > 0)
                            using (var fileStream = new FileStream(filePath1, FileMode.Create))
                            {
                                await value.Files[1].CopyToAsync(fileStream);
                            }
                    }
                }

                return Ok(Result);
            }
            catch (Exception ex)
            {
                CommonHelper.WriteToJsonFile("PointCltLog", InputData + " Error-" + ex.Message, LogfilePath);

                return Ok(ex.Message);
            }
        }
        public string GetValidateTripStart(PointCollectionInfo obj, DateTime TDate)
        {
            string Result = string.Empty;
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                 {

                  new SqlParameter("@RouteId",obj.RouteId),
                  new SqlParameter("@TripId",obj.TripId),
                  new SqlParameter("@VUId",obj.VehicleUId),
                  new SqlParameter("@TDate",TDate),

                 };

                Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spValidateTripsStartStatus, parameters);

            }
            catch (Exception ex)
            {
                var EResult = new { Result = 1, Msg = "Please Attach Before And After Photo" };
                string ErrorMsg = JsonConvert.SerializeObject(EResult);
                return ErrorMsg;
            }
            return Result;
        }
        public string GetValidateTripStart_N(PointCollectionInfo obj, DateTime TDate)
        {
            string Result = string.Empty;
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                 {

                  new SqlParameter("@RouteId",obj.RouteId),
                  new SqlParameter("@TripId",obj.TripId),
                  new SqlParameter("@VUId",obj.VehicleUId),
                  new SqlParameter("@TDate",TDate),
                  new SqlParameter("@TRefId",obj.TRefId),

                 };

                Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spValidateTripsStartStatus_Shift, parameters);

            }
            catch (Exception ex)
            {
                var EResult = new { Result = 1, Msg = "Please Attach Before And After Photo" };
                string ErrorMsg = JsonConvert.SerializeObject(EResult);
                return ErrorMsg;
            }
            return Result;
        }
        [HttpPost]
        [Route("GetAllPointCollectionDetail_Paging")]
        public IActionResult GetAllPointCollectionDetail_Paging(DataTableAjaxPostModel requestModel)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            Order _order = new Order();

            //string FolderName = @"\content\Point_Collection\";


            //string filePath = Path.Combine(HostingEnvironment.WebRootPath + FolderName);
            ////string filePath = baseUrl + FolderName;

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
                    SortColumn = "RouteCode";
                    break;
                case 3:
                    SortColumn = "TripName";
                    break;
                case 4:
                    SortColumn = "PointName";
                    break;
                case 5:
                    SortColumn = "Status";
                    break;
                case 6:
                    SortColumn = "Address";
                    break;
                case 7:
                    SortColumn = "PickDTime";
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
                  new SqlParameter("@FPath",baseUrl),
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@ToDate",requestModel.ToDate),
                  new SqlParameter("@VehicleNumber",!string.IsNullOrEmpty(requestModel.VehicleUid)?requestModel.VehicleUid:"0"),
                  new SqlParameter("@PointName",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"0"),
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),
                  new SqlParameter("@RouteId",!string.IsNullOrEmpty(requestModel.Route)?requestModel.Route:"0"),
                    // new SqlParameter("@FPath1",filePath),

                };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllPointCollectionDetail_Paging, parameters);
            return Ok(Result);
        }



        [HttpPost]
        [Route("GetAllPointCollectionDetail_PagingB64")]
        public IActionResult GetAllPointCollectionDetail_PagingB64(DataTableAjaxPostModel requestModel)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            Order _order = new Order();

            //string FolderName = @"\content\Point_Collection\";


            //string filePath = Path.Combine(HostingEnvironment.WebRootPath + FolderName);
            ////string filePath = baseUrl + FolderName;

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
                    SortColumn = "RouteCode";
                    break;
                case 3:
                    SortColumn = "TripName";
                    break;
                case 4:
                    SortColumn = "PointName";
                    break;
                case 5:
                    SortColumn = "Status";
                    break;
                case 6:
                    SortColumn = "Address";
                    break;
                case 7:
                    SortColumn = "PickDTime";
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
                  new SqlParameter("@FPath",baseUrl),
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@ToDate",requestModel.ToDate),
                  new SqlParameter("@VehicleNumber",!string.IsNullOrEmpty(requestModel.VehicleUid)?requestModel.VehicleUid:"0"),
                  new SqlParameter("@PointName",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"0"),
                    // new SqlParameter("@FPath1",filePath),

                };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllPointCollectionDetail_Paging, parameters);

            List<AllVehicleB64> Mhlst = JsonConvert.DeserializeObject<List<AllVehicleB64>>(Result);
            string strJson = JsonConvert.SerializeObject(Mhlst);
            return Ok(strJson);

            //return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllRouteWiseCollection_Paging")]
        public IActionResult GetAllRouteWiseCollection_Paging(DataTableAjaxPostModel requestModel)
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
                    SortColumn = "RouteCode";
                    break;
                case 3:
                    SortColumn = "TripName";
                    break;
                case 4:
                    SortColumn = "PointName";
                    break;
                case 5:
                    SortColumn = "Status";
                    break;
                case 6:
                    SortColumn = "Address";
                    break;
                case 7:
                    SortColumn = "PickDTime";
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
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@ToDate",requestModel.ToDate),
                  new SqlParameter("@RouteId",!string.IsNullOrEmpty(requestModel.Route)?requestModel.Route:"0"),
            };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllRouteWiseCollection_Paging, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllColPointByRouteDate")]
        public IActionResult GetAllColPointByRouteDate(JObject obj)
        {
            string RouteId = obj.GetValue("RouteId").Value<string>();
            DateTime TDate = obj.GetValue("TDate").Value<DateTime>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@RouteId",RouteId),
                  new SqlParameter("@TDate",TDate),

             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllColPointByRouteDate, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllPointForTimeline")]
        public IActionResult GetAllPointForTimeline(JObject obj)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");

            DateTime FromDate = obj.GetValue("FromDate").Value<DateTime>();
            DateTime ToDate = obj.GetValue("ToDate").Value<DateTime>();
            string UserId = obj.GetValue("UserId").Value<string>();
            string RouteId = obj.GetValue("RouteId").Value<string>();
            string ZoneId = obj.GetValue("ZoneId").Value<string>();
            string CircleId = obj.GetValue("CircleId").Value<string>();
            string TripId = obj.GetValue("TripId").Value<string>();
            int PageSize = obj.GetValue("PageSize").Value<int>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@FromDate",FromDate),
                  new SqlParameter("@ToDate",ToDate),
                  new SqlParameter("@UserId",UserId),
                  new SqlParameter("@ZoneId",ZoneId),
                  new SqlParameter("@CircleId",CircleId),
                  new SqlParameter("@RouteId",RouteId),
                  new SqlParameter("@TripId",TripId),
                  new SqlParameter("@PageSize",PageSize),
                  new SqlParameter("@FPath",baseUrl),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllPointForTimeline, parameters);//ChangeS

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllEmerPointForTimeline")]
        public IActionResult GetAllEmerPointForTimeline(JObject obj)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");

            DateTime FromDate = obj.GetValue("FromDate").Value<DateTime>();
            DateTime ToDate = obj.GetValue("ToDate").Value<DateTime>();
            string UserId = obj.GetValue("UserId").Value<string>();
            //string RouteId = obj.GetValue("RouteId").Value<string>();
            string ZoneId = obj.GetValue("ZoneId").Value<string>();
            string CircleId = obj.GetValue("CircleId").Value<string>();
            //string TripId = obj.GetValue("TripId").Value<string>();
            int PageSize = obj.GetValue("PageSize").Value<int>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@FromDate",FromDate),
                  new SqlParameter("@ToDate",ToDate),
                  new SqlParameter("@UserId",UserId),
                  new SqlParameter("@ZoneId",ZoneId),
                  new SqlParameter("@CircleId",CircleId),

                  new SqlParameter("@PageSize",PageSize),
                  new SqlParameter("@FPath",baseUrl),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllEmergencyPointForTimeline, parameters);//ChangeS

            return Ok(Result);
        }
        //older version
        [HttpPost]
        //[Route("StartTripAndEndTrip")]
        [Route("StartTripAndEndTrip_V1")]
        public IActionResult StartTripAndEndTrip_V1(JObject obj)
        {
            try
            {
                string RouteId = obj.GetValue("RouteId").Value<string>();
                string RouteCode = obj.GetValue("RouteCode").Value<string>();
                string TripId = obj.GetValue("TripId").Value<string>();
                string VehicleUId = obj.GetValue("VehicleUId").Value<string>();
                string StatusTypeId = obj.GetValue("StatusTypeId").Value<string>();
                string CreatedBy = obj.GetValue("CreatedBy").Value<string>();
                string Lat = obj.GetValue("Lat").Value<string>();
                string Lng = obj.GetValue("Lng").Value<string>();
                bool IsDeviated = obj.GetValue("IsDeviated").Value<bool>();
                string Address = obj.GetValue("Address").Value<string>();
                SqlParameter[] parameters = new SqlParameter[]
                 {
                  new SqlParameter("@RouteId",RouteId),
                  new SqlParameter("@RouteCode",RouteCode),
                  new SqlParameter("@TripId",TripId),
                  new SqlParameter("@VehicleUId",VehicleUId),
                  new SqlParameter("@StatusTypeId",StatusTypeId),
                  new SqlParameter("@CreatedBy",CreatedBy),
                  new SqlParameter("@CreatedDate",CommonHelper.IndianStandard(DateTime.UtcNow)),
                  new SqlParameter("@Lat",Lat),
                  new SqlParameter("@Lng",Lng),
                  new SqlParameter("@Address",Address),
                  new SqlParameter("@IsDeviated",IsDeviated),
                 };

                string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spStartTripAndEndTrip, parameters);

                return Ok(Result);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpPost]
        [Route("StartTripAndEndTrip_N")]
        public IActionResult StartTripAndEndTrip_N(JObject obj)
        {
            try
            {
                string RouteId = obj.GetValue("RouteId").Value<string>();
                string RouteCode = obj.GetValue("RouteCode").Value<string>();
                string TripId = obj.GetValue("TripId").Value<string>();
                string VehicleUId = obj.GetValue("VehicleUId").Value<string>();
                string StatusTypeId = obj.GetValue("StatusTypeId").Value<string>();
                string CreatedBy = obj.GetValue("CreatedBy").Value<string>();
                string Lat = obj.GetValue("Lat").Value<string>();
                string Lng = obj.GetValue("Lng").Value<string>();
                bool IsDeviated = obj.GetValue("IsDeviated").Value<bool>();
                string Address = obj.GetValue("Address").Value<string>();
                string TRefId = CommonHelper.generateID();
                SqlParameter[] parameters = new SqlParameter[]
                 {
                  new SqlParameter("@RouteId",RouteId),
                  new SqlParameter("@RouteCode",RouteCode),
                  new SqlParameter("@TripId",TripId),
                  new SqlParameter("@VehicleUId",VehicleUId),
                  new SqlParameter("@StatusTypeId",StatusTypeId),
                  new SqlParameter("@CreatedBy",CreatedBy),
                  new SqlParameter("@CreatedDate",CommonHelper.IndianStandard(DateTime.UtcNow)),
                  new SqlParameter("@Lat",Lat),
                  new SqlParameter("@Lng",Lng),
                  new SqlParameter("@Address",Address),
                  new SqlParameter("@IsDeviated",IsDeviated),
                  new SqlParameter("@TRefID",TRefId),
                 };

                string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spStartTripAndEndTrip_1, parameters);

                return Ok(Result);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetSRouteInfoById")]
        public IActionResult GetSRouteInfoById(int RouteId)
        {
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@RouteId",RouteId)
             };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetSRouteInfoById, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("AddSRouteInfo")]
        public IActionResult AddSRouteInfo(JObject obj)
        {
            string RouteId = obj.GetValue("RouteId").Value<string>();
            // string RouteName = obj.GetValue("RouteName").Value<string>();
            string RouteCode = obj.GetValue("RouteCode").Value<string>();
            string ZoneId = obj.GetValue("ZoneId").Value<string>();
            string CircleId = obj.GetValue("CircleId").Value<string>();
            string ShiftId = obj.GetValue("ShiftId").Value<string>();
            string IsActive = obj.GetValue("IsActive").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            string CCode = obj.GetValue("CCode").Value<string>();


            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@RouteId",!string.IsNullOrEmpty(RouteId)?RouteId:"0"),
                 new SqlParameter("@RouteCode",RouteCode),
                 new SqlParameter("@IsActive",IsActive),
                 new SqlParameter("@CreatedDate",CommonHelper.IndianStandard(DateTime.UtcNow)),
                 new SqlParameter("@CreatedBy",UserId),
                  new SqlParameter("@RouteName",""),
                  new SqlParameter("@ZoneId",ZoneId),
                  new SqlParameter("@CircleId",CircleId),
                  new SqlParameter("@ShiftId",ShiftId),
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spAddAndUpdateSRoute, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("AllSRouteInfo")]
        public IActionResult AllSRouteInfo(DataTableAjaxPostModel requestModel)
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
            if (requestModel.NotiId == "1")
            {
                switch (requestModel.order[0].column)
                {




                    case 2:
                        SortColumn = "RouteCode";
                        break;
                    case 3:
                        SortColumn = "ZoneNo";
                        break;
                    case 4:
                        SortColumn = "CircleName";
                        break;
                    case 5:
                        SortColumn = "TotalStop";
                        break;
                    case 6:
                        SortColumn = "TotalTrips";
                        break;

                    default:
                        SortDir = String.Empty;
                        SortColumn = string.Empty;
                        break;
                }
            }
            else
            {
                switch (requestModel.order[0].column)
                {



                    //case 1:
                    //    SortColumn = "RouteName";
                    //    break;
                    case 1:
                        SortColumn = "RouteCode";
                        break;
                    case 2:
                        SortColumn = "ZoneNo";
                        break;
                    case 3:
                        SortColumn = "CircleName";
                        break;
                    case 4:
                        SortColumn = "ShiftName";
                        break;
                    case 5:
                        SortColumn = "TotalStop";
                        break;
                    case 6:
                        SortColumn = "TotalTrips";
                        break;
                    default:
                        SortDir = String.Empty;
                        SortColumn = string.Empty;
                        break;
                }
            }

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
                  new SqlParameter("@Status",!string.IsNullOrEmpty(requestModel.ContratorId)?requestModel.ContratorId:"-1"),
                  new SqlParameter("@RouteId",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"0"),
            };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllSRoute_Paging, parameters);
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetAllRouteTripByRoute")]
        public IActionResult GetAllRouteTripByRoute(int RouteId)
        {
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@RouteId",RouteId),
             };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetAllRouteTripByRoute, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("AddRouteTrip")]
        public IActionResult AddRouteTrip(JObject obj)
        {
            string RouteId = obj.GetValue("RouteId").Value<string>();
            string JArrayval = obj.GetValue("JArrayval").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();

            //List<NTripInfo> _lst = JsonConvert.DeserializeObject<List<NTripInfo>>(JArrayval);

            DataTable dt = CommonHelper.toDataTable(JArrayval);
            SqlParameter[] parameters1 = new SqlParameter[]
         {
                  new SqlParameter("@RouteTripType",dt),
                  new SqlParameter("@RouteId",RouteId),
                  new SqlParameter("@CreatedDate",CommonHelper.IndianStandard(DateTime.UtcNow)),
                  new SqlParameter("@CreatedBy",UserId),
         };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spAddRouteTrip, parameters1);



            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllTripPoint_Paging")]
        public IActionResult GetAllTripPoint_Paging(DataTableAjaxPostModel requestModel)
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

                case 2:
                    SortColumn = "RouteCode";
                    break;
                case 3:
                    SortColumn = "TripName";
                    break;
                case 4:
                    SortColumn = "VehicleNo";
                    break;
                case 5:
                    SortColumn = "TId";
                    break;
                case 6:
                    SortColumn = "BufferMin";
                    break;
                case 7:
                    SortColumn = "TotalStop";
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
                  new SqlParameter("@IsReport",requestModel.IsReport),
            };

            //string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllTripPoint_Paging, parameters);//ChangeSS
            //return Ok(Result);
            string Result = string.Empty;
            if (requestModel.IsReport == 1)
                Result = _dataRepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetAllTripPoint_Paging, parameters);
            else
                Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllTripPoint_Paging, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllTripPoint_Paging_New")]
        public IActionResult GetAllTripPoint_Paging_New(DataTableAjaxPostModel requestModel)
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

                case 2:
                    SortColumn = "RouteCode";
                    break;

                case 4:
                    SortColumn = "VehicleNo";
                    break;
                case 5:
                    SortColumn = "TId";
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
                  new SqlParameter("@LoginId",requestModel.UserId)

            };

            string Result = string.Empty;
            Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllTripPoint_Paging_New, parameters);
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetPointsInfoByTripId")]
        public IActionResult GetPointsInfoByTripId(int RTDId)
        {
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@RTDId",RTDId),
             };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetPointsInfoByTripId, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("AddSTripPointInfo")]
        public IActionResult AddSTripPointInfo(JObject obj)//KHSB
        {
            DataTable dt = new DataTable();
            string RouteId = obj.GetValue("RouteId").Value<string>();
            string RouteCode = obj.GetValue("RouteCode").Value<string>();
            int TripId = obj.GetValue("TripId").Value<int>();
            int ShiftId = obj.GetValue("ShiftId").Value<int>();
            string JArrayval = obj.GetValue("JArrayval").Value<string>();

            List<NRouteStopInfo> _lst = JsonConvert.DeserializeObject<List<NRouteStopInfo>>(JArrayval);

            if (ShiftId == 4)
            {
                dt = CommonHelper.ToDataTable(_lst);
            }
            else
            {
                dt = CommonHelper.ToDataTable(_lst.OrderBy(i => i.PickupTime).ToList());
            }
            SqlParameter[] parameters1 = new SqlParameter[]
         {
                  new SqlParameter("@NRouteStopType",dt),
                  new SqlParameter("@RouteCode",RouteCode),
                  new SqlParameter("@RouteId",RouteId),
                  new SqlParameter("@TripId",TripId),
         };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spAddRouteStop, parameters1);

            return Ok(Result);
        }
        [HttpPost]
        [Route("AddVehicleToTrip")]
        public IActionResult AddVehicleToTrip(JObject obj)
        {
            string RTDId = obj.GetValue("RTDId").Value<string>();
            string VehicleUId = obj.GetValue("VehicleUId").Value<string>();

            SqlParameter[] parameters1 = new SqlParameter[]
         {
                  new SqlParameter("@RTDId",RTDId),
                  new SqlParameter("@VehicleUId",VehicleUId),
         };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spAddVehicleToTrip, parameters1);

            return Ok(Result);
        }
        [HttpGet]
        [Route("GetNewRouteCode")]
        public IActionResult GetNewRouteCode(int ZoneId, int CircleId)
        {
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@ZoneId",ZoneId),
                  new SqlParameter("@CircleId",CircleId),
             };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetNewRouteCode, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllRouteWiseCollectionSummary_Paging")]
        public IActionResult GetAllRouteWiseCollectionSummary_Paging(DataTableAjaxPostModel requestModel)
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
                    SortColumn = "TDate";
                    break;
                case 2:
                    SortColumn = "RouteCode";
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
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@ToDate",requestModel.ToDate),
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  new SqlParameter("@RouteId",!string.IsNullOrEmpty(requestModel.Route)?requestModel.Route:"0"),
                  new SqlParameter("@TripId",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"0"),
            };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllRouteWiseCltnSummary, parameters);
            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllGeoPointsVisitSummary")]
        public IActionResult GetAllGeoPointsVisitSummary(DataTableAjaxPostModel requestModel)
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
                    SortColumn = "A.GeoPointName";
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
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@ToDate",requestModel.ToDate),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),
                  new SqlParameter("@VistingType",requestModel.VisitingType),
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  new SqlParameter("@RouteId",!string.IsNullOrEmpty(requestModel.Route)?requestModel.Route:"0"),
                  new SqlParameter("@TripId",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"0"),
                  new SqlParameter("@IsReport",requestModel.IsReport),
                  new SqlParameter("@StatusId",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"-1"),
                  new SqlParameter("@CategoryId",!string.IsNullOrEmpty(requestModel.CategoryId)?requestModel.CategoryId:"0"),
                   new SqlParameter("@VehicleNumber",!string.IsNullOrEmpty(requestModel.VehicleUid)?requestModel.VehicleUid:"0"),

            };
            string Result = string.Empty;
            if (requestModel.IsReport == 1)
                Result = _dataRepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetAllGeoPointsVisitSummary_Paging, parameters);
            else
                Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllGeoPointsVisitSummary_Paging, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllGeoPointVisitByPointId")]
        public IActionResult GetAllGeoPointVisitByPointId(JObject obj)
        {
            int PointId = obj.GetValue("Pointid").Value<int>();
            string Zoneid = obj.GetValue("ZoneId").Value<string>();
            string CircleId = obj.GetValue("CircleId").Value<string>();
            string WardId = obj.GetValue("WardId").Value<string>();
            string RouteId = obj.GetValue("RouteId").Value<string>();
            string TripId = obj.GetValue("TripId").Value<string>();
            string Visitingtype = obj.GetValue("Visitingtype").Value<string>();
            //int ShiftId = obj.GetValue("ShiftId").Value<int>();
            DateTime FromDate = obj.GetValue("FromDate").Value<DateTime>();
            DateTime ToDate = obj.GetValue("ToDate").Value<DateTime>();
            SqlParameter[] parameters = new SqlParameter[]
          {
                  new SqlParameter("@PointId",PointId),
                  new SqlParameter("@ZoneId",Zoneid),
                  new SqlParameter("@CircleId",CircleId),
                  new SqlParameter("@WardId",WardId),
                  new SqlParameter("@RouteId",RouteId),
                  new SqlParameter("@TripId",TripId),
                  new SqlParameter("@VisitingType",Visitingtype),
                  new SqlParameter("@Fromdate",FromDate),
                  new SqlParameter("@ToDate",ToDate),
          };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllGeoPointVisitTagByPointId, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllPointNoti")]
        public IActionResult GetAllPointNoti(JObject obj)
        {
            string Zoneid = obj.GetValue("ZoneId").Value<string>();
            string CircleId = obj.GetValue("CircleId").Value<string>();
            string WardId = obj.GetValue("WardId").Value<string>();
            string RouteId = obj.GetValue("RouteId").Value<string>();
            string TripId = obj.GetValue("TripId").Value<string>();
            string LoginId = obj.GetValue("LoginId").Value<string>();
            string fromDate = obj.GetValue("fromDate").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                 new SqlParameter("@LoginId",LoginId),
                 new SqlParameter("@ZoneId",Zoneid),
                 new SqlParameter("@CircleId",CircleId),
                 new SqlParameter("@WardId",WardId),
                  new SqlParameter("@RouteId",RouteId),
                  new SqlParameter("@TripId",TripId),
                   new SqlParameter("@SDate",fromDate),
             };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetAllPointNoti, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllEmerGencyPointNoti")]
        public IActionResult GetAllEmerGencyPointNoti(JObject obj)
        {
            string Zoneid = obj.GetValue("ZoneId").Value<string>();
            string CircleId = obj.GetValue("CircleId").Value<string>();
            string WardId = obj.GetValue("WardId").Value<string>();

            string LoginId = obj.GetValue("LoginId").Value<string>();
            string fromDate = obj.GetValue("fromDate").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                 new SqlParameter("@LoginId",LoginId),
                 new SqlParameter("@ZoneId",Zoneid),
                 new SqlParameter("@CircleId",CircleId),
                 new SqlParameter("@WardId",WardId),

                   new SqlParameter("@SDate",fromDate),
             };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetAllEmerGencyPointNoti, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllZoneWiseCltnNoti")]
        public IActionResult GetAllZoneWiseCltnNoti(JObject obj)
        {
            string Zoneid = obj.GetValue("ZoneId").Value<string>();
            string CircleId = obj.GetValue("CircleId").Value<string>();
            string WardId = obj.GetValue("WardId").Value<string>();
            string RouteId = obj.GetValue("RouteId").Value<string>();
            string TripId = obj.GetValue("TripId").Value<string>();
            string LoginId = obj.GetValue("LoginId").Value<string>();
            string fromDate = obj.GetValue("fromDate").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                 new SqlParameter("@LoginId",LoginId),
                 new SqlParameter("@ZoneId",Zoneid),
                 new SqlParameter("@CircleId",CircleId),
                 new SqlParameter("@WardId",WardId),
                  new SqlParameter("@RouteId",RouteId),
                  new SqlParameter("@TripId",TripId),
                   new SqlParameter("@SDate",fromDate),
             };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetAllZoneWiseCollectionNoti, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllZoneWiseCollectionNoti")]
        public IActionResult GetAllZoneWiseCollectionNoti(DataTableAjaxPostModel requestModel)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            Order _order = new Order();

            //string SearchTXT = requestModel.search.value;
            int draw = requestModel.draw;
            int start = requestModel.start;
            int length = requestModel.length;

            string str = string.Empty;
            //if (SearchTXT != null)
            //  str = SearchTXT.Trim();

            string SortColumn = string.Empty;
            // string SortDir = requestModel.order[0].dir;
            //switch (requestModel.order[0].column)
            //{

            //    case 1:
            //        SortColumn = "TotalPointZ1";
            //        break;
            //    case 2:
            //        SortColumn = "CollectionPointZ1";
            //        break;
            //    case 3:
            //        SortColumn = "NotCollectedZ1";
            //        break;
            //    case 4:
            //        SortColumn = "UniqueCollectionZ1";
            //        break;
            //    case 5:
            //        SortColumn = "MoreThanOnceZ1";
            //        break;
            //    case 6:
            //        SortColumn = "RouteCountz1";
            //        break;
            //    case 7:
            //        SortColumn = "TripCountZ1";
            //        break;
            //    case 8:
            //        SortColumn = "RoutesProgressZ1";
            //        break;
            //    case 9:
            //        SortColumn = "EarlyCompletionZ1";
            //        break;
            //    case 10:
            //        SortColumn = "LateCompletionZ1";
            //        break;


            //    default:
            //       // SortDir = String.Empty;
            //        SortColumn = string.Empty;
            //        break;
            //}

            SqlParameter[] parameters = new SqlParameter[]
            {
                 new SqlParameter("@SearchTerm",""),
                  new SqlParameter("@SortColumn",""),
                  new SqlParameter("@SortOrder",""),
                  new SqlParameter("@PageNumber",start),
                  new SqlParameter("@PageSize",length),
                  new SqlParameter("@LoginId",requestModel.UserId),
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                   new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                   new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),

            };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.sp_GetAllCountGeoCollectionCount, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllGeoPointNoti_paging")]
        public IActionResult GetAllGeoPointNoti_paging(DataTableAjaxPostModel requestModel)
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
                case 2:
                    SortColumn = "A.GeoPointName";
                    break;
                case 3:
                    SortColumn = "B.GeoPointCategory";
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
                case 7:
                    SortColumn = "A.IsActive";
                    break;

                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@SearchTerm",""),
                  new SqlParameter("@SortColumn",""),
                  new SqlParameter("@SortOrder",""),
                  new SqlParameter("@PageNumber",start),
                  new SqlParameter("@PageSize",length),
                  new SqlParameter("@LoginId",requestModel.UserId),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.WardId:"0"),
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                 // new SqlParameter("@PointId",!string.IsNullOrEmpty(requestModel.PointId)?requestModel.PointId:"1"),
            };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllGVPPoint_Paging, parameters);
            return Ok(Result);

        }
        [HttpPost]
        [Route("AllRouteTripNotiInfo")]
        public IActionResult AllRouteTripNotiInfo(DataTableAjaxPostModel requestModel)
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



                //case 1:
                //    SortColumn = "RouteName";
                //    break;
                case 1:
                    SortColumn = "B.RouteCode";
                    break;
                case 2:
                    SortColumn = "A.TId";
                    break;
                case 3:
                    SortColumn = "D.ZoneNo";
                    break;
                case 4:
                    SortColumn = "E.CircleName";
                    break;
                case 5:
                    SortColumn = "C.VehicleNo";
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
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  new SqlParameter("@Status",!string.IsNullOrEmpty(requestModel.ContratorId)?requestModel.ContratorId:"-1"),
                  new SqlParameter("@RouteId",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"0"),
                  new SqlParameter("@TripId",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"0"),
            };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllRTripNoti_Paging, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("AllPointCltNoti_Paging")]
        public IActionResult AllPointCltNoti_Paging(DataTableAjaxPostModel requestModel)
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



                //case 1:
                //    SortColumn = "RouteName";
                //    break;
                case 1:
                    SortColumn = "A.GeoPointName";
                    break;
                case 2:
                    SortColumn = "B.GeoPointCategory";
                    break;
                case 3:
                    SortColumn = "C.ZoneNo";
                    break;
                case 4:
                    SortColumn = "D.CircleName";
                    break;
                case 5:
                    SortColumn = "E.WardNo";
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
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),

                  new SqlParameter("@RouteId",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"0"),
                  new SqlParameter("@TripId",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"0"),
                   new SqlParameter("@TypeId",requestModel.ContratorId),
                   new SqlParameter("@SDate",requestModel.FromDate),
            };
            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllPointCltNoti_Paging, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("AllPointEarlyCompletion_Paging")]
        public IActionResult AllPointEarlyCompletion_Paging(DataTableAjaxPostModel requestModel)
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




                case 2:
                    SortColumn = "RouteCode";
                    break;
                case 3:
                    SortColumn = "ZoneNo";
                    break;
                case 4:
                    SortColumn = "CircleName";
                    break;
                case 5:
                    SortColumn = "TotalStop";
                    break;
                case 6:
                    SortColumn = "TotalTrips";
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
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  new SqlParameter("@Status",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"-1"),
                  new SqlParameter("@RouteId",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"0"),
                  new SqlParameter("@TypeId",requestModel.ContratorId),
                  new SqlParameter("@SDate",requestModel.FromDate),
            };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllEarlyLateCompletion_Paging, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllEarlyCompletionByRouteId")]
        public IActionResult GetAllEarlyCompletionByRouteId(JObject obj)
        {
            string RouteId = obj.GetValue("RouteId").Value<string>();
            string TypeId = obj.GetValue("TypeId").Value<string>();
            string TripId = obj.GetValue("TripId").Value<string>();
            string SDate = obj.GetValue("SDate").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@RouteId",RouteId),
                   new SqlParameter("@TypeId",TypeId),
                   new SqlParameter("@TripId",TripId),
                   new SqlParameter("@SDate",SDate),
             };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetAllEarlyCompletionByRouteId, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("AllGeoPointNotCollected_Paging")]
        public IActionResult AllGeoPointNotCollected_Paging(DataTableAjaxPostModel requestModel)
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
                case 2:
                    SortColumn = "A.AssetCode";
                    break;
                case 3:
                    SortColumn = "A.AssetName";
                    break;
                case 4:
                    SortColumn = "A.ImeiNo";
                    break;
                case 5:
                    SortColumn = "A.OtherNo";
                    break;
                case 6:
                    SortColumn = "A.WardNo";
                    break;
                case 7:
                    SortColumn = "A.ZoneNo";
                    break;
                case 9:
                    SortColumn = "C.ReaderCategory";
                    break;
                case 10:
                    SortColumn = "C.ReaderVehicleNo";
                    break;
                case 15:
                    SortColumn = "B.Dated";
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
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@ToDate",requestModel.ToDate),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),

                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),


                  new SqlParameter("@StatusId",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"-1"),
                  new SqlParameter("@CategoryId",!string.IsNullOrEmpty(requestModel.CategoryId)?requestModel.CategoryId:"0"),
                  new SqlParameter("@PointId",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"0"),


            };
            string Result = string.Empty;

            Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllGeoPointsNotCollected_Paging, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllVisitSummaryForMap_Paging")]
        public IActionResult GetAllVisitSummaryForMap_Paging(DataTableAjaxPostModel requestModel)
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
            if (requestModel.ContratorId == "1")
            {
                switch (requestModel.order[0].column)
                {

                    case 1:
                        SortColumn = "A.GeoPointName";
                        break;

                    case 2:
                        SortColumn = "A.GeoPointCategory";
                        break;
                    case 3:
                        SortColumn = "C.ZoneNo";
                        break;
                    case 4:
                        SortColumn = "D.CircleName";
                        break;

                    default:
                        SortDir = String.Empty;
                        SortColumn = string.Empty;
                        break;
                }
            }
            else
            {
                switch (requestModel.order[0].column)
                {

                    case 3:
                        SortColumn = "A.GeoPointName";
                        break;

                    case 4:
                        SortColumn = "A.GeoPointCategory";
                        break;
                    case 5:
                        SortColumn = "C.ZoneNo";
                        break;
                    case 6:
                        SortColumn = "D.CircleName";
                        break;
                    case 7:
                        SortColumn = "E.WardNo";
                        break;

                    case 9:
                        SortColumn = "A.Radius";
                        break;
                    case 10:
                        SortColumn = "A.Remarks";
                        break;
                    case 11:
                        SortColumn = "A.CreatedBy";
                        break;
                    case 12:
                        SortColumn = "A.ModifiedDate";
                        break;

                    default:
                        SortDir = String.Empty;
                        SortColumn = string.Empty;
                        break;
                }
            }
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
                   new SqlParameter("@RouteId",!string.IsNullOrEmpty(requestModel.Route)?requestModel.Route:"0"),
                  new SqlParameter("@TripId",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"0"),
                  new SqlParameter("@StatusId",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"-1"),
                  new SqlParameter("@VisitCount",!string.IsNullOrEmpty(requestModel.ContratorId)?requestModel.ContratorId:"-1"),
                   new SqlParameter("@SDate",requestModel.FromDate),
                };
            //spGetAllVisitSummaryForMap_Paging
            //string Result = _dataRepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetAllVisitSummaryForMap_Paging, parameters);
            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllVisitSummaryForMap_Paging, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllVisitSummaryForEmergencyMap_Paging")]
        public IActionResult GetAllVisitSummaryForEmergencyMap_Paging(DataTableAjaxPostModel requestModel)
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
            if (requestModel.ContratorId == "1")
            {
                switch (requestModel.order[0].column)
                {

                    case 1:
                        SortColumn = "A.GeoPointName";
                        break;
                    case 3:
                        SortColumn = "C.ZoneNo";
                        break;
                    case 4:
                        SortColumn = "D.CircleName";
                        break;

                    default:
                        SortDir = String.Empty;
                        SortColumn = string.Empty;
                        break;
                }
            }
            else
            {
                switch (requestModel.order[0].column)
                {

                    case 3:
                        SortColumn = "A.GeoPointName";
                        break;


                    case 5:
                        SortColumn = "C.ZoneNo";
                        break;
                    case 6:
                        SortColumn = "D.CircleName";
                        break;
                    case 7:
                        SortColumn = "E.WardNo";
                        break;

                    case 9:
                        SortColumn = "A.Radius";
                        break;
                    case 10:
                        SortColumn = "A.Remarks";
                        break;


                    default:
                        SortDir = String.Empty;
                        SortColumn = string.Empty;
                        break;
                }
            }
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
                  new SqlParameter("@VisitCount",!string.IsNullOrEmpty(requestModel.ContratorId)?requestModel.ContratorId:"-1"),
                   new SqlParameter("@SDate",requestModel.FromDate),
                };
            //spGetAllVisitSummaryForMap_Paging
            //string Result = _dataRepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetAllVisitSummaryForMap_Paging, parameters);
            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetEmergencyDataForMap_Paging, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("GetTotalUniqueVehicle")]
        public IActionResult GetTotalUniqueVehicle(DataTableAjaxPostModel requestModel)
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
                    SortColumn = "UId";
                    break;

                case 4:
                    SortColumn = "VehicleType";
                    break;
                case 5:
                    SortColumn = "OwnerType";
                    break;
                case 6:
                    SortColumn = "ZoneNo";
                    break;
                case 7:
                    SortColumn = "CircleName";
                    break;
                case 8:
                    SortColumn = "WardNo";
                    break;
                case 9:
                    SortColumn = "DriverName";
                    break;
                case 10:
                    SortColumn = "ContactNo";
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
                     new SqlParameter("@RouteId",!string.IsNullOrEmpty(requestModel.Route)?requestModel.Route:"0"),
                  new SqlParameter("@TripId",!string.IsNullOrEmpty(requestModel.ContratorId)?requestModel.ContratorId:"0"),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetTotalVehicleforindex, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetTotalAssignedMinitrippers")]
        public IActionResult GetTotalAssignedMinitrippers(DataTableAjaxPostModel requestModel)
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
                    new SqlParameter("@RouteId",!string.IsNullOrEmpty(requestModel.Route)?requestModel.Route:"0"),
                  new SqlParameter("@TripId",!string.IsNullOrEmpty(requestModel.ContratorId)?requestModel.ContratorId:"0"),
             };
            string Result = "";
            if (requestModel.NotiId == "1")
            {
                Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetTotalDeployedVehicle_Paging, parameters);
            }
            else
            {
                Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetTotalDeployedVehicleNotAssigned_Paging, parameters);
            }


            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllZoneWiseSummary_Paging")]
        public IActionResult GetAllZoneWiseSummary_Paging(DataTableAjaxPostModel requestModel)
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

                case 3:
                    SortColumn = "A.GeoPointName";
                    break;

                case 4:
                    SortColumn = "A.GeoPointCategory";
                    break;
                case 5:
                    SortColumn = "C.ZoneNo";
                    break;
                case 6:
                    SortColumn = "D.CircleName";
                    break;
                case 7:
                    SortColumn = "E.WardNo";
                    break;

                case 9:
                    SortColumn = "A.Radius";
                    break;
                case 10:
                    SortColumn = "A.Remarks";
                    break;
                case 11:
                    SortColumn = "A.CreatedBy";
                    break;
                case 12:
                    SortColumn = "A.ModifiedDate";
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
                   new SqlParameter("@FPath",baseUrl),
                   new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                   new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                   new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),
                   new SqlParameter("@PointId",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"-1"),
                   new SqlParameter("@StatusId",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"-1"),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllZoneWiseSummary_Paging, parameters);
            return Ok(Result);
        }

        [HttpPost]
        [Route("AllPointSummaryNoti_Paging")]
        public IActionResult AllPointSummaryNoti_Paging(DataTableAjaxPostModel requestModel)
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



                //case 1:
                //    SortColumn = "RouteName";
                //    break;
                case 1:
                    SortColumn = "A.GeoPointName";
                    break;
                case 2:
                    SortColumn = "B.GeoPointCategory";
                    break;
                case 3:
                    SortColumn = "C.ZoneNo";
                    break;
                case 4:
                    SortColumn = "D.CircleName";
                    break;
                case 5:
                    SortColumn = "E.WardNo";
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
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),

                  new SqlParameter("@RouteId",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"0"),
                  new SqlParameter("@TripId",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:"0"),
                   new SqlParameter("@TypeId",requestModel.ContratorId),
                   new SqlParameter("@SDate", requestModel.FromDate),
            };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllPointSummary_Paging, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("SaveActualGreyPoint")]
        public IActionResult SaveActualGreyPoint(JObject obj)
        {
            string PointId = obj.GetValue("PointId").Value<string>();
            // string PointName = obj.GetValue("PointName").Value<string>();
            string PClId = obj.GetValue("PClId").Value<string>();

            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@PointId",Convert.ToInt32(PointId)),
                  // new SqlParameter("@PointName",PointName),
                   
                   new SqlParameter("@PClId",Convert.ToInt32(PClId)),
             };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.sp_SavegreypointintoActual, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("DeletegreypointintoActual")]
        public IActionResult DeletegreypointintoActual(JObject obj)
        {


            string PClId = obj.GetValue("PClId").Value<string>();

            SqlParameter[] parameters = new SqlParameter[]
             {


                   new SqlParameter("@PClId",Convert.ToInt32(PClId)),
             };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.sp_DeletegreypointintoActual, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllEmergencyPointDetail_Paging")]
        public IActionResult GetAllEmergencyPointDetail_Paging(DataTableAjaxPostModel requestModel)
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

                case 4:
                    SortColumn = "VehicleNo";
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

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetEmrVehicleDetail_Paging, parameters);
            return Ok(Result);
        }

        [HttpPost]
        [Route("GetEmergencyPointsInfo")]
        public IActionResult GetEmergencyPointsInfo(DataTableAjaxPostModel requestModel)
        {
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@EmrUId",requestModel.VehicleUid),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetEmergencyPoints, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllDPointsName")]
        public IActionResult GetAllDPointsName(DataTableAjaxPostModel requestModel)
        {
            // int WardId = obj.GetValue("WardId").Value<int>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                 //new SqlParameter("@WardId",WardId)
              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllDPointsName, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllDEmergencyPointsName")]
        public IActionResult GetAllDEmergencyPointsName(DataTableAjaxPostModel requestModel)
        {
            // int WardId = obj.GetValue("WardId").Value<int>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                 //new SqlParameter("@WardId",WardId)
              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllEmergencyDPointsName, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllDPointsName1")]
        public IActionResult GetAllDPointsName1(JObject obj)
        {
            int WardId = obj.GetValue("WardId").Value<int>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                 new SqlParameter("@WardId",WardId)
              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllDPointsName1, parameters);
            return Ok(Result);
        }

        [HttpPost]
        [Route("InsertEmrPointdetail")]
        public IActionResult InsertEmrPointdetail(JObject obj)
        {
            string VehicleUid = obj.GetValue("VehicleUid").Value<string>();
            string SheuleTime = obj.GetValue("SheuleTime").Value<string>();
            string Remarks = obj.GetValue("Remarks").Value<string>();
            int DDLId = obj.GetValue("DDLId").Value<int>();
            int ZoneId = obj.GetValue("ZoneId").Value<int>();
            int CircleId = obj.GetValue("CircleId").Value<int>();
            int WardId = obj.GetValue("WardId").Value<int>();
            int CategoryId = obj.GetValue("CategoryId").Value<int>();
            string PickupTime = obj.GetValue("PickupTime").Value<string>();
            int isactive = obj.GetValue("isactive").Value<int>();
            string UserId = obj.GetValue("UserId").Value<string>();
            string jobjS = obj.GetValue("jobjS").Value<string>();


            DataTable dt = JsonConvert.DeserializeObject<DataTable>(jobjS);//KHBS
            DataTable dt1 = new DataTable();

            dt1.Columns.Add(new DataColumn("PointId", typeof(int)));
            dt1.Columns.Add(new DataColumn("PointName", typeof(string)));
            //dt1.Columns.Add(new DataColumn("Lat", typeof(string)));
            //dt1.Columns.Add(new DataColumn("Lng", typeof(string)));
            //dt1.Columns.Add(new DataColumn("TypeId", typeof(int)));

            dt1.Columns.Add(new DataColumn("CreatedDate", typeof(DateTime)));
            dt1.Columns.Add(new DataColumn("CretedBy", typeof(string)));
            dt1.Columns.Add(new DataColumn("ModifyDate", typeof(DateTime)));
            dt1.Columns.Add(new DataColumn("ModifyBy", typeof(string)));
            dt1.Columns.Add(new DataColumn("RefEmrUid", typeof(int)));
            dt1.Columns.Add(new DataColumn("OrderId", typeof(int)));
            //dt1.Columns.Add(new DataColumn("Radius", typeof(string)));
            dt1.Columns.Add(new DataColumn("CategoryId", typeof(int)));
            dt1.Columns.Add(new DataColumn("PickupTime", typeof(DateTime)));
            DateTime Tdate = CommonHelper.IndianStandard(DateTime.UtcNow); ;
            //DateTime ModifyDate = DateTime.Now;
            string userName = this.User.GetUserName();
            //DataTable dtMaster = new DataTable();
            //dtMaster.Columns.Add(new DataColumn("PointId", typeof(int)));
            //dtMaster.Columns.Add(new DataColumn("PointName", typeof(string)));
            //dtMaster.Columns.Add(new DataColumn("Lat", typeof(string)));
            //dtMaster.Columns.Add(new DataColumn("Lng", typeof(string)));
            //dtMaster.Columns.Add(new DataColumn("TypeId", typeof(int)));
            //dtMaster.Columns.Add(new DataColumn("CreatedDate", typeof(DateTime)));
            //dtMaster.Columns.Add(new DataColumn("CretedBy", typeof(string)));
            //dtMaster.Columns.Add(new DataColumn("ModifyDate", typeof(DateTime)));
            //dtMaster.Columns.Add(new DataColumn("ModifyBy", typeof(string)));
            //dtMaster.Columns.Add(new DataColumn("RefEmrUid", typeof(int)));
            //dtMaster.Columns.Add(new DataColumn("OrderId", typeof(int)));
            //dtMaster.Columns.Add(new DataColumn("Radius", typeof(string)));
            //dtMaster.Columns.Add(new DataColumn("ZoneId", typeof(int)));
            //dtMaster.Columns.Add(new DataColumn("CircleId", typeof(int)));
            //dtMaster.Columns.Add(new DataColumn("WardId", typeof(int)));
            //dtMaster.Columns.Add(new DataColumn("CategoryId", typeof(int)));
            //dtMaster.Columns.Add(new DataColumn("PickupTime", typeof(DateTime)));

            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {

                dt1.Rows.Add(Convert.ToInt32(dt.Rows[i]["PointId"]),

                     dt.Rows[i]["PointName"].ToString(),
                     //dt.Rows[i]["Lat"].ToString(),
                     //dt.Rows[i]["Lng"].ToString(),
                     //Convert.ToInt32(dt.Rows[i]["TypeId"]),

                     Tdate,
                    // dt.Rows[i]["CretedBy"].ToString(),
                    userName,
                     Tdate,
                     userName,
                     //dt.Rows[i]["ModifyBy"].ToString(),
                     Convert.ToInt32(dt.Rows[i]["RefEmrUid"]),
                     Convert.ToInt32(dt.Rows[i]["OrderId"])
                      //dt.Rows[i]["Radius"].ToString()
                      , Convert.ToInt32(dt.Rows[i]["Category"])
                    , Convert.ToDateTime(dt.Rows[i]["PickupTime"])

                      );
            }


            //          for (int i = 0; i <= dt.Rows.Count - 1; i++)
            //          {

            //              dtMaster.Rows.Add(Convert.ToInt32(dt.Rows[i]["PointId"]),

            //                   dt.Rows[i]["PointName"].ToString(),
            //                   dt.Rows[i]["Lat"].ToString(),
            //                   dt.Rows[i]["Lng"].ToString(),
            //                   Convert.ToInt32(dt.Rows[i]["TypeId"]),

            //                   Tdate,
            //                  // dt.Rows[i]["CretedBy"].ToString(),
            //                  userName,
            //                   Tdate,
            //                   userName,
            //                   //dt.Rows[i]["ModifyBy"].ToString(),
            //                   Convert.ToInt32(dt.Rows[i]["RefEmrUid"]),
            //                   Convert.ToInt32(dt.Rows[i]["OrderId"]),
            //                   dt.Rows[i]["Radius"].ToString()
            //                  ,0
            //                  ,0
            //                  ,0
            //, Convert.ToInt32(dt.Rows[i]["Category"])
            //                  , Convert.ToDateTime(dt.Rows[i]["PickupTime"])
            //                    );
            //          }

            string Result = "";
            SqlParameter[] parameters = new SqlParameter[]
            {

                    new SqlParameter("@PointType",dt1),
                    //new SqlParameter("@PointType1",dtMaster),
                   new SqlParameter("@VehicleUId",VehicleUid),
                  new SqlParameter("@SheduleTime",SheuleTime),
                  new SqlParameter("@Remarks",Remarks),
                   new SqlParameter("@CID",DDLId),
                   new SqlParameter("@CreatedDate",Tdate),
                   new SqlParameter("@CretedBy",UserId),
                   new SqlParameter("@ModifyDate",Tdate),
                   new SqlParameter("@ModifyBy",UserId),
                   new SqlParameter("@ZoneId",ZoneId),
                   new SqlParameter("@CircleId",CircleId),

                   new SqlParameter("@WardId",WardId),
                   new SqlParameter("@CategoryId",CategoryId),
                   new SqlParameter("@PickupTime",PickupTime),
                   new SqlParameter("@IsActive",isactive),
            };
            Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spInsertEmrPointdetail, parameters);

            return Ok(Result);
        }


        [HttpPost]
        [Route("GetEmergencyCollection")]
        public IActionResult GetEmergencyCollection(JObject obj)
        {

            string VehicleUId = obj.GetValue("VehicleUId").Value<string>();
            string ContactNo = obj.GetValue("ContactNo").Value<string>();

            SqlParameter[] parameters = new SqlParameter[]
            {

                new SqlParameter("@VehicleUId",VehicleUId),
                new SqlParameter("@ContactNo",ContactNo),


            };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.SP_GetEmrCollectionDetails, parameters);

            return Ok(Result);
        }

        [HttpGet]
        [Route("GetEmergencyPointsInfo1")]
        public IActionResult GetEmergencyPointsInfo1(int EmrUid)
        {
            SqlParameter[] parameters = new SqlParameter[]
             {
                new SqlParameter("@EmrUId",EmrUid),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetEmergencyPoints, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("ViewEmergencyCollection")]
        public IActionResult ViewEmergencyCollection(int EmrUid)
        {

            SqlParameter[] parameters = new SqlParameter[]
            {

                new SqlParameter("@EmrUId",EmrUid),

            };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.SP_ViewEmrPointDetails, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("AddEmerGencyPointCollectTrans")]
        public async Task<IActionResult> AddEmerGencyPointCollectTrans([FromForm] IFormCollection value)
        {
            string ErrorMsg = string.Empty;
            if (value.Files.Count < 2)
            {
                var EResult = new { Result = 0, Msg = "Please Attach Before And After Photo" };
                ErrorMsg = JsonConvert.SerializeObject(EResult);
                return Ok(ErrorMsg);
            }
            try
            {
                DateTime TDate = CommonHelper.IndianStandard(DateTime.UtcNow);
                string FName = string.Empty;
                string FName1 = string.Empty;
                EmergencyPointCollectionInfo obj = JsonConvert.DeserializeObject<EmergencyPointCollectionInfo>(value["Input"]);
                string FolderName = "/content/Point_Collection/";
                if (value.Files.Count > 0)
                    if (value.Files[0].Length > 0)
                        FName = CommonHelper.generateID() + Path.GetExtension(value.Files[0].FileName);

                if (value.Files.Count > 1)
                    if (value.Files[1].Length > 0)
                        FName1 = CommonHelper.generateID() + Path.GetExtension(value.Files[1].FileName);

                string filePath = Path.Combine(HostingEnvironment.WebRootPath + FolderName, FName);
                string filePath1 = Path.Combine(HostingEnvironment.WebRootPath + FolderName, FName1);

                SqlParameter[] parameters = new SqlParameter[]
                  {
                  new SqlParameter("@PointId",obj.PointId),
                  new SqlParameter("@PointName",obj.PointName),
                  new SqlParameter("@PickUpTime",TDate),
                  //new SqlParameter("@RouteCode",obj.RouteCode),
                  new SqlParameter("@EmrUId",obj.EmrUId),
                  new SqlParameter("@Lat",obj.Lat),
                  new SqlParameter("@Lng",obj.Lng),
                  new SqlParameter("@Address",obj.Address),
                  new SqlParameter("@Remarks",obj.Remarks),
                  new SqlParameter("@ImgUrl1",FName),
                  new SqlParameter("@ImgUrl2",FName1),
                  new SqlParameter("@CreatedDate",TDate),
                  new SqlParameter("@CreatedBy",obj.LoginId),
                 // new SqlParameter("@TripId",obj.TripId),
                  new SqlParameter("@VehicleUId",obj.VehicleUId),
                  new SqlParameter("@FolderName",FolderName),
                  new SqlParameter("@IsDeviated",obj.IsDeviated),
                  new SqlParameter("@ActPointId",obj.ActPointId),
                  new SqlParameter("@AppVersion",!string.IsNullOrEmpty(obj.AppVersion)?obj.AppVersion:string.Empty),
                  new SqlParameter("@BPhotoStamp",TDate),
                  new SqlParameter("@TypeId",obj.TypeId),
                  };

                string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spAddEmergencyPointCollectTrans, parameters);
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
                    if (value.Files.Count > 1)
                    {
                        if (value.Files[1].Length > 0)
                            using (var fileStream = new FileStream(filePath1, FileMode.Create))
                            {
                                await value.Files[1].CopyToAsync(fileStream);
                            }
                    }
                }
                return Ok(Result);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetAllEmergencyPointCollectionDetail_Paging")]
        public IActionResult GetAllEmergencyPointCollectionDetail_Paging(DataTableAjaxPostModel requestModel)
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
                    SortColumn = "Status";
                    break;
                case 3:
                    SortColumn = "Address";
                    break;
                case 4:
                    SortColumn = "PickDTime";
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
                  new SqlParameter("@FPath",baseUrl),
                  new SqlParameter("@FromDate",requestModel.FromDate),
                  new SqlParameter("@ToDate",requestModel.ToDate),
                  new SqlParameter("@VehicleNumber",!string.IsNullOrEmpty(requestModel.VehicleUid)?requestModel.VehicleUid:"0"),


            };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllEmergency_PointCollectionDetail_Paging, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("GetEmergencyPointsInfoById")]
        public IActionResult GetEmergencyPointsInfoById(JObject obj)
        {
            int EmrUid = obj.GetValue("EmrUid").Value<int>();

            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@EmrUId",EmrUid),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetEmergencyVehicleInfoById, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllDPointsNameByZoneCircle")]
        public IActionResult GetAllDPointsNameByZoneCircle(JObject obj)
        {
            int WardId = obj.GetValue("WardId").Value<int>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                 new SqlParameter("@WardId",WardId)
              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllPointNameByZoneCircle, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("AllEmergencyPointCltNoti_Paging")]
        public IActionResult AllEmergencyPointCltNoti_Paging(DataTableAjaxPostModel requestModel)
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



                //case 1:
                //    SortColumn = "RouteName";
                //    break;
                case 1:
                    SortColumn = "A.PointName";
                    break;
                case 2:
                    SortColumn = "B.GeoPointCategory";
                    break;
                case 3:
                    SortColumn = "C.ZoneNo";
                    break;
                case 4:
                    SortColumn = "D.CircleName";
                    break;
                case 5:
                    SortColumn = "E.WardNo";
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
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),

                   new SqlParameter("@TypeId",requestModel.ContratorId),
                   new SqlParameter("@SDate",requestModel.FromDate),
            };
            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllEmerPointCltNoti_Paging, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("ExportAllCollectdPoint")]
        public byte[] ExportAllCollectdPoint(string result, string Name)
        {
            string ContentType = string.Empty;
            DateTime ReportTime = DateTime.Now;
            byte[] Response = null;
            JArray jresult = JArray.Parse(result);
            try
            {
                using (ExcelPackage excel = new ExcelPackage())
                {
                    int Pic1 = 1;
                    int Pic2 = 2;
                    int Count = 1;
                    int RowCount = 4;
                    excel.Workbook.Properties.Author = "Ajeevi Technologies";
                    excel.Workbook.Properties.Title = Name;

                    ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add(Name);

                    #region Creating Header
                    worksheet.Cells[1, 1, 1, 10].Value = Name; worksheet.Cells[1, 1, 2, 10].Merge = true;


                    worksheet.Cells[3, 1].Value = "Sr No.";
                    worksheet.Cells[3, 2].Value = "Vehicle No"; worksheet.Column(2).Width = 25;
                    worksheet.Cells[3, 3].Value = "RouteCode"; worksheet.Column(3).Width = 25;
                    worksheet.Cells[3, 4].Value = "TripName"; worksheet.Column(4).Width = 25;
                    worksheet.Cells[3, 5].Value = "Point Name"; worksheet.Column(5).Width = 25;
                    worksheet.Cells[3, 6].Value = "Status"; worksheet.Column(6).Width = 15;
                    worksheet.Cells[3, 7].Value = "Address"; worksheet.Column(7).Width = 25;
                    worksheet.Cells[3, 8].Value = "Collection DateTime"; worksheet.Column(8).Width = 25;
                    worksheet.Cells[3, 9].Value = "Before Photo"; worksheet.Column(9).Width = 25;
                    worksheet.Cells[3, 10].Value = "After Photo"; worksheet.Column(10).Width = 25;



                    var cells = worksheet.Cells[3, 1, 3, 10];
                    var fill = cells.Style.Fill;
                    //cells.AutoFitColumns();
                    cells.Style.Locked = true;
                    cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    cells.Style.Font.Bold = true;
                    cells.Style.WrapText = true;
                    cells.Style.ShrinkToFit = true;

                    fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(Color.FromArgb(197, 190, 151));

                    var border = cells.Style.Border;
                    border.Top.Style = border.Left.Style = border.Bottom.Style = border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                    #endregion



                    #region filling data

                    foreach (JObject item in jresult)
                    {

                        worksheet.Cells[RowCount, 1].Value = Count;



                        worksheet.Cells[RowCount, 2].Value = item.GetValue("VehicleNo").ToString();
                        worksheet.Cells[RowCount, 3].Value = item.GetValue("RouteCode").ToString();
                        worksheet.Cells[RowCount, 4].Value = item.GetValue("TripName").ToString();
                        worksheet.Cells[RowCount, 5].Value = item.GetValue("PointName").ToString();
                        worksheet.Cells[RowCount, 6].Value = item.GetValue("Status").ToString();
                        //if (item.GetValue("Status").ToString() == "1")
                        //    worksheet.Cells[RowCount, 6].Value = "ACTIVE";
                        //else
                        //    worksheet.Cells[RowCount, 6].Value = "DE-ACTIVE";
                        worksheet.Cells[RowCount, 7].Value = item.GetValue("Address").ToString();

                        worksheet.Cells[RowCount, 8].Value = item.GetValue("PickDTime").ToString();

                        string FPath = "";
                        FPath = item.GetValue("ImgExcelPathUrl").ToString();
                        // if (Pic1 == 1)

                        if (!string.IsNullOrEmpty(item.GetValue("Img1Url").ToString()))
                        {
                            string lastWord = item.GetValue("Img1Url").ToString().Substring(item.GetValue("Img1Url").ToString().LastIndexOf('/') + 1);
                            string FullPath = string.Empty;
                            FullPath = FPath + lastWord;

                            //if (File.Exists(FullPath))
                            //{
                            //  Uri newUri = new Uri(item.GetValue("Img1Url").ToString());
                            string Picturename = "Pic" + Pic1;
                            Uri newUri = new Uri(item.GetValue("Img1Url").ToString());

                            Image img1 = DownloadImage(item.GetValue("Img1Url").ToString());

                            var img = worksheet.Drawings.AddPicture(Picturename, img1);

                            int w = img.Image.Width;
                            int h = img.Image.Height;

                            if (h > 140) // because height of 84 is 140 pixels in excel
                            {
                                double scale = h / 140.0;
                                w = (int)Math.Floor(w / scale);
                                h = 140;
                            }

                            int xOff = (150 - w) / 2;
                            int yOff = (140 - h) / 2;

                            img.SetPosition(RowCount - 1, 0, 9 - 1, 0);
                            img.SetSize(135, 75);

                            // }
                        }



                        if (!string.IsNullOrEmpty(item.GetValue("Img2Url").ToString()))
                        {
                            string lastWord1 = item.GetValue("Img2Url").ToString().Substring(item.GetValue("Img2Url").ToString().LastIndexOf('/') + 1);
                            string FullPath = string.Empty;
                            FullPath = FPath + lastWord1;
                            //if (File.Exists(FullPath))
                            //{

                            string Picturename = "PicTwo" + Pic2;
                            //Image img1 = Image.FromFile(FullPath);

                            // var logo = FullPath;
                            Uri newUri = new Uri(item.GetValue("Img2Url").ToString());

                            Image img1 = DownloadImage(item.GetValue("Img2Url").ToString());

                            var img = worksheet.Drawings.AddPicture(Picturename, img1);

                            int w = img.Image.Width;
                            int h = img.Image.Height;

                            if (h > 140) // because height of 84 is 140 pixels in excel
                            {
                                double scale = h / 140.0;
                                w = (int)Math.Floor(w / scale);
                                h = 140;
                            }

                            int xOff = (150 - w) / 2;
                            int yOff = (140 - h) / 2;

                            img.SetPosition(RowCount - 1, 0, 10 - 1, 0);
                            img.SetSize(135, 75);

                            // }

                        }


                        worksheet.Cells[RowCount, 1, RowCount, 8].Style.WrapText = true;
                        worksheet.Cells[RowCount, 1, RowCount, 8].Style.ShrinkToFit = true;
                        worksheet.Cells[RowCount, 1, RowCount, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells[RowCount, 1, RowCount, 8].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                        Pic1++;
                        Pic2++;
                        Count++;
                        RowCount++;
                    }
                    #endregion


                    #region formatting

                    using (var range = worksheet.Cells[1, 1, 2, 10])
                    {
                        // Setting bold font
                        range.Style.Font.Bold = true;
                        // Setting fill type solid
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        // Setting background gray
                        range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(197, 190, 151));

                        // Setting font color
                        range.Style.Font.Color.SetColor(Color.Black);

                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        range.Style.Font.Size = 12;
                        range.Style.WrapText = true;
                        range.Style.ShrinkToFit = true;
                        range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                        var border1 = range.Style.Border;
                        border1.Top.Style = border1.Left.Style = border1.Bottom.Style = border1.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }


                    worksheet.PrinterSettings.FitToPage = true;
                    worksheet.PrinterSettings.ShowGridLines = true;

                    #endregion
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response = excel.GetAsByteArray();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Response;
        }
        Image DownloadImage(string fromUrl)
        {
            using (System.Net.WebClient webClient = new System.Net.WebClient())
            {
                using (Stream stream = webClient.OpenRead(fromUrl))
                {
                    return Image.FromStream(stream);
                }
            }
        }
        [HttpPost]
        [Route("GetMapDataForGrey")]
        public IActionResult GetMapDataForGrey(JObject obj)
        {
            string PointId = obj.GetValue("PointId").Value<string>();

            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@PointId",PointId)

             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllLastSevenDaysPoint, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("InsertAndUpdateGVP")]
        public IActionResult InsertAndUpdateGVP(JObject obj)
        {
            string PointId = obj.GetValue("PointId").Value<string>();
            string Lat = obj.GetValue("Lat").Value<string>();
            string Lng = obj.GetValue("Lng").Value<string>();
            int Flag = obj.GetValue("Flag").Value<int>();
            string UserId = obj.GetValue("UserId").Value<string>();

            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@PointId",PointId),
                  new SqlParameter("@Lat",Lat),
                  new SqlParameter("@Lng",Lng),
                  new SqlParameter("@Flag",Flag),
                  new SqlParameter("@UserId",UserId)

             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.InsertAndUpdateGVP, parameters);

            return Ok(Result);
        }
        [HttpGet]
        [Route("GetGeoPointHistory")]
        public IActionResult GetGeoPointHistory(int GeoPointId)
        {

            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@GeoPointId",GeoPointId),

              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetGeoPointHistoryById, parameters);
            return Ok(Result);
        }
    }
}
