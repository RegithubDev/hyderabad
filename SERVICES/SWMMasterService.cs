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
    public class SWMMasterService : ISWMMaster<ReasonInfo, OwnerTypeInfo, PropertyTypeInfo, GResposnse, HouseholdInfo, HouseHold_Paging, CircleInfo, WardInfo, IdentityTypeInfo, ShiftInfo, DesignationInfo, SectorInfo>
    {
        private IRepository<ReasonInfo> _masterRepository;
        private IRepository<OwnerTypeInfo> _masterRepository1;
        private IRepository<PropertyTypeInfo> _masterRepository2;
        private IRepository<GResposnse> _masterRepository3;
        private IRepository<HouseholdInfo> _masterRepository4;
        private IRepository<HouseHold_Paging> _masterRepository5;
        private IRepository<CircleInfo> _masterRepository6;
        private IRepository<WardInfo> _masterRepository7;
        private IRepository<IdentityTypeInfo> _masterRepository8;
        private IRepository<ShiftInfo> _masterRepository9;
        private IRepository<DesignationInfo> _masterRepository10;
        private IRepository<SectorInfo> _masterRepository11;
        public SWMMasterService(IRepository<ReasonInfo> masterRepository, IRepository<OwnerTypeInfo> masterRepository1, IRepository<PropertyTypeInfo> masterRepository2, IRepository<GResposnse> masterRepository3, IRepository<HouseholdInfo> masterRepository4, IRepository<HouseHold_Paging> masterRepository5, IRepository<CircleInfo> masterRepository6, IRepository<WardInfo> masterRepository7, IRepository<IdentityTypeInfo> masterRepository8, IRepository<ShiftInfo> masterRepository9, IRepository<DesignationInfo> masterRepository10, IRepository<SectorInfo> masterRepository11)
        {
            this._masterRepository = masterRepository;
            this._masterRepository1 = masterRepository1;
            this._masterRepository2 = masterRepository2;
            this._masterRepository3 = masterRepository3;
            this._masterRepository4 = masterRepository4;
            this._masterRepository5 = masterRepository5;
            this._masterRepository6 = masterRepository6;
            this._masterRepository7 = masterRepository7;
            this._masterRepository8 = masterRepository8;
            this._masterRepository9 = masterRepository9;
            this._masterRepository10 = masterRepository10;
            this._masterRepository11 = masterRepository11;
        }
        public List<ReasonInfo> GetAllReason(string spQuery, object[] Param)
        {
            return _masterRepository.ExecuteQuery(spQuery, Param);
        }
        public List<OwnerTypeInfo> GetAllOwnerType(string spQuery, object[] Param)
        {
            return _masterRepository1.ExecuteQuery(spQuery, Param);
        }
        public List<PropertyTypeInfo> GetAllPropertyType(string spQuery, object[] Param)
        {
            return _masterRepository2.ExecuteQuery(spQuery, Param);
        }
        public GResposnse AddAndUpdate(string spQuery, object[] Param)
        {
            return _masterRepository3.ExecuteQuerySingle(spQuery, Param);
        }
        public HouseholdInfo GetHouseHoldInfoById(string spQuery, object[] Param)
        {
            return _masterRepository4.ExecuteQuerySingle(spQuery, Param);
        }
        public List<HouseHold_Paging> GetAllHouseHoldInfo(string spQuery, object[] Param)
        {
            return _masterRepository5.ExecuteQuery(spQuery, Param);
        }
        public List<CircleInfo> GetAllCircle(string spQuery, object[] Param)
        {
            return _masterRepository6.ExecuteQuery(spQuery, Param);
        }
        public List<WardInfo> GetAllWard(string spQuery, object[] Param)
        {
            return _masterRepository7.ExecuteQuery(spQuery, Param);
        }
        public List<IdentityTypeInfo> GetAllIdentityType(string spQuery, object[] Param)
        {
            return _masterRepository8.ExecuteQuery(spQuery, Param);
        }
        public List<ShiftInfo> GetAllShift(string spQuery, object[] Param)
        {
            return _masterRepository9.ExecuteQuery(spQuery, Param);
        }
        public List<DesignationInfo> GetAllDesignation(string spQuery, object[] Param)
        {
            return _masterRepository10.ExecuteQuery(spQuery, Param);
        }
        public List<SectorInfo> GetAllSector(string spQuery, object[] Param)
        {
            return _masterRepository11.ExecuteQuery(spQuery, Param);
        }
        public string GetAllCircleByUser(string spQuery, object[] Param)
        {
            return _masterRepository3.ExecuteQueryDynamicList(spQuery, Param);
        }
        public string GetAllWardByUser(string spQuery, object[] Param)
        {
            return _masterRepository3.ExecuteQueryDynamicList(spQuery, Param);
        }
        public string GetAllContrator(string spQuery, object[] Param)
        {
            return _masterRepository3.ExecuteQueryDynamicList(spQuery, Param);
        }
        public string ExcuteRowSqlCommand(string spQuery, object[] Param)
        {
            return _masterRepository1.ExecuteQueryDynamicList(spQuery, Param);
        }
        public string ExcuteSingleRowSqlCommand(string spQuery, object[] Param)
        {
            return _masterRepository1.ExecuteQuerySingleDynamic(spQuery, Param);
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
