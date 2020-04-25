using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using AutomationFramework;
namespace AutomationFramework.Interactions
{
   

    public static class UiAction
    {
        //TODO: put this path in config file..?
        private const string SCREENSHOT_FOLDER_ROOT_PATH = "//kirk/Development/Internal/DevImpl/UIAutomation/Screenshots/E2E/";
        private delegate T AnonMethod<T>(By e, string v, double t);



        public static void TakeScreenShot(string Folder, string Filename)
        {
            string FullLocationPath;
            var currentURL = Browser.webDriver.Url;
            var validEnv = new List<string>()
                        {"sandbox-21", "sandbox-36",  "sandbox-78", "tin", "copper", "bronze", "pewter", "presb", "prodevensb", "dc1demosb", "carbon"};

            var environment = validEnv.FirstOrDefault(env => currentURL.Contains(env));
            if (string.IsNullOrEmpty(environment))
            {
                environment = "OtherEnvironment";
            }
            FullLocationPath = $"{SCREENSHOT_FOLDER_ROOT_PATH}{Folder}/{environment}/{Filename}.png";

            try
            {
                Screenshot ss = ((ITakesScreenshot)Browser.webDriver).GetScreenshot();
                ss.SaveAsFile(FullLocationPath, ScreenshotImageFormat.Png);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        private static string ExecuteAction<T>(AnonMethod<T> execMethod, By element, string value, double timeout = 0, string logMessage = "")
        {
            double seconds = 0;
            bool actionComplete = false;
            bool checkForTDError = true;
            string returned = "";
            DateTime exeStart = DateTime.Now;
            DateTime exeEnd;

            // apply the timeout multiplier
            timeout *= 5;

            do
            {
                if (seconds > 0)
                {
                    Thread.Sleep(250);
                }

                returned = execMethod(element, value, timeout).ToString();


                try
                {
                    Browser.webDriver.SwitchTo().Alert();
                    checkForTDError = false;
                }
                catch (NoAlertPresentException)
                {
                    checkForTDError = true;
                }

                if (checkForTDError)
                {
                    // check for technical difficulties after the action has attempted to complete
                    var hasTechnicalDifficulties = Browser.webDriver.PageSource.Contains("having technical difficulties");

                    if (hasTechnicalDifficulties)
                    {
                        var tdGuid = Browser.webDriver.FindElement(By.CssSelector("span#lblErrorGuid")).Text;
                        Console.WriteLine(string.Format("Action Produced a Technical Difficulties Error: GUID = {0}", tdGuid));
                        throw new InvalidElementStateException(
                          string.Format("Action Produced a Technical Difficulties Error: GUID = {0}", tdGuid));
                    }
                }

                actionComplete = !returned.Contains("stack trace");

                if (!actionComplete)
                {
                    seconds = seconds + 0.25;
                }
            } while (seconds < timeout && !actionComplete);

            exeEnd = DateTime.Now;



            if (!string.IsNullOrEmpty(logMessage))
            {
                Console.WriteLine(string.Format("EVENT: {0} | [RETURN]: {1} | {2}", logMessage, returned, (exeEnd - exeStart).TotalSeconds.ToString("0.000") + "s"));
            }

            if (returned.Contains("stack trace"))
            {
                throw new Exception(returned);
            }

            return returned;
        }

        public static string ClearAlert(bool acceptAlertMessage, double timeout = 0)
        {
            AnonMethod<string> method = delegate (By e, string v, double t)
            {
                try
                {
                    if (acceptAlertMessage)
                        Browser.webDriver.SwitchTo().Alert().Accept();
                    else
                        Browser.webDriver.SwitchTo().Alert().Dismiss();
                    return "";

                }
                catch (Exception except)
                {
                    var trace = except.StackTrace;
                    return ($"This is the stack trace {trace}");
                }
            };

            return ExecuteAction(method, null, "", timeout, string.Format("[ClearAlert] | {0} alert message", (acceptAlertMessage) ? "Accepting" : "Dismissing"));
        }

        public static string Click(By element, double timeout = 0, string page = null)
        {
            AnonMethod<string> method = delegate (By e, string v, double t)
            {
                try
                {
                    Browser.webDriver.FindElements(e).First().Click();
                    return "";
                }
                catch (Exception except)
                {

                    var trace = except.StackTrace;
                    return ($"Unable to click element {element} on page {page} after {timeout} seconds and this is the stack trace {trace}");

                }

            };

            return ExecuteAction(method, element, "", timeout, string.Format("[Click] | {0}", element.ToString()));
        }

        public static string GetText(By element, double timeout = 0)
        {
            AnonMethod<string> method = delegate (By e, string v, double t)
            {
                IWebElement uiElement = Browser.webDriver.FindElement(e);
                string text = default(string);

                try
                {
                    if (uiElement.TagName == "select")
                    {
                        var selectedVal = uiElement.GetAttribute("value");
                        SelectElement selectEle = new SelectElement(uiElement);

                        text = selectEle.SelectedOption.Text;
                    }
                    else
                    {
                        text = uiElement.Text;
                    }

                    return text;
                }
                catch (Exception except)
                {
                    var trace = except.StackTrace;
                    return ($"This is the stack trace {trace}");
                }
            };

            return ExecuteAction(method, element, "", timeout, string.Format("[GetText] | {0}", element.ToString()));
        }

        public static string GetValue(By element, double timeout = 0)
        {
            AnonMethod<string> method = delegate (By e, string v, double t)
            {


                try
                {
                    IWebElement uiElement = Browser.webDriver.FindElement(e);
                    string text = default(string);

                    if (uiElement.TagName == "input" && (uiElement.GetAttribute("type") == "checkbox" || uiElement.GetAttribute("type") == "radio"))
                    {
                        text = uiElement.Selected.ToString();
                    }
                    else if (uiElement.TagName == "select")
                    {
                        var selectedVal = uiElement.GetAttribute("value");
                        SelectElement selectEle = new SelectElement(uiElement);

                        text = selectEle.SelectedOption.GetAttribute("value");
                    }
                    else
                    {
                        text = uiElement.GetAttribute("value");
                    }

                    return text;
                }
                catch (Exception except)
                {
                    var trace = except.StackTrace;
                    return ($"This is the stack trace {trace}");
                }
            };

            return ExecuteAction(method, element, "", timeout, string.Format("[GetValue] | {0}", element.ToString()));
        }

        public static string HasClass(By element, string className, double timeout = 0, string page = null)
        {
            AnonMethod<string> method = delegate (By e, string v, double t)
            {
                try
                {
                    var containsClass = Browser.webDriver.FindElement(e).GetAttribute("class").Contains(v);
                    if (containsClass)
                    {
                        return "";
                    }
                    else
                    {
                        return ($"The class {className} was not found in element {element} on page {page} within {timeout} seconds and there is no stack trace");
                    }
                }
                catch (Exception except)
                {
                    var trace = except.StackTrace;
                    return ($"The class {className} was not found in element {element} on page {page} within {timeout} seconds and this is the stack trace {trace}");
                }
            };

            return ExecuteAction(method, element, className, timeout, string.Format("[HasClass] | {0}", element.ToString()));
        }

        public static string Hover(By element, double timeout = 0)
        {
            AnonMethod<string> method = delegate (By e, string v, double t)
            {
                try
                {
                    Actions action = new Actions(Browser.webDriver);
                    action.MoveToElement(Browser.webDriver.FindElement(element)).Perform();

                    Thread.Sleep(500);
                    return "";
                }
                catch (Exception except)
                {
                    var trace = except.StackTrace;
                    return ($"This is the stack trace {trace}");
                }
            };

            return ExecuteAction(method, element, "", timeout, string.Format("[Hover] | {0}", element.ToString()));
        }

        public static string HoverAndSelect(By hoverElement, By clickElement, double timeout = 0, string page = null)
        {
            double seconds = 0.0;
            string actionCompleted;
            try
            {
                do
                {
                    if (seconds > 0)
                    {
                        Thread.Sleep(250);
                    }

                    Hover(hoverElement);
                    actionCompleted = Click(clickElement);

                    seconds = seconds + 0.25;
                } while (seconds < timeout && actionCompleted == "");
                return "";
            }
            catch (Exception except)
            {
                var trace = except.StackTrace;
                return ($"Unable to hover over {hoverElement} and select {clickElement} on page {page} after {timeout} seconds and this is the stack trace {trace}");

            }


        }

        public static string IsDisplayed(By element, double timeout = 0)
        {
            AnonMethod<string> method = delegate (By e, string v, double t)
            {
                try
                {
                    var displayed = Browser.webDriver.FindElement(e).Displayed;
                    if (displayed)
                    {
                        return "";
                    }
                    else
                    {
                        return ($"The {element} is not displaying and there is no stack trace");
                    }
                }
                catch (Exception except)
                {
                    var trace = except.StackTrace;
                    return ($"The {element} is not displaying and this is the stack trace {trace}");
                }
            };

            return ExecuteAction(method, element, "", timeout, string.Format("[IsDisplayed] | {0}", element.ToString()));
        }

        public static string IsEnabled(By element, double timeout = 0)
        {
            AnonMethod<string> method = delegate (By e, string v, double t)
            {
                try
                {
                    if (Browser.webDriver.FindElement(e).Enabled)
                    {
                        return "";

                    }
                    else
                    {
                        return ($"The {element} is not enabled and there is no stack trace");
                    }

                }
                catch (Exception except)
                {
                    var trace = except.StackTrace;
                    return ($"The {element} is not enabled and this is the stack trace {trace}");

                }
            };

            return ExecuteAction(method, element, "", timeout, string.Format("[IsEnabled] | {0}", element.ToString()));
        }

        public static string NotExists(By element, double timeout = 0, string page = null)
        {
            AnonMethod<string> method = delegate (By e, string v, double t)
            {
                try
                {
                    if (!Browser.webDriver.FindElement(e).Displayed)
                    {
                        return "";
                    }
                    else
                    {

                        return ($"The {element} element exists on the page {page} after {timeout} seconds and there is no stack trace");
                    }
                }
                catch (Exception except)
                {
                    var trace = except.StackTrace;
                    return ($"The {element} element exists on the page {page} after {timeout} seconds and this is the stack trace {trace}");
                }
            };

            return ExecuteAction(method, element, "", timeout, string.Format("[NotExists] | {0}", element.ToString()));
        }

        public static string Exists(By element, double timeout = 0, string page = null)
        {
            AnonMethod<string> method = delegate (By e, string v, double t)
            {
                try
                {
                    if (Browser.webDriver.FindElement(e).Displayed)
                    {
                        return "";
                    }
                    else
                    {

                        return ($"The {element} element could not be found on page {page} after {timeout} seconds and there is no stack trace");
                    }

                }
                catch (Exception except)
                {
                    var trace = except.StackTrace;
                    return ($"The {element} could not be found on page {page} after {timeout} seconds and this is the stack trace {trace}");
                }
            };

            return ExecuteAction(method, element, "", timeout, string.Format("[Exists] | {0}", element.ToString()));
        }

        public static string SetCheckbox(By element, string value, double timeout = 0, string page = null)
        {
            AnonMethod<string> method = delegate (By e, string v, double t)
            {
                bool expectedState;

                try
                {
                    if (bool.TryParse(v, out expectedState))
                    {
                        var currentState = Browser.webDriver.FindElement(e).Selected;

                        if (currentState != expectedState)
                        {
                            var id = Browser.webDriver.FindElement(e).GetAttribute("id");

                            try
                            {
                                var label = Browser.webDriver.FindElement(e).FindElement(By.XPath("..")).FindElement(By.XPath(string.Format(".//label[contains(@for, '{0}')]", id)));
                                label.Click();
                            }
                            catch (Exception)
                            {
                                Browser.webDriver.FindElement(e).Click();
                            }
                            finally
                            {
                                currentState = Browser.webDriver.FindElement(e).Selected;
                            }
                        }

                        if (currentState == expectedState)
                        {
                            return "";
                        }
                        else
                        {
                            return ($"Unable to change the value on checkbox {element} to {value} on {page} after {timeout} seconds and there is no stack trace");
                        }
                    }
                    else
                    {
                        return ($"Unable to convert the {value} to a bool on  {page} after {timeout} seconds and there is no stack trace");
                    }
                }
                catch (Exception except)
                {
                    var trace = except.StackTrace;
                    return ($"Unable to change the value on checkbox {element} to {value} on {page} after {timeout} seconds and this is the stack trace {trace}");
                }
            };

            return ExecuteAction(method, element, value, timeout, string.Format("[SetCheckbox] | {0}", element.ToString()));
        }

        public static string SelectDropdownItem(By element, string value, double timeout = 0, string page = null)
        {
            AnonMethod<string> method = delegate (By e, string v, double t)
            {
                try
                {
                    string selectedValue = null;
                    var list = new SelectElement(Browser.webDriver.FindElement(e));
                    list.SelectByText(v);
                    selectedValue = list.SelectedOption.Text;
                    if (selectedValue == null)
                    {
                        var newList = new SelectElement(Browser.webDriver.FindElement(e));
                        var item = Browser.webDriver.FindElement(e).FindElement(By.XPath(string.Format(".//option[contains(., '{0}')]", v)));
                        var index = newList.Options.IndexOf(item);
                        newList.SelectByIndex(index);
                        selectedValue = newList.SelectedOption.Text;

                    }
                    if (selectedValue == value)
                    {
                        return "";
                    }
                    else
                    {
                        if (selectedValue == null)
                        {
                            selectedValue = "";
                        }
                        return ($"Selected {selectedValue} from dropdown {element} instead of {value} on page {page} after {timeout} seconds and there is no stack trace");
                    }

                }
                catch (Exception except)
                {
                    var trace = except.StackTrace;
                    return ($"Unable to select {value} from dropdown {element} on page {page} after {timeout} seconds and this is the stack trace {trace}");
                }
            };

            return ExecuteAction(method, element, value, timeout, string.Format("[SelectDropDown] | {0}", element.ToString()));
        }

        public static string SelectMultiselectItem(By element, string value, double timeout = 0, string page = null)
        {
            AnonMethod<string> method = delegate (By e, string v, double t)
            {
                try
                {
                    IWebElement msOption;

                    var msDisplay = Browser.webDriver.FindElement(e).FindElement(By.XPath("./div[contains(@class, 'k-multiselect-wrap')]"));
                    var msSelect = Browser.webDriver.FindElement(e).FindElement(By.XPath("./select"));
                    var selectId = msSelect.GetAttribute("id");

                    msDisplay.Click();
                    Thread.Sleep(250);

                    var msOptions = Browser.webDriver.FindElements(By.XPath(string.Format(".//div[contains(@id, '{0}-list')]//li[contains(., '{1}')]", selectId, v)));
                    if (msOptions.Count == 1)
                    {
                        msOption = msOptions[0];
                    }
                    else
                    {
                        msOption = msOptions.Where(o => o.Displayed).FirstOrDefault();
                    }
                    msOption.Click();

                    if (Browser.webDriver.FindElement(e).Text.Contains(v))
                    {
                        return "";

                    }
                    else
                    {
                        return ($"The value {value} could not be selected from the multiselect list {element} on page {page} after {timeout} seconds and there is no stack trace");
                    }
                }
                catch (Exception except)
                {
                    var trace = except.StackTrace;
                    return ($"The value {value} could not be selected from the multiselect list {element} on page {page} after {timeout} seconds and this is the stack trace {trace}");
                }
            };

            return ExecuteAction(method, element, value, timeout, string.Format("[SelectMultiselectItem] | {0}", element.ToString()));
        }

        public static string DeleteMultiselectItem(By element, string value, double timeout = 0, string page = null)
        {
            AnonMethod<string> method = delegate (By e, string v, double t)
            {
                try
                {
                    var msItem = Browser.webDriver.FindElement(e).FindElement(By.XPath(string.Format("./div[contains(@class, 'k-multiselect-wrap')]//span[contains(., '{0}')]/../span[contains(@class, 'k-delete')]", value)));
                    msItem.Click();

                    if (!Browser.webDriver.FindElement(e).Text.Contains(v))
                    {
                        return "";

                    }
                    else
                    {
                        return ($"The value {value} could not be deleted from the multiselect list {element} on page {page} after {timeout} seconds and there is no stack trace");
                    }
                }
                catch (Exception except)
                {
                    var trace = except.StackTrace;
                    return ($"The value {value} could not be deleted from the multiselect list {element} on page {page} after {timeout} seconds and this is the stack trace {trace}");
                }
            };

            return ExecuteAction(method, element, value, timeout, string.Format("[DeleteMultiselectItem] | {0}", element.ToString()));
        }

        public static string SelectTableFieldElement(By tableElement, string field, double timeout = 0, string page = null)
        {
            AnonMethod<string> method = delegate (By e, string v, double t)
            {
                try
                {
                    var Table = Browser.webDriver.FindElement(tableElement);
                    IWebElement ValueRow = null;
                    IWebElement HeaderRow;

                    try
                    {
                        HeaderRow = Table.FindElement(By.XPath(".//thead//tr[1]"));
                    }
                    catch (NoSuchElementException)
                    {
                        HeaderRow = Table.FindElement(By.XPath(".//tr[1]"));
                        ValueRow = Table.FindElement(By.XPath(".//tr[2]"));
                    }

                    var HeaderField = HeaderRow.FindElement(By.XPath(string.Format(".//td[contains(., '{0}')] | .//th[contains(., '{0}')]", field)));
                    var HeaderFieldIndex = HeaderRow.FindElements(By.XPath(".//td | .//th")).IndexOf(HeaderField) + 1;

                    try
                    {
                        if (ValueRow == null)
                        {
                            ValueRow = Table.FindElement(By.XPath(".//tbody//tr[1]"));
                        }
                    }
                    catch (NoSuchElementException)
                    {
                        ValueRow = Table.FindElement(By.XPath(".//tr[2]"));
                    }

                    var ValueCell = ValueRow.FindElement(By.XPath(string.Format(".//td[{0}]", HeaderFieldIndex)));
                    var CellChildElement = ValueCell.FindElement(By.XPath(".//a[1] | .//button[1] | .//label[1]"));

                    CellChildElement.Click();

                    return "";
                }
                catch (Exception except)
                {
                    var trace = except.StackTrace;
                    return ($"Unable to select the element in field {field} on page {page} after {timeout} seconds and this is the stack trace {trace}");
                }
            };

            return ExecuteAction(method, tableElement, field, timeout, string.Format("[SelectTableFieldElement] | {0}", tableElement.ToString()));
        }

        public static string GetTableFieldValue(By tableElement, string field, double timeout = 0)
        {
            AnonMethod<string> method = delegate (By e, string v, double t)
            {
                try
                {
                    string cellValue;
                    var Table = Browser.webDriver.FindElement(tableElement);
                    IWebElement ValueRow = null;
                    IWebElement HeaderRow;

                    try
                    {
                        HeaderRow = Table.FindElement(By.XPath(".//thead//tr[1]"));
                    }
                    catch (NoSuchElementException)
                    {
                        HeaderRow = Table.FindElement(By.XPath(".//tr[1]"));
                        ValueRow = Table.FindElement(By.XPath(".//tr[2]"));
                    }

                    var HeaderField = HeaderRow.FindElement(By.XPath(string.Format(".//td[contains(., '{0}')] | .//th[contains(., '{0}')]", field)));
                    var HeaderFieldIndex = HeaderRow.FindElements(By.XPath(".//td | .//th")).IndexOf(HeaderField) + 1;

                    try
                    {
                        if (ValueRow == null)
                        {
                            ValueRow = Table.FindElement(By.XPath(".//tbody//tr[1]"));
                        }
                    }
                    catch (NoSuchElementException)
                    {
                        ValueRow = Table.FindElement(By.XPath(".//tr[2]"));
                    }

                    var ValueCell = ValueRow.FindElement(By.XPath(string.Format(".//td[{0}]", HeaderFieldIndex)));

                    cellValue = ValueCell.Text;

                    return cellValue;
                }
                catch (Exception except)
                {
                    var trace = except.StackTrace;
                    return ($"This is the stack trace {trace}");
                }
            };

            return ExecuteAction(method, tableElement, field, timeout, string.Format("[GetTableFieldValue] | {0}", tableElement.ToString()));
        }

        public static string SendKey(By element, string key, double timeout = 0)
        {
            AnonMethod<string> method = delegate (By e, string v, double t)
            {
                try
                {
                    switch (key.ToLower())
                    {
                        case "backspace":
                            Browser.webDriver.FindElement(e).SendKeys(Keys.Backspace);
                            break;
                        case "delete":
                            Browser.webDriver.FindElement(e).SendKeys(Keys.Delete);
                            break;
                        case "enter":
                            Browser.webDriver.FindElement(e).SendKeys(Keys.Enter);
                            break;
                        case "escape":
                        case "esc":
                            Browser.webDriver.FindElement(e).SendKeys(Keys.Escape);
                            break;
                        case "return":
                            Browser.webDriver.FindElement(e).SendKeys(Keys.Return);
                            break;
                        case "tab":
                            Browser.webDriver.FindElement(e).SendKeys(Keys.Tab);
                            break;
                        case "null":
                            Browser.webDriver.FindElement(e).SendKeys(Keys.Null);
                            break;
                        case "space":
                            Browser.webDriver.FindElement(e).SendKeys(Keys.Space);
                            break;
                        case "arrowleft":
                            Browser.webDriver.FindElement(e).SendKeys(Keys.ArrowLeft);
                            break;
                        case "arrowright":
                            Browser.webDriver.FindElement(e).SendKeys(Keys.ArrowRight);
                            break;
                        case "arrowdown":
                            Browser.webDriver.FindElement(e).SendKeys(Keys.ArrowDown);
                            break;
                        case "arrowup":
                            Browser.webDriver.FindElement(e).SendKeys(Keys.ArrowUp);
                            break;
                        case "ctrl arrowdown":
                            var action = new Actions(Browser.webDriver);
                            action.KeyDown(Keys.Control).SendKeys(Keys.ArrowDown).Perform();
                            break;
                        case "clear":
                            Browser.webDriver.FindElement(e).SendKeys(Keys.Clear);
                            break;
                        case "add":
                            Browser.webDriver.FindElement(e).SendKeys(Keys.Add);
                            break;
                        case "alt":
                            Browser.webDriver.FindElement(e).SendKeys(Keys.Alt);
                            break;
                        case "shift":
                            Browser.webDriver.FindElement(e).SendKeys(Keys.Shift);
                            break;
                        case "home":
                            Browser.webDriver.FindElement(e).SendKeys(Keys.Home);
                            break;
                        case "f10":
                            Browser.webDriver.FindElement(e).SendKeys(Keys.F10);
                            break;
                        case "f11":
                            Browser.webDriver.FindElement(e).SendKeys(Keys.F11);
                            break;
                        case "f12":
                            Browser.webDriver.FindElement(e).SendKeys(Keys.F12);
                            break;
                        default:
                            throw new NotImplementedException($"SendKeys has not been implemented to use key: '{key.ToLower()}'");
                    }

                    return "";
                }
                catch (Exception except)
                {
                    var trace = except.StackTrace;
                    return ($"This is the stack trace {trace}");
                }
            };

            return ExecuteAction(method, element, key, timeout, string.Format("[SendKey] | {0}", element.ToString()));
        }

        public static string SetText(By element, string value, double timeout = 0, string page = null)
        {
            AnonMethod<string> method = delegate (By e, string v, double t)
            {
                string elementValue;

                try
                {
                    Browser.webDriver.FindElement(e).Clear();
                    Browser.webDriver.FindElement(e).SendKeys(v);

                    elementValue = Browser.webDriver.FindElement(element).GetAttribute("value");

                    if (elementValue != v)
                    {
                        var id = "#" + Browser.webDriver.FindElement(e).GetAttribute("id");

                            // id not defined -> search by class
                            if (id == "#")
                        {
                            id = "." + Browser.webDriver.FindElement(e).GetAttribute("class").Replace(' ', '.');
                        }

                            // class not defined -> search by name
                            if (id == ".")
                        {
                            id = "[name='" + Browser.webDriver.FindElement(e).GetAttribute("name") + "']";
                        }

                            // name not defined -> search by xpath
                            if (id == ".")
                        {
                            id = "[xpath='" + Browser.webDriver.FindElement(e).GetAttribute("xpath") + "']";
                        }
                        IJavaScriptExecutor jse = (IJavaScriptExecutor)Browser.webDriver;
                        jse.ExecuteScript(string.Format("$('{0}').val('{1}').trigger('change').blur();", id, value));
                        elementValue = Browser.webDriver.FindElement(element).GetAttribute("value");
                    }

                    if (elementValue == v)
                    {
                        return "";
                    }
                    else
                    {
                        return ($"Unable to set text {value} to element {element} on page {page} after {timeout} seconds and there is no stack trace");
                    }
                }
                catch (Exception except)
                {
                    var trace = except.StackTrace;
                    return ($"Unable to set text {value} to element {element} on page {page} after {timeout} seconds and this is the stack trace {trace}");
                }
            };

            return ExecuteAction(method, element, value, timeout, string.Format("[SetText] | {0}", element.ToString()));
        }

        public static string GetAttribute(By element, string value, double timeout = 0)
        {
            AnonMethod<string> method = delegate (By e, string v, double t)
            {
                string attributeValue;

                try
                {
                    attributeValue = Browser.webDriver.FindElement(element).GetAttribute(value);

                    return attributeValue;
                }
                catch (Exception except)
                {
                    var trace = except.StackTrace;
                    return ($"This is the stack trace {trace}");
                }
            };

            return ExecuteAction(method, element, value, timeout, string.Format("[GetAttribute] | {0}", element.ToString()));
        }

        public static string WaitToExist(By element, double timeout)
        {
            return Exists(element, timeout, null);
        }

        public static string WaitToNotExist(By element, double timeout)
        {
            return NotExists(element, timeout, null);
        }

        public static string WaitUntilTextIsEqual(By element, string value, double timeout = 0)
        {
            AnonMethod<string> method = delegate (By e, string v, double t)
            {
                bool textMatches = false;
                double elapsedTime = 0D;
                try
                {
                    do
                    {
                        if (elapsedTime > 0)
                        {
                            Thread.Sleep(250);
                        }

                        IWebElement uiElement = Browser.webDriver.FindElement(e);
                        string text = default(string);
                        if (uiElement.TagName == "select")
                        {
                            var selectedVal = uiElement.GetAttribute("value");
                            SelectElement selectEle = new SelectElement(uiElement);

                            text = selectEle.SelectedOption.Text;
                        }
                        else
                        {
                            text = uiElement.Text;
                        }
                        textMatches = value.Equals(text, StringComparison.OrdinalIgnoreCase);
                        elapsedTime += 0.25D;
                    } while (!textMatches && elapsedTime < timeout);
                    if (textMatches)
                    {
                        return "";
                    }
                    else
                    {
                        return ($"The text {value} cannot not be found  after {timeout} seconds and there is no stack trace");

                    }

                }
                catch (Exception except)
                {
                    var trace = except.StackTrace;
                    throw new Exception($"The text {value} cannot not be found  after {timeout} seconds and this is the stack trace {trace}");
                }

            };

            return ExecuteAction(method, element, "", timeout, string.Format("[WaitUntilTextIsEqual] | {0}", element.ToString()));
        }
    }
}

