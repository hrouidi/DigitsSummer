using System;
using System.IO;
using System.Text;

namespace DigitsSummer.Tests
{
    public class Helper
    {
        public static string GenerateData(int max)
        {
            Random random = new();
            var sb = new StringBuilder(max);
            for (int index = 0; index < max; ++index)
                sb.Append(random.Next(0, 10));

            return sb.ToString();
        }

        public static ulong GenerateDataFile(string fileName, int max)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            var text = GenerateData(max);
            File.WriteAllText(fileName, text);
            return DigitsSummer.SumV4(text);
        }

        public static void GenerateDataFile(string fileName, string content)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
            File.WriteAllText(fileName, content);
        }
    }
}
