using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using COMMON;
using COMMON.SWMENTITY;
using HYDSWMAPI.INTERFACE;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using COMMON.GENERIC;
using Newtonsoft.Json;

namespace HYDSWMAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private SMSSenderHelper _SMSSenderHelper;
        private IUser<tbl_User, LoginResponse, GUserInfo, GResposnse> _dataRepository;
        public UserController(IUser<tbl_User, LoginResponse, GUserInfo, GResposnse> dataRepository, SMSSenderHelper SMSSenderHelper)
        {
            _dataRepository = dataRepository;
            this._SMSSenderHelper = SMSSenderHelper;
        }
        [HttpGet]
        [Route("Login")]
        public IActionResult Login(string userName, string password)
        {

            string baseUrl = Startup.StaticConfig.GetValue<string>("ApiBaseUrl");
            object[] parameters = { userName, PasswordHelper.EncryptPwd(password), CommonHelper.IndianStandard(DateTime.UtcNow), "MOBILE APP" };
            LoginResponse usr = _dataRepository.Login(StoredProcedureHelper.spGetValidateLogin, parameters);
            return Ok(usr);
        }
        [HttpPost]
        [Route("ValidateLogin")]
        public IActionResult ValidateLogin(BUserLoginInfo info)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@LoginId",info.UserName),
                  new SqlParameter("@Pwd",PasswordHelper.EncryptPwd(info.Password)),
                  new SqlParameter("@LastLogin",CommonHelper.IndianStandard(DateTime.UtcNow)),
                  new SqlParameter("@LoginType",info.LoginType)
              };
            // object[] parameters = { info.UserName, PasswordHelper.EncryptPwd(info.Password), CommonHelper.IndianStandard(DateTime.UtcNow), info.LoginType };
            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamic(StoredProcedureHelper.spGetValidateLogin, parameters);
            LoginResponse usr = JsonConvert.DeserializeObject<LoginResponse>(Result);
            return Ok(usr);
        }
        [HttpPost]
        [Route("spGetEXTValidateLogin")]
        public IActionResult spGetEXTValidateLogin(BUserLoginInfo info)
        {

            object[] parameters = { info.UserName, PasswordHelper.EncryptPwd(info.Password), CommonHelper.IndianStandard(DateTime.UtcNow), info.LoginType };
            LoginResponse usr = _dataRepository.Login(StoredProcedureHelper.spGetEXTValidateLogin, parameters);
            return Ok(usr);
        }
        //[HttpPost]
        //[Route("ValidateLoginWithQr")]
        //public IActionResult ValidateLoginWithQr(QRUserLoginInfo info)
        //{
        //    if (info.HasPwd == "0")
        //    {
        //        info.Otp = "000000";// CommonHelper.Get6DigitOTP();
        //    }
        //    object[] parameters = { info.UserName, CommonHelper.IndianStandard(DateTime.UtcNow), info.LoginType, info.UId, info.Otp, info.HasPwd };
        //    LoginResponse usr = _dataRepository.Login(StoredProcedureHelper.spValidateLoginWithQr, parameters);
        //    if (usr != null)
        //        if (usr.Result == 1)
        //        {
        //            //string Message = "Your Login Password-" + CommonHelper.Decrypt(DPassword);
        //            string Message = "Dear Citizen, Your OTP for registration is " + info.Otp + ". Chennai Enviro. #NammaChennai#NammaPoruppu.";
        //            _SMSSenderHelper.SendMessage(Message, info.UserName);
        //        }
        //    return Ok(usr);
        //}
        [HttpPost]
        [Route("ValidateVehicleWithQr")]
        public IActionResult ValidateVehicleWithQr(QRUserLoginInfo info)
        {

            object[] parameters = { info.UserName, CommonHelper.IndianStandard(DateTime.UtcNow), info.LoginType, info.UId };
            LoginResponse usr = _dataRepository.Login(StoredProcedureHelper.spValidateLVehicleWithQr, parameters);

            return Ok(usr);
        }
        [HttpPost]
        [Route("Logout")]
        public IActionResult Logout(BUserLoginInfo info)
        {
            object[] parameters = { info.UserName, CommonHelper.IndianStandard(DateTime.UtcNow), info.LoginType };
            string usr = _dataRepository.ExcuteSingleRowSqlCommand(StoredProcedureHelper.spUserLogout, parameters);
            return Ok(usr);
        }
        public IActionResult AccessDenied()
        {
            return Ok();
        }

        [HttpPost]
        [Route("GetAllUser")]
        public IActionResult GetAllUser(DataTableAjaxPostModel requestModel)
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
                    SortColumn = "EmpCode";
                    break;
                case 3:
                    SortColumn = "EmailId";
                    break;
                case 4:
                    SortColumn = "RoleName";
                    break;
                case 5:
                    SortColumn = "MobileNo";
                    break;

                default:
                    SortDir = String.Empty;
                    SortColumn = string.Empty;
                    break;
            }
            object[] parameters = {
                                          str,
                                          SortColumn,
                                          SortDir,
                                          start,
                                          length,
                                        requestModel.CCode
                                        };
            string _lst = _dataRepository.ExcuteRowSqlCommand(StoredProcedureHelper.GetAllUsers, parameters);

            return Ok(_lst);
        }
        [HttpGet]
        [Route("GetAllRole")]
        public IActionResult GetAllRole(string CCode)
        {
            object[] mparameters = { CCode };
            string _lst = _dataRepository.ExcuteRowSqlCommand(StoredProcedureHelper.spGetAllRoles, mparameters);

            return Ok(_lst);
        }
        [HttpGet]
        [Route("GetUserDataById")]
        public IActionResult GetUserDataById(int UserId)
        {
            object[] mparameters = { UserId };
            string _lst = _dataRepository.ExcuteSingleRowSqlCommand(StoredProcedureHelper.GetUserDataById, mparameters);

            return Ok(_lst);
        }
        [HttpGet]
        [Route("GetMenuByRole")]
        public IActionResult GetMenuByRole(string rolename)
        {
            object[] mparameters = { rolename };
            string _lst = _dataRepository.ExcuteRowSqlCommand(StoredProcedureHelper.spGetMenuByRole, mparameters);

            return Ok(_lst);
        }
        [HttpGet]
        [Route("GetAllMenuMaster")]
        public IActionResult GetAllMenuMaster(int roleId)
        {
            object[] mparameters = { roleId };
            string _lst = _dataRepository.ExcuteRowSqlCommand(StoredProcedureHelper.spGetALLMenuMaster, mparameters);
            string _lstSubMenu = _dataRepository.ExcuteRowSqlCommand(StoredProcedureHelper.spGetAllSubMenuMaster, mparameters);
            var response = new { data1 = _lst, data2 = _lstSubMenu };
            return Ok(response);
        }
        [HttpPost]
        [Route("GetAllCircleAndWardMaster")]
        public IActionResult GetAllCircleAndWardMaster(JObject obj)
        {
            object[] mparameters1 = { obj.GetValue("Ccode").Value<string>(), };
            object[] mparameters = { obj.GetValue("LoginId").Value<string>() };
            // string _lst = _dataRepository.ExcuteRowSqlCommand(StoredProcedureHelper.spGetALLCircleMaster, mparameters1);
            string _lst = _dataRepository.ExcuteRowSqlCommand(StoredProcedureHelper.spGetAllUZone, mparameters1);
            string _lstSubMenu = _dataRepository.ExcuteRowSqlCommand(StoredProcedureHelper.spGetAllCircleWardMaster, mparameters);
            var response = new { data1 = _lst, data2 = _lstSubMenu };
            return Ok(response);
        }
        [HttpGet]
        [Route("GetSubMenuByRole")]
        public IActionResult GetSubMenuByRole(string rolename)
        {
            object[] mparameters = { rolename };
            string _lst = _dataRepository.ExcuteRowSqlCommand(StoredProcedureHelper.spGetAllSubMenuByRole, mparameters);

            return Ok(_lst);
        }
        [HttpGet]
        [Route("GetSubMenuByRolev1")]
        public IActionResult GetSubMenuByRolev1(string rolename)
        {
            object[] mparameters = { rolename };
            string _lst = _dataRepository.ExcuteRowSqlCommand(StoredProcedureHelper.spGetAllSubMenuByRoleV1, mparameters);

            return Ok(_lst);
        }
        [HttpPost]
        [Route("SaveandupdateMenu")]
        public IActionResult SaveandupdateMenu(JObject obj)
        {
            string JArrayval = obj.GetValue("JArrayval").Value<string>();
            string ccode = obj.GetValue("CCode").Value<string>();
            bool IsActive = Convert.ToBoolean(obj.GetValue("IsActive").Value<string>());
            string RoleName = obj.GetValue("RoleName").Value<string>();
            string RoleId = obj.GetValue("RoleId").Value<string>();
            string VehicleTypeId = obj.GetValue("VehicleTypeId").Value<string>();

            var dattable = CommonHelper.toDataTable(JArrayval);
            SqlParameter[] mparameters = new SqlParameter[]
                {
                    new SqlParameter("@RoleId",RoleId),
                    new SqlParameter("@CCode",ccode),
                    new SqlParameter("@fbIsActive",IsActive),
                    new SqlParameter("@RoleName",RoleName),
                    new SqlParameter("@fnVehicleTypeId",VehicleTypeId),

                    new SqlParameter("@tblSubMneuType",dattable)
                };
            string _lst = _dataRepository.ExcuteDataTableRowSqlCommand(StoredProcedureHelper.spSaveNupdateRole, mparameters);

            return Ok(_lst);
        }
        [HttpPost]
        [Route("SaveandupdateUser")]
        public IActionResult SaveandupdateUser(JObject obj)
        {
            string JArrayval = obj.GetValue("JArrayval").Value<string>();

            string UserId = obj.GetValue("UserId").Value<string>();
            bool IsActive = Convert.ToBoolean(obj.GetValue("IsActive").Value<string>());
            string FullName = obj.GetValue("FullName").Value<string>();
            string EmailId = obj.GetValue("EmailId").Value<string>();
            string Pwd = obj.GetValue("Pwd").Value<string>();
            string Mobile = obj.GetValue("Mobile").Value<string>();
            string CCode = obj.GetValue("CCode").Value<string>();
            string EmpCode = obj.GetValue("EmpCode").Value<string>();
            string RoleId = obj.GetValue("RoleId").Value<string>();
            var dattable = CommonHelper.toDataTable(JArrayval);
            SqlParameter[] mparameters = new SqlParameter[]
                {
                    new SqlParameter("@UserId",UserId),
                    new SqlParameter("@FullName",FullName),
                    new SqlParameter("@EmailId",EmailId),
                    new SqlParameter("@Pwd",Pwd),
                    new SqlParameter("@Mobile",Mobile),
                    new SqlParameter("@IsActive",IsActive),
                    new SqlParameter("@CCode",CCode),
                    new SqlParameter("@EmpCode",!string.IsNullOrEmpty(EmpCode)?EmpCode:string.Empty),
                    new SqlParameter("@RoleId",RoleId),
                    new SqlParameter("@AssinedCircle",dattable)
                };
            string _lst = _dataRepository.ExcuteDataTableRowSqlCommand(StoredProcedureHelper.SaveandupdateUser, mparameters);

            return Ok(_lst);
        }
        [HttpPost]
        [Route("SaveChangePassword")]
        public IActionResult SaveChangePassword(JObject obj)
        {
            string LoginId = obj.GetValue("LoginId").Value<string>();
            string CurrentPwd = PasswordHelper.EncryptPwd(obj.GetValue("CurrentPwd").Value<string>());
            string NewPwd = PasswordHelper.EncryptPwd(obj.GetValue("NewPwd").Value<string>());

            object[] mparameters = { LoginId, CurrentPwd, NewPwd };
            string Result = _dataRepository.ExcuteSingleRowSqlCommand(StoredProcedureHelper.spChangeUserPassword, mparameters);

            return Ok(Result);
        }
        [HttpGet]
        [Route("GetAllActiveUser")]
        public IActionResult GetAllActiveUser()
        {
            object[] mparameters = { };
            string Result = _dataRepository.ExcuteRowSqlCommand(StoredProcedureHelper.spGetAllActiveUser, mparameters);
            return Ok(Result);
        }
        //[HttpPost]
        //public IActionResult RegisterNewUser(UserInfo info)
        //{
        //    object[] parameters = { info.FullName, info.EmailId, PasswordHelper.EncryptPwd(info.Pwd), info.Mobile, (int)Enums.UserRole.COLLECTOR };
        //    GResposnse usr = _dataRepository.RegisterNewUser(StoredProcedureHelper.spRegisterNewUser, parameters);

        //    return View(usr);
        //}
        //[Authorize(Roles = "Admin")]
        //public IActionResult GetAllCollector()
        //{
        //    object[] parameters = { (int)Enums.UserRole.COLLECTOR };
        //    List<GUserInfo> result = _dataRepository.GetUserByRoleId(StoredProcedureHelper.spGetUserByRoleId, parameters);
        //    result.Select(i =>
        //    {
        //        i.Pwd = PasswordHelper.DecryptPwd(i.Pwd);
        //        return i;
        //    }).ToList();
        //    return View(result);
        //}



        //[HttpGet]
        //[Route("GetAllCategories")]
        //public IActionResult GetAllCategories(int vendorcode)
        //{
        //    ReturnData<bool, List<shop_Categories>, string> rdata = new ReturnData<bool, List<shop_Categories>, string>();
        //    List<shop_Categories> obj = new List<shop_Categories>();
        //    try
        //    {
        //        obj = _shopServices.GetCategories(vendorcode);
        //        rdata.Data1 = true;
        //        rdata.Data2 = obj;
        //        return Ok(rdata);
        //    }
        //    catch (Exception ex)
        //    {
        //        rdata.Data1 = false;
        //        rdata.Data2 = null;
        //        rdata.Message = ex.Message;
        //        return Ok(rdata);
        //    }
        //}

        [HttpGet]
        [Route("GetMobileSubMenuByRoleId")]
        public IActionResult GetMobileSubMenuByRoleId(int RoleId)
        {
            SqlParameter[] parameters = new SqlParameter[]
              {
                  new SqlParameter("@RoleId",RoleId),
              };

            string Result = _dataRepository.ExecuteQuerySingleDataTableDynamicDataset(StoredProcedureHelper.spGetAllMobileMenuSMenuByRoleId, parameters);

            return Ok(Result);
        }
        [HttpGet]
        [Route("GetAllMobileMenuMaster")]
        public IActionResult GetAllMobileMenuMaster(int roleId)
        {
            SqlParameter[] parameters1 = new SqlParameter[]
             {

             };
            SqlParameter[] parameters = new SqlParameter[]
             {
                  new SqlParameter("@RoleId",roleId),
             };
            string _lst = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllMobileMenu, parameters1);
            string _lstSubMenu = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllMobileSubMenuByRole, parameters);
            var response = new { data1 = _lst, data2 = _lstSubMenu };
            return Ok(response);
        }
        [HttpPost]
        [Route("SaveandupdateMobileMenu")]
        public IActionResult SaveandupdateMobileMenu(JObject obj)
        {
            string JArrayval = obj.GetValue("JArrayval").Value<string>();
            string LoginId = obj.GetValue("LoginId").Value<string>();

            var dattable = CommonHelper.toDataTable(JArrayval);
            SqlParameter[] mparameters = new SqlParameter[]
                {
                    new SqlParameter("@MobileSubMenuType",dattable),
                    new SqlParameter("@CreatedBy",LoginId),
                    new SqlParameter("@CreatedDate",CommonHelper.IndianStandard(DateTime.UtcNow))
                };
            string _lst = _dataRepository.ExcuteDataTableRowSqlCommand(StoredProcedureHelper.spSaveNupdateMobileRole, mparameters);

            return Ok(_lst);
        }

        [HttpPost]
        [Route("GetAllUserLog")]
        public IActionResult GetAllUserLog(DataTableAjaxPostModel requestModel)
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
                    SortColumn = "LoginId";
                    break;
                case 2:
                    SortColumn = "LoginType";
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
             };

            string Result = _dataRepository.ExecuteQueryDynamicSqlParameter(StoredProcedureHelper.spGetAllUserLog, parameters);

            return Ok(Result);
        }
        [HttpGet]
        [Route("SendOTP")]
        public IActionResult SendOTP()
        {
            string Otp = "123456";
            string Message = "Dear Citizen, Your OTP for registration is " + Otp + ". Chennai Enviro. #NammaChennai#NammaPoruppu.";
            _SMSSenderHelper.SendMessage(Message, "7042618366");
            return Ok("");
        }
        [HttpPost]
        [Route("ChangeInchargePassword")]
        public IActionResult ChangeInchargePassword(JObject obj)
        {

            string LoginId = obj.GetValue("LoginId").Value<string>();
            string OldPwd = obj.GetValue("OldPwd").Value<string>();
            string NewPwd = obj.GetValue("NewPwd").Value<string>();

            SqlParameter[] mparameters = new SqlParameter[]
                {
                    new SqlParameter("@LoginId",LoginId),
                    new SqlParameter("@OldPwd",PasswordHelper.EncryptPwd(OldPwd)),
                    new SqlParameter("@NewPwd",PasswordHelper.EncryptPwd(NewPwd))
                };
            string _lst = _dataRepository.ExcuteDataTableRowSqlCommand(StoredProcedureHelper.spChangeInchargePassword, mparameters);

            return Ok(_lst);
        }

    }

}
