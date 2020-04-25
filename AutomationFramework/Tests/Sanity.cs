using AutomationFramework;
using AutomationFramework.Test;
using NUnit.Framework;
using OpenQA.Selenium;
using AutomationFramework.Pages.Test;
using AutomationFramework.Interactions;

namespace AutomatedTestCases
{
    [TestFixture]
    public class Sanity : AutomationCore
    {
        [Test]
        public void ClickContactUs()
        {
            UiAction.Click(Home.metrics, 3);
            UiAction.Click(Home.metrics, 3);

        }
    }
}
