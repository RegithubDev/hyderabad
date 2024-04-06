using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HYDSWMAPI.INTERFACE
{
   public interface IRamky<T1>
    {
        string ExecuteQuerySingleDataTableDynamic(string sqlQuery, SqlParameter[] usernameParam);
    }
}
