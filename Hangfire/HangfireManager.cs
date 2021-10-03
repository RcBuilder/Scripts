using System;
using System.Diagnostics;

// Install-Package Hangfire.Core
// Install-Package Hangfire.AspNet
// Install-Package Hangfire.SqlServer
// Install-Package Hangfire.MySqlStorage
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MySql;
using Hangfire.SqlServer;
using Owin;

namespace DistributionServiceBLL
{
    public class HangfireManager
    {
        public class DashboardNoAuthorizationFilter : IDashboardAuthorizationFilter {
            public bool Authorize(DashboardContext dashboardContext) {                
                return true;
            }
        }

        public static bool Init(IAppBuilder app) {
            try
            {
                var config = GlobalConfiguration.Configuration;

                // basic config 
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings();

                // use MS-SQL Storage
                config.UseSqlServerStorage(ConfigSingleton.Instance.HangfireConnStr);
                
                // use MySQL Storage
                // can also install manually using the 'hangfire_Install.sql' procedure
                ///config.UseStorage(new MySqlStorage(Config.Instance.WPConnStr, new MySqlStorageOptions { TablesPrefix = "hangfire_" }));

                // add dashboard                
                app.UseHangfireDashboard("/hangfire", new DashboardOptions
                {
                    IgnoreAntiforgeryToken = true,
                    IsReadOnlyFunc = (DashboardContext context) => true,  // readOnly dashboard
                    Authorization = new[] { 
                        new DashboardNoAuthorizationFilter() 
                    }                  
                });
                
                // add background server
                app.UseHangfireServer(new BackgroundJobServerOptions { WorkerCount = 1 });
                
                return true;
            }
            catch (AggregateException aex) {
                aex.Data.Add("Method", "Init");
                aex.Data.Add("ExType", "AggregateException");
                LoggerSingleton.Instance.Error("HangfireManager", aex);

                Debug.WriteLine($"[ERROR] HangfireManager.Init. ex: {aex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                ex.Data.Add("Method", "Init");
                ex.Data.Add("ExType", "Exception");                
                LoggerSingleton.Instance.Error("HangfireManager", ex);

                Debug.WriteLine($"[ERROR] HangfireManager.Init. ex: {ex.Message}");
                return false;
            } 
        }

        public static bool Start() {
            try
            {                
                var p1 = new NotificationsProcess();
                RecurringJob.AddOrUpdate("NotificationsProcess", () => p1.RunAsync(), Cron.Minutely);

                var p2 = new RedialProcess();
                RecurringJob.AddOrUpdate("RedialProcess", () => p2.RunAsync(), Cron.MinuteInterval(10));
                return true;
            }
            catch (AggregateException aex)
            {
                aex.Data.Add("Method", "Start");
                aex.Data.Add("ExType", "AggregateException");
                LoggerSingleton.Instance.Error("HangfireManager", aex);

                Debug.WriteLine($"[ERROR] HangfireManager.Start. ex: {aex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                ex.Data.Add("Method", "Start");
                ex.Data.Add("ExType", "AggregateException");
                LoggerSingleton.Instance.Error("HangfireManager", ex);

                Debug.WriteLine($"[ERROR] HangfireManager.Start. ex: {ex.Message}");
                return false;
            }
        }

        public static void Stop() {}
    }
}
