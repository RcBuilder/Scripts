using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using CommonEntities.SSO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Web;

namespace MHCampus.BL.Tests
{
    // members: 
    // https://msdn.microsoft.com/en-us/library/system.web.hosting.simpleworkerrequest_members(v=vs.71).aspx
    public class HttpPostWorker : SimpleWorkerRequest /*HttpWorkerRequest*/
    {
        public string QueryString { get; set; }
        public Dictionary<string, string> PostData { get; set; }
        public string Method = "POST";
        public string ContentType = "application/x-www-form-urlencoded";
        public Dictionary<string, string> ServerVariables { get; set; }

        public HttpPostWorker() : base("", "", "", null, new StringWriter()) {
            this.PostData = new Dictionary<string, string>();
            this.ServerVariables = new Dictionary<string, string>();
        }

        public override string GetQueryString() {
            return this.QueryString;
        }

        public override string GetHttpVerbName() {
            return Method;
        }

        public override string GetServerVariable(string name) {
            if (this.ServerVariables.ContainsKey(name))
                return this.ServerVariables[name];
            return null;
        }

        public override string GetKnownRequestHeader(int index) {
            switch (index) {
                case HttpWorkerRequest.HeaderContentLength:
                    return this.PostData.Count.ToString();
                case HttpWorkerRequest.HeaderContentType:
                    return ContentType;
                default:
                    return base.GetKnownRequestHeader(index);
            }
        }

        public override byte[] GetPreloadedEntityBody() {
            var strPostData = string.Join("&", this.PostData.Select(x => string.Concat(x.Key, "=", x.Value)));
            return Encoding.UTF8.GetBytes(strPostData);
        }

        public override string GetFilePathTranslated() {
            return "HttpPostWorker";
        }

        public override string GetServerName() {
            return "HttpPostWorker";
        }

        public override string GetAppPath() {
            return "HttpPostWorker";
        }
    }
}
