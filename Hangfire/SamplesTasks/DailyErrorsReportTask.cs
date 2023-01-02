using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.HangfireTasks
{
    public class DailyErrorsReportTask : IHangfireTask
    {
        private DateTime DateFilter = DateTime.Now.AddDays(-1);  // Yesterday        
        private string DestEmail = "ravit@openbook.co.il";

        public string CronExpressions { get; private set; } =  "0 1 * * *";
        public string sDateFilter {
            get {
                return this.DateFilter.ToString("yyyy-MM-dd");
            }
        }


        public void Exexute() {
            try
            {
                var errors = Logs.GetDailyErrors(this.DateFilter);

                // no errors to process
                if (errors == null || errors.Count == 0)
                    return;
                
                ProcessReport(errors);
            }
            catch (Exception ex)
            {
                Logs.Add(new Entities.Log {
                    Name = "HangfireTask",
                    Method = "BLL.HangfireTasks.DailyErrorsReportTask",
                    Title = "Generic Task Exception",
                    Description = ex.Message,
                    LogType = (byte)Entities.eLogType.Error
                });                
            }
        }

        public void ProcessReport(List<Entities.Log> errors) {
            try
            {
                var sbRows = new StringBuilder();

                var isOdd = false;
                foreach (var error in errors) {
                    var bgColor = isOdd ? "#f3f3f3" : "none";
                    isOdd = !isOdd; // switch

                    sbRows.Append($@"
                            <tr style=""background-color: {bgColor};"">
                                <td style=""vertical-align:top; padding: 10px 0; padding-right: 10px;"">{error.Id}</td>
                                <td style=""vertical-align:top; padding: 10px 0;"">{error.Name}</td>
                                <td style=""vertical-align:top; padding: 10px 0;"">{error.Title}</td>
                                <td style=""vertical-align:top; padding: 10px 0;"">{error.Description}</td>
                            </tr>
                    ");
                }

                var Body = Email.LoadTemplate("DailyErrorsReportEmail.html");
                Body = string.Format(Body, this.sDateFilter, sbRows.ToString(), errors.Count);
                Email.Send(this.DestEmail, Body, "דוח שגיאות יומי", "ravit@openbook.co.il");

                Logs.Add(new Entities.Log
                {
                    Name = "HangfireTask",
                    Method = "BLL.HangfireTasks.ProcessReport",
                    Title = $"Daily Errors Report",
                    Description = $"Daily errors report has sent to {this.DestEmail} | Date: {this.sDateFilter} ({errors.Count} rows)",
                    LogType = (byte)Entities.eLogType.Info
                });
            }
            catch (Exception ex)
            {
                Logs.Add(new Entities.Log
                {
                    Name = "HangfireTask",
                    Method = "BLL.HangfireTasks.ProcessReport",
                    Title = $"Report Exception",
                    Description = ex.Message,
                    LogType = (byte)Entities.eLogType.Error
                });
            }
        }       
    }
}
