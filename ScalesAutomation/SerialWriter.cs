using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;
using log4net;
using System.Reflection;
using System.Threading;

namespace ScalesAutomation
{
    public class MySerialWriter : IDisposable
    {
        private SerialPort serialPort;

        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MySerialWriter()
        {
            serialPort = new SerialPort("COM6", 4800, Parity.Even, 7, StopBits.Two);
            serialPort.Open();
        }

        public void Dispose()
        {
            if (serialPort != null)
                serialPort.Dispose();
        }

        public void StartTransmission()
        {
            for (var i = 0; i < 10; i++)
            {
                WriteThread();
                Thread.Sleep(100);
            }
        }

        public void WriteThread()
        {
            SerialPort comPortPC = new SerialPort("COM6", 4800, Parity.Even, 7, StopBits.Two);

            log.Debug(System.Environment.NewLine);
            log.Debug("Serial Transmission Started!" + Environment.NewLine);

            comPortPC.Open();

            byte[] buf = new byte[15];

            buf[0] = 0x24; // $ = start
            buf[1] = 0x00; // 0 = stable, 1 = unstable, 3 = not valid
            buf[2] = 0x31;
            buf[3] = 0x32;
            buf[4] = 0x33;
            buf[5] = 0x34;
            buf[6] = 0x36; // weight unit
            buf[7] = 0x20; // 20 = space
            buf[8] = 0x61; // 6 code digits
            buf[9] = 0x62;
            buf[10] = 0x63;
            buf[11] = 0x64;
            buf[12] = 0x65;
            buf[13] = 0x66;
            buf[14] = 0x0D; // CR = end character

            comPortPC.Write(buf, 0, buf.Length);

            comPortPC.Close();
        }


    }
}
