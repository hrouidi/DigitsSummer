using System;
using System.Buffers.Text;
using System.IO;
using System.Threading.Tasks;

namespace DigitsSummer
{
    public static partial class DigitsSummer
    {

        public static ulong SumFromFile(string fileName)
        {
            string text = File.ReadAllText(fileName);
            return Sum(text);
        }

        public static ulong SumV2FromFile(string fileName)
        {
            string text = File.ReadAllText(fileName);
            return SumV2(text);
        }

        public static ulong SumV3FromFile(string fileName)
        {
            string text = File.ReadAllText(fileName);
            return SumV3(text);
        }

        public static ulong SumV3_5FromFile(string fileName)
        {
            string text = File.ReadAllText(fileName);
            return SumV3_5(text);
        }

        public static ulong SumV4FromFile(string fileName, int bufferSize = 1024 * 16)
        {
            Span<char> buffer = stackalloc char[bufferSize];
            ulong ret = 0;
            using StreamReader fsSource = new(fileName);
            while (!fsSource.EndOfStream)
            {
                var cpt = fsSource.Read(buffer);
                if (cpt < buffer.Length)
                    ret += SumV4(buffer[..cpt]);
                else
                    ret += SumV4(buffer);
            }
            return ret;
        }

        public static ulong SumVx2FromFile(string fileName, int bufferSize = 1024 * 16)
        {
            Span<char> buffer = stackalloc char[bufferSize];
            Span<char> current = buffer;
            ulong ret = 0;
            using StreamReader fsSource = new(fileName);
            while (!fsSource.EndOfStream)
            {
                var cpt = fsSource.Read(buffer);
                current = cpt < buffer.Length ? buffer[..cpt] : buffer;
                ret +=  SumVx2(current);
            }
            return ret;
        }

    }
}