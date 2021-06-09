using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace How_to_create_HMI_Control_Real_Time
{
    public class ModbusRTUProtocol
    {
        // Declares variables
        private const byte slaveAddress = 2;
        public static List<Register> Registers = new List<Register>();
        public static SerialPort serialPort1 = new SerialPort("COM9", 9600, Parity.None, 8, StopBits.One);
        
        /// <summary>
        /// Starts Modbus RTU Service.
        /// </summary>
        public static void Start()
        {
            try
            {
                
                if (serialPort1.IsOpen) serialPort1.Close();
                serialPort1.Open();
                ThreadPool.QueueUserWorkItem(new WaitCallback((obj) =>
                {
                    while (true)
                    {
                        ReadHoldingRegisters(00000, 10);
                        Thread.Sleep(10); // Delay 20ms
                        
                    }
                }));
                /*
                if (serialPort1.IsOpen) serialPort1.Close();
                serialPort1.Open();
                Thread newThread = new Thread((obj) =>
                {
                    while (true)
                    {
                        ReadHoldingRegisters(00000, 10);
                        Thread.Sleep(100); // Delay 100ms
                        
                    }
                });
                newThread.IsBackground = true;
                newThread.Start();
                */
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Stops Modbus RTU Service.
        /// </summary>
        public static void Stop()
        {
            try
            {
                if (serialPort1.IsOpen) serialPort1.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Writes

        /// <summary>
        /// Writes a value into a single holding register.
        /// </summary>
        /// <param name="startAddress">Address of the register</param>
        /// <param name="value">Value of the register</param>
        public static void WriteSingleRegister(ushort startAddress, ushort value)
        {
            /*
            const byte function = 6;
            byte[] values = Word.ToByteArray(value);
            byte[] frame = WriteSingleRegisterMsg(slaveAddress, startAddress, function, values);
            serialPort1.Write(frame, 0, frame.Length);
            */
            const byte function = 6;
            byte[] values = Word.ToByteArray(value);
            byte[] frame = WriteSingleRegisterMsg(slaveAddress, startAddress, function, values);
            try
            {
                bool again = true;
                while (again)
                    lock (serialPort1)
                    {
                        serialPort1.Write(frame, 0, frame.Length);
                        Thread.Sleep(100); // Delay 100ms
                        if (serialPort1.BytesToRead >= 5)
                        {
                            byte[] bufferReceiver = new byte[serialPort1.BytesToRead];
                            serialPort1.Read(bufferReceiver, 0, serialPort1.BytesToRead);
                            if (bufferReceiver[1] == function)
                            {
                                again = false;
                            }
                        }

                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }

        /// <summary>
        /// Function 06 (06hex)  Write Single Register
        /// </summary>
        /// <param name="slaveAddress">Slave Address</param>
        /// <param name="startAddress">Starting Address</param>
        /// <param name="function">Function</param>
        /// <param name="values">Data</param>
        /// <returns>Byte Array</returns>
        private static byte[] WriteSingleRegisterMsg(byte slaveAddress, ushort startAddress, byte function, byte[] values)
        {
            byte[] frame = new byte[8];                     // Message size
            frame[0] = slaveAddress;                        // Slave address
            frame[1] = function;                            // Function code            
            frame[2] = (byte)(startAddress >> 8);           // Register Address Hi
            frame[3] = (byte)startAddress;                  // Register Address lo
            Array.Copy(values, 0, frame, 4, values.Length); // Write Data
            byte[] crc = CalculateCRC(frame);          // Calculate CRC
            frame[frame.Length - 2] = crc[0];               //Error Check Lo
            frame[frame.Length - 1] = crc[1];               //Error Check Hi
            return frame;
        }

        #endregion

        #region Reads

        /// <summary>
        /// Function 03 (03hex) Read Holding Registers
        /// </summary>
        /// <param name="slaveAddress">Slave Address</param>
        /// <param name="startAddress">Starting Address</param>
        /// <param name="function">Function</param>
        /// <param name="numberOfPoints">Quantity of inputs</param>
        /// <returns>Byte Array</returns>
        private static byte[] ReadHoldingRegistersMsg(byte slaveAddress, ushort startAddress, byte function, uint numberOfPoints)
        {
            byte[] frame = new byte[8];
            frame[0] = slaveAddress;			    // Slave Address
            frame[1] = function;				    // Function             
            frame[2] = (byte)(startAddress >> 8);	// Starting Address High
            frame[3] = (byte)startAddress;		    // Starting Address Low            
            frame[4] = (byte)(numberOfPoints >> 8);	// Quantity of Registers High
            frame[5] = (byte)numberOfPoints;		// Quantity of Registers Low
            byte[] crc = CalculateCRC(frame);  // Calculate CRC.
            frame[frame.Length - 2] = crc[0];       // Error Check Low
            frame[frame.Length - 1] = crc[1];       // Error Check High
            return frame;
        }

        /// <summary>
        /// Read the binary contents of holding registers in the slave.
        /// </summary>
        /// <param name="startAddress">Starting Address</param>
        /// <param name="numberOfPoints">Quantity of inputs</param>
        /// <returns>Registers /returns>
        public static List<Register> ReadHoldingRegisters(ushort startAddress, uint numberOfPoints)
        {
            try
            {
                const byte function = 3;
                if (serialPort1.IsOpen)
                {
                    byte[] frame = ReadHoldingRegistersMsg(slaveAddress, startAddress, function, numberOfPoints);
                    serialPort1.Write(frame, 0, frame.Length);
                    Thread.Sleep(100); // Delay 100ms
                    if (serialPort1.BytesToRead >= 5 ) //5
                    {
                        byte[] bufferReceiver = new byte[serialPort1.BytesToRead];
                        serialPort1.Read(bufferReceiver, 0, serialPort1.BytesToRead);
                        serialPort1.DiscardInBuffer();

                        // Process data.
                        byte[] data = new byte[bufferReceiver.Length - 5];
                        Array.Copy(bufferReceiver, 3, data, 0, data.Length);
                        UInt16[] result = Word.ByteToUInt16(data);
                        for (int i = 0; i < result.Length; i++)
                        {
                            if (i < 10)
                            {
                                Registers[i].Value = result[i];

                            }

                        }
                    }
                }
                
            } catch (Exception ex)
            {

            }
            return Registers;
        }

        #endregion



        /// <summary>
        /// CRC Calculation 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static byte[] CalculateCRC(byte[] data)
        {
            /*
            ushort CRCFull = 0xFFFF; // Set the 16-bit register (CRC register) = FFFFH.
            char CRCLSB;
            byte[] CRC = new byte[2];
            for (int i = 0; i < (data.Length) - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ data[i]); // 

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = (byte)(CRCFull & 0xFF);
            return CRC;
            */
            ushort CRCFull = 0xFFFF; // Set the 16-bit register (CRC register) = FFFFH.
            char CRCLSB;
            byte[] CRC = new byte[2];
            for (int i = 0; i < (data.Length) - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ data[i]); // 

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = (byte)(CRCFull & 0xFF);
            return CRC;
        }
    }
}
