using ObjectMapper.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Data
{
    //public class MyObj
    //{
    //    public string s { get; set; }

    //    public void m()
    //    {
    //        System.Console.WriteLine();
    //    }

    //    public override string ToString()
    //    {
    //        return string.Format("s: [{0}]", s);
    //    }
    //}

    //public class A
    //{
    //    public int Int { get; set; }
    //    public string String { get; set; }
    //    public MyObj Obj { get; set; }
    //    public B myB { get; set; }
    //    public int x { get; set; }

    //    public override string ToString()
    //    {
    //        return String.Format("Int: [{0}] String:[{1}] Obj:[{2}] x:[{3}]", Int, String, Obj, x);
    //    }
    //}

    //public class B
    //{
    //    public int x { get; set; }
    //    public string y { get; set; }
    //    public MyObj obj { get; set; }
    //    public int a { get; set; }

    //    public override string ToString()
    //    {
    //        return String.Format("x: [{0}] y:[{1}] obj:[{2}] a:[{3}]", x, y, obj, a);
    //    }
    //}

    public class Obj
    {
        public int Int { get; set; }
        public string String { get; set; }
        public DateTime Date { get; set; }

        //nested object type needs parameterless constructor
        public Obj()
        {

        }

        public Obj(int Int, string String, DateTime Date)
        {
            this.Int = Int;
            this.String = String;
            this.Date = Date;
        }

        //for easing debugging
        public override string ToString()
        {
            return string.Format("Int: [{0}], String: [{1}], Date: [{2}]", this.Int, this.String, this.Date.ToShortDateString());
        }
    }

    public class A
    {
        
        public int Int { get; set; }
        [Mapping("myProp", "string2")]
        public string String { get; set; }
        public Obj Obj { get; set; }

        public A(int Int, string String, Obj Obj)
        {
            this.Int = Int;
            this.String = String;
            this.Obj = Obj;
        }

        //for easing debugging
        public override string ToString()
        {
            return string.Format("Int: [{0}], String: [{1}], Obj: [{2}]", this.Int, this.String, this.Obj.ToString());
        }
    }

    public class B
    {
        public int Int { get; set; }
        public string String { get; set; }
        public Obj Obj { get; set; }

        //destination type needs to have parameterless constructor
        public B()
        {

        }

        public B(int Int, string String, Obj Obj)
        {
            this.Int = Int;
            this.String = String;
            this.Obj = Obj;
        }

        //for easing debugging
        public override string ToString()
        {
            return string.Format("Int: [{0}], String: [{1}], Obj: [{2}]", this.Int, this.String, this.Obj.ToString());
        }
    }

    public class B_TC2
    {
        public int IntTC2 { get; set; }
        public string StringTC2 { get; set; }
        public Obj ObjTC2 { get; set; }

        //destination type needs to have parameterless constructor
        public B_TC2()
        {

        }

        public B_TC2(int Int, string String, Obj Obj)
        {
            this.IntTC2 = Int;
            this.StringTC2 = String;
            this.ObjTC2 = Obj;
        }

        //for easing debugging
        public override string ToString()
        {
            return string.Format("Int: [{0}], String: [{1}], Obj: [{2}]", this.IntTC2, this.StringTC2, this.ObjTC2.ToString());
        }
    }

    public class C
    {
        public int Int { get; set; }
        public string String { get; set; }
        public string String2 { get; set; }
    }
}
