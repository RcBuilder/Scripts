using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zaybu.Compare.Data.BaseClasses
{
    public abstract class EntityBase
    {
        public int Id { get; set; }

        public override string ToString()
        {
            //return Type.Descritpion();
            return GetType().ToString();
        }
    }
}
