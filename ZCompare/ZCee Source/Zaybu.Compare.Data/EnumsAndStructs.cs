using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaybu.Compare.Data
{
    public enum ZCompareOption
    {
        Simple, Verbose, DifferencesOnly
    }

    public struct ZCompareStruct
    {
        public int ID { get; set; }
        public float Value { get; set; }
    }

    public class ZCompareOptions
    {
        public ZCompareOption Option { get; set; }
        public int ID { get; set; }
        public ZCompareStruct Result { get; set; }
    }


    public struct StructWithStaticProperty
    {
        private readonly int _x;
        private readonly int _y;

        public StructWithStaticProperty(int x, int y)
        {
            _x = x;
            _y = y;
        }

        private static StructWithStaticProperty _origin = new StructWithStaticProperty(0, 0);

        public static StructWithStaticProperty Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }
    }

    public struct Point
    {
        private readonly int _x;
        private readonly int _y;

        public Point(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public int X
        {
            get { return _x; }
        }

        public int Y
        {
            get { return _y; }
        }
    }

}
