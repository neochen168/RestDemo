using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestDemo;

namespace UnitTestDynnApiWrapper
{
    [TestClass]
    public class DynDemoTest
    {
        [TestMethod]
        public void TetAddZone()
        {
            DynApiWrapper wrapper = new DynApiWrapper();
            wrapper.Login();
            wrapper.AddZone("www.test.com");
            wrapper.GetZone("www.test.com");
        }
    }
}
