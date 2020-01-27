using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BeaverSoft.Texo.Core.Path;
using StrongBeaver.Core.Constants;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Core.Logging
{
    public class UserAppDataLogService : ILogService, IDisposable
    {
        private const string LOG_DIRECTORY = "logs";
        private const string LOG_FILE_EXTENSION = ".log";

        private static readonly object locker = new object();
        private readonly LogMessageLevelEnum minimumLevel;
        private string logFilePrefix;
        private TextWriter currentWriter;

        public UserAppDataLogService(LogMessageLevelEnum minimumLevel, string logFilePrefix = null)
        {
            this.logFilePrefix = string.IsNullOrWhiteSpace(logFilePrefix) ? string.Empty : logFilePrefix;
            this.minimumLevel = minimumLevel;
        }

        private TextWriter GetLogWriter()
        {
            if (currentWriter == null)
            {
                lock (locker)
                {
                    OpenLogWriter();
                }
            }

            return currentWriter;
        }

        public void ProcessMessage(ILogMessage message)
        {
            message.PerformMessage(this);
        }

        public void Log(LogMessageLevelEnum level, string message, params object[] args)
        {
            if (level < minimumLevel)
            {
                return;
            }

            TextWriter writer = GetLogWriter();
            writer.WriteLine($"[{level} {DateTime.Now:hh-mm-ss}]: {message ?? "NULL"}");
            WriteObjects(args, writer);
        }

        public void Dispose()
        {
            currentWriter?.Dispose();
        }

        private void OpenLogWriter()
        {
            if (currentWriter != null)
            {
                return;
            }

            string logDirectory = PathExtensions.GetAndCreateDataDirectoryPath(LOG_DIRECTORY);
            string fileName = $"{logFilePrefix}{DateTime.Now:yy-MM-dd-hh-mm-ss}-{Guid.NewGuid():D}{LOG_FILE_EXTENSION}";

            FileStream logFile = new FileStream(logDirectory.CombinePathWith(fileName),
                FileMode.OpenOrCreate, FileAccess.Write);

            currentWriter = new StreamWriter(logFile, Encoding.UTF8);
        }

        private static void WriteObjects(IEnumerable<object> args, TextWriter writer)
        {
            if (args == null)
            {
                return;
            }

            foreach (object objectToWrite in args)
            {
                WriteObject(objectToWrite, writer);
            }
        }

        private static void WriteObject(object objectToWrite, TextWriter writer)
        {
            if (objectToWrite == null)
            {
                return;
            }

            writer.WriteLine(objectToWrite);
            Exception exception = objectToWrite as Exception;

            if (exception == null)
            {
                return;
            }

            writer.WriteLine("EXCEPTION DETAILS");
            WriteException(exception, 0, writer);
        }

        private static void WriteException(Exception exception, int nestingLevel, TextWriter writer)
        {
            if (exception == null
                || nestingLevel > GlobalConstatns.EXCEPTION_MAX_NESTING_LEVEL)
            {
                return;
            }

            writer.WriteLine($"MESSAGE: {exception.Message ?? string.Empty}");
            writer.WriteLine($"SOURCE: {exception.Source ?? string.Empty}");
            writer.WriteLine(exception.StackTrace ?? string.Empty);
            WriteExceptionData(exception, writer);

            if (exception.InnerException == null)
            {
                return;
            }

            int nextNestingLevel = nestingLevel + 1;
            writer.WriteLine($"INNER EXCEPTION LEVEL {nextNestingLevel}");
            WriteException(exception.InnerException, nextNestingLevel, writer);
        }

        private static void WriteExceptionData(Exception exception, TextWriter writer)
        {
            if (exception?.Data == null || exception.Data.Count < 1)
            {
                return;
            }

            StringBuilder stb = new StringBuilder();

            foreach (DictionaryEntry entry in exception.Data)
            {
                stb.Append(entry.Key ?? "[Unknown]");
                stb.Append(" = ");
                stb.Append(entry.Value ?? string.Empty);
                stb.Append("; ");
            }

            if (stb.Length < 2)
            {
                return;
            }

            stb.Remove(stb.Length - 2, 2);
            writer.WriteLine("EXCEPTION DATA:");
            writer.WriteLine(stb.ToString());
        }
    }
}