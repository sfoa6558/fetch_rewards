using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System.Configuration;
using System.IO;

namespace AutomationFramework
{
  public class Browser
  {
    public static IWebDriver webDriver;

    private static readonly string baseURL = ConfigurationManager.AppSettings["url"];
    private static readonly string browser = ConfigurationManager.AppSettings["browser"];

    public static void Init()
    {
      switch (browser)
      {
        case "Chrome":
          webDriver = new ChromeDriver();
          break;
        case "IE":
          webDriver = new InternetExplorerDriver();
          break;
        case "Firefox":
          webDriver = new FirefoxDriver();
          break;
      }
      webDriver.Manage().Window.Maximize();
      Goto(baseURL);
    }
    public static string Title
    {
      get { return webDriver.Title; }
    }
    public static IWebDriver getDriver
    {
      get { return webDriver; }
    }

    public static void TakeScreenshot()
    {

      string path = ConfigurationManager.AppSettings["screenshot"];
      var dateTimeString = DateTime.Now.ToString("yyyy-MM-dd-HH_mm_ss");
      Directory.CreateDirectory(path);
      Screenshot ss = ((ITakesScreenshot)getDriver).GetScreenshot();
      ss.SaveAsFile(path + "\\" + "Internet" + "-" + dateTimeString + ".jpg", ScreenshotImageFormat.Png);

    }

   

    public static void Goto(string url)
    {
      webDriver.Url = url;
      
    }
    public static void Close()
    {
      webDriver.Quit();
    }
  }
}
