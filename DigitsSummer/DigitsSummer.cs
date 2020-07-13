using System;
using System.IO;
using System.Threading.Tasks;

namespace DigitsSummer
{
    public static class DigitsSummer
    {

        #region Sum

        public static ulong Sum(string data)
        {
            ulong ret = 0;
            foreach (char current in data)
            {
                var tmp = uint.Parse(current.ToString());
                ret += tmp;
            }

            return ret;
        }

        public static ulong SumV2(string data)
        {
            ulong ret = 0;
            var size = uint.MaxValue.ToString().Length - 1;
            foreach (var current in data.Partition(size))
            {
                var tmp = uint.Parse(current);
                ret += tmp.Sum();
            }

            return ret;
        }

        public static ulong SumV3(string data)
        {
            ulong ret = 0;
            var size = ulong.MaxValue.ToString().Length - 1;
            foreach (var current in data.Partition(size))
            {
                var tmp = ulong.Parse(current);
                ret += tmp.Sum();
            }

            return ret;
        }

        public static ulong SumV3_5(string data)
        {
            ulong ret = 0;
            var size = ulong.MaxValue.ToString().Length - 1;
            for (int index = 0; index < data.Length; index += size)
            {
                var current = data.Substring(index, Math.Min(size, data.Length - index));
                var tmp = ulong.Parse(current);
                ret += tmp.Sum();
            }

            return ret;
        }

        public static ulong SumV4(in ReadOnlySpan<char> data)
        {
            ulong ret = 0;
            var size = ulong.MaxValue.ToString().Length - 1;
            for (int index = 0; index < data.Length; index += size)
            {
                var current = data.Slice(index, Math.Min(size, data.Length - index));
                var tmp = ulong.Parse(current);
                ret += tmp.Sum();
            }

            return ret;
        }

        #endregion

        #region Sum from file

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

        public static ulong SumV4FromFile(string fileName)
        {
            Span<char> buffer = stackalloc char[1024];
            ulong ret = 0;
            using StreamReader fsSource = new StreamReader(fileName);
            while (!fsSource.EndOfStream)
            {
                var cpt = fsSource.Read(buffer);
                if (cpt < buffer.Length)
                    ret += SumV4(buffer.Slice(0, cpt));
                else
                    ret += SumV4(buffer);
            }

            return ret;
        }

        public static ulong SumV5FromFile(string fileName)
        {
            ulong ret = 0;

            
            return ret;
        }

        #endregion

    }
}