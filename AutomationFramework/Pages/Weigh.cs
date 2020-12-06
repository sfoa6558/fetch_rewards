using AutomationFramework;
using AutomationFramework.Test;
using OpenQA.Selenium;

namespace AutomationFramework.Pages
{

  public class Weigh
  {


    public static By reset = By.CssSelector("#reset");
    public static By weigh = By.Id("weigh");
    public static By firstWeighing = By.CssSelector("ol  > li:first-of-type");
    public static By secondWeighing = By.CssSelector("ol > li:nth-of-type(2)");
   
    public void enterBars(string[] left_bars, string[] right_bars)
    {

      for (int i = 0; i < left_bars.Length; i++)
      {

        Browser.getDriver.FindElement(By.Id("left_" + i.ToString())).SendKeys(left_bars[i]);
      }

      for (int i = 0; i < right_bars.Length; i++)
      {
        Browser.getDriver.FindElement(By.Id("right_" + i.ToString())).SendKeys(right_bars[i]);
      }
    }

    public void resetBars()
    {
      for (int i = 0; i < 3; i++)
      {
        Browser.getDriver.FindElement(By.Id("right_" + i.ToString())).SendKeys(Keys.Backspace);
        Browser.getDriver.FindElement(By.Id("left_" + i.ToString())).SendKeys(Keys.Backspace);
      }
    }

  }



}


