using COMMON;
using COMMON.ASSET;
using COMMON.CITIZEN;
using COMMON.COLLECTION;
using COMMON.DEPLOYMENT;
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
using System.Threading.Tasks;

namespace HYDSWMAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DeploymentController : ControllerBase
    {
        private TReport _report;
        private IDeployement<CLoginResponseInfo> _dataRepository;
        private readonly IWebHostEnvironment HostingEnvironment;
        public DeploymentController(IDeployement<CLoginResponseInfo> dataRepository, IWebHostEnvironment hostingEnvironment, TReport report)
        {
            this._dataRepository = dataRepository;
            this.HostingEnvironment = hostingEnvironment;
            this._report = report;
        }
        [HttpPost]
        [Route("GetAllNotFilledContainer")]
        public IActionResult GetAllNotFilledContainer(JObject obj)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
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
                  new SqlParameter("@UTSId","0"),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllNotFilledContainer, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllNotFilledContainer_Paging")]
        public IActionResult GetAllNotFilledContainer_Paging(DataTableAjaxPostModel requestModel)
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
                    SortColumn = "CreatedOn";
                    break;
                case 2:
                    SortColumn = "EntityCode";
                    break;

                case 3:
                    SortColumn = "TStationName";
                    break;
                case 4:
                    SortColumn = "CreatedOn";
                    break;

                case 6:
                    SortColumn = "CreatedBy";
                    break;
                case 7:
                    SortColumn = "LastModifiedOn";
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
                  new SqlParameter("@TSId",requestModel.NotiId),
                   new SqlParameter("@UTSId",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"0"),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllNotFilledContainer, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllNotFilledHKL_Paging")]
        public IActionResult GetAllNotFilledHKL_Paging(DataTableAjaxPostModel requestModel)
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
                    SortColumn = "CreatedOn";
                    break;
                case 2:
                    SortColumn = "EntityCode";
                    break;

                case 3:
                    SortColumn = "TStationName";
                    break;
                case 4:
                    SortColumn = "CreatedOn";
                    break;

                case 5:
                    SortColumn = "CreatedBy";
                    break;
                case 6:
                    SortColumn = "LastModifiedOn";
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
                  new SqlParameter("@TSId",requestModel.NotiId),
                   new SqlParameter("@UTSId",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"0"),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllNotFilledHKL, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("UpdateContainerLocation")]
        public IActionResult UpdateContainerLocation(JObject obj)
        {
            int DHLTId = obj.GetValue("DHLTId").Value<int>();
            string TSId = obj.GetValue("TSId").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                 new SqlParameter("@DHLTId",DHLTId),
                  new SqlParameter("@CreatedDate",CommonHelper.IndianStandard(DateTime.UtcNow)),
                  new SqlParameter("@CreatedBy",UserId),
                  new SqlParameter("@TSId",TSId),
            };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spUpdateContainerLocation, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("UpdateHKLLocation")]
        public IActionResult UpdateHKLLocation(JObject obj)
        {
            int DHLTId = obj.GetValue("DHLTId").Value<int>();
            string TSId = obj.GetValue("TSId").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                 new SqlParameter("@DHLTId",DHLTId),
                  new SqlParameter("@CreatedDate",CommonHelper.IndianStandard(DateTime.UtcNow)),
                  new SqlParameter("@CreatedBy",UserId),
                  new SqlParameter("@TSId",TSId),
            };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spUpdateHKLLocation, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllHLForDLink")]
        public IActionResult GetAllHLForDLink(JObject obj)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
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
                  new SqlParameter("@UTSId","0"),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllHLForDLink, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllHLForDLink_Paging")]
        public IActionResult GetAllHLForDLink_Paging(DataTableAjaxPostModel requestModel)
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
                    SortColumn = "VehicleNo";
                    break;

                case 3:
                    SortColumn = "TStationName";
                    break;
                case 4:
                    SortColumn = "CreatedOn";
                    break;

                case 5:
                    SortColumn = "CreatedBy";
                    break;
                case 6:
                    SortColumn = "ReplaceVehicleNo";
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
                  new SqlParameter("@TSId",requestModel.NotiId),
                   new SqlParameter("@UTSId",!string.IsNullOrEmpty(requestModel.Status)?requestModel.Status:"0"),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllHLForDLink, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("AddDelinkHL")]
        public IActionResult AddDelinkHL(JObject obj)
        {
            int CTDId = obj.GetValue("CTDId").Value<int>();
            string UserId = obj.GetValue("UserId").Value<string>();
            string Remarks = obj.GetValue("Remarks").Value<string>();
            string UId = obj.GetValue("UId").Value<string>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                 new SqlParameter("@CTDId",CTDId),
                 new SqlParameter("@Remarks",Remarks),
                 new SqlParameter("@CreatedBy",UserId),
                  new SqlParameter("@CreatedDate",CommonHelper.IndianStandard(DateTime.UtcNow)),
                  new SqlParameter("@UId",UId),
            };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spDelinkHL, parameters);

            return Ok(Result);
        }
        [HttpGet]
        [Route("GetAllUnAllocatedHKL")]
        public IActionResult GetAllUnAllocatedHKL()
        {

            SqlParameter[] parameters = new SqlParameter[]
             {

             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllUnAllocatedHKL, parameters);

            return Ok(Result);
        }
        [HttpGet]
        [Route("RemoveArvlContainerById")]
        public IActionResult RemoveArvlContainerById(int DHLTId)
        {

            SqlParameter[] parameters = new SqlParameter[]
             {
                 new SqlParameter("@DHLTId",DHLTId)
             };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spRemoveArvlContainer, parameters);

            return Ok(Result);
        }
        [HttpGet]
        [Route("RemoveArvlHKLById")]
        public IActionResult RemoveArvlHKLById(int DHLTId)
        {

            SqlParameter[] parameters = new SqlParameter[]
             {
                 new SqlParameter("@DHLTId",DHLTId)
             };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spRemoveArvlHKL, parameters);

            return Ok(Result);
        }

        [HttpGet]
        [Route("GetAllHKLForOperation")]
        public IActionResult GetAllHKLForOperation(int TSId)
        {

            SqlParameter[] parameters = new SqlParameter[]
             {
                 new SqlParameter("@TSId",TSId)
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllHKLForOperation, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("AddHKL")]
        public IActionResult AddHKL(JObject obj)
        {
            string VUId = obj.GetValue("VUId").Value<string>();
            string CUId = obj.GetValue("CUId").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            string CDHLTId = obj.GetValue("CDHLTId").Value<string>();
            string CTSId = obj.GetValue("CTSId").Value<string>();
            string PCId = obj.GetValue("PCId").Value<string>();
            DateTime LastTDate = obj.GetValue("LastTDate").Value<DateTime>();
            DateTime TDate = obj.GetValue("TDate").Value<DateTime>();

            string UId = VUId.Split(',')[0];
            string VDHLTId = VUId.Split(',')[1];
            string VTSId = VUId.Split(',')[2];

            SqlParameter[] parameters = new SqlParameter[]
            {
                 new SqlParameter("@CUId",CUId),
                  new SqlParameter("@CDHLTId",CDHLTId),
                  new SqlParameter("@IPCId",PCId),
                  new SqlParameter("@CTSId",CTSId),
                  new SqlParameter("@VUID",UId),
                  new SqlParameter("@VDHLTId",VDHLTId),
                  new SqlParameter("@VTSId",VTSId),
                  new SqlParameter("@OperationTypeId",(int)Enums.OperationType.SECONDARY_COLLECTION),
                  new SqlParameter("@LastTDate",LastTDate),
                  new SqlParameter("@TDate",TDate),
                  new SqlParameter("@CreatedBy",UserId),
            };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spAddManualHKL, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("AddManualWBRelease")]
        public IActionResult AddManualWBRelease(JObject obj)
        {
            string PCId = obj.GetValue("PCId").Value<string>();
            string CDHLTId = obj.GetValue("CDHLTId").Value<string>();
            string VDHLTId = obj.GetValue("VDHLTId").Value<string>();
            string VUID = obj.GetValue("VUID").Value<string>();
            string VehicleNo = obj.GetValue("VehicleNo").Value<string>();
            string TSId = obj.GetValue("TSId").Value<string>();
            string OperationTypeId = obj.GetValue("OperationTypeId").Value<string>();
            string TSName = obj.GetValue("TSName").Value<string>();
            decimal GrossWt = obj.GetValue("GrossWt").Value<decimal>();
            decimal TareWt = obj.GetValue("TareWt").Value<decimal>();
            decimal NetWt = obj.GetValue("NetWt").Value<decimal>();
            string UserId = obj.GetValue("UserId").Value<string>();


            DateTime TDate = obj.GetValue("TDate").Value<DateTime>();
            DateTime CurrentDate = CommonHelper.IndianStandard(DateTime.UtcNow);


            SqlParameter[] parameters = new SqlParameter[]
            {
                 new SqlParameter("@PCId",PCId),
                  new SqlParameter("@CDHLTId",CDHLTId),
                  new SqlParameter("@VDHLTId",VDHLTId),
                  new SqlParameter("@VUID",VUID),
                  new SqlParameter("@VehicleNo",VehicleNo),
                  new SqlParameter("@TSId",TSId),
                  new SqlParameter("@OperationTypeId",OperationTypeId),
                  new SqlParameter("@TSName",TSName),
                  new SqlParameter("@GrossWt",GrossWt),
                  new SqlParameter("@TareWt",TareWt),
                  new SqlParameter("@NetWt",NetWt),
                  new SqlParameter("@TDate",TDate),
                  new SqlParameter("@CreatedBy",UserId),
                  new SqlParameter("@CreatedDate",CurrentDate),
            };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spAddManualWBRelease, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetEntityInfoByCodeNdQR")]
        public IActionResult GetEntityInfoByCodeNdQR(JObject obj)
        {
            string ModeOfScan = obj.GetValue("ModeOfScan").Value<string>();
            string Code = obj.GetValue("Code").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@EntityType",ModeOfScan),
                  new SqlParameter("@Code",Code),
             };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.GetEntityInfoByCodeNdQR, parameters);
            if (string.IsNullOrEmpty(Result))
                Result = "{}";
            return Ok(Result);
        }
        [HttpPost]
        [Route("AddEntityDeployment")]
        public IActionResult AddEntityDeployment(JObject obj)
        {
            string EntityType = obj.GetValue("EntityType").Value<string>();
            string UId = obj.GetValue("UId").Value<string>();
            string ZoneId = obj.GetValue("ZoneId").Value<string>();
            string CircleId = obj.GetValue("CircleId").Value<string>();
            string WardId = obj.GetValue("WardId").Value<string>();
            string Lat = obj.GetValue("Lat").Value<string>();
            string Lng = obj.GetValue("Lng").Value<string>();
            string Address = obj.GetValue("Address").Value<string>();
            string Remarks = obj.GetValue("Remarks").Value<string>();
            string CreatedBy = obj.GetValue("CreatedBy").Value<string>();
            DateTime TDate = CommonHelper.IndianStandard(DateTime.UtcNow);

            SqlParameter[] parameters = new SqlParameter[]
            {
                 new SqlParameter("@EntityType",EntityType),
                  new SqlParameter("@UId",UId),
                  new SqlParameter("@ZoneId",ZoneId),
                  new SqlParameter("@CircleId",CircleId),
                  new SqlParameter("@WardId",WardId),
                  new SqlParameter("@Lat",Lat),
                  new SqlParameter("@Lng",Lng),
                  new SqlParameter("@Address",Address),
                  new SqlParameter("@Remarks",Remarks),
                  new SqlParameter("@CreatedDate",TDate),
                  new SqlParameter("@CreatedBy",CreatedBy),
            };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spAddEntityDeployment, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllDeployedEntity")]
        public IActionResult GetAllDeployedEntity(JObject obj)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            string PageNumber = obj.GetValue("PageNumber").Value<string>();
            string PageSize = obj.GetValue("PageSize").Value<string>();
            string UserId = obj.GetValue("UserId").Value<string>();
            string SearchTerm = obj.GetValue("SearchTerm").Value<string>();
            DateTime TDate = obj.GetValue("TDate").Value<DateTime>();
            string UId = obj.GetValue("UId").Value<string>();
            string EntityType = obj.GetValue("EntityType").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@SearchTerm",SearchTerm),
                  new SqlParameter("@SortColumn",""),
                  new SqlParameter("@SortOrder",""),
                  new SqlParameter("@PageNumber",PageNumber),
                  new SqlParameter("@PageSize",PageSize),
                  new SqlParameter("@UserId",UserId),
                  new SqlParameter("@TDate",TDate),
                  new SqlParameter("@UId",UId),
                  new SqlParameter("@EntityType",EntityType),
                  new SqlParameter("@BUrl",baseUrl),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllDeployedEntiy, parameters);

            return Ok(Result);
        }
        [HttpGet]
        [Route("GetAllEntityInfo")]
        public IActionResult GetAllEntityInfo()
        {
            SqlParameter[] parameters = new SqlParameter[]
             {

             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllEntityInfo, parameters);

            return Ok(Result);
        }


        [HttpPost]
        [Route("GetAllDeployedEntity_Paging")]
        public IActionResult GetAllDeployedEntity_Paging(DataTableAjaxPostModel requestModel)
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
                    SortColumn = "EntityNo";
                    break;
                case 3:
                    SortColumn = "VEntityNo";
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
                    SortColumn = "Address";
                    break;
                case 8:
                    SortColumn = "CreatedOn";
                    break;
                case 9:
                    SortColumn = "ShiftName";
                    break;
                case 10:
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
                   new SqlParameter("@ToDate",requestModel.ToDate),
                   new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                   new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                   new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),
                   new SqlParameter("@UId",!string.IsNullOrEmpty(requestModel.VehicleUid)?requestModel.VehicleUid:string.Empty),
                   new SqlParameter("@EntityType",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:string.Empty),
                   new SqlParameter("@IsReport",!string.IsNullOrEmpty(requestModel.ContratorId)?requestModel.ContratorId:"0"),
                   new SqlParameter("@OwnerType",!string.IsNullOrEmpty(requestModel.Route)?requestModel.Route:"0"),
                   new SqlParameter("@ShiftId",!string.IsNullOrEmpty(requestModel.CategoryId)?requestModel.CategoryId:"0"),
             };

            string Result = string.Empty;
            if (requestModel.ContratorId == "1")
                Result = _dataRepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetAllDeployedEntity_Paging, parameters);
            else
                Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllDeployedEntity_Paging, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetEntityTrans")]
        public IActionResult GetEntityTrans(JObject obj)
        {
            DateTime TDate = obj.GetValue("TDate").Value<DateTime>();
            string UId = obj.GetValue("UId").Value<string>();
            string EntityType = obj.GetValue("EntityType").Value<string>();
            string VehicleTypeId = obj.GetValue("VehicleTypeId").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@TDate",TDate),
                  new SqlParameter("@UId",UId),
                  new SqlParameter("@EntityType",EntityType),
                  new SqlParameter("@VehicleTypeId",VehicleTypeId),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetEntityTrans, parameters);

            return Ok(Result);
        }


        [HttpPost]
        [Route("GetVehicleDepoyment")]
        public IActionResult GetVehicleDepoyment(DataTableAjaxPostModel requestModel)
        {


            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),
                  new SqlParameter("@UserId",requestModel.UserId),
                  new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(requestModel.VehicleTypeId)?requestModel.VehicleTypeId:"0"),
                  new SqlParameter("@SearchDate",requestModel.FromDate),

             };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spDeploydata, parameters);

            return Ok(Result);
        }





        [HttpPost]
        [Route("GetDeploymentTsReport")]
        public IActionResult GetDeploymentTsReport(DataTableAjaxPostModel requestModel)
        {


            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                  new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                  new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),
                  new SqlParameter("@UserId",requestModel.UserId),
                  new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(requestModel.VehicleTypeId)?requestModel.VehicleTypeId:"0"),
                  new SqlParameter("@SearchDate",requestModel.FromDate),

             };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spDeploymentTSReport, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("ValidateInchargeLogin")]
        public async Task<IActionResult> ValidateInchargeLogin([FromForm] IFormCollection value)
        {
            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            DateTime TDate = CommonHelper.IndianStandard(DateTime.UtcNow);
            string FolderName = "/content/Deployment/";
            string FName = string.Empty;
            InchargeLoginInfo obj = JsonConvert.DeserializeObject<InchargeLoginInfo>(value["Input"]);

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


            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@LoginId",obj.LoginId),
                  new SqlParameter("@Pwd",PasswordHelper.EncryptPwd(obj.Pwd)),
                  new SqlParameter("@LastLoginDate",TDate),
                  new SqlParameter("@ImgUrl",FName),
                  new SqlParameter("@FolderName",FolderName),
                  new SqlParameter("@BUrl",baseUrl),
             };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spValidateInchargeLogin, parameters);
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
        [Route("GetDeployedLocationByLoginId")]
        public IActionResult GetDeployedLocationByLoginId(string LoginId)
        {
            //string LoginId = obj.GetValue("LoginId").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@LoginId",LoginId),

             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetDeployedLocationByLoginId, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetPVehicleInfoByQR")]
        public IActionResult GetPVehicleInfoByQR(JObject obj)
        {
            string ModeOfScan = obj.GetValue("ModeOfScan").Value<string>();
            string Code = obj.GetValue("Code").Value<string>();
            string LoginId = obj.GetValue("LoginId").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@EntityType",ModeOfScan),
                  new SqlParameter("@Code",Code),
                  new SqlParameter("@LoginId",LoginId),
             };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.GetPVehicleInfoByQR, parameters);
            if (string.IsNullOrEmpty(Result))
                Result = "{}";
            return Ok(Result);
        }
        [HttpPost]
        [Route("AddPVehicleDeployment")]
        public async Task<IActionResult> AddPVehicleDeployment([FromForm] IFormCollection value)
        {

            string FName = string.Empty;
            PVehicleDeployInfo obj = JsonConvert.DeserializeObject<PVehicleDeployInfo>(value["Input"]);

            string FolderName = "/content/Deployment/";
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
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@EntityType",obj.EntityType),
                  new SqlParameter("@UId",obj.UId),
                  new SqlParameter("@ZoneId",obj.ZoneId),
                  new SqlParameter("@CircleId",obj.CircleId),
                  new SqlParameter("@WardId",obj.WardId),
                  new SqlParameter("@LandmarkId",obj.LandmarkId),
                  new SqlParameter("@Lat",obj.Lat),
                  new SqlParameter("@Lng",obj.Lng),
                 new SqlParameter("@Address",obj.Address.Trim()),
                 new SqlParameter("@Remarks",!string.IsNullOrEmpty(obj.Remarks)? obj.Remarks.Trim():string.Empty),
                  new SqlParameter("@CreatedDate",CommonHelper.IndianStandard(DateTime.UtcNow)),
                  new SqlParameter("@CreatedBy",obj.CreatedBy),
                  new SqlParameter("@Imgurl",FName),
                  new SqlParameter("@FolderName",FolderName),
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spAddPVehicleDeployment, parameters);

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
        [Route("GetAllDeployedVehicle")]
        public IActionResult GetAllDeployedVehicle(JObject obj)
        {
            string LoginId = obj.GetValue("LoginId").Value<string>();
            string SearchDate = obj.GetValue("SearchDate").Value<string>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@LoginId",LoginId),
                  new SqlParameter("@SearchDate",Convert.ToDateTime(SearchDate)),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllDeployedVehicle, parameters);

            List<AllVehicleDeployInfo> vehicleList = null;
            try
            {
                vehicleList = JsonConvert.DeserializeObject<List<AllVehicleDeployInfo>>(Result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ok(vehicleList);
            // return Ok(new List<AllVehicleDeployInfo> { vehicleList });

        }

        [HttpPost]
        [Route("GetAllVDeployment_Paging")]
        public IActionResult GetAllVDeployment_Paging(DataTableAjaxPostModel requestModel)
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
                    SortColumn = "EntityNo";
                    break;
                case 3:
                    SortColumn = "VEntityNo";
                    break;
                case 4:
                    SortColumn = "UId";
                    break;
                case 5:
                    SortColumn = "ZoneNo";
                    break;
                case 6:
                    SortColumn = "CircleName";
                    break;

                case 7:
                    SortColumn = "WardNo";
                    break;
                case 8:
                    SortColumn = "LandMark";
                    break;
                case 9:
                    SortColumn = "Address";
                    break;
                case 10:
                    SortColumn = "OwnerType";
                    break;
                case 11:
                    SortColumn = "CreatedOn";
                    break;

                case 12:
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
                   new SqlParameter("@ToDate",requestModel.ToDate),
                   new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                   new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                   new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),
                   new SqlParameter("@UId",!string.IsNullOrEmpty(requestModel.VehicleUid)?requestModel.VehicleUid:"0"),
                   new SqlParameter("@EntityType",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:string.Empty),
                   new SqlParameter("@IsReport",!string.IsNullOrEmpty(requestModel.ContratorId)?requestModel.ContratorId:"0"),
                   new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(requestModel.VehicleTypeId)?requestModel.VehicleTypeId:"0"),
                   new SqlParameter("@FPath",baseUrl),
             };

            string Result = string.Empty;
            if (requestModel.ContratorId == "1")
                Result = _dataRepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetAllVDeployed_Paging, parameters);
            else
                Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllVDeployed_Paging, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllSATTrips_Paging")]
        public IActionResult GetAllSATTrips_Paging(DataTableAjaxPostModel requestModel)
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
                    SortColumn = "RZone";
                    break;
                case 4:
                    SortColumn = "RCircle";
                    break;

                case 5:
                    SortColumn = "RWard";
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
                   new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                   new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                   new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),
                   new SqlParameter("@UId",!string.IsNullOrEmpty(requestModel.VehicleUid)?requestModel.VehicleUid:"0"),
                   new SqlParameter("@EntityType",!string.IsNullOrEmpty(requestModel.NotiId)?requestModel.NotiId:string.Empty),

                   new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(requestModel.VehicleTypeId)?requestModel.VehicleTypeId:"0"),

             };

            string Result = string.Empty;

            Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllSATTrips_Paging, parameters);

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

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllVehicleTypeByLogin, parameters);

            return Ok(Result);
        }


        [HttpPost]
        [Route("GetAllAttendencePaging")]
        public IActionResult GetAllAttendencePaging(DataTableAjaxPostModel requestModel)

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
                    SortColumn = "VehicleNo";
                    break;
                case 3:
                    SortColumn = "RUID";
                    break;

                case 4:
                    SortColumn = "VehicleType";
                    break;
                case 5:
                    SortColumn = "RZoneNo";
                    break;

                case 6:
                    SortColumn = "RCircle";
                    break;
                case 7:
                    SortColumn = "RTSName";
                    break;
                case 8:
                    SortColumn = "RWard";
                    break;

                case 9:
                    SortColumn = "TripNo";
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
                  new SqlParameter("@SearchDate",requestModel.FromDate),
                   new SqlParameter("@ZoneId",!string.IsNullOrEmpty(requestModel.ZoneId)?requestModel.ZoneId:"0"),
                   new SqlParameter("@CircleId",!string.IsNullOrEmpty(requestModel.CircleId)?requestModel.CircleId:"0"),
                   new SqlParameter("@WardId",!string.IsNullOrEmpty(requestModel.WardId)?requestModel.WardId:"0"),
                    new SqlParameter("@VehicleTypeId",!string.IsNullOrEmpty(requestModel.VehicleTypeId)?requestModel.VehicleTypeId:"0"),
                   new SqlParameter("@TSId",!string.IsNullOrEmpty(requestModel.TSId)?requestModel.TSId:"0"),
                   new SqlParameter("@TripCount",!string.IsNullOrEmpty(requestModel.TripCount)?requestModel.TripCount:"0"),


             };



            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.rptDeployATVsReport, parameters);

            return Ok(Result);
        }
    }
}
