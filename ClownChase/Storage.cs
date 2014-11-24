using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Documents;

namespace ClownChase
{
    public class Storage
    {
        public void Load(ICaptureFrameCollection captureFrameCollection)
        {
            var fileNames = Directory.GetFiles(".", "*.cln");
            Array.Sort(fileNames);
            foreach (var fileName in fileNames)
            {
                captureFrameCollection.Add(Load(fileName));
            }
        }
        
        public void Save(ICaptureFrameCollection captureFrameCollection)
        {
            int i = 0;
            foreach (var frame in captureFrameCollection.CapturedFrames)
            {
                Save(i++, frame);
            }
        }

        private void Save(int order, CapturedFrame frame)
        {
            var fileName = GetFileName(order, frame)+".cln";
            using (var fs = File.OpenWrite(fileName))
            {
                WriteInt(fs, frame.NearPosition.Depth);
                WriteInt(fs, frame.NearPosition.X);
                WriteInt(fs, frame.NearPosition.Y);
                WriteArray(fs, frame.ColorPixels);
                WriteArray(fs, frame.Mask);
            }
        }

        private void WriteInt(FileStream fs, int value)
        {
            var data = Encoding.UTF8.GetBytes(value.ToString(CultureInfo.InvariantCulture));
            fs.Write(data, 0, data.Length);
            fs.WriteByte(0);            
        }
        private void WriteArray(FileStream fs, byte[] value)
        {
            WriteInt(fs, (value==null)?-1:value.Length);
            if (value != null)
            {
                fs.Write(value, 0, value.Length);                
            }
        }

        private CapturedFrame Load(string fileName)
        {
            var frame = new CapturedFrame();
            using (var fs = File.OpenRead(fileName))
            {
                frame.NearPosition = new Position { Depth = ReadInt(fs), X = ReadInt(fs), Y = ReadInt(fs) };
                frame.ColorPixels = ReadBytes(fs);
                frame.Mask = ReadBytes(fs);
            }

            return frame;
        }

        private int ReadInt(FileStream fs)
        {
            var data = new List<byte>();
            byte b;
            do
            {
                b = (byte) fs.ReadByte();
                if (b == 0)
                {
                    break;
                }
                data.Add(b);
            } while (true);
            return Int32.Parse(Encoding.UTF8.GetString(data.ToArray()));
        }

        private byte[] ReadBytes(FileStream fs)
        {
            var len = ReadInt(fs);
            if (len == -1)
            {
                return null;
            }

            var data = new byte[len];
            fs.Read(data, 0, len);
            return data;
        }

        private string GetFileName(int order, CapturedFrame frame)
        {
            return String.Format("{0:00000}-{1}", order, frame.NearPosition.Depth);
        }
    }
}
