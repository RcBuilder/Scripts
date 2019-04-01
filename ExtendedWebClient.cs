using System;
using System.Net;

namespace BLL
{
    public class ExtendedWebClient : WebClient
    {
        public Uri ResponseUri { private set; get; }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);
            this.ResponseUri = response.ResponseUri;
            return response;
        }
    }
}
