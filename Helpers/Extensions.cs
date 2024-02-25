using Avalonia;
using System;
using System.Diagnostics;
using System.IO;
using Avalonia.Controls;
using Avalonia.Reactive;
using Avalonia.VisualTree;

namespace DAGlynEditor
{
    public static class Extension
    {
        #region Static Methods

        // Subscribe 를 static 에서 사용하기 위해서
        public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> action)
        {
            return observable.Subscribe(new AnonymousObserver<T>(action));
        }
        public static T? GetParentControlOfType<T>(this Control child) where T : Control
        {
            var current = child;

            while (current != null)
            {
                if (current is T target)
                {
                    return target;
                }
                current = current.GetVisualParent() as Control;
            }

            return default;
        }

        public static T? GetParentVisualOfType<T>(this Visual child) where T : Visual
        {
            var current = child;

            while (current != null)
            {
                if (current is T target)
                {
                    return target;
                }
                current = current.GetVisualParent();
            }

            return default;
        }

        public static T? GetParentVisualByName<T>(this Visual child, string name) where T : Visual
        {
            var current = child;

            while (current != null)
            {
                if (current is T target && target.Name == name)
                {
                    return target;
                }
                current = current.GetVisualParent();
            }

            return default;
        }

        public static T? GetElementUnderMouse<T>(this Visual container, Point pointerPosition) where T : Visual
        {
            foreach (var visual in container.GetVisualDescendants())
            {
                if (visual.Bounds.Contains(pointerPosition) && visual is T foundElement)
                {
                    return foundElement;
                }
            }

            return null;
        }

        #endregion

        #region 개발중 간단한 테스트 

        // 사용하지 않을 듯 하지만 일단 남겨 놓는다.
        // 로그 파일을 저장할 폴더와 파일 이름 정의
        private static readonly string logDirectory = Path.Combine(Environment.CurrentDirectory, "Logs");
        private static readonly string logFilePath = Path.Combine(logDirectory, "PerformanceLog.txt");

        [Conditional("DEBUG")]
        public static void Log(bool condition, string format, params object[] args)
        {
            if (condition)
            {
                string output = DateTime.Now.ToString("hh:mm:ss") + ": " + string.Format(format, args); //+ Environment.NewLine + Environment.StackTrace;
                                                                                                        //Console.WriteLine(output);
                Debug.WriteLine(output);
            }
        }
        public static void LogWriteToFile(bool condition, string format, params object[] args)
        {
            if (condition)
            {
                string output = DateTime.Now.ToString("hh:mm:ss") + ": " + string.Format(format, args);
                WriteToFile(output);
            }
        }

        public static void LogPerformance(string message)
        {
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            Process currentProcess = Process.GetCurrentProcess();
            long memoryUsage = currentProcess.WorkingSet64; // 메모리 사용량
            TimeSpan cpuTime = currentProcess.TotalProcessorTime; // CPU 사용 시간


            // 성능 정보 로깅
            LogWriteToFile(true, "{0} - Memory Usage: {1} bytes, CPU Time: {2} ms", message, memoryUsage, cpuTime.TotalMilliseconds);
        }

        private static void WriteToFile(string message)
        {
            try
            {
                // TODO 간단한 방식으로 바꾸자. 일단 테스트
                using (var writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Log file write error: " + ex.Message);
            }
        }

        #endregion
    }
}

