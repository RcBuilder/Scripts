using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web.SessionState;
using System.Linq;
using System.Web;

namespace BLL
{
    public class ConnectedSession
    {
        public HttpSessionState Session { get; set; }
        public int StudentId { get; set; }
        public bool IsRemoved { get; set; }
    }

    public class ConnectedSessionsSingleTon
    {        
        private static volatile ConnectedSessionsSingleTon _instance;
        private static object syncRoot = new Object();

        private ConnectedSessionsSingleTon() { }

        public static ConnectedSessionsSingleTon Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new ConnectedSessionsSingleTon();
                    }
                }

                return _instance;
            }
        }

        // ---

        private List<ConnectedSession> ConnectedSessions = new List<ConnectedSession>();

        public void Add(ConnectedSession cs) {
            this.ConnectedSessions.Add(cs);
        }

        public void MarkAllToRemove(int studentId) {
            this.ConnectedSessions.Where(x => x.StudentId == studentId).ToList()?.ForEach(cs => cs.IsRemoved = true);
        }

        public void Remove(ConnectedSession cs)
        {
            cs.Session[Sessions.KEY_STUDENT] = "";
            cs.Session.Remove(Sessions.KEY_STUDENT);
            this.ConnectedSessions.Remove(cs);
        }

        public void ProcessCurrentContext(HttpContextBase context)
        {
            if (context?.Session?.SessionID == null) return;            
            var scToRemove = this.ConnectedSessions.FirstOrDefault(cs => cs.IsRemoved && cs.Session.SessionID == context.Session.SessionID);
            if (scToRemove == null) return;
            this.Remove(scToRemove);

            var cookie = context.Request.Cookies[Sessions.KEY_STUDENT];
            if (cookie == null) return;

            cookie.Value = null;
            cookie.Expires = DateTime.Now.AddDays(-1);
            context.Response.Cookies.Add(cookie);
        }
    }
}