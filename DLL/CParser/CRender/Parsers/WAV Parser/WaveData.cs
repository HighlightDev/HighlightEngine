using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Audio.OpenAL;
using OpenTK.Audio;

namespace CParser.WAV_Parser
{
    public class WaveData
    {
        public WaveData(string fileName)
        {
            _soundData = LoadWave(File.Open(fileName, FileMode.Open), out _channels, out _bitsPerSample, out _sampleRate);
        }


        private Int32 _channels, 
            _bitsPerSample, 
            _sampleRate;
        private byte[] _soundData;
        public byte[] SoundData { get { return _soundData; } }
        public Int32 Channels { get { return _channels; } }
        public Int32 BitsPerSample { get { return _bitsPerSample; } }
        public Int32 SampleRate { get { return _sampleRate; } }
        public ALFormat SoundFormat { get { return GetSoundFormat(_channels, _bitsPerSample); } }


        private byte[] LoadWave(Stream stream, out Int32 channels, out Int32 bits, out Int32 rate)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            BinaryReader reader = new BinaryReader(stream);
            // RIFF header
            string signature = new string(reader.ReadChars(4));

            if (signature != "RIFF")
                throw new NotSupportedException("Specified stream is not a wave file.");

            Int32 riffChunckSize = reader.ReadInt32();
            string format = new string(reader.ReadChars(4));

            if (format != "WAVE")
                throw new NotSupportedException("Specified stream is not a wave file.");

            // WAVE header
            string formatSignature = new string(reader.ReadChars(4));

            if (formatSignature != "fmt ")
                throw new NotSupportedException("Specified wave file is not supported.");

            Int32 formatChunkSize = reader.ReadInt32();
            Int32 audioFormat = reader.ReadInt16();
            Int32 numChannels = reader.ReadInt16();
            Int32 sampleRate = reader.ReadInt32();
            Int32 byteRate = reader.ReadInt32();
            Int32 blockAlign = reader.ReadInt16();
            Int32 bitsPerSample = reader.ReadInt16();

            string dataSignature = new string(reader.ReadChars(4));
            if (dataSignature != "data")
                throw new NotSupportedException("Specified wave file is not supported.");

            Int32 dataChunkSize = reader.ReadInt32();

            channels = numChannels;
            bits = bitsPerSample;
            rate = sampleRate;

            byte[] waveData = reader.ReadBytes((Int32)reader.BaseStream.Length);
            reader.Dispose();

            return waveData;
        }

        private ALFormat GetSoundFormat(Int32 channels, Int32 bits)
        {
            switch (channels)
            {
                case 1:
                    return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2:
                    return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default:
                    throw new NotSupportedException("The specified sound format is not supported.");
            }
        }

        public void Dispose()
        {
            _soundData = null;
            _channels = 0;
            _bitsPerSample = 0;
            _sampleRate = 0;
        }

    }
}
