using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwaggerEngine
{
    public class SwaggerConfigDefaultMetaData : SwaggerConfigMetaData
    {
        public SwaggerConfigDefaultMetaData(string Version, string Title) {
            this.Version = Version;
            this.Title = Title;
        }
    }
}
