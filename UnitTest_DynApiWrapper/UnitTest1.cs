using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest_DynApiWrapper
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            DynApiWrapper wrapper = new DynApiWrapper();
            wrapper.Login();
        }
    }
}
