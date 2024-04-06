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
    public class RamkyService : IRamky<RamkyResposnse>
    {
        private IRepository<RamkyResposnse> _dataRepository;
        public RamkyService(IRepository<RamkyResposnse> dataRepository)
        {
            this._dataRepository = dataRepository;
        }
        public string ExecuteQuerySingleDataTableDynamic(string spQuery, SqlParameter[] Param)
        {
            return _dataRepository.ExecuteQuerySingleDataTableDynamic(spQuery, Param);
        }
    }
}
