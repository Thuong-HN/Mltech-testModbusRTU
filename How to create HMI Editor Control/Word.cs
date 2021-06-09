using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace How_to_create_HMI_Control_Real_Time
{
    public class Word
    {

        public static UInt16 FromByteArray(byte[] bytes)
        {
            // bytes[0] -> HighByte
            // bytes[1] -> LowByte
            return FromBytes(bytes[1], bytes[0]);
        }

        public static UInt16 FromBytes(byte LoVal, byte HiVal)
        {
            return (UInt16)(HiVal * 256 + LoVal);
        }

        public static UInt16[] ByteToUInt16(byte[] bytes)
        {
            UInt16[] values = new UInt16[bytes.Length / 2];
            int counter = 0;
            for (int cnt = 0; cnt < bytes.Length / 2; cnt++)
                values[cnt] = FromByteArray(new byte[] { bytes[counter++], bytes[counter++] });
            return values;
        }

        /// <summary>
        /// Convert UInt16 to byte array
        /// </summary>
        /// <param name="value">UInt16</param>
        /// <returns>byte[]</returns>
        public static byte[] ToByteArray(UInt16 value)
        {
            byte[] array = BitConverter.GetBytes(value);
            Array.Reverse(array);
            return array;
        }
    }
}
