using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTest.Data;
using System.Collections.Generic;
using ObjectMapper.Types;
using System.Diagnostics;

namespace UnitTest
{
    [TestClass]
    public class MapperUnitTest
    {
        [TestMethod]
        /* ----------------------------------------
         * Test Case 1
         * Description: Map objects containing value and reference type properties plus a nested object
         * ---------------------------------------*/
        public void TC1()
        {
            #region Arrange
            A a = new A(1000, "hello world", new Obj(50, "github", new DateTime(1982, 11, 14)));
            //B b = new B(-1, "b string", new Obj(999, "google", new DateTime(2015, 1, 1)));

            #endregion Arrange

            #region Act
            Mapper mapper = new Mapper();
            B mappedObject = mapper.Map<A, B>(a);
            #endregion Act

            #region Assert

            Assert.IsTrue(
                    a.Int == mappedObject.Int
                && a.String == mappedObject.String
                && a.Obj.Int == mappedObject.Obj.Int
                && a.Obj.String == mappedObject.Obj.String
                && a.Obj.Date == mappedObject.Obj.Date);

            #endregion Assert
        }

        /* ----------------------------------------
         * Test Case 2
         * Description: Map objects containing value and reference type properties plus a nested object, with DIFFERENT NAMES (and thus custom mappings)
         * ---------------------------------------*/
        [TestMethod]
        public void TC2()
        {
            #region Arrange
            A a = new A(1000, "hello world", new Obj(50, "github", new DateTime(1982, 11, 14)));

            #endregion Arrange

            #region Act
            Mapper mapper = new Mapper();
            Dictionary<string, List<string>> customMappings = new Dictionary<string, List<string>>
            {
                {"int", new List<string> {"inttc2"}}, 
                { "string", new List<string> {"stringTC2"} }
            };
            B_TC2 mappedObject = mapper.Map<A, B_TC2>(a, customMappings: customMappings);
            #endregion Act

            #region Assert

            Assert.IsTrue(
                    a.Int == mappedObject.IntTC2
                && a.String == mappedObject.StringTC2/*
                && a.Obj.Int == mappedObject.ObjTC2.Int
                && a.Obj.String == mappedObject.ObjTC2.String
                && a.Obj.Date == mappedObject.ObjTC2.Date*/);

            #endregion Assert
        }

        /* ----------------------------------------
        * Test Case 3
        * Description: Map objects containing value and reference type properties plus a nested object, with DIFFERENT NAMES (and thus custom mappings - using "contains")
        * If more than one property contains the string, the 1st is used as the map destination.
        * ---------------------------------------*/
        [TestMethod]
        public void TC3()
        {
            #region Arrange
            A a = new A(1000, "hello world", new Obj(50, "github", new DateTime(1982, 11, 14)));

            #endregion Arrange

            #region Act
            Mapper mapper = new Mapper();
            Dictionary<string, List<string>> customMappings = new Dictionary<string, List<string>>
            {
                {"int", new List<string> { "inttc2" }}, 
                { "contains", new List<string> { "bj"} }
            };
            B_TC2 mappedObject = mapper.Map<A, B_TC2>(a, customMappings: customMappings);
            #endregion Act

            #region Assert

            Assert.IsTrue(
                    a.Int == mappedObject.IntTC2
                //&& a.String == mappedObject.StringTC2
                && a.Obj.Int == mappedObject.ObjTC2.Int
                && a.Obj.String == mappedObject.ObjTC2.String
                && a.Obj.Date == mappedObject.ObjTC2.Date);

            #endregion Assert
        }
        /* ----------------------------------------
        * Test Case 4
        * Description: Map objects marked with the "Mapping" attribute, to the specified map. If more than one match is specified and more than one match is found in the destination type, the property will be mapped to the first match.
        * ---------------------------------------*/
        [TestMethod]
        public void TC4()
        {
            #region Arrange
            A a = new A(1000, "hello world", new Obj(50, "github", new DateTime(1982, 11, 14)));

            #endregion Arrange

            #region Act
            Mapper mapper = new Mapper();
            C mappedObject = mapper.Map<A, C>(a);
            #endregion Act

            #region Assert

            Assert.IsTrue(
                    a.Int == mappedObject.Int
                //&& a.String == mappedObject.StringTC2
                && a.String == mappedObject.String
                && a.String == mappedObject.String2);

            #endregion Assert
        }
    }
}
