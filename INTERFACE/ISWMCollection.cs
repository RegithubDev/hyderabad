using System.Data.SqlClient;

namespace HYDSWMAPI.INTERFACE
{
    public interface ISWMCollection<T1>
    {
        string ExecuteQueryDynamicSqlParameter(string sqlQuery, SqlParameter[] usernameParam);
        string ExecuteQuerySingleDataTableDynamic(string sqlQuery, SqlParameter[] usernameParam);
        string ExecuteQuerySingleDataTableDynamicDataset(string sqlQuery, SqlParameter[] usernameParam);
    }
}
