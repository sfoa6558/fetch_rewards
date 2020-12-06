using NUnit.Framework;

namespace AutomationFramework
{
    

    namespace Test
    {
        [TestFixture]
        public class AutomationCore
        {
            
            
            // Our Core Test Automation class
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
}
