using System;
using System.Diagnostics;

// Install-Package Hangfire.Core
// Install-Package Hangfire.AspNet
// Install-Package Hangfire.SqlServer
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Owin;

namespace BLL
{
    public class HangfireManager
    {
        public class CONFIG {
            public const string TimeZoneName = "Israel Standard Time";
        }

        public class DashboardNoAuthorizationFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize(DashboardContext dashboardContext)
            {
                return true;
            }
        }

        public static bool Init(IAppBuilder app)
        {
            try
            {
                var config = GlobalConfiguration.Configuration;

                // basic config 
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings();

                // use MS-SQL Storage
                config.UseSqlServerStorage(ConfigSingleton.Instance.ConnStr);

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
            catch (AggregateException aex)
            {
                aex.Data.Add("Method", "Init");
                aex.Data.Add("ExType", "AggregateException");
                aex.Data.Add("InnerException", aex.InnerException?.Message);
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

        public static bool Start()
        {
            // TODO ->> (NTH) Improve tasks registration using IHangfireTask
            ////public interface IHangfireTask
            ////{
            ////    string CronExpressions { get; }
            ////    void Exexute();
            ////}

            ////public class SomeTask : IHangfireTask
            ////{
            ////    public string CronExpressions { get; private set; } = "*/15 * * * *";
            ////    public void Exexute()
            ////    {
            ////        Debug.WriteLine("In SomeTask");
            ////    }
            ////}

            ////public class OtherTask : IHangfireTask
            ////{
            ////    public string CronExpressions { get; private set; } = "*/1 * * * *";
            ////    public void Exexute()
            ////    {
            ////        Debug.WriteLine("In OtherTask");
            ////    }
            ////}

            try
            {
                var eventNotificationProcess = new EventNotificationProcess();
                string[] time_splited = ConfigSingleton.Instance.DailyEventNotificationHour.Split(':');
                int hour = int.Parse(time_splited[0]);
                int minutes = int.Parse(time_splited[1]);
                RecurringJob.AddOrUpdate(
                    "EventNotification", 
                    () => eventNotificationProcess.RunAsync(), 
                    Cron.Daily(hour, minutes), // $"{minutes} {hour} * * *", 
                    TimeZoneInfo.FindSystemTimeZoneById(CONFIG.TimeZoneName) // TimeZoneInfo.Local
                );  // everyday at 9 AM. also Cron.Daily(9)

                var eventUtilitiesProcess = new EventUtilitiesProcess();
                RecurringJob.AddOrUpdate(
                    "EventUtilities", 
                    () => eventUtilitiesProcess.RunAsync(), 
                    Cron.Daily(hour, minutes), // $"{minutes} {hour} * * *", 
                    TimeZoneInfo.FindSystemTimeZoneById(CONFIG.TimeZoneName) // TimeZoneInfo.Local
                );

                /* 
                    Test:
                    DateTime t = DateTime.Now.AddMinutes(1);
                    string jobId = BackgroundJob.Schedule(() => eventUtilitiesProcess.RunAsync(), t);
                */

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
                ex.Data.Add("ExType", "Exception");
                LoggerSingleton.Instance.Error("HangfireManager", ex);

                Debug.WriteLine($"[ERROR] HangfireManager.Start. ex: {ex.Message}");
                return false;
            }
        }

        public static void Stop() { }
    }
}
