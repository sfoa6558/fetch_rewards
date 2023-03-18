using NUnit.Framework;

namespace AutomationFramework
{


    [TestFixture]
    public class AutomationCore
    {

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
