#nullable disable
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace JsonPath
{
    public class Log
    {
        public enum Level
        {
            Silent,
            Error,
            Warning,
            Debug,
            User,
            Info,
            Verbose,
            Flooding,
        }

        public delegate void CallbackToApplication(Level level, string context, string message);

        internal void Error(Exception ex, [CallerMemberName] string context = null, [CallerFilePath] string callerFilePath = null) { try { LogX(Level.Error, GetContext(context, callerFilePath), "Exception: " + ExceptionDetail(ex)); } catch (Exception) { } }

        internal void Warning(Exception ex, [CallerMemberName] string context = null, [CallerFilePath] string callerFilePath = null) { try { LogX(Level.Warning, GetContext(context, callerFilePath), "Exception: " + ExceptionDetail(ex)); } catch (Exception) { } }
        internal void Error(string message, [CallerMemberName] string context = null, [CallerFilePath] string callerFilePath = null) { try { LogX(Level.Error, GetContext(context, callerFilePath), message); } catch (Exception) { } }
        internal void Warning(string message, [CallerMemberName] string context = null, [CallerFilePath] string callerFilePath = null) { try { LogX(Level.Warning, GetContext(context, callerFilePath), message); } catch (Exception) { } }
        internal void Debug(string message, [CallerMemberName] string context = null, [CallerFilePath] string callerFilePath = null) { try { LogX(Level.Debug, GetContext(context, callerFilePath), message); } catch (Exception) { } }
        internal void User(string message, [CallerMemberName] string context = null, [CallerFilePath] string callerFilePath = null) { try { LogX(Level.User, GetContext(context, callerFilePath), message); } catch (Exception) { } }
        internal void Info(string message, [CallerMemberName] string context = null, [CallerFilePath] string callerFilePath = null) { try { LogX(Level.Info, GetContext(context, callerFilePath), message); } catch (Exception) { } }
        internal void Verbose(string message, [CallerMemberName] string context = null, [CallerFilePath] string callerFilePath = null) { try { LogX(Level.Verbose, GetContext(context, callerFilePath), message); } catch (Exception) { } }
        internal void Flooding(string message, [CallerMemberName] string context = null, [CallerFilePath] string callerFilePath = null) { try { LogX(Level.Flooding, GetContext(context, callerFilePath), message); } catch (Exception) { } }

        internal bool IsVerbose => LogLevel >= Level.Verbose;
        internal bool IsFlooding => LogLevel >= Level.Flooding;

        private struct LogLine
        {
            public readonly Level Level;
            public readonly string Context;
            public readonly string Message;

            public LogLine(Level level, string context, string message)
            {
                Level = level;
                Context = context;
                Message = message;
            }
        }
        private readonly List<LogLine> Backlog = new List<LogLine>();

        internal string LogFile = "";
        const string LogFileTempFolder = @"%TEMP%";
        internal long LogFileLimit = 10 * 1024 * 1024;

        static CallbackToApplication _callback;
        public CallbackToApplication LogHandler
        {
            get { return _callback; }
            set {
                _callback = value;

                ReplayBacklog();
            }
        }

        public Level LogLevel = Level.Info;

        public Level LevelFromString(string levels)
        {
            var lowerLevels = levels.ToLower();
            var maxLevel = Level.Silent;
            foreach (var level in typeof(Level).GetEnumValues()) {
                if (lowerLevels.Contains(level.ToString().ToLower())) {
                    if (maxLevel < (Level)level) {
                        maxLevel = (Level)level;
                    }
                }
            }
            return maxLevel;
        }

        private readonly object Mutex = new object();

        private int _pid = -1;
        private int Pid { get { if (_pid == -1) { try { _pid = Process.GetCurrentProcess().Id; } catch (Exception) { } } return _pid; } set { _pid = value; } }

        internal void LogX(Level level, string context, string message)
        {
            if (context == null) { context = ""; }
            if (message == null) { message = ""; }
            message = message.Replace("\r", "\\r").Replace("\n", "\\n");

            if (level <= LogLevel) {
                if (!string.IsNullOrEmpty(LogFile)) {
                    LogToFile(level, context, message);
                }

                if (_callback == null) {
                    AddToBacklog(level, context, message);
                }

                _callback?.Invoke(level, context, message);
            }
        }

        private void AddToBacklog(Level level, string context, string message)
        {
            lock (Mutex) {
                Backlog?.Add(new LogLine(level, context, message));
            }
        }

        private void ReplayBacklog()
        {
            lock (Mutex) {
                try {
                    if (Backlog != null) {
                        foreach (var line in Backlog.ToArray()) {
                            _callback?.Invoke(line.Level, line.Context, line.Message);
                        }
                    }
                } catch (Exception) {
                    // Dont let the logger crash the app
                }
            }
        }

        private void LogToFile(Level level, string context, string message)
        {
            lock (Mutex) {
                try {
                    var logFile = LogFile;
                    logFile = logFile.Replace(LogFileTempFolder, Path.GetTempPath());

                    if (File.Exists(logFile) && LogFileLimit > 0 && new FileInfo(logFile).Length > LogFileLimit) {
                        RotateLogFile(logFile);
                    }

                    var now = DateTime.Now;
                    // ReSharper disable once LocalizableElement
                    File.AppendAllText(logFile, $"{now.ToString(CultureInfo.InvariantCulture)}.{now.Millisecond:D3} {Pid} " + level.ToString().Replace($"{nameof(Level.Debug)}", "#########") + $" {context} {message}" + Environment.NewLine);
                } catch (Exception) {
                    // file errors dont prevent other logging
                }
            } // lock
        }

        private void RotateLogFile(string logFile)
        {
            var path = Path.GetDirectoryName(logFile);
            if (path != null) {
                var name = Path.GetFileName(logFile);
                var now = DateTime.Now;
                var newName = now.ToString("yyMMdd-HHmmss-") + now.Millisecond + "-" + name;
                var newPathName = Path.Combine(path, newName);
                File.Move(logFile, newPathName);
            }
        }

        private string ExceptionDetail(Exception ex)
        {
            return string.Join(" | ", AllExceptionMessages(ex).ToArray()) + " | " + string.Join(" | ", InnerExceptionDetail(ex).ToArray());
        }

        private string MethodName(int skip = 0)
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1 + skip);
            if (sf != null) {
                var mb = sf.GetMethod();
                if (mb != null) {
                    return mb.Name;
                }
            }
            return "<unknown>";
        }

        private string GetContext(string context, string callerFilePath)
        {
            var result = string.IsNullOrEmpty(callerFilePath) ? "" : Path.GetFileNameWithoutExtension(callerFilePath);
            result += (string.IsNullOrEmpty(result) ? "" : ".") + (context ?? MethodName(3));
            return result;
        }

        private List<string> AllExceptionMessages(Exception self)
        {
            var result = new List<string>();

            var ex = self;
            var previousMessage = "";
            while (ex != null) {
                if (ex.Message != previousMessage) {
                    previousMessage = ex.Message;
                    result.Add(ex.Message);
                }
                ex = ex.InnerException;
            }

            return result;
        }

        private List<string> InnerExceptionDetail(Exception self)
        {
            var result = new List<string>();

            var ex = self;
            if (self.InnerException != null) {
                ex = self.InnerException;
            }

            if (ex.Source != null) { result.Add("Source: " + ex.Source); }
            if (ex.StackTrace != null) { result.Add("Stack Trace: " + ex.StackTrace.Replace(Environment.NewLine, "\\n")); }
            if (ex.TargetSite != null) { result.Add("TargetSite: " + ex.TargetSite); }

            return result;
        }
    }
}
