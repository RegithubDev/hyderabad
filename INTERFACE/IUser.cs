using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HYDSWMAPI.INTERFACE
{
    public interface IUser<TEntity, T2, T3, T4>
    {
        IEnumerable<TEntity> GetAll();

        Task<List<TEntity>> GetAllList();

        Task<TEntity> GetById(int id);
        Task<TEntity> Add(TEntity entity);
        //  bool IsExist(TEntity entity);
        Task<TEntity> Update(TEntity entity, object key);
        Task<TEntity> Delete(int id);
        T2 Login(string spQuery, object[] Param);
        T4 RegisterNewUser(string spQuery, object[] Param);
        List<T3> GetUserByRoleId(string spQuery, object[] Param);
        string ExcuteRowSqlCommand(string spQuery, object[] Param);
        string ExcuteSingleRowSqlCommand(string spQuery, object[] Param);
        string ExcuteDataTableRowSqlCommand(string spQuery, SqlParameter[] Param);
        string ExecuteQuerySingleDataTableDynamicDataset(string spQuery, SqlParameter[] Param);
        string ExecuteQueryDynamicSqlParameter(string sqlQuery, SqlParameter[] usernameParam);
        string ExecuteQuerySingleDataTableDynamic(string sqlQuery, SqlParameter[] usernameParam);
    }
}
