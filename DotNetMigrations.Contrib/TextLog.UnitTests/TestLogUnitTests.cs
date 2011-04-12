using System.Configuration;
using System.IO;
using NUnit.Framework;

namespace DotNetMigrations.Contrib.UnitTests_TextLog
{
    [TestFixture]
    public class TestLogUnitTests
    {
        private string _logPath;

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            _logPath = ConfigurationManager.AppSettings["logFilePath"];
        }

        [SetUp]
        public void Setup()
        {
            if (File.Exists(_logPath))
            {
                File.Delete(_logPath);
            }
        }

        [TestFixtureTearDown]
        public void TearDownFixture()
        {
            if (File.Exists(_logPath))
            {
                File.Delete(_logPath);
            }
        }

        [Test]
        public void Should_Create_The_Log_File_Upon_Instantiation()
        {
            using (var log = new TextLog.TextLog())
            {
                // do nothing.
            }

            Assert.IsTrue(File.Exists(_logPath));
        }

        [Test]
        public void Should_Log_A_Message()
        {
            string message = "This is a test.";
            
            using (var log = new TextLog.TextLog())
            {
                log.WriteLine(message);
            }

            Assert.IsTrue(ReadFile().Contains(message));
        }

        [Test]
        public void Should_Log_A_Warning_Message()
        {
            string message = "This is a warning.";

            using (var log = new TextLog.TextLog())
            {
                log.WriteWarning(message);
            }

            string actual = ReadFile();
            Assert.IsTrue(actual.Contains(message));
            Assert.IsTrue(actual.Contains("__WARNING__"));
        }

        [Test]
        public void Should_Log_An_Error_Message()
        {
            string message = "This is a error.";

            using (var log = new TextLog.TextLog())
            {
                log.WriteError(message);
            }

            string actual = ReadFile();
            Assert.IsTrue(actual.Contains(message));
            Assert.IsTrue(actual.Contains("__ERROR__"));
        }

        private string ReadFile()
        {
            string contents = null;

            if (File.Exists(_logPath))
            {
                contents = File.ReadAllText(_logPath);
            }
            
            return contents;
        }
    }
}
