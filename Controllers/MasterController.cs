using COMMON;
using COMMON.MASTER;
using COMMON.SWMENTITY;
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HYDSWMAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MasterController : ControllerBase
    {
        private IMaster<GResposnse> _dataRepository;
        private readonly IWebHostEnvironment HostingEnvironment;
        public MasterController(IMaster<GResposnse> dataRepository, IWebHostEnvironment hostingEnvironment)
        {
            this._dataRepository = dataRepository;
            this.HostingEnvironment = hostingEnvironment;
        }
        [HttpGet]
        [Route("GetAllZone")]
        public IActionResult GetAllZone(string IsAll)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@IsAll",IsAll)
              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllZone, parameters);
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetAllDIncharge")]
        public IActionResult GetAllDIncharge(string IsAll)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@IsAll",IsAll)
              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllDIncharge, parameters);
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetAllDLocation")]
        public IActionResult GetAllDLocation(int WardId)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@WardId",WardId)
              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllDLocation, parameters);
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetAllTransferStationByWardId")]
        public IActionResult GetAllTransferStationByWardId(int WardId)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@WardId",WardId)
              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllTransferStationByWardId, parameters);
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetAllTrip")]
        public IActionResult GetAllTrip(string IsAll)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@IsAll",IsAll)
              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllTrip, parameters);
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetZoneInfoById")]
        public IActionResult GetZoneInfoById(int ZId)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@ZId",ZId)
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetZoneInfoById, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("SaveAndUpdateZone")]
        public IActionResult SaveAndUpdateZone(JObject obj)
        {
            int ZId = obj.GetValue("ZId").Value<int>();
            string ZoneNo = obj.GetValue("ZoneNo").Value<string>();
            string Zonecode = obj.GetValue("Zonecode").Value<string>();
            bool IsActive = obj.GetValue("IsActive").Value<bool>();
            string UserId = obj.GetValue("UserId").Value<string>();
            DateTime TDate = CommonHelper.IndianStandard(DateTime.UtcNow);
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@ZId",ZId),
                  new SqlParameter("@ZoneNo",ZoneNo),
                  new SqlParameter("@Zonecode",Zonecode),
                  new SqlParameter("@IsActive",IsActive),
                  new SqlParameter("@CreatedBy",UserId),
                  new SqlParameter("@CreatedDate",TDate)
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spSaveAndUpdateZone, parameters);

            return Ok(Result);
        }

        [HttpGet]
        [Route("GetAllVehicleType")]
        public IActionResult GetAllVehicleType()
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                //  new SqlParameter("@IsAll",IsAll)
              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllVehicleType, parameters);
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetVehicleTypeInfoById")]
        public IActionResult GetVehicleTypeInfoById(int VehicleTypeId)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@VehicleTypeId",VehicleTypeId)
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetVehicleTypeInfoById, parameters);
            return Ok(Result);
        }

        [HttpPost]
        [Route("SaveAndUpdateVehicleType")]
        public IActionResult SaveAndUpdateVehicleType(JObject obj)
        {
            int VehicleTypeId = obj.GetValue("VehicleTypeId").Value<int>();
            string VehicleType = obj.GetValue("VehicleType").Value<string>();
            float Volume = obj.GetValue("Volume").Value<float>();
            float Density = obj.GetValue("Density").Value<float>();
            int OperationTypeId = obj.GetValue("OperationTypeId").Value<int>();
            bool IsActive = obj.GetValue("IsActive").Value<bool>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@VehicleTypeId",VehicleTypeId),
                  new SqlParameter("@VehicleType", VehicleType),
                  new SqlParameter("@Volume",Volume),
                  new SqlParameter("@Density",Density),
                  new SqlParameter("@OperationTypeId",OperationTypeId),
                  new SqlParameter("@IsActive",IsActive)
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spSaveAndUpdateVehicleType, parameters);

            return Ok(Result);
        }
        [HttpGet]
        [Route("GetAllGeoPointCategory")]
        public IActionResult GetAllGeoPointCategory(string IsAll)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@IsAll",IsAll)
              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllGeoPointCategory, parameters);
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetGeoPointCategoryInfoById")]
        public IActionResult GetGeoPointCategoryInfoById(int GPCId)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@GPCId",GPCId)
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetGeoPointCategoryInfoById, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("SaveAndUpdateGeoPointCategory")]
        public IActionResult SaveAndUpdateGeoPointCategory(JObject obj)
        {
            string GPCId = obj.GetValue("GPCId").Value<string>();
            string GeoPointCategory = obj.GetValue("GeoPointCategory").Value<string>();
            bool IsActive = obj.GetValue("IsActive").Value<bool>();
            DateTime TDate = CommonHelper.IndianStandard(DateTime.UtcNow);
            string CreatedBy = obj.GetValue("CreatedBy").Value<string>();
            string ModifiedBy = obj.GetValue("ModifiedBy").Value<string>();

            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@GPCId",!string.IsNullOrEmpty(GPCId)?GPCId:"0"),
                  new SqlParameter("@GeoPointCategory",GeoPointCategory),
                  new SqlParameter("@IsActive",IsActive),
                  new SqlParameter("@CreatedDate",TDate),
                  new SqlParameter("@ModifiedDate",TDate),
                  new SqlParameter("@CreatedBy",CreatedBy),
                  new SqlParameter("@ModifiedBy",ModifiedBy),
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spSaveAndUpdateGeoPointCategory, parameters);

            return Ok(Result);
        }

        [HttpGet]
        [Route("GetAllTripMaster")]
        public IActionResult GetAllTripMaster(string IsAll)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@IsAll",IsAll)
              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllTripMaster, parameters);
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetTripMasterInfoById")]
        public IActionResult GetTripMasterInfoById(int TMId)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@TMId",TMId)


              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetTripMasterInfoById, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("SaveAndUpdateTripMaster")]
        public IActionResult SaveAndUpdateTripMaster(JObject obj)
        {
            string TMId = obj.GetValue("TMId").Value<string>();
            string TripId = obj.GetValue("TripId").Value<string>();
            string PrefixName = obj.GetValue("PrefixName").Value<string>();
            bool IsActive = obj.GetValue("IsActive").Value<bool>();
            DateTime TDate = CommonHelper.IndianStandard(DateTime.UtcNow);
            string CreatedBy = obj.GetValue("CreatedBy").Value<string>();
            string ModifiedBy = obj.GetValue("ModifiedBy").Value<string>();


            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@TMId",!string.IsNullOrEmpty(TMId)?TMId:"0"),
                  new SqlParameter("@TripId",TripId),
                  new SqlParameter("@PrefixName",!string.IsNullOrEmpty(PrefixName)?PrefixName:""),
                  new SqlParameter("@IsActive",IsActive),
                  new SqlParameter("@CreatedDate",TDate),
                  new SqlParameter("@ModifiedDate",TDate),
                  new SqlParameter("@CreatedBy",CreatedBy),
                  new SqlParameter("@ModifiedBy",ModifiedBy),
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spSaveAndUpdateTripMaster, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllCircle")]
        public IActionResult GetAllCircle(JObject obj)
        {
            string CCode = obj.GetValue("CCode").Value<string>();
            string IsAll = obj.GetValue("IsAll").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@CCode",CCode),
                  new SqlParameter("@IsAlll",IsAll),
              };
            string _lst = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetMAllCircle, parameters);

            return Ok(_lst);
        }
        [HttpGet]
        [Route("GetCircleInfoById")]
        public IActionResult GetCircleInfoById(int CircleId)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@CircleId",CircleId)
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetCircleInfoById, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("SaveAndUpdateCircle")]
        public IActionResult SaveAndUpdateCircle(JObject obj)
        {
            int CircleId = obj.GetValue("CircleId").Value<int>();
            string CircleName = obj.GetValue("CircleName").Value<string>();
            string CircleCode = obj.GetValue("CircleCode").Value<string>();
            string CCode = obj.GetValue("CCode").Value<string>();
            int ZoneId = obj.GetValue("ZoneId").Value<int>();
            bool IsActive = obj.GetValue("IsActive").Value<bool>();
            DateTime TDate = CommonHelper.IndianStandard(DateTime.UtcNow);
            string CreatedBy = obj.GetValue("CreatedBy").Value<string>();
            string ModifiedBy = obj.GetValue("ModifiedBy").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@CircleId",CircleId),
                  new SqlParameter("@CircleName",CircleName),
                  new SqlParameter("@IsActive",IsActive),
                  new SqlParameter("@CCode",CCode),
                  new SqlParameter("@ZoneId",ZoneId),
                  new SqlParameter("@CircleCode",CircleCode),
                   new SqlParameter("@CreatedDate",TDate),
                  new SqlParameter("@ModifiedDate",TDate),
                  new SqlParameter("@CreatedBy",CreatedBy),
                  new SqlParameter("@ModifiedBy",ModifiedBy),
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spSaveAndUpdateCircle, parameters);

            return Ok(Result);
        }
        [HttpGet]
        [Route("GetAllCircleByZone")]
        public IActionResult GetAllCircleByZone(int ZoneId)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                   new SqlParameter("@ZoneId",ZoneId)
              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllCircleByZone, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllWard")]
        public IActionResult GetAllWard(JObject obj)
        {
            string IsAll = obj.GetValue("IsAll").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
              {

                  new SqlParameter("@IsAlll",IsAll),
              };
            string _lst = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllMWard, parameters);

            return Ok(_lst);
        }
        [HttpGet]
        [Route("GetWardInfoById")]
        public IActionResult GetWardInfoById(int WardId)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@WardId",WardId)
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetWardInfoById, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("SaveAndUpdateWard")]
        public IActionResult SaveAndUpdateWard(JObject obj)
        {
            int WardId = obj.GetValue("WardId").Value<int>();
            string WardNo = obj.GetValue("WardNo").Value<string>();
            string WardCode = obj.GetValue("WardCode").Value<string>();
            string CCode = obj.GetValue("CCode").Value<string>();
            int CirlceId = obj.GetValue("CirlceId").Value<int>();
            bool IsActive = obj.GetValue("IsActive").Value<bool>();
            DateTime TDate = CommonHelper.IndianStandard(DateTime.UtcNow);
            string CreatedBy = obj.GetValue("CreatedBy").Value<string>();
            string ModifiedBy = obj.GetValue("ModifiedBy").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@WardId",WardId),
                  new SqlParameter("@WardNo",WardNo),
                  new SqlParameter("@WardCode",WardCode),
                  new SqlParameter("@IsActive",IsActive),
                  new SqlParameter("@CirlceId",CirlceId),
                  new SqlParameter("@CCode",CCode),
                   new SqlParameter("@CreatedDate",TDate),
                  new SqlParameter("@ModifiedDate",TDate),
                  new SqlParameter("@CreatedBy",CreatedBy),
                  new SqlParameter("@ModifiedBy",ModifiedBy),

              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spSaveAndUpdateWard, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllTransferStation")]
        public IActionResult GetAllTransferStation(DataTableAjaxPostModel requestModel)
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
                    SortColumn = "Location";
                    break;
                case 3:
                    SortColumn = "TStationName";
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
                    SortColumn = "Radius";
                    break;
                case 8:
                    SortColumn = "TSType";
                    break;
                case 9:
                    SortColumn = "NoOfContainer";
                    break;
                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
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
              };
            string _lst = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllTransferStation_Paging, parameters);

            return Ok(_lst);
        }


        [HttpPost]
        [Route("GetAllTransferStationB64")]
        public IActionResult GetAllTransferStationB64(DataTableAjaxPostModel requestModel)
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
                    SortColumn = "Location";
                    break;
                case 3:
                    SortColumn = "TStationName";
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
                    SortColumn = "Radius";
                    break;
                case 8:
                    SortColumn = "TSType";
                    break;
                case 9:
                    SortColumn = "NoOfContainer";
                    break;
                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
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
              };
            string _lst = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllTransferStation_Paging, parameters);


            List<MasterInfo> Mhlst = JsonConvert.DeserializeObject<List<MasterInfo>>(_lst);
            string strJson = JsonConvert.SerializeObject(Mhlst);
            return Ok(strJson);

            //return Ok(_lst);
        }


        [HttpGet]
        [Route("GetTStationInfoById")]
        public IActionResult GetTStationInfoById(int TSId)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@TSId",TSId),
                  new SqlParameter("@FPath",baseUrl)
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetTStationInfoById, parameters);
            return Ok(Result);
        }

        [HttpPost]
        [Route("SaveAndUpdateTStationInfo")]
        public async Task<IActionResult> SaveAndUpdateTStationInfo([FromForm] IFormCollection value)
        {
            try
            {
                string FName = string.Empty;
                dynamic obj = JObject.Parse(value["Input"]);
                string FolderName = "/content/Transferstation/";

                if (value.Files.Count > 0)
                    if (value.Files[0].Length > 0)
                        FName = CommonHelper.generateID() + Path.GetExtension(value.Files[0].FileName);

                string filePath = Path.Combine(HostingEnvironment.WebRootPath + FolderName, FName);

                string TSId = obj.TSId;
                string TStationName = obj.TStationName;
                string Lat = obj.Lat;
                string Lng = obj.Lng;
                string IsActive = obj.IsActive;
                string Location = obj.Location;
                string CircleId = obj.CircleId;
                string WardId = obj.WardId;
                string ZoneId = obj.ZoneId;
                DateTime TDate = CommonHelper.IndianStandard(DateTime.UtcNow);
                string UserId = obj.UserId;
                string Radius = obj.Radius;
                string TSType = obj.TSType;
                string NoOfContainer = obj.NoOfContainer;

                SqlParameter[] parameters = new SqlParameter[]
                   {
                  new SqlParameter("@TSId",!string.IsNullOrEmpty(TSId)?Convert.ToInt32( TSId):0),
                  new SqlParameter("@TStationName",TStationName),
                  new SqlParameter("@IsActive",IsActive),
                  new SqlParameter("@Lat",Lat),
                  new SqlParameter("@Lng",Lng),
                  new SqlParameter("@CreatedDate",TDate),
                  new SqlParameter("@CreatedBy",UserId),
                  new SqlParameter("@ZoneId",ZoneId),
                  new SqlParameter("@CircleId",CircleId),
                  new SqlParameter("@WardId",WardId),
                  new SqlParameter("@Location",Location),
                  new SqlParameter("@ImgUrl",FName),
                  new SqlParameter("@FolderName",FolderName),
                  new SqlParameter("@Radius",Radius),
                  new SqlParameter("@TSType",TSType),
                  new SqlParameter("@NoOfContainer",NoOfContainer),

                   };

                string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spSaveAndUpdateTStationInfo, parameters);
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
                return Ok(ex);
            }
        }
        [HttpGet]
        [Route("GetAllContainerTypeInfo")]
        public IActionResult GetAllContainerTypeInfo()
        {
            SqlParameter[] parameters = new SqlParameter[]
              {

              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllContainerTypeInfo, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllAssetStatus")]
        public IActionResult GetAllAssetStatus(JObject obj)
        {
            string CCode = obj.GetValue("CCode").Value<string>();
            string IsAll = obj.GetValue("IsAll").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
             {  new SqlParameter("@CCode",CCode),
              new SqlParameter("@IsAlll",IsAll),
             };
            string _lst = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllAssetStatus, parameters);
            return Ok(_lst);
        }
        //[HttpGet]
        //[Route("GetAllVehicleType")]
        //public IActionResult GetAllVehicleType()
        //{
        //    SqlParameter[] parameters = new SqlParameter[]
        //      {

        //      };

        //    string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllVehicleType, parameters);
        //    return Ok(Result);
        //}
        [HttpGet]
        [Route("GetAllOwnerType")]
        public IActionResult GetAllOwnerType()
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@IsAll","NO"),
              new SqlParameter("@OwnerTId","0"),
              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllOwnerTypeInfo, parameters);
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetAllOperationType")]
        public IActionResult GetAllOperationType()
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllOperationType, parameters);
            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllOwnerTypeInfo")]
        public IActionResult GetAllOwnerTypeInfo(JObject obj)
        {
            string Result = string.Empty;
            int OwnerTId = obj.GetValue("OwnerTId").Value<int>();
            string IsAll = obj.GetValue("IsAll").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@IsAll",IsAll),
              new SqlParameter("@OwnerTId",OwnerTId),
              };
            if (OwnerTId > 0)
                Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetAllOwnerTypeInfo, parameters);
            else
                Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllOwnerTypeInfo, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [Route("SaveAndUpdateOwnerType")]
        public IActionResult SaveAndUpdateOwnerType(JObject obj)//ChangeS
        {
            int OwnerTId = obj.GetValue("OwnerTId").Value<int>();
            string OwnerType = obj.GetValue("OwnerType").Value<string>();
            bool IsActive = obj.GetValue("IsActive").Value<bool>();
            string UserId = obj.GetValue("UserId").Value<string>();
            DateTime TDate = CommonHelper.IndianStandard(DateTime.UtcNow);
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@OwnerTId",OwnerTId),
                  new SqlParameter("@OwnerType",OwnerType),
                  new SqlParameter("@IsActive",IsActive),
                  new SqlParameter("@CreatedBy",UserId),
                  new SqlParameter("@CreatedDate",TDate)
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.SaveAndUpdateOwnerType, parameters);

            return Ok(Result);
        }

        [HttpGet]
        [Route("GetAllActiveVehicle")]
        public IActionResult GetAllActiveVehicle()
        {
            SqlParameter[] parameters = new SqlParameter[]
              {

              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllActiveVehicle, parameters);
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetAllActiveGeoPoint")]
        public IActionResult GetAllActiveGeoPoint(int ZoneId, int PointCatId)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                     new SqlParameter("@ZoneId",ZoneId),
                     new SqlParameter("@PointCatId",PointCatId),
              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllActiveGeoPoint, parameters);
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetAllActiveRoute")]
        public IActionResult GetAllActiveRoute(int ZoneId, int CircleId)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@ZoneId",ZoneId),
                     new SqlParameter("@CircleID",CircleId),
              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllActiveRoute, parameters);
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetAllActiveRouteTrip")]
        public IActionResult GetAllActiveRouteTrip(int RouteId)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@RouteId",RouteId),
              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllActiveRouteTrip, parameters);
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetAllAssignedVehicleNumber")]
        public IActionResult GetAllAssignedVehicleNumber()
        {
            SqlParameter[] parameters = new SqlParameter[]
              {

              };


            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllAssignedVehicleNumber, parameters);
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetAllAssignedVehicleNumber1")]
        public IActionResult GetAllAssignedVehicleNumber1(int RouteId, int TripId)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@RouteId",RouteId),
                    new SqlParameter("@TripId",TripId),
              };


            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllAssignedVehicleNumberbyRoteId, parameters);
            return Ok(Result);
        }

        [HttpGet]
        [Route("GetAllGreyPoint")]
        public IActionResult GetAllGreyPoint(string RouteTripCode, DateTime SDate)
        {
            // string sd= Convert.ToDateTime(SDate).ToString("mm/d/yyyy");
            //DateTime thisDate1 = new DateTime(SDate);
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@RouteTripCode",RouteTripCode),
                  new SqlParameter("@SDate",SDate),
              };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllGreyPoint, parameters);
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetAllVehicleNumber")]
        public IActionResult GetAllVehicleNumber(int VehicleTypeId)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                   new SqlParameter("@VehicleTypeId",VehicleTypeId),
              };


            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllVehicleNumber, parameters);
            return Ok(Result);
        }
    }
}
