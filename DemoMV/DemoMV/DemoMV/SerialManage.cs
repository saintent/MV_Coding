using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Timers;

namespace PortManage
{
    class SerialManage : SerialPort
    {

        public delegate void EndOfData(object sender, SerialMessage e);
        public event EndOfData DataIn;
        private int dataLengthReceive;
        //private int dataLengthSent;
        private byte[] dataReceive;
        //private byte[] dataSent;
        
        Timer stopwatch;

        public SerialManage()
        {
            this.BaudRate = 9600;
            this.DataBits = 8;
            this.Parity = Parity.None;
            this.StopBits = StopBits.One;
            this.PortName = "COM1";
            dataReceive = new byte[1024];
            stopwatch = new Timer(100);
            stopwatch.Elapsed += stopwatch_Elapsed;
            
            // Register Event
            this.DataReceived += SerialManage_DataReceived;

        }

        public void InitSerailPort(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            this.BaudRate = baudRate;
            this.DataBits = dataBits;
            this.Parity = parity;
            this.StopBits = stopBits;
            this.PortName = portName;
        }

        protected virtual void SerialManage_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int byteRec = this.BytesToRead;
            this.Read(dataReceive, dataLengthReceive, byteRec);
            dataLengthReceive += byteRec;
            stopwatch.Start();
            //throw new NotImplementedException();
        }
        protected virtual void stopwatch_Elapsed(object sender, ElapsedEventArgs e)
        {
            stopwatch.Stop();
            if (DataIn != null)
            {
                SerialMessage sm = new SerialMessage(dataReceive, dataLengthReceive);
                DataIn(this, sm);
                dataLengthReceive = 0;
            }
            //throw new NotImplementedException();
        }
    }

    class SerialMessage
    {
        private int length;
        private byte[] data;

        public SerialMessage(byte[] data, int length)
        {
            this.data = data;
            this.length = length;
        }

        public int Length
        {
            get { return this.length; }
        }
        public byte[] Data
        {
            get { return this.data; }
        }
    }
}
