using AutomationFramework;
using NUnit.Framework;
using OpenQA.Selenium;
using AutomationFramework.Pages;


namespace AutomatedTestCases
{
  [TestFixture]
  public class WeightComparison : AutomationCore
  {


    [Test]
    public void FindEmptyBar()
    {

      new Weigh().EnterBars(new string[] { "0", "1", "2" }, new string[] { "3", "4", "5" });
      Browser.getDriver.FindElement(Weigh.weigh).Click();
      var result = Browser.getDriver.FindElement(Weigh.reset).Text;

      switch (result)
      {
        case "=":
          CompareWeights(new string[] { "6" }, new string[] { "8" }, new string[] { "6", "7", "8" });
          break;
        case "<":
          CompareWeights(new string[] { "0" }, new string[] { "2" }, new string[] { "0", "1", "2" });
          break;
        case ">":
          CompareWeights(new string[] { "3" }, new string[] { "5" }, new string[] { "3", "4", "5" });
          break;
      }


    }

    public void CompareWeights(string[] leftBar, string[] rightBar, string[] bars)
    {

      new Weigh().ResetBars();

      new Weigh().EnterBars(leftBar, rightBar);
      Browser.getDriver.FindElement(Weigh.weigh).Click();
      var result = Browser.getDriver.FindElement(Weigh.reset).Text;
      Browser.TakeScreenshot();

      switch (result)
      {
        case "=":
          Browser.getDriver.FindElement(By.Id("coin_" + bars[1])).Click();
          break;
        case "<":
          Browser.getDriver.FindElement(By.Id("coin_" + bars[0])).Click();
          break;
        case ">":
          Browser.getDriver.FindElement(By.Id("coin_" + bars[2])).Click();
          break;
      }

      Assert.AreEqual(Browser.SaveAlertText(), "Yay! You find it!");
    }

  }


}
