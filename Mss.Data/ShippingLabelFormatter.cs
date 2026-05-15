using DacQuest.DFX.Core.ByteBuffers;
using DacQuest.DFX.Core.Strings;
using DacQuest.DFX.Devices;
using Mss.Collections;
using Mss.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Data
{
    public static class ShippingLabelFormatter
    {
        public static string LabelVin = "VIN";
        public static string LabelRotation = "ROTATION";
        public static string LabelVehicleLocation = "VEHICLE_LOCATION";
        public static string LabelBarcode = "SHIPPING_LABEL_BARCODE";
//         public static string LabelTriangleData = "TRIANGLE_DATA";

        public static XLabelDataPairs Format(
            LoadItem loadItem,
            string vehicleLocation)
        {
            BroadcastItem broadcastItem = loadItem.Broadcast;
            string rotation = loadItem.Broadcast.LabelRotationText;
            string barcode = $"{rotation}-{vehicleLocation}";
            return new XLabelDataPairs(Constant.ShippingLabelName)
            {
                {LabelVin, broadcastItem.Vin},
                {LabelRotation, rotation},
                {LabelVehicleLocation, vehicleLocation},
                {LabelBarcode, barcode},
            };
        }

//         public static int WidthInBytes = 16;
//         public static int HeightInDots = 112;
// 
//         public static string GetTriangleData()
//         {
//             string triangleData = string.Empty;
// 
//             byte leftMask = 0b1100_0000;
//             byte rightMask = 0b0000_0011;
// 
//             double slope = Math.Sqrt(3);
// 
//             XFixedByteBuffer[] data = new XFixedByteBuffer[HeightInDots];
//             data[0] = new XFixedByteBuffer(Enumerable.Repeat((byte)0xFF, WidthInBytes).ToArray());
//             data[1] = new XFixedByteBuffer(Enumerable.Repeat((byte)0xFF, WidthInBytes).ToArray());
//             for (int index = 2;index < HeightInDots;index++)
//             {
//                 data[index] = new XFixedByteBuffer(WidthInBytes);
//             }
// 
//             int lastLine = 0;
// 
//             for (int line = 2;line < HeightInDots;line++)
//             {
//                 int shift = (int)Math.Round((line - 1) / slope, MidpointRounding.AwayFromZero);
//                 XFixedByteBuffer buffer = data[line];
// 
//                 int leftBufferIndex = shift / 8;
//                 int rightBufferIndex = WidthInBytes - leftBufferIndex - 1;
// 
//                 if (leftBufferIndex <= rightBufferIndex)
//                 {
//                     shift %= 8;
// 
//                     byte leftByte = buffer.PeekByte(leftBufferIndex);
//                     if (leftByte == 0)
//                     {
//                         leftByte = (byte)(leftMask >> shift);
//                     }
//                     else
//                     {
//                         leftByte |= leftMask;
//                     }
//                     buffer.PokeByte(leftBufferIndex, leftByte);
//                     if (shift == 7)
//                     {
//                         buffer.PokeByte(leftBufferIndex + 1, 0b1000_0000);
//                     }
// 
//                     byte rightByte = buffer.PeekByte(leftBufferIndex);
//                     if (rightByte == 0)
//                     {
//                         rightByte = (byte)(rightMask >> shift);
//                     }
//                     else
//                     {
//                         rightByte |= rightMask;
//                     }
//                     buffer.PokeByte(rightBufferIndex, (byte)(rightMask << shift));
//                     if (shift == 7)
//                     {
//                         buffer.PokeByte(rightBufferIndex - 1, 0b0000_0001);
//                     }
// 
//                     lastLine = line;
//                 }
//             }
//             data[HeightInDots - 1] = data[HeightInDots - 2];
// 
//             List<BufferGroup> bufferGroups = new List<BufferGroup>();
//             int count;
//             BufferGroup bufferGroup ;
//             for (count = 0; count < data.Length - 6; count += 6)
//             {
//                 bufferGroup = new BufferGroup();
//                 bufferGroup.Buffers[0] = data[count];
//                 bufferGroup.Buffers[1] = data[count + 1];
//                 bufferGroup.Buffers[2] = data[count + 2];
//                 bufferGroup.Buffers[3] = data[count + 3];
//                 bufferGroup.Buffers[4] = data[count + 4];
//                 bufferGroup.Buffers[5] = data[count + 5];
//                 bufferGroups.Add(bufferGroup);
//             }
// 
//             bufferGroup = new BufferGroup();
//             int paddingCount = HeightInDots - count;
//             for (count = HeightInDots - paddingCount; count < HeightInDots; count++)
//             {
//                 bufferGroup.Buffers[count - (HeightInDots - paddingCount)] = data[count];
//             }
//             for (count = paddingCount; count < 6; count++)
//             {
//                 bufferGroup.Buffers[count] = new XFixedByteBuffer(Enumerable.Repeat((byte)0, WidthInBytes).ToArray());
//             }
//             bufferGroups.Add(bufferGroup);
//             int lineCounter = 0;
//             StringBuilder uLines = new StringBuilder();
//             for (int byteIndex = 0;byteIndex < WidthInBytes;byteIndex++)
//             {
//                 for (int bitIndex = 7; bitIndex >= 0; bitIndex--)
//                 {
//                     string uChars = string.Empty;
//                     for (int listIndex = 0;listIndex < bufferGroups.Count;listIndex++)
//                     {
//                         byte newByte = 0b01000000;
//                         BufferGroup bg = bufferGroups[listIndex];
//                         for (int lineIndex = 5; lineIndex >= 0; lineIndex--)
//                         {
//                             XFixedByteBuffer buffer = bg.Buffers[lineIndex];
// 
//                             byte by = buffer[byteIndex];
//                             if ((by & (0b00000001 << bitIndex)) > 0)
//                             {
//                                 newByte |= (byte)(0b00000001 << lineIndex);
//                             }
//                         }
//                         if (newByte == 127)
//                         {
//                             uChars += "<DEL>";
//                         }
//                         else
//                         {
//                             uChars += (char)newByte;
//                         }
// 
//                     }
//                     uLines.Append($"<STX>u{lineCounter++},{uChars}<ETX>\r\n");
//                 }
//             }
//             triangleData = uLines.ToString();
//             return triangleData;
//         }
// 
//         private class BufferGroup
//         {
//             public XFixedByteBuffer[] Buffers { get; } = new XFixedByteBuffer[6];
//         }

    }

}
