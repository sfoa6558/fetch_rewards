using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationFramework.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.PageObjects;
    using System.Threading;
    using NUnit.Framework;
    using System.IO;

    namespace Test
    {
        public class Home
        {
            
 
            public static By metrics = By.CssSelector("a[data-automation-id='home-metrics']");
            //public By metrics => By.CssSelector("a[data-automation-id='home-metrics']");

            
            public bool isAt()
            {
                return Browser.Title.Contains("Contact Us");
            }

           
        }
    }
}
