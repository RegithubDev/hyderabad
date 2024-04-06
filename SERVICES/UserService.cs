using COMMON.SWMENTITY;
using HYDSWMAPI.INTERFACE;
//using CKCAPI.Models;
using HYDSWMAPI.REPOSITORY;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace HYDSWMAPI.SERVICES
{
    public class UserService : IUser<tbl_User, LoginResponse, GUserInfo, GResposnse>
    {
        private IRepository<tbl_User> EmpRepository;
        private IRepository<LoginResponse> LoginRepository;
        private IRepository<GUserInfo> LoginRepository1;
        private IRepository<GResposnse> LoginRepository2;
        public UserService(IRepository<tbl_User> empRepository, IRepository<LoginResponse> loginRepository, IRepository<GUserInfo> loginRepository1, IRepository<GResposnse> loginRepository2)
        {
            this.EmpRepository = empRepository;
            this.LoginRepository = loginRepository;
            this.LoginRepository1 = loginRepository1;
            this.LoginRepository2 = loginRepository2;
        }

        public IEnumerable<tbl_User> GetAll()
        {
            return EmpRepository.GetAll();
        }

        public async Task<List<tbl_User>> GetAllList()
        {
            return await EmpRepository.GetAllRow();
        }
        public async Task<tbl_User> GetById(int id)
        {
            return await EmpRepository.GetById(id);
        }
        //public bool IsExist(tbl_User employee)
        //{
        //    Func<tbl_User, bool> mypredicate = (p => p.EmpCode == employee.EmpCode && p.UserId != employee.UserId);
        //    return EmpRepository.IsExist(mypredicate);
        //}
        //public tbl_User Login(string LoginId, string password)
        //{
        //    Func<tbl_User, bool> mypredicate = (p => p.EmpCode == LoginId && p.Pwd == password && p.IsActive == true);
        //    return EmpRepository.GetDataSingle(mypredicate);
        //}
        public async Task<tbl_User> Add(tbl_User employee)
        {
            return await EmpRepository.Add(employee);
        }
        public async Task<tbl_User> Update(tbl_User employee, object key)
        {
            return await EmpRepository.Update(employee, key);
        }
        public async Task<tbl_User> Delete(int id)
        {
            return await EmpRepository.Delete(id);
        }
        public LoginResponse Login(string spQuery, object[] Param)
        {
            return LoginRepository.ExecuteQuerySingle(spQuery, Param);
        }
        public GResposnse RegisterNewUser(string spQuery, object[] Param)
        {
            return LoginRepository2.ExecuteQuerySingle(spQuery, Param);
        }
        public List<GUserInfo> GetUserByRoleId(string spQuery, object[] Param)
        {
            return LoginRepository1.ExecuteQuery(spQuery, Param);
        }
        public string ExcuteRowSqlCommand(string spQuery, object[] Param)
        {
            return LoginRepository1.ExecuteQueryDynamicList(spQuery, Param);
        }
        public string ExcuteSingleRowSqlCommand(string spQuery, object[] Param)
        {
            return LoginRepository1.ExecuteQuerySingleDynamic(spQuery, Param);
        }
        public string ExcuteDataTableRowSqlCommand(string spQuery, SqlParameter[] Param)
        {
            return LoginRepository1.ExecuteQuerySingleDataTableDynamic(spQuery, Param);
        }
        public string ExecuteQuerySingleDataTableDynamicDataset(string spQuery, SqlParameter[] Param)
        {
            return LoginRepository1.ExecuteQuerySingleDataTableDynamicDataset(spQuery, Param);
        }
        public string ExecuteQueryDynamicSqlParameter(string spQuery, SqlParameter[] Param)
        {
            return LoginRepository1.ExecuteQueryDynamicSqlParameter(spQuery, Param);
        }
        public string ExecuteQuerySingleDataTableDynamic(string spQuery, SqlParameter[] Param)
        {
            return LoginRepository1.ExecuteQuerySingleDataTableDynamic(spQuery, Param);
        }
    }
}
