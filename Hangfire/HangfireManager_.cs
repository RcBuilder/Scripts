using System;
using System.Diagnostics;

// Install-Package Hangfire.Core
// Install-Package Hangfire.AspNet
// Install-Package Hangfire.MySqlStorage
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Owin;
using System.Collections.Generic;

namespace Website
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
                config.UseSqlServerStorage(BLL.Config.HangfireConnStr);
                                
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
                aex.Data.Add("Method", "HangfireManager.Init");
                aex.Data.Add("ExType", "AggregateException");
                Log.Error(aex);

                Debug.WriteLine($"[ERROR] HangfireManager.Init. ex: {aex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                ex.Data.Add("Method", "HangfireManager.Init");
                ex.Data.Add("ExType", "Exception");
                Log.Error(ex);

                Debug.WriteLine($"[ERROR] HangfireManager.Init. ex: {ex.Message}");
                return false;
            } 
        }

        public static bool Start() {
            try
            {
                var tasks = new List<BLL.HangfireTasks.IHangfireTask>()
                {
                    new BLL.HangfireTasks.CartCompletionReminderTask()
                };

                tasks.ForEach(task => {
                    RecurringJob.AddOrUpdate(task.GetType().Name, () => task.Exexute(), task.CronExpressions);
                });
                              
                return true;
            }
            catch (AggregateException aex)
            {
                aex.Data.Add("Method", "HangfireManager.Start");
                aex.Data.Add("ExType", "AggregateException");
                Log.Error(aex);

                Debug.WriteLine($"[ERROR] HangfireManager.Start. ex: {aex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                ex.Data.Add("Method", "HangfireManager.Start");
                ex.Data.Add("ExType", "AggregateException");
                Log.Error(ex);

                Debug.WriteLine($"[ERROR] HangfireManager.Start. ex: {ex.Message}");
                return false;
            }
        }

        public static void Stop() {}
    }
}
