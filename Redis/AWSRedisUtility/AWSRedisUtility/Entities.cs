using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSRedisUtility.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte Age { get; set; }

        public override string ToString()
        {
            return string.Format("[USER] {0} aged {1}", this.Name, this.Age);
        }
    }
}
