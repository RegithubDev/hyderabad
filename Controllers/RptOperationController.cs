using COMMON;
using COMMON.ASSET;
using COMMON.CITIZEN;
using HYDSWMAPI.INTERFACE;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;

namespace HYDSWMAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RptOperationController : ControllerBase
    {
        private TReport _report;
        private IRptOperation<CLoginResponseInfo> _dataRepository;
        private readonly IWebHostEnvironment HostingEnvironment;
        public RptOperationController(IRptOperation<CLoginResponseInfo> dataRepository, IWebHostEnvironment hostingEnvironment, TReport report)
        {
            this._dataRepository = dataRepository;
            this.HostingEnvironment = hostingEnvironment;
            this._report = report;
        }
        [HttpPost]
        [Route("GetAllContainerWisePerformance_Paging")]
        public IActionResult GetAllContainerWisePerformance_Paging(DataTableAjaxPostModel requestModel)
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
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetContainerWisePerformance, parameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllHKLWisePerformance_Paging")]
        public IActionResult GetAllHKLWisePerformance_Paging(DataTableAjaxPostModel requestModel)
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
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGHKLWisePerformance, parameters);

            return Ok(Result);
        }
        [HttpPost]
        [Route("GetAllPVehicleWiseInfo")]
        public IActionResult GetAllPVehicleWiseInfo(DataTableAjaxPostModel requestModel)
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
                    SortColumn = "B.ZoneNo";
                    break;
                case 3:
                    SortColumn = "C.TStationName";
                    break;
                case 4:
                    SortColumn = "B.VehicleNo";
                    break;
                case 5:
                    SortColumn = "B.OwnerType";
                    break;
                case 6:
                    SortColumn = "B.VehicleType";
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
                  new SqlParameter("@TSId",requestModel.NotiId),
                  new SqlParameter("@UTSId",requestModel.Status),
                  new SqlParameter("@OwnerType",requestModel.Route),
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.rptPVehileWiseInfo, parameters);

            return Ok(Result);
        }
    }
}
