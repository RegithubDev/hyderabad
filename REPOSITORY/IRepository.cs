using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HYDSWMAPI.REPOSITORY
{
    public interface IRepository<T> : IDisposable where T : class
    {
        //IEnumerable<T> ExecuteQuery(string spQuery, object[] parameters);
        //T ExecuteQuerySingle(string spQuery, object[] parameters);
        //int ExecuteCommand(string spQuery, object[] parameters);


        Task<List<T>> GetAllRow();
        Task<T> GetById(int id);
        Task<T> Add(T entity);
        Task<T> Update(T entity, object key);
        bool IsExist(Func<T, bool> predicate);
        T GetDataSingle(Func<T, bool> predicate);
        Task<T> Delete(int id);
        IQueryable<T> GetAll();

        List<T> ExecuteQuery(string sqlQuery, object[] usernameParam);
       T ExecuteQuerySingle(string sqlQuery, object[] usernameParam);
        string ExecuteQuerySingleDynamic(string sqlQuery, object[] usernameParam);
        string ExecuteQueryDynamicList(string sqlQuery, object[] usernameParam);
        string BExecuteQuerySingleDynamic(string sqlQuery, object[] usernameParam);
        string BExecuteQueryDynamicList(string sqlQuery, object[] usernameParam);
        string ExecuteQueryDynamicDataset(string sqlQuery, object[] usernameParam);
        string ExecuteQuerySingleDataTableDynamic(string sqlQuery, SqlParameter[] usernameParam);
        string ExecuteQueryDynamicSqlParameter(string sqlQuery, SqlParameter[] usernameParam);
        string ExecuteQuerySingleDataTableDynamicDataset(string sqlQuery, SqlParameter[] usernameParam);
        int ExecuteCommand(string spQuery, object[] parameters);
    }
}
