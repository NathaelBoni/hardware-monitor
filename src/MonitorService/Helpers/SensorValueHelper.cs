using LibreHardwareMonitor.Hardware;
using System;

namespace MonitorService.Helpers
{
    public static class SensorValueHelper
    {
        public static int GetRoundSensorValue(ISensor sensor)
        {
            if (!sensor.Value.HasValue) return 0;

            var sensorValue = Math.Floor(sensor.Value.Value);
            return (int)sensorValue;
        }

        public static float GetSensorValue(ISensor sensor)
        {
            if (!sensor.Value.HasValue) return 0;

            var sensorValue = Math.Round(sensor.Value.Value, 1);
            return (float)sensorValue;
        }
    }
}
