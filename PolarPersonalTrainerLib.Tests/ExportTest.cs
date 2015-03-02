using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PolarPersonalTrainerLib.Tests
{
    [TestClass]
    public class ExportTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            PPTExport export = new PPTExport(Settings.UserName, Settings.Password);
            var doc = export.downloadSessions(DateTime.Now.AddDays(-7), DateTime.Now);
            Assert.IsNotNull(doc);
        }
    }
}
