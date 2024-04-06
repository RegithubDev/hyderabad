using COMMON;
using COMMON.SWMENTITY;
using HYDSWMAPI.INTERFACE;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HYDSWMAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SWMMasterController : ControllerBase
    {
        private readonly ILogger<SWMMasterController> _logger;
        private readonly IWebHostEnvironment _host;
        private ISWMMaster<ReasonInfo, OwnerTypeInfo, PropertyTypeInfo, GResposnse, HouseholdInfo, HouseHold_Paging, CircleInfo, WardInfo, IdentityTypeInfo, ShiftInfo, DesignationInfo, SectorInfo> _masterRepository;
        public SWMMasterController(ILogger<SWMMasterController> logger, IWebHostEnvironment host, ISWMMaster<ReasonInfo, OwnerTypeInfo, PropertyTypeInfo, GResposnse, HouseholdInfo, HouseHold_Paging, CircleInfo, WardInfo, IdentityTypeInfo, ShiftInfo, DesignationInfo, SectorInfo> masterRepository)
        {
            _logger = logger;
            _masterRepository = masterRepository;
            this._host = host;
        }

        [HttpGet]
        [Route("GetAllZone")]
        public IActionResult GetAllZone()
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
              };

            string Result = _masterRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllZone, parameters);
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

            string Result = _masterRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllCircleByZone, parameters);
            return Ok(Result);
        }
        [HttpPost]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Route("GetAllCircle")]
        public IActionResult GetAllCircle(JObject obj)
        {
            string CCode = obj.GetValue("CCode").Value<string>();
            string IsAll = obj.GetValue("IsAll").Value<string>();
            object[] mparameters = { CCode, IsAll };
            List<CircleInfo> _lst = _masterRepository.GetAllCircle(StoredProcedureHelper.spGetAllCircle, mparameters);

            return Ok(_lst);
        }

        [HttpPost]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Route("GetAllWard")]
        public IActionResult GetAllWard(JObject obj)
        {
            string CircleId = obj.GetValue("CircleId").Value<string>();
            string IsAll = obj.GetValue("IsAll").Value<string>();
            object[] mparameters = { IsAll, !string.IsNullOrEmpty(CircleId) ? Convert.ToInt32(CircleId) : 0 };
            List<WardInfo> _lst = _masterRepository.GetAllWard(StoredProcedureHelper.spGetAllWard, mparameters);

            return Ok(_lst);
        }

        [HttpPost]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Route("GetWardByCircle")]
        public IActionResult GetWardByCircle(JObject obj)
        {
            string IsAll = obj.GetValue("IsAll").Value<string>();
            string CircleId = obj.GetValue("CircleId").Value<string>();
            object[] mparameters = { IsAll, Convert.ToInt32(CircleId) };
            List<WardInfo> _lst = _masterRepository.GetAllWard(StoredProcedureHelper.spGetAllWard, mparameters);

            return Ok(_lst);
        }


        [HttpPost]
        [Route("GetAllDesignation")]
        public IActionResult GetAllDesignation(JObject obj)
        {
            string IsAll = obj.GetValue("IsAll").Value<string>();
            string CCode = obj.GetValue("CCode").Value<string>();
            object[] mparameters = { CCode, IsAll };
            List<DesignationInfo> _lst = _masterRepository.GetAllDesignation(StoredProcedureHelper.spGetAllDesignation, mparameters);

            return Ok(_lst);
        }

        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Route("GetAllCircleByUser")]
        public IActionResult GetAllCircleByUser(string UserId)
        {
            object[] mparameters = { UserId };
            string Result = _masterRepository.GetAllCircleByUser(StoredProcedureHelper.spGetAllCircleByUser, mparameters);

            return Ok(Result);
        }
        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Route("GetAllWardByUser")]
        public IActionResult GetAllWardByUser(string UserId, string CircleId)
        {
            object[] mparameters = { UserId, CircleId };
            string Result = _masterRepository.GetAllWardByUser(StoredProcedureHelper.spGetAllWardByUser, mparameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("GetAllShift")]
        public IActionResult GetAllShift(JObject obj)
        {
            object[] mparameters = { obj.GetValue("CCode").Value<string>(),
                                     obj.GetValue("IsAll").Value<string>()
                                    };

            List<ShiftInfo> Result = _masterRepository.GetAllShift(StoredProcedureHelper.spGetAllShift, mparameters);

            return Ok(Result);
        }

        [HttpPost]
        [Route("AddAndUpdateShift")]
        public IActionResult AddAndUpdateShift(ShiftInfo info)
        {
            //object[] mparameters = { info.ShiftId,
            //                         info.ShiftName,
            //                         info.ShiftSTime,
            //                         1,
            //                         info.ShiftETime,
            //                         info.CCode,
            //                         info.BeforBMin,
            //                         info.AfterBMin
            //                        };
            DateTime TDate = CommonHelper.IndianStandard(DateTime.UtcNow);
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@ShiftId",info.ShiftId),
                  new SqlParameter("@ShiftName",info.ShiftName),
                  new SqlParameter("@ShiftSTime",info.ShiftSTime),
                  new SqlParameter("@IsActive",1),
                  new SqlParameter("@ShiftETime",info.ShiftETime),
                  new SqlParameter("@CCode",info.CCode),
                  new SqlParameter("@BeforBMin",info.BeforBMin),
                  new SqlParameter("@AfterBMin",info.AfterBMin),
                  new SqlParameter("@CreatedBy",info.UserId),
                  new SqlParameter("@CreatedDate",TDate),
              };
            string Result = _masterRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spAddAndUpdateShift, parameters);

            return Ok(Result);
        }
        [HttpGet]
        [Route("AllShiftInfo")]
        public IActionResult AllShiftInfo(string CCode)
        {
            object[] mparameters = {CCode
                                    };

            string Result = _masterRepository.ExcuteRowSqlCommand(StoredProcedureHelper.spGetAllShiftInfo, mparameters);

            return Ok(Result);
        }
        [HttpGet]
        [Route("ShiftInfoById")]
        public IActionResult ShiftInfoById(int ShiftId)
        {
            object[] mparameters = {ShiftId
                                    };

            string Result = _masterRepository.ExcuteSingleRowSqlCommand(StoredProcedureHelper.spGetShiftById, mparameters);
            return Ok(Result);
        }
        [HttpGet]
        [Route("GetAllDesignationFromEmpTbl")]
        public IActionResult GetAllDesignationFromEmpTbl()
        {
            object[] mparameters = {
                                    };

            string Result = _masterRepository.ExcuteRowSqlCommand(StoredProcedureHelper.spGetAllDesignationFromEmpTbl, mparameters);
            return Ok(Result);
        }
    }
}
