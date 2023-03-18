using AutomationFramework;
using NUnit.Framework;
using OpenQA.Selenium;
using AutomationFramework.Pages;


namespace AutomatedTestCases
{
    [TestFixture]
    public class Internet : AutomationCore
    {
        public readonly AddRemovePage addRemovePage = new AddRemovePage();

      
        [TestCase(7)]
        public void AddTest(int el)
        {

            AddElements(el);
            VerifyElements(el);


        }

        public void AddElements(int n)
        {

            for (var i = 0; i < n; i++)
            {
                Browser.getDriver.FindElement(addRemovePage.addElement).Click();

            }
        }

        public void VerifyElements(int n)
        {
            Assert.AreEqual(Browser.getDriver.FindElements(addRemovePage.addedElement).Count, n);

        }


    }

    

}



