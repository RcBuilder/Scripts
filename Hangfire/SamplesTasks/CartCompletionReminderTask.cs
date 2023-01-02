using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.HangfireTasks
{
    public class CartCompletionReminderTask : IHangfireTask
    {        
        private int NumOfMinutes { get; set; } = 200;
        private int ToleranceInMinutes { get; set; } = 30;
        private bool DEBUG = false;
        private string DebugEmail = "ravit@openbook.co.il";

        public string CronExpressions { get; private set; } =  "*/15 * * * *";  
             
        public void Exexute() {
            try
            {
                var from = DateTime.Now.AddMinutes(-NumOfMinutes);
                var to = DateTime.Now.AddMinutes(-ToleranceInMinutes); 
                var carts = Carts.GetHistory(from, to)?.ToList();

                // no carts to process
                if (carts == null || carts.Count == 0)
                    return;

                foreach (var cart in carts)
                    ProcessCart(cart);
            }
            catch (Exception ex)
            {
                Logs.Add(new Entities.Log {
                    Name = "HangfireTask",
                    Method = "BLL.HangfireTasks.CartCompletionReminderTask",
                    Title = "Generic Task Exception",
                    Description = ex.Message,
                    LogType = (byte)Entities.eLogType.Error
                });                
            }
        }

        public void ProcessCart(Entities.Cart cart) {
            try
            {
                if (cart.Notified) return;

                var sbRows = new StringBuilder();
                foreach (var item in cart.Items)
                    sbRows.Append($"<div><a href=\"{Carts.GetItemHref(item)}\" style=\"color:#777;\">{Carts.GetItemTypeName(item)} {item.ItemName}</a></div>");

                var Body = Email.LoadTemplate("CartCompletionReminderEmail.html");
                Body = string.Format(Body, cart.Student.FirstName, sbRows.ToString());
                Email.Send(DEBUG ? DebugEmail : cart.Student.Email, Body, "סיום רכישה באתר openbook", "ravit@openbook.co.il");

                Logs.Add(new Entities.Log
                {
                    Name = "HangfireTask",
                    Method = "BLL.HangfireTasks.ProcessCart",
                    Title = $"Cart Reminder Task",
                    Description = $"Completion reminder has sent to student #{cart.Student.Id} regarding cart #{cart.Id} with ({cart.ItemsCount} items)",
                    LogType = (byte)Entities.eLogType.Info
                });

                // update flag
                cart.Notified = true;
                if(!DEBUG) Carts.Save(cart);
            }
            catch (Exception ex)
            {
                Logs.Add(new Entities.Log
                {
                    Name = "HangfireTask",
                    Method = "BLL.HangfireTasks.ProcessCart",
                    Title = $"Task Exception for cart #{cart.Id}",
                    Description = ex.Message,
                    LogType = (byte)Entities.eLogType.Error
                });
            }
        }       
    }
}
