using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Timers;
using PortManage;

namespace DemoMV
{
    class Program
    {
        struct DataValue
        {
            public ushort Volt;
            public uint Amp;
            public uint Power;
            public uint Energy;
            public uint PF;
            public uint Fequency;
            public uint LightLV;
            public uint LightStatus;
        }

        static DataValue dataValue;
        static HttpGetService getService;
        static Timer getStatusTimer;
        static SerialManage serialManage;
        static SerialManage modbusSerial;
        static Modbus modbus;
        static bool readAll;
        static void Main(string[] args)
        {
            string command;
            string[] argv = new string[5];
            string response;
            string pushURL = "Http://203.158.253.189:1234/push";
            string getURL = "http://203.158.253.189:1234/status";
            byte[] mpkg = new byte[8];
            dataValue = new DataValue();
            getStatusTimer = new Timer(5000);
            serialManage = new SerialManage();
            modbusSerial = new SerialManage();
            modbus = new Modbus(1);
            getService = new HttpGetService("Http://203.158.253.189:1234/push");
            serialManage.DataIn += serialManage_DataIn;
            modbusSerial.DataIn += modbusSerial_DataIn;
            getStatusTimer.Elapsed += getStatusTimer_Elapsed;
            while (true)
            { 
                Console.Write("MV :> ");
                command = Console.ReadLine();
                if (command != "")
                {
                    argv.Initialize();
                    try
                    {
                        argv = command.Split(' ');
                        switch (argv[0].ToLower())
                        {
                            #region Timer App
                            case "timer" :
                                if (argv[1] != "")
                                {
                                    if (argv[1].ToLower() == "start")
                                    {
                                        getStatusTimer.Start();
                                        Console.WriteLine("Start Timer to get status");
                                    }
                                    else
                                    {
                                        getStatusTimer.Stop();
                                        Console.WriteLine("Stop Timer");
                                    }
                                }
                                break;
                            #endregion
                            #region Serail App
                            case "serial":
                                if (argv[1] != "")
                                {
                                    if (argv[1].ToLower() == "open")
                                    {
                                        try
                                        {
                                            serialManage.PortName = argv[2];
                                            serialManage.BaudRate = int.Parse(argv[3]);
                                            serialManage.Close();
                                            serialManage.Open();
                                            Console.WriteLine("Port Open at {0} Speed: {1} bps ",argv[2],argv[3]);
                                            
                                        }
                                        catch
                                        {
                                            Console.WriteLine("Cannot open port");
                                        }
                                    }
                                    else if (argv[1].ToLower() == "close") 
                                    {
                                        try
                                        {
                                            serialManage.Close();
                                            Console.WriteLine("Close Port");
                                        }
                                        catch
                                        {
                                            Console.WriteLine("Cannot close port");
                                        }
                                    }
                                    else if (argv[1].ToLower() == "get_port")
                                    {
                                        Console.WriteLine("Port avarible : ");
                                        foreach (string str in SerialManage.GetPortNames())
                                        {
                                            Console.WriteLine("\t {0}", str);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Command Error");
                                    }
                                }
                                break;
                            #endregion
                            #region HTTP App
                            case "push":
                                if (argv[1] != "" && argv[2] != "")
                                {
                                    getService.URL = pushURL;
                                    getService.AddPrameter("master_id", argv[1]);
                                    getService.AddPrameter("raw_data", argv[2]);
                                    response = getService.Request();
                                    if (response != "")
                                    {
                                        if (response == "OK")
                                        {
                                            Console.WriteLine("Push Data Success");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Push Data Failure");
                                    }
                                }
                                break;
                            case "get":
                                getService.URL = getURL;
                                response = getService.Request();
                                if (response != "")
                                {
                                    if (response == "0")
                                    {
                                        Console.WriteLine("Close Relay");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Open Relay");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Cannot get data");
                                }
                                break;
                            #endregion
                            #region modbus
                            case "modbus" :
                                if (argv[1] != "")
                                {
                                    if (argv[1].ToLower() == "open")
                                    {
                                        try
                                        {
                                            modbusSerial.PortName = argv[2];
                                            modbusSerial.BaudRate = int.Parse(argv[3]);
                                            modbusSerial.Close();
                                            modbusSerial.Open();
                                            Console.WriteLine("Port Open at {0} Speed: {1} bps ", argv[2], argv[3]);

                                        }
                                        catch
                                        {
                                            Console.WriteLine("Cannot open port");
                                        }
                                    }
                                    else if (argv[1].ToLower() == "close")
                                    {
                                        try
                                        {
                                            modbusSerial.Close();
                                            Console.WriteLine("Close Port");
                                        }
                                        catch
                                        {
                                            Console.WriteLine("Cannot close port");
                                        }
                                    }
                                    
                                    else if (argv[1].ToLower() == "v")
                                    {
                                        mpkg = modbus.GenPkg(MOBUSFnCode_typedef.ReadInputRegister,
                                            (ushort)ReadInputRegister_typedef.VOLT, 1);

                                    }
                                    else if (argv[1].ToLower() == "i")
                                    {
                                        mpkg = modbus.GenPkg(MOBUSFnCode_typedef.ReadInputRegister,
                                            (ushort)ReadInputRegister_typedef.AMP, 1);
                                    }

                                    else if (argv[1].ToLower() == "p")
                                    {
                                        mpkg = modbus.GenPkg(MOBUSFnCode_typedef.ReadInputRegister,
                                            (ushort)ReadInputRegister_typedef.POWER, 1);
                                    }
                                    else if (argv[1].ToLower() == "e")
                                    {
                                        mpkg = modbus.GenPkg(MOBUSFnCode_typedef.ReadInputRegister,
                                            (ushort)ReadInputRegister_typedef.ENERGY, 1);
                                    }
                                    else if (argv[1].ToLower() == "pf")
                                    {
                                        mpkg = modbus.GenPkg(MOBUSFnCode_typedef.ReadInputRegister,
                                            (ushort)ReadInputRegister_typedef.POWER_FACTOR, 1);
                                    }
                                    else if (argv[1].ToLower() == "fq")
                                    {
                                        mpkg = modbus.GenPkg(MOBUSFnCode_typedef.ReadInputRegister,
                                            (ushort)ReadInputRegister_typedef.FREQUENCY, 1);
                                    }
                                    else if (argv[1].ToLower() == "all")
                                    {
                                        mpkg = modbus.GenPkg(MOBUSFnCode_typedef.ReadInputRegister,
                                            (ushort)ReadInputRegister_typedef.VOLT, 7);
                                        readAll = true;
                                    }
                                    if (serialManage.IsOpen)
                                    {
                                        serialManage.Write(mpkg, 0, 8);
                                        Console.WriteLine("\n Mode Bus Sending :" + ByteToHex(mpkg, 0, 8));
                                    }
                                }
                                break;
                            #endregion
                            default:
                                Console.WriteLine("Command Error");
                                break;
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Command Error");
                    }
                }
            }
        }

        static void modbusSerial_DataIn(object sender, SerialMessage e)
        {
            Console.WriteLine("\n Data Incomming :" + ByteToHex(e.Data, 0, e.Length - 2));
            modbus.ReadValue(e.Data);
            if (readAll)
            {
                Console.WriteLine("\n V : {0}", modbus.Volt.ToString());
                Console.WriteLine("\n I : {0}", modbus.Amp.ToString());
                Console.WriteLine("\n Power : {0}", modbus.Power.ToString());
                Console.WriteLine("\n Energy : {0}", modbus.Energy.ToString());
                Console.WriteLine("\n Frequency : {0}", modbus.Frequency.ToString());
                Console.WriteLine("\n PowerFactor : {0}", modbus.PowerFactor.ToString());
                readAll = false;
            }
            else if (modbus.LastRead == ReadInputRegister_typedef.VOLT)
            {
                Console.WriteLine("\n V : {0}", modbus.Volt.ToString());
            }
            else if (modbus.LastRead == ReadInputRegister_typedef.AMP)
            {
                Console.WriteLine("\n I : {0}", modbus.Amp.ToString());
            }
            else if (modbus.LastRead == ReadInputRegister_typedef.POWER)
            {
                Console.WriteLine("\n Power : {0}", modbus.Power.ToString());
            }
            else if (modbus.LastRead == ReadInputRegister_typedef.ENERGY)
            {
                Console.WriteLine("\n Energy : {0}", modbus.Energy.ToString());
            }
            else if (modbus.LastRead == ReadInputRegister_typedef.FREQUENCY)
            {
                Console.WriteLine("\n Frequency : {0}", modbus.Frequency.ToString());
            }
            else if (modbus.LastRead == ReadInputRegister_typedef.POWER_FACTOR)
            {
                Console.WriteLine("\n PowerFactor : {0}", modbus.PowerFactor.ToString());
            }

            //throw new NotImplementedException();
        }

        static void getStatusTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Get Status");
            string response;
            getService.URL = "http://203.158.253.189:1234/status";
            response = getService.Request();
            if (response != "")
            {
                if (response == "0")
                {
                    Console.WriteLine("Close Relay");
                    serialManage.Write("<0\r\n");
                }
                else
                {
                    Console.WriteLine("Open Relay");
                    serialManage.Write("<1\r\n");
                }
            }
            //throw new NotImplementedException();
        }

        static void PushService(string masterId, string rawData)
        {

            string response;
            getService.URL = "Http://203.158.253.189:1234/push";
            getService.AddPrameter("master_id", masterId);
            getService.AddPrameter("raw_data", rawData);
            response = getService.Request();
            if (response != "")
            {
                if (response == "OK :-)")
                {
                    Console.WriteLine("Push Data Success");
                }
            }
            else
            {
                Console.WriteLine("Push Data Failure");
            }
        }

        static void serialManage_DataIn(object sender, SerialMessage e)
        {
            string raw;
            if (e.Data[0] == '>' && e.Length != 0)
            {
                dataValue.Volt = (ushort)(e.Data[1] << 8);
                dataValue.Volt |= (ushort)e.Data[2];
                Console.WriteLine("\tVolt : {0} mV", dataValue.Volt.ToString());
                dataValue.Amp = (uint)e.Data[4] << 8;
                dataValue.Amp |= (uint)e.Data[5];
                Console.WriteLine("\tAmp : {0} mA", dataValue.Amp.ToString());
                dataValue.Power = (uint)e.Data[7] << 24;
                dataValue.Power |= (uint)e.Data[8] << 16;
                dataValue.Power |= (uint)e.Data[9] << 8;
                dataValue.Power |= (uint)e.Data[10];
                Console.WriteLine("\tPower : {0} W", dataValue.Power.ToString());
                dataValue.Energy = (uint)e.Data[12] << 24;
                dataValue.Energy |= (uint)e.Data[13] << 16;
                dataValue.Energy |= (uint)e.Data[14] << 8;
                dataValue.Energy |= (uint)e.Data[15];
                Console.WriteLine("\tEnergy : {0} Wh", dataValue.Energy.ToString());
                dataValue.PF = (uint)e.Data[17];
                Console.WriteLine("\tPower Factor : 0.{0} ", dataValue.PF.ToString());
                dataValue.Fequency = (uint)e.Data[19] << 8;
                dataValue.Fequency |= (uint)e.Data[20];
                Console.WriteLine("\tFrequency : {0} Hz", (dataValue.Fequency / 10).ToString());
                dataValue.LightLV = (uint)e.Data[22] << 24;
                dataValue.LightLV |= (uint)e.Data[23] << 16;
                dataValue.LightLV |= (uint)e.Data[24] << 8;
                dataValue.LightLV |= (uint)e.Data[25];
                Console.WriteLine("\tLight Level : {0} ", dataValue.LightLV.ToString());
                dataValue.LightStatus = (uint)e.Data[27];
                Console.Write("Raw_data :");
                raw = String.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}",
                    dataValue.Volt.ToString("x4"),
                    dataValue.Amp.ToString("x4"),
                    dataValue.Power.ToString("x8"),
                    dataValue.Energy.ToString("x8"),
                    dataValue.PF.ToString("x2"),
                    dataValue.Fequency.ToString("x4"),
                    dataValue.LightLV.ToString("x8"),
                    dataValue.LightStatus.ToString("x2"));
                Console.WriteLine(raw);
                PushService("1", raw);

            }
            //throw new NotImplementedException();
        }

        static string ByteToHex(byte[] combyte, int start, int length)
        {
            //create a new stringbuilder object
            StringBuilder builder = new StringBuilder(256);
            //loop through each byte in the array
            for (int i = start; i < (start + length); i++)
            {
                builder.Append(Convert.ToString(combyte[i], 16).PadLeft(2, '0').PadRight(3, ' '));
            }
            return builder.ToString().ToUpper();
        }
    }
}
