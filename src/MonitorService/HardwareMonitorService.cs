using Microsoft.Extensions.Configuration;
using MonitorService.Services;
using Serilog;
using Serilog.Core;
using System;
using System.ServiceProcess;
using System.Timers;

namespace MonitorService
{
    public partial class HardwareMonitorService : ServiceBase
    {
        private readonly Logger logger;
        private readonly Timer timer;
        private readonly int updateInterval;
        private readonly SystemService system;
        private readonly SerialService serial;

        public HardwareMonitorService()
        {
            InitializeComponent();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File($"{AppDomain.CurrentDomain.BaseDirectory}\\logs\\hardware-monitor-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                updateInterval = configuration.GetValue<int>("SensorUpdateMilliseconds");

                system = new SystemService(configuration["GPUManufacturer"], configuration["MotherBoardCPUFanConfigName"]);

                var comPort = configuration.GetValue<string>("COMPort");
                var baudRate = configuration.GetValue<int>("BaudRate");
                serial = new SerialService(comPort, baudRate);

                timer = new Timer();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to initialize Hardware Monitor service");
                Stop();
            }
        }

        protected override void OnStart(string[] args)
        {
            timer.Elapsed += new ElapsedEventHandler(ReadSensors);
            timer.Interval = updateInterval;
            timer.Enabled = true;

            logger.Information("Service started, sending data");
        }

        protected override void OnStop()
        {
            system.Close();
            serial.Close();
            timer.Stop();

            logger.Information("Service stopped");
        }

        private void ReadSensors(object source, ElapsedEventArgs e)
        {
            try
            {
                var sensorValues = system.GetSensorValues();
                serial.Write(sensorValues.ToString());
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error sending data");
            }
        }
    }
}
