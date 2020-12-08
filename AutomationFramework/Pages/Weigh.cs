using OpenQA.Selenium;

namespace AutomationFramework.Pages
{

  public class Weigh
  {


    public static By reset = By.CssSelector(".result > #reset");
    public static By weigh = By.Id("weigh");
   
    
    //Using two for loops, just in case there is a different number of left or right bars
    public void EnterBars(string[] left_bars, string[] right_bars)
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

    public void ResetBars()
    {
     var resets = Browser.getDriver.FindElements(By.Id("reset"));
     resets[1].Click();
      
    }

  }



}


