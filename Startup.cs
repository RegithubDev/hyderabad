using HYDSWMAPI.DAL;
using HYDSWMAPI.INTERFACE;
using HYDSWMAPI.REPOSITORY;
using HYDSWMAPI.SECURITY;
using HYDSWMAPI.SERVICES;
using COMMON;
using COMMON.CITIZEN;
using COMMON.GENERIC;
using COMMON.SWMENTITY;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using COMMON.ASSET;

namespace HYDSWMAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            StaticConfig = configuration;
        }
        public static IConfiguration StaticConfig { get; private set; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = long.MaxValue;
            });
            services.AddCors();
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.Formatting = Formatting.Indented;
                //options.JsonSerializerOptions.WriteIndented = true;
            });

            services.AddCors(option => option.AddPolicy("MyBlogPolicy", builder =>
            {
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();

            }));

            services.AddDbContext<CKCContext>(options =>
                 options.UseSqlServer(Configuration.GetConnectionString("CKCENTITY")));
            services.AddDbContext<CKCContext>(options =>
                 options.UseSqlServer(Configuration.GetConnectionString("BELENTITY")));
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUser<tbl_User, LoginResponse, GUserInfo, GResposnse>, UserService>();
            services.AddScoped<ISWMMaster<ReasonInfo, OwnerTypeInfo, PropertyTypeInfo, GResposnse, HouseholdInfo, HouseHold_Paging, CircleInfo, WardInfo, IdentityTypeInfo, ShiftInfo, DesignationInfo, SectorInfo>, SWMMasterService>();
            services.AddScoped<IAsset<CLoginResponseInfo>, AssetService>();
            services.AddTransient<IOperation<CLoginResponseInfo>, OperationService>();
            services.AddTransient<IDeployement<CLoginResponseInfo>, DeploymentService>();
            services.AddTransient<IRptOperation<CLoginResponseInfo>, RptOperationService>();
            services.AddScoped<IMaster<GResposnse>, MasterService>();
            services.AddScoped<IComplaint<CLoginResponseInfo>, ComplaintService>();
            services.AddScoped<IRamky<RamkyResposnse>, RamkyService>();
            services.AddTransient<ISWMCollection<GResposnse>, CollectionService>();
            services.AddScoped<SMSSenderHelper>();
            services.AddScoped<TReport>();
            // configure basic authentication 
            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);




        }

                //HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            //app.UseMiddleware<AuthenticationMiddleware>();

            app.UseCors("MyBlogPolicy");
            //app.UseCors(x => x
            //   .AllowAnyOrigin()
            //   .AllowAnyMethod()
            //   .AllowAnyHeader());

            // Enable directory browsing
            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                     Path.Combine(env.ContentRootPath, "wwwroot")),
                RequestPath = "/content"
            });


            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
