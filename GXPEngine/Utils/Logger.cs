using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace GXPEngine {
    public class Logger {
        private const string LogFormat = " at {0}.{1}:{2} ({3}:line {2})";

        private static string GetString(object message) {
            if (message == null) {
                return "Null";
            }

            var formattable = message as IFormattable;
            return formattable != null ? formattable.ToString(null, CultureInfo.InvariantCulture) : message.ToString();
        }

        public static void Log(object message, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "") {
            var fileName = filePath.Substring(filePath.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            var mth = new StackTrace().GetFrame(1).GetMethod();
            var className = mth.ReflectedType?.Name;
            var method = new StringBuilder();
            method.Append(mth.Name);
            method.Append("(");
            var methodParameters = mth.GetParameters();
            for (int i = 0; i < methodParameters.Length; i++) {
                method.Append(methodParameters[i].ParameterType);
                if (i != methodParameters.Length - 1) method.Append(", ");
            }

            method.Append(")");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.Write("[LOG]");
            Console.ResetColor();
            Console.Write(" " + GetString(message));
            Console.ResetColor();
            Console.WriteLine(LogFormat.format(className, method, lineNumber, fileName));
        }

        public static void LogWarn(object message, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "") {
            var fileName = filePath.Substring(filePath.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            var mth = new StackTrace().GetFrame(1).GetMethod();
            var className = mth.ReflectedType?.Name;
            var method = new StringBuilder();
            method.Append(mth.Name);
            method.Append("(");
            var methodParameters = mth.GetParameters();
            for (int i = 0; i < methodParameters.Length; i++) {
                method.Append(methodParameters[i].ParameterType);
                if (i != methodParameters.Length - 1) method.Append(", ");
            }
            method.Append(")");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.Write("[WARN]");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(" " + GetString(message));
            Console.ResetColor();
            Console.WriteLine(LogFormat.format(className, method, lineNumber, fileName));
            Console.ResetColor();
        }
        public static void LogError(object message, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "") {
            var fileName = filePath.Substring(filePath.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            var mth = new StackTrace().GetFrame(1).GetMethod();
            var className = mth.ReflectedType?.Name;
            var method = new StringBuilder();
            method.Append(mth.Name);
            method.Append("(");
            var methodParameters = mth.GetParameters();
            for (int i = 0; i < methodParameters.Length; i++) {
                method.Append(methodParameters[i].ParameterType);
                if (i != methodParameters.Length - 1) method.Append(", ");
            }
            method.Append(")");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Write("[ERROR]");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(" " + GetString(message));
            Console.ResetColor();
            Console.WriteLine(LogFormat.format(className, method, lineNumber, fileName));
            Console.ResetColor();
        }
    }
}