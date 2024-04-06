using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HYDSWMAPI.INTERFACE
{
    public interface ISWMMaster<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
    {
        List<T1> GetAllReason(string spQuery, object[] Param);
        List<T2> GetAllOwnerType(string spQuery, object[] Param);
        List<T3> GetAllPropertyType(string spQuery, object[] Param);
        T4 AddAndUpdate(string spQuery, object[] Param);
        T5 GetHouseHoldInfoById(string spQuery, object[] Param);
        List<T6> GetAllHouseHoldInfo(string spQuery, object[] Param);
        List<T7> GetAllCircle(string spQuery, object[] Param);
        List<T8> GetAllWard(string spQuery, object[] Param);
        List<T9> GetAllIdentityType(string spQuery, object[] Param);
        List<T10> GetAllShift(string spQuery, object[] Param);
        List<T11> GetAllDesignation(string spQuery, object[] Param);
        List<T12> GetAllSector(string spQuery, object[] Param);
        string GetAllCircleByUser(string spQuery, object[] Param);
        string GetAllWardByUser(string spQuery, object[] Param);
        string GetAllContrator(string spQuery, object[] Param);
        string ExcuteRowSqlCommand(string spQuery, object[] Param);
        string ExcuteSingleRowSqlCommand(string spQuery, object[] Param);
        string ExecuteQueryDynamicSqlParameter(string sqlQuery, SqlParameter[] usernameParam);
        string ExecuteQuerySingleDataTableDynamic(string sqlQuery, SqlParameter[] usernameParam);

    }
}
