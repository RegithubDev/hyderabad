using COMMON.SWMENTITY;
using HYDSWMAPI.INTERFACE;
using HYDSWMAPI.REPOSITORY;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HYDSWMAPI.SERVICES
{
    public class MasterService: IMaster<GResposnse>
    {
        private IRepository<GResposnse> _masterRepository1;
        public MasterService(IRepository<GResposnse> masterRepository1)
        {
            this._masterRepository1 = masterRepository1;
        }
        public string ExecuteQueryDynamicSqlParameter(string spQuery, SqlParameter[] Param)
        {
            return _masterRepository1.ExecuteQueryDynamicSqlParameter(spQuery, Param);
        }
        public string ExecuteQuerySingleDataTableDynamic(string spQuery, SqlParameter[] Param)
        {
            return _masterRepository1.ExecuteQuerySingleDataTableDynamic(spQuery, Param);
        }
    }
}
