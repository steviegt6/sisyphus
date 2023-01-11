using System;
using System.Collections;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Sisyphus.Build.Tests;

internal static class Logger {
    private sealed class LoggerBuildEngine : IBuildEngine {
        private string Name { get; }

        public LoggerBuildEngine(string name) {
            Name = name;
        }

        // TODO: Better formatting.
        private string DirtyFormat(
            DateTime time,
            string cat,
            string? msg,
            string level
        ) {
            return $"[{time}] [{level}] {Name} ({cat}): {msg}";
        }

        void IBuildEngine.LogErrorEvent(BuildErrorEventArgs e) {
            Console.WriteLine(
                DirtyFormat(
                    e.Timestamp,
                    e.Subcategory,
                    e.Message,
                    "Error"
                )
            );
        }

        void IBuildEngine.LogWarningEvent(BuildWarningEventArgs e) {
            Console.WriteLine(
                DirtyFormat(
                    e.Timestamp,
                    e.Subcategory,
                    e.Message,
                    "Warning"
                )
            );
        }

        void IBuildEngine.LogMessageEvent(BuildMessageEventArgs e) {
            Console.WriteLine(
                DirtyFormat(
                    e.Timestamp,
                    e.Subcategory,
                    e.Message,
                    "Message"
                )
            );
        }

        void IBuildEngine.LogCustomEvent(CustomBuildEventArgs e) {
            Console.WriteLine(
                DirtyFormat(
                    e.Timestamp,
                    "n/a",
                    e.Message,
                    "Custom"
                )
            );
        }

        bool IBuildEngine.BuildProjectFile(
            string projectFileName,
            string[] targetNames,
            IDictionary globalProperties,
            IDictionary targetOutputs
        ) {
            return false;
        }

        bool IBuildEngine.ContinueOnError => true;

        int IBuildEngine.LineNumberOfTaskNode => 0;

        int IBuildEngine.ColumnNumberOfTaskNode => 0;

        string IBuildEngine.ProjectFileOfTaskNode => "<unknown>";
    }

    private sealed class LoggerTaskHost : ITaskHost { }

    private sealed class LoggerTask : ITask {
        public LoggerTask(string name) {
            BuildEngine = new LoggerBuildEngine(name);
        }

        bool ITask.Execute() {
            return true;
        }

        public IBuildEngine BuildEngine { get; set; }

        ITaskHost ITask.HostObject { get; set; } = new LoggerTaskHost();
    }

    public static TaskLoggingHelper Create(string name) {
        return new TaskLoggingHelper(new LoggerTask(name));
    }
}
