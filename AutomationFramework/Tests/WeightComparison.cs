using AutomationFramework;
using AutomationFramework.Test;
using NUnit.Framework;
using OpenQA.Selenium;
using AutomationFramework.Pages;
using AutomationFramework.Interactions;
using System;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras;

namespace AutomatedTestCases
{
  [TestFixture]
  public class WeightComparison : AutomationCore
  {

    [Test]
    public void FindEmptyBar()
    {
      new Weigh().enterBars(new string[] { "0", "1", "2" }, new string[] { "3", "4", "5" });
      Browser.getDriver.FindElement(Weigh.weigh).Click();
      var first_weigh_text = Browser.getDriver.FindElement(Weigh.firstWeighing).Text;


      if (first_weigh_text.Contains("="))
      {
        compareWeights(new string[] { "6" }, new string[] { "8" }, new string[] { "6", "7", "8" });
      }

      if (first_weigh_text.Contains("<"))
      {
        compareWeights(new string[] { "0" }, new string[] { "2" }, new string[] { "0", "1", "2" });
      }

      if (first_weigh_text.Contains(">"))
      {
        compareWeights(new string[] { "3" }, new string[] { "5" }, new string[] { "3", "4", "5" });
      }

    }

    public void compareWeights(string[] leftBar, string[] rightBar, string[] bars)
    {
      new Weigh().resetBars();
      //string[] leftBar = { "3" };
      //string[] rightBar = { "5" };
      new Weigh().enterBars(leftBar, rightBar);
      Browser.getDriver.FindElement(Weigh.weigh).Click();
      var second_weigh_text = Browser.getDriver.FindElement(Weigh.secondWeighing).Text;
      if (second_weigh_text.Contains("="))
      {
        Browser.getDriver.FindElement(By.Id("coin_" + bars[1])).Click();
      }
      if (second_weigh_text.Contains(">"))
      {
        Browser.getDriver.FindElement(By.Id("coin_" + bars[2])).Click();
      }
      if (second_weigh_text.Contains("<"))
      {
        Browser.getDriver.FindElement(By.Id("coin_" + bars[0])).Click();
      }
    }

  }


}
