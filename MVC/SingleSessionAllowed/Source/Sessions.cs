using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace BLL
{
    public class Sessions
    {
        public const string KEY_STUDENT = "Student";        

        public static Entities.LoggedInAccount SetStudent(HttpContext context, Entities.Student student, bool fullProcess = false) 
        {
            var account = new Entities.LoggedInAccount {
                Id = student.Id,
                UserName = student.Email,
                accountType = Entities.eAccountType.Student
            };

            context.Response.Cookies.Add(new HttpCookie(KEY_STUDENT, account.CookieValue) {
                Expires = DateTime.Now.AddDays(7),
                Secure = true,
                
            });
                        
            context.Session[KEY_STUDENT] = account;

            if (fullProcess)
            {
                // no multiple connections allowed
                // clear all connected sessions and add only the current one
                ConnectedSessionsSingleTon.Instance.MarkAllToRemove(student.Id);
                ConnectedSessionsSingleTon.Instance.Add(new ConnectedSession
                {
                    StudentId = student.Id,
                    Session = context.Session
                });
            }

            return account;
        }

        public static Entities.LoggedInAccount GetStudent(HttpContext context)
        {
            try
            {                
                // get from session
                var accountSession = context.Session[KEY_STUDENT];
                if (accountSession != null) return (Entities.LoggedInAccount)accountSession;

                // check for cookie
                var Cookie = context.Request.Cookies[KEY_STUDENT];
                if (Cookie == null) return null;

                // get student
                var student = Students.Get(Convert.ToInt32(Cookie.Value));
                if (student == null) return null;

                // update session and return the created login account
                return SetStudent(context, student);
            }
            catch { return null; }
        }

        public static void ClearStudent(HttpSessionState Session)
        {
            Session[KEY_STUDENT] = "";
            Session.Remove(KEY_STUDENT);            
        }

        public static void ClearStudent(ConnectedSession connectedSession) {
            ClearStudent(connectedSession.Session);
        }

        public static void ClearStudent(HttpContext context)
        {            
            ClearStudent(context.Session);

            var cookie = context.Request.Cookies[KEY_STUDENT];
            if (cookie == null) return;
            cookie.Value = null;
            cookie.Expires = DateTime.Now.AddDays(-1);
            context.Response.Cookies.Add(cookie);
        }
       
        public static bool IsStudent(HttpContext context)
        {
            return GetStudent(context) != null;
        }        
    }
}
