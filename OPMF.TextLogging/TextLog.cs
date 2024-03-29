﻿using System;
using System.IO;

namespace OPMF.TextLogging
{
    public class TextLog
    {
        private readonly StreamWriter fileStream;

        private TextLog()
        {
            string textFileLogPath = Settings.ConfigHelper.ReadonlySettings.GetTextLogFile();
            fileStream = File.Exists(textFileLogPath) ? File.AppendText(textFileLogPath) : File.CreateText(textFileLogPath);
        }

        /// <summary>
        /// Fallback ...
        /// </summary>
        /// <param name="error"></param>
        public void LogEntry(string error)
        {
            fileStream.WriteLine($"\r\nDATETIME: {DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
            fileStream.WriteLine("ERROR:");
            fileStream.WriteLine($"{error}");
            fileStream.WriteLine("-------------------------------");
            fileStream.Flush();
        }

        private static TextLog __currentInstance;

        public static TextLog GetCurrent()
        {
            if (__currentInstance == null)
            {
                __currentInstance = new TextLog();
            }
            return __currentInstance;
        }
    }
}
