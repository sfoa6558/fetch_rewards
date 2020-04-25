using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
namespace AutomationFramework
{
    

    namespace Test
    {
        [TestFixture]
        public class AutomationCore
        {
            IWebDriver driver;
            
            // Our Core Test Automation class
            [SetUp]
            public void startTest() // This method will be fired at the start of the test
            {
                Browser.Init();
            }
            [TearDown]
            public void endTest() // This method will be fired at the end of the test
            {
                Browser.Close();
            }

        }
    }
}
