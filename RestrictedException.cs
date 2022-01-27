using System;

namespace Telegram
{
    public class RestrictedException : Exception {
        public RestrictedException(Exception InnerEx) : base("## Restricted ##", InnerEx) { }
    }
}
