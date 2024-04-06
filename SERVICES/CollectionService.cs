using COMMON.SWMENTITY;
using HYDSWMAPI.INTERFACE;
using HYDSWMAPI.REPOSITORY;
using System.Data.SqlClient;

namespace HYDSWMAPI.SERVICES
{
    public class CollectionService : ISWMCollection<GResposnse>
    {
        private IRepository<GResposnse> _masterRepository1;
        public CollectionService(IRepository<GResposnse> masterRepository1)
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
        public string ExecuteQuerySingleDataTableDynamicDataset(string spQuery, SqlParameter[] Param)
        {
            return _masterRepository1.ExecuteQuerySingleDataTableDynamicDataset(spQuery, Param);
        }
    }
}
