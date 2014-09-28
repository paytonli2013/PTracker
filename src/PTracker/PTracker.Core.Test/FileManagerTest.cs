using PTracker.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace PTracker.Core.Test
{
    
    
    /// <summary>
    ///This is a test class for FileManagerTest and is intended
    ///to contain all FileManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FileManagerTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for FileManager Constructor
        ///</summary>
        [TestMethod()]
        public void FileManagerConstructorTest()
        {
            var fileInfo = new FileInfo(Assembly.GetAssembly(typeof(SyncTaskScheduler)).Location);
            string path = Path.Combine(fileInfo.Directory.FullName, "TestFile\\Test01.txt"); // TODO: Initialize to an appropriate value
            bool isTracking = false; // TODO: Initialize to an appropriate value
            Encoding encoding = null; // TODO: Initialize to an appropriate value
            TaskScheduler tScheduler = new SyncTaskScheduler(); // TODO: Initialize to an appropriate value
            ILogger logger = null; // TODO: Initialize to an appropriate value
            FileManager target = new FileManager(path, isTracking, encoding, tScheduler, logger);

            Assert.AreEqual(path, target.Path);
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PTracker.Core.dll")]
        public void LoadTest()
        {
            var fileInfo = new FileInfo(Assembly.GetAssembly(typeof(SyncTaskScheduler)).Location);
            string path = Path.Combine(fileInfo.Directory.FullName, "TestFile\\Test01.txt"); // TODO: Initialize to an appropriate value
            bool isTracking = false; // TODO: Initialize to an appropriate value
            Encoding encoding = null; // TODO: Initialize to an appropriate value
            TaskScheduler tScheduler = new SyncTaskScheduler(); // TODO: Initialize to an appropriate value
            ILogger logger = null; // TODO: Initialize to an appropriate value
            FileManager fm = new FileManager(path, isTracking, encoding, tScheduler, logger);

            PrivateObject param0 = new PrivateObject(fm); // TODO: Initialize to an appropriate value
            FileManager_Accessor target = new FileManager_Accessor(param0); // TODO: Initialize to an appropriate value
            target.Load();

            Assert.AreEqual(path, target.Path);
        }

        /// <summary>
        ///A test for Subscribe
        ///</summary>
        [TestMethod()]
        public void SubscribeTest()
        {
            var fileInfo = new FileInfo(Assembly.GetAssembly(typeof(SyncTaskScheduler)).Location);
            string path = Path.Combine(fileInfo.Directory.FullName, "TestFile\\Test01.txt"); // TODO: Initialize to an appropriate value
            bool isTracking = false; // TODO: Initialize to an appropriate value
            Encoding encoding = null; // TODO: Initialize to an appropriate value
            TaskScheduler tScheduler = new SyncTaskScheduler(); // TODO: Initialize to an appropriate value
            ILogger logger = null; // TODO: Initialize to an appropriate value


            FileManager target = new FileManager(path, isTracking, encoding, tScheduler, logger); // TODO: Initialize to an appropriate value
            IObserver<DocumentChangeSet> observer = null; // TODO: Initialize to an appropriate value
            IDisposable expected = null; // TODO: Initialize to an appropriate value
            IDisposable actual;
            actual = target.Subscribe(observer);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
