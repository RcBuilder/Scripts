using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAL
{
    public class DBProviderFactory
    {
        public IDalLayer Produce(string Name) {
            switch (Name) {
                case "Pervasive": return new Pervasive();
                case "Access": return new Access();
                default:
                case "Dummy": return new Dummy();
            }
        }
    }
}