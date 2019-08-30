using System;
using System.Diagnostics;

namespace PerformanceCheck
{
    public static class OperationsTimer
    {
        private static DateTime StartTime, StopTime;
        private static readonly Stopwatch TotalTime = new Stopwatch();

        private static long frequency;
        private static long nanosecPerTick;

        private static long numTicks = 0;

        public static void StartMeasurement()
        {
            frequency = Stopwatch.Frequency;
            nanosecPerTick = (1000L * 1000L * 1000L) / frequency;

            StartTime = DateTime.Now;
            TotalTime.Start();
        }

        public static (string nanosecTotal, string milisecTotal, bool isHihgPrecisionEnabled) StopMeasurement(int numIterations = 1)
        {
            StopTime = DateTime.Now;
            TotalTime.Stop();

            TimeSpan elapsed = StopTime.Subtract(StartTime);

            numTicks = TotalTime.ElapsedTicks;
            TotalTime.Reset();

            return (((numTicks * nanosecPerTick) / numIterations).ToString(), elapsed.TotalSeconds.ToString("0.000000"), Stopwatch.IsHighResolution);
        }
    }
}