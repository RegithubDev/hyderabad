using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HYDSWMAPI.INTERFACE
{
    public interface IOperation<T1>
    {
        string ExcuteRowSqlCommand(string spQuery, object[] Param);
        string ExcuteSingleRowSqlCommand(string spQuery, object[] Param);
        string ExecuteQueryDynamicDataset(string spQuery, object[] Param);
        string ExecuteQuerySingleDataTableDynamicDataset(string spQuery, SqlParameter[] Param);
        string ExecuteQueryDynamicSqlParameter(string sqlQuery, SqlParameter[] usernameParam);
        string ExecuteQuerySingleDataTableDynamic(string sqlQuery, SqlParameter[] usernameParam);
    }
}
