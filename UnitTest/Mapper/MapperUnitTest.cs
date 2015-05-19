using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTest.Mapper.Data;
using System.Collections.Generic;
using ObjectMapper;
using ObjectMapper.Types;

namespace UnitTest.Mapper
{
    [TestClass]
    public class MapperUnitTest
    {
        [TestMethod]
        public void MapValueTypes()
        {
            #region Arrange
            A a = new A { a = 10, b = 20, c = "ccc", x = 9 };
            B b = new B { a = 1000, x = 12300, y = "yyyyy" };

            ObjectMapper.Mapper mapper = new ObjectMapper.Mapper(); 
            #endregion
            
            #region Act
            B mappedObject = mapper.Map<A, B>(a); 
            #endregion

            #region Assert
            Assert.IsTrue(a.a == mappedObject.a && a.x == mappedObject.x);
            #endregion
        }

        [TestMethod]
        public void MapReferenceTypes()
        {
            #region Arrange
            A a = new A { a = 10, b = 20, c = "ccc", obj = new MyObj { s = "sssss" } };
            B b = new B { a = 1000, obj = new MyObj { s = "ydsydgsauqwdwudhawud" }, x = 12300, y = "yyyyy" };

            ObjectMapper.Mapper mapper = new ObjectMapper.Mapper();
            #endregion

            #region Act
            B mappedObject = mapper.Map<A, B>(a);
            #endregion

            #region Assert
            Assert.IsTrue(a.a == mappedObject.a && a.obj.s == mappedObject.obj.s);
            #endregion
        }

        [TestMethod]
        public void MapExtraction()
        {
            #region Arrange
            A a = new A { a = 10, b = 20, c = "ccc", obj = new MyObj { s = "sssss" }, myB = new B { a = 1, x = 2, obj = new MyObj { s = "123513ijudadas" } } };
            B b = new B { a = 1000, obj = new MyObj { s = "ydsydgsa" }, x = 12300, y = "yyyyy" };

            ObjectMapper.Mapper mapper = new ObjectMapper.Mapper();
            #endregion

            #region Act
            MappedObject<B> mappedObject = mapper.Map<A, B>(a, new Dictionary<string, List<string>> { { "myB", new List<string> { "x" } } });
            #endregion

            #region Assert
            Assert.IsTrue(a.a == mappedObject.RegularMapping.a && a.obj.s == mappedObject.RegularMapping.obj.s && a.myB.x == mappedObject.Extras.x);
            #endregion
        }

        [TestMethod]
        public void MapWithIgnoreList()
        {
            #region Arrange
            A a = new A { a = 10, b = 20, c = "ccc", obj = new MyObj { s = "sssss" }, x = 1000 };
            B b = new B { a = 1000, obj = new MyObj { s = "ydsydgsa" }, x = 12300, y = "yyyyy" };
            B untouchedB = new B { a = 1000, obj = new MyObj { s = "ydsydgsa" }, x = 12300, y = "yyyyy" };

            ObjectMapper.Mapper mapper = new ObjectMapper.Mapper();
            #endregion

            #region Act
            B mappedObject = mapper.Map<A, B>(a, overrideWithDefaultValues: false, fieldsToIgnore: new List<string> { "obj", "x" });
            #endregion

            #region Assert
            Assert.IsTrue(a.a == mappedObject.a && mappedObject.obj == null && mappedObject.x == default(Int32));
            #endregion
        }

        /* ----------------------------------------------------------------------------------------------------------------------------------
         * Not applicable: a new instance of the destination object is created at each mapping, it will thus always start with default values. 
         * If we choose to skip some properties, the destination counterparts will be left with their initial values.
         * ----------------------------------------------------------------------------------------------------------------------------------
         */

        //[TestMethod]
        //public void MapWithoutOverridingDefaults()
        //{
        //    #region Arrange
        //    A a = new A { a = 0, b = 20, c = "ccc", x = 1000 };
        //    B b = new B { a = 1000, obj = new MyObj { s = "ydsydgsa" }, x = 12300, y = "yyyyy" };

        //    ObjectMapper.Mapper.Mapper mapper = new ObjectMapper.Mapper.Mapper();
        //    #endregion

        //    #region Act
        //    B mappedObject = mapper.Map<A, B>(a, overrideWithDefaultValues: false, fieldsToIgnore: new List<string> { "obj", "x" });
        //    #endregion

        //    #region Assert
        //    Assert.IsTrue(mappedObject.a == b.a && mappedObject.obj.s == b.obj.s);
        //    #endregion
        //}

        [TestMethod]
        public void MapWithDefaultsOverriding()
        {
            #region Arrange
            A a = new A { a = 0, b = 20, c = "ccc", x = 1000 };
            B b = new B { a = 1000, obj = new MyObj { s = "ydsydgsa" }, x = 12300, y = "yyyyy" };

            ObjectMapper.Mapper mapper = new ObjectMapper.Mapper();
            #endregion

            #region Act
            B mappedObject = mapper.Map<A, B>(a, overrideWithDefaultValues: false, fieldsToIgnore: new List<string> { "obj", "x" });
            #endregion

            #region Assert
            Assert.IsTrue(a.a == mappedObject.a && mappedObject.obj == null);
            #endregion
        }

        [TestMethod]
        public void MapWithCustomMapping()
        {
            #region Arrange
            A a = new A { a = 0, b = 20, c = "ccc", x = 1000 };
            B b = new B { a = 1000, obj = new MyObj { s = "ydsydgsa" }, x = 12300, y = "yyyyy" };

            ObjectMapper.Mapper mapper = new ObjectMapper.Mapper();
            #endregion

            #region Act
            B mappedObject = mapper.Map<A, B>(a, overrideWithDefaultValues: false, fieldsToIgnore: new List<string> { "obj" }, customMapping: new Dictionary<string, string> { { "x", "y" } });
            #endregion

            #region Assert
            Assert.IsTrue(a.a == mappedObject.a && mappedObject.obj == null && mappedObject.y == a.x.ToString());
            #endregion
        }
    }
}
