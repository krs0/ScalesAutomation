﻿using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using log4net;
using System.Reflection;
using System.Threading;

namespace ScalesAutomation
{
    public class MySerialWriter : IDisposable
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private SerialPort serialPort;

        public MySerialWriter()
        {
            serialPort = new SerialPort("COM6", 4800, Parity.Even, 7, StopBits.Two);
            Thread.Sleep(1000);

            if (!serialPort.IsOpen)
                serialPort.Open();

            startTransmissionFromFile();

            serialPort.Close();
            this.Dispose();

        }

        public void Dispose()
        {
            if (serialPort != null)
            {
                if (serialPort.IsOpen)
                    serialPort.Close();

                serialPort.Dispose();
            }
        }

        private void startTransmissionFromFile()
        {
            byte[][] dataToTransmit = loadSimulatedDataFromFile();
            int i = 0;

            for (i = 0; i < dataToTransmit.Length; i++)
            {
                writeData(dataToTransmit[i]);
                Thread.Sleep(10);
            }
        }

        public byte[][] loadSimulatedDataFromFile()
        {
            string line;
            string[] lineAsStringArray;
            string filePath = "transmission.txt";
            int i = 0;


            var lineCount = File.ReadLines(filePath).Count();
            byte[][] byteArray2d = new byte[lineCount][];

            log.Debug("Loading from file: " + Environment.NewLine);

            System.IO.StreamReader file = new System.IO.StreamReader(filePath);
            while ((line = file.ReadLine()) != null)
            {
                lineAsStringArray = line.Split('-');
                byte[] lineAsByteArray = lineAsStringArray.Select(s => Convert.ToByte(s, 16)).ToArray();
                byteArray2d[i] = new byte[lineAsByteArray.Length];
                lineAsByteArray.CopyTo(byteArray2d[i], 0);
                i++;
            }

            file.Close();

            return byteArray2d;

        }

        public void writeData(byte[] txBuffer)
        {
            log.Debug("Writing bytes: " + BitConverter.ToString(txBuffer) + Environment.NewLine);

            serialPort.Write(txBuffer, 0, txBuffer.Length);
        }

    }
}
