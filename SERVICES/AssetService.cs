using HYDSWMAPI.INTERFACE;
using HYDSWMAPI.REPOSITORY;
using COMMON.CITIZEN;
using System.Data.SqlClient;

namespace HYDSWMAPI.SERVICES
{
    public class AssetService : IAsset<CLoginResponseInfo>
    {
        private IRepository<CLoginResponseInfo> _dataRepository;
        public AssetService(IRepository<CLoginResponseInfo> dataRepository)
        {
            this._dataRepository = dataRepository;
        }
        public string ExcuteRowSqlCommand(string spQuery, object[] Param)
        {
            return _dataRepository.ExecuteQueryDynamicList(spQuery, Param);
        }
        public string ExcuteSingleRowSqlCommand(string spQuery, object[] Param)
        {
            return _dataRepository.ExecuteQuerySingleDynamic(spQuery, Param);
        }
        public string ExecuteQueryDynamicDataset(string spQuery, object[] Param)
        {
            return _dataRepository.ExecuteQueryDynamicDataset(spQuery, Param);
        }
        public string ExecuteQuerySingleDataTableDynamicDataset(string spQuery, SqlParameter[] Param)
        {
            return _dataRepository.ExecuteQuerySingleDataTableDynamicDataset(spQuery, Param);
        }
        public string ExecuteQueryDynamicSqlParameter(string spQuery, SqlParameter[] Param)
        {
            return _dataRepository.ExecuteQueryDynamicSqlParameter(spQuery, Param);
        }
        public string ExecuteQuerySingleDataTableDynamic(string spQuery, SqlParameter[] Param)
        {
            return _dataRepository.ExecuteQuerySingleDataTableDynamic(spQuery, Param);
        }
    }
}
