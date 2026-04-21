using System;
using System.IO;
using UnityEngine;

namespace ARPOC.Utils
{
    public static class AudioWavSerializer
    {
        private const int HeaderSize = 44;

        public static byte[] GetWavBinary(AudioClip clip)
        {
            using (var memoryStream = new MemoryStream())
            {
                WriteWavHeader(memoryStream, clip);
                WriteWavData(memoryStream, clip);
                return memoryStream.ToArray();
            }
        }

        private static void WriteWavHeader(Stream stream, AudioClip clip)
        {
            int hz = clip.frequency;
            int channels = clip.channels;
            int samples = clip.samples;

            stream.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"), 0, 4);
            stream.Write(BitConverter.GetBytes(HeaderSize + samples * channels * 2 - 8), 0, 4);
            stream.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"), 0, 4);
            stream.Write(System.Text.Encoding.UTF8.GetBytes("fmt "), 0, 4);
            stream.Write(BitConverter.GetBytes(16), 0, 4);
            stream.Write(BitConverter.GetBytes((short)1), 0, 2);
            stream.Write(BitConverter.GetBytes((short)channels), 0, 2);
            stream.Write(BitConverter.GetBytes(hz), 0, 4);
            stream.Write(BitConverter.GetBytes(hz * channels * 2), 0, 4);
            stream.Write(BitConverter.GetBytes((short)(channels * 2)), 0, 2);
            stream.Write(BitConverter.GetBytes((short)16), 0, 2);
            stream.Write(System.Text.Encoding.UTF8.GetBytes("data"), 0, 4);
            stream.Write(BitConverter.GetBytes(samples * channels * 2), 0, 4);
        }

        private static void WriteWavData(Stream stream, AudioClip clip)
        {
            float[] samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);

            short[] intData = new short[samples.Length];
            byte[] bytesData = new byte[samples.Length * 2];

            for (int i = 0; i < samples.Length; i++)
            {
                intData[i] = (short)(samples[i] * 32767);
                byte[] byteArr = BitConverter.GetBytes(intData[i]);
                byteArr.CopyTo(bytesData, i * 2);
            }

            stream.Write(bytesData, 0, bytesData.Length);
        }
    }
}
