using COMMON.CITIZEN;
using COMMON.GENERIC;
using COMMON.SWMENTITY;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HYDSWMAPI.DAL
{
    public class CKCContext : DbContext
    {
        public CKCContext() { }
        public CKCContext(DbContextOptions<CKCContext> options)
            : base(options)
        {
            Database.Migrate();
        }
        // public DbSet<tbl_User> tbl_User { get; set; }
        [NotMapped]
        public DbSet<LoginResponse> LoginResponse { get; set; }
        [NotMapped]
        public DbSet<GResposnse> GResposnse { get; set; }
        [NotMapped]
        public DbSet<GUserInfo> GUserInfo { get; set; }
        [NotMapped]
        public DbSet<ReasonInfo> ReasonInfo { get; set; }
        [NotMapped]
        public DbSet<OwnerTypeInfo> OwnerTypeInfo { get; set; }

        [NotMapped]
        public DbSet<PropertyTypeInfo> PropertyTypeInfo { get; set; }
        [NotMapped]
        public DbSet<HouseholdInfo> HouseholdInfo { get; set; }
        [NotMapped]
        public DbSet<HouseHold_Paging> HouseHold_Paging { get; set; }
        [NotMapped]
        public DbSet<CircleInfo> CircleInfo { get; set; }
        [NotMapped]
        public DbSet<WardInfo> WardInfo { get; set; }
        [NotMapped]
        public DbSet<IdentityTypeInfo> IdentityTypeInfo { get; set; }
        [NotMapped]
        public DbSet<QRScannedInfo> QRScannedInfo { get; set; }
        [NotMapped]
        public DbSet<ShiftWiseCollectionInfo> ShiftWiseCollectionInfo { get; set; }
        [NotMapped]
        public DbSet<ShiftInfo> ShiftInfo { get; set; }
        
        [NotMapped]
        public DbSet<NDayCollectionInfo> NDayCollectionInfo { get; set; }
        [NotMapped]
        public DbSet<DashboardNotification> DashboardNotification { get; set; }
        [NotMapped]
        public DbSet<MapViewInfo> MapViewInfo { get; set; }
        
        [NotMapped]
        public DbSet<EmployeeInfo_Paging> EmployeeInfo_Paging { get; set; }
        [NotMapped]
        public DbSet<EmployeeInfo> EmployeeInfo { get; set; }
        [NotMapped]
        public DbSet<DesignationInfo> DesignationInfo { get; set; }
        [NotMapped]
        public DbSet<SectorInfo> SectorInfo { get; set; }
        
        [NotMapped]
        public DbSet<CLoginResponseInfo> CLoginResponseInfo { get; set; }
        [NotMapped]
        public DbSet<EmpTypeInfo> EmpTypeInfo { get; set; }
        [NotMapped]
        public DbSet<RamkyResposnse> RamkyResposnse { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // throw new UnintentionalCodeFirstException();

        }
    }
}
