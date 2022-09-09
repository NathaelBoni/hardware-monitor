using MonitorService.Helpers;
using System;
using System.IO.Ports;

namespace MonitorService.Services
{
    public class SerialService
    {
        private readonly SerialPort serialPort;

        public SerialService(string portName, int baudRate)
        {
            serialPort = new SerialPort
            {
                PortName = portName,
                BaudRate = baudRate
            };
            Open();
        }

        public void Close()
        {
            if (serialPort.IsOpen)
                serialPort.Close();
        }

        public void Reconnect()
            => Open();

        public void Write(string message)
        {
            try
            {
                serialPort.WriteLine(message);
            }
            catch (InvalidOperationException ex)
            {
                if (!ex.Message.Equals("The port is closed."))
                    throw;

                Close();
                Reconnect();
            }
        }

        private void Open()
            => RetryPolicyHelper.RetryAction(0, 3, () => serialPort.Open());
    }

    public class SerialConfig
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }
    }
}
