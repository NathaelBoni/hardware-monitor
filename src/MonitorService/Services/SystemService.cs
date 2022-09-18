using LibreHardwareMonitor.Hardware;
using MonitorService.Helpers;
using MonitorService.Models;
using System;
using System.Linq;

namespace MonitorService.Services
{
    public class SystemService
    {
        private readonly Computer system;
        private readonly HardwareType gpuType;

        private readonly IHardware cpu;
        private readonly IHardware gpu;
        private readonly IHardware memory;
        private readonly IHardware motherboard;

        private readonly ISensor cpuLoad;
        private readonly ISensor cpuCoreTemp;
        private readonly ISensor cpuFanSpeed;
        private readonly ISensor gpuLoad;
        private readonly ISensor gpuCoreTemp;
        private readonly ISensor gpuHotSpotTemp;
        private readonly ISensor gpuFanSpeed;
        private readonly ISensor memoryUsage;
        private readonly ISensor fps;

        public SystemService(string gpuManufacturer, string motherBoardCPUFanConfigName)
        {
            if (gpuManufacturer.Equals("AMD"))
                gpuType = HardwareType.GpuAmd;
            else if (gpuManufacturer.Equals("NVIDIA"))
                gpuType = HardwareType.GpuNvidia;
            else if (gpuManufacturer.Equals("INTEL"))
                gpuType = HardwareType.GpuIntel;

            system = new Computer()
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true
            };

            Open();
            system.Accept(new UpdateVisitor());

            cpu = GetCpu();
            gpu = GetGpu();
            memory = GetMemory();
            motherboard = GetMotherboard();

            if (cpu == null || gpu == null || memory == null)
                throw new ArgumentNullException("Could not instantiate hardware components");

            cpuLoad = GetCPUSensor(SensorStrings.CPU_LOAD, SensorType.Load);
            cpuCoreTemp = GetCPUSensor(SensorStrings.CPU_CORE_TEMP, SensorType.Temperature);
            cpuFanSpeed = GetMotherboardSensor(motherBoardCPUFanConfigName, SensorType.Fan);
            gpuLoad = GetGPUSensor(SensorStrings.GPU_LOAD, SensorType.Load);
            gpuCoreTemp = GetGPUSensor(SensorStrings.GPU_CORE_TEMP, SensorType.Temperature);
            gpuHotSpotTemp = GetGPUSensor(SensorStrings.GPU_HOTSPOT_TEMP, SensorType.Temperature);
            gpuFanSpeed = GetGPUSensor(SensorStrings.GPU_FAN_SPEED, SensorType.Fan);
            memoryUsage = GetMemorySensor(SensorStrings.MEMORY_USAGE, SensorType.Data);
            fps = GetGPUSensor(SensorStrings.FPS_COUNTER, SensorType.Factor);
        }

        public SensorValuesDto GetSensorValues()
        {
            RetryPolicyHelper.RetryAction(1, 1, () => UpdateSensors());

            return new SensorValuesDto
            {
                CPULoad = SensorValueHelper.GetRoundSensorValue(cpuLoad),
                CPUTemp = SensorValueHelper.GetRoundSensorValue(cpuCoreTemp),
                CPUFanSpeed = SensorValueHelper.GetRoundSensorValue(cpuFanSpeed),
                GPULoad = SensorValueHelper.GetRoundSensorValue(gpuLoad),
                GPUTemp = SensorValueHelper.GetRoundSensorValue(gpuCoreTemp),
                GPUHotSpotTemp = SensorValueHelper.GetRoundSensorValue(gpuHotSpotTemp),
                GPUFanSpeed = SensorValueHelper.GetRoundSensorValue(gpuFanSpeed),
                MemoryUsage = SensorValueHelper.GetSensorValue(memoryUsage),
                FPS = SensorValueHelper.GetRoundSensorValue(fps)
            };
        }

        public void Close()
            => system.Close();

        private void UpdateSensors()
        {
            cpu.Update();
            gpu.Update();
            memory.Update();
            motherboard.Update();
        }

        private void Open()
            => RetryPolicyHelper.RetryAction(3, 3, () => system.Open());

        private IHardware GetCpu()
            => GetHardware(HardwareType.Cpu);

        private IHardware GetGpu()
            => GetHardware(gpuType);

        private IHardware GetMemory()
            => GetHardware(HardwareType.Memory);

        private IHardware GetMotherboard()
        {
            var motherboard = GetHardware(HardwareType.Motherboard);
            return RetryPolicyHelper.RetryFunc(3, 3, () => motherboard.SubHardware.First());
        }

        private IHardware GetHardware(HardwareType type)
            => RetryPolicyHelper.RetryFunc(3, 3, () => system.Hardware.First(_ => _.HardwareType == type));

        private ISensor GetCPUSensor(string sensorName, SensorType sensorType)
            => cpu.Sensors.First(_ => _.Control == null && _.Name.Equals(sensorName) && _.SensorType == sensorType);

        private ISensor GetGPUSensor(string sensorName, SensorType sensorType)
            => gpu.Sensors.First(_ => _.Control == null && _.Name.Equals(sensorName) && _.SensorType == sensorType);

        private ISensor GetMemorySensor(string sensorName, SensorType sensorType)
            => memory.Sensors.First(_ => _.Control == null && _.Name.Equals(sensorName) && _.SensorType == sensorType);

        private ISensor GetMotherboardSensor(string sensorName, SensorType sensorType)
            => motherboard.Sensors.First(_ => _.Control == null && _.Name.Equals(sensorName) && _.SensorType == sensorType);
    }
}
