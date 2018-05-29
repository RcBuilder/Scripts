using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaybu.Compare.Data
{
    public interface ITestInterface
    {
        string TestName { get; set; }
    }

    public class TestInterfaceObjectA : ITestInterface
    {
        public int ID { get; set; }

        public string TestName
        {
            get
            {
                return "Test A";
            }

            set
            {
                string temp = value;
            }
        }
    }

    public class TestInterfaceObjectB : ITestInterface
    {
        public DateTime Created { get; set; }

        public string TestName
        {
            get
            {
                return "Test B";
            }

            set
            {
                string temp = value;
            }
        }
    }

    public class TestInterfaceParent
    {
        public List<ITestInterface> Children { get; set; }
    }

}
