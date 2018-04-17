using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace LudumDare41.ContentManagement
{
    public static class OggSoundEffectLoader
    {
        public static SoundEffect LoadFromStream(Stream stream)
        {
            using (var foo = new NVorbis.VorbisReader(stream, true))
            {
                float[] buffer = new float[foo.TotalSamples * foo.Channels];

                foo.ReadSamples(buffer, 0, buffer.Length);

                using (var memoryStream = new MemoryStream())
                {
                    // Convert to wave format
                    using (var writer = new BinaryWriter(memoryStream, System.Text.Encoding.UTF8, true))
                    {
                        writer.Write("RIFF".ToCharArray());
                        writer.Write(44 + buffer.Length * 2);
                        writer.Write("WAVE".ToCharArray());

                        writer.Write("fmt ".ToCharArray());
                        writer.Write((int)16); // 16 bytes 
                        writer.Write((ushort)1);    // PCM
                        writer.Write((ushort)foo.Channels);    // 2 channels
                        writer.Write((uint)foo.SampleRate);  // 44100 sample rate
                        writer.Write((uint)(foo.SampleRate * 16 * foo.Channels) / 8); // byte rate:  sampleRate * channel count * bits per sample / 8
                        writer.Write((ushort)(foo.Channels * 16 / 8)); // blockAlign numChannels * bits per sample / 8
                        writer.Write((ushort)16); // sample size 8 / 16

                        writer.Write("data".ToCharArray());
                        writer.Write(buffer.Length * 2);

                        // TODO this is gross
                        foreach (var sample in buffer)
                        {
                            short sampleShort = (short)(sample * 32767);
                            writer.Write(sampleShort);
                        }
                    }

                    memoryStream.Position = 0;

                    return SoundEffect.FromStream(memoryStream);
                }
            }
        }
    }

}