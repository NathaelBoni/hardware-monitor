using System.Text.Json;

namespace MonitorService.Models
{
    public class SensorValuesDto
    {
        public int CPULoad { get; set; }
        public int CPUTemp { get; set; }
        public int CPUFanSpeed { get; set; }
        public int GPULoad { get; set; }
        public int GPUTemp { get; set; }
        public int GPUHotSpotTemp { get; set; }
        public int GPUFanSpeed { get; set; }
        public float MemoryUsage { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
