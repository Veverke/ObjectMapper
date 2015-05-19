using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Mapper.Data
{
    public class MyObj
    {
        public string s { get; set; }

        public void m()
        {
            System.Console.WriteLine();
        }

        public override string ToString()
        {
            return string.Format("s: [{0}]", s);
        }
    }

    public class A
    {
        public int a { get; set; }
        public int b { get; set; }
        public string c { get; set; }
        public MyObj obj { get; set; }
        public B myB { get; set; }
        public int x { get; set; }

        public override string ToString()
        {
            return String.Format("a: [{0}] b:[{1}] c:[{2}] obj:[{3}] x:[{4}]", a, b, c, obj, x);
        }
    }

    public class B
    {
        public int x { get; set; }
        public string y { get; set; }
        public MyObj obj { get; set; }
        public int a { get; set; }

        public override string ToString()
        {
            return String.Format("x: [{0}] y:[{1}] obj:[{2}] a:[{3}]", x, y, obj, a);
        }
    }
}
