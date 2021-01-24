using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Fury.Events;

namespace Fury.Utils
{
    public class Logger
    {
        public static List<Log> Logs = new List<Log>();

        public const string format = "HH:mm:ss";

        public static void Info(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(DateTime.Now.ToString(format));
            Console.Write(" [INFO] ");
            Console.Write(msg + "\n");
            Console.ForegroundColor = ConsoleColor.White;

            Logs.Add(new Log(msg, Severity.Info));
        }

        public static void Warn(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(DateTime.Now.ToString(format));
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(" [WARN] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(msg + "\n");
            Console.ForegroundColor = ConsoleColor.White;

            Logs.Add(new Log(msg, Severity.Warn));
        }

        public static void Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(DateTime.Now.ToString(format));
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(" [ERROR] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(msg + "\n");
            Console.ForegroundColor = ConsoleColor.White;

            Logs.Add(new Log(msg, Severity.Error));
        }
    }

    public struct Log
    {
        private Severity severity;
        private string message;
        private DateTime time;

        public Log(string message, Severity severity)
        {
            this.severity = severity;
            this.message = message;
            this.time = DateTime.Now;
        }

        public Severity Severity => severity;
        public string Message => message;
        public DateTime Time => time;
    }

    public enum Severity
    {
        Info,
        Warn,
        Error
    }
}
