using System;
using System.Configuration;
using System.IO;
using DotNetMigrations.Core;

namespace DotNetMigrations.Contrib.Logs
{
    public class TextLog : LoggerBase
    {
        private StreamWriter _stream;
        private const string _logName = "TextLog";
        private string _padding = string.Empty.PadLeft(4);

        /// <summary>
        /// The name of the Log
        /// </summary>
        public override string LogName { get; set;}

        /// <summary>
        /// Instantiates a new instance of the TextLog class
        /// </summary>
        public TextLog()
        {
            LogName = _logName;
            Initialize();
        }
        
        /// <summary>
        /// Writes an error message to the textual log file.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        public override void WriteError(string message)
        {
            _stream.WriteLine(DateTime.Now.ToString());
            _stream.WriteLine(_padding + "__ERROR__");
            _stream.WriteLine(_padding + message);
            _stream.WriteLine(string.Empty);
        }

        /// <summary>
        /// Write a message to the textual log file.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public override void WriteLine(string message)
        {
            _stream.WriteLine(DateTime.Now.ToString());
            _stream.WriteLine(_padding + message);
            _stream.WriteLine(string.Empty);
        }

        /// <summary>
        /// Writes a warning message to the textual log file.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        public override void WriteWarning(string message)
        {
            _stream.WriteLine(DateTime.Now.ToString());
            _stream.WriteLine(_padding + "__WARNING__");
            _stream.WriteLine(_padding + message);
            _stream.WriteLine(string.Empty);
        }

        /// <summary>
        /// Disposes of resources that were holding memory and other resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (_stream != null)
            {
                _stream.Dispose();
            }
        }

        /// <summary>
        /// Initializes the logfile to write to.
        /// </summary>
        private void Initialize()
        {
            if (_stream == null)
            {
                string filePath = GetLogFilePath();
                _stream = File.CreateText(filePath);
            }
        }

        /// <summary>
        /// Obtains the Log File path from the configuration file's AppSettings section
        /// </summary>
        /// <returns>The file path from the Configuration File's AppSettings 'logFilePath' entry.</returns>
        private string GetLogFilePath()
        {
            return ConfigurationManager.AppSettings["logFilePath"];
        }
    }
}
