using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DigitsSummer.Tests
{
    public class Helper
    {
        const string DataFile = "../../../data1M.txt";

        public static async Task GenerateDataAsFile(int max)
        {
            var data = GenerateData(max);
            await File.WriteAllTextAsync(DataFile, data);
        }

        public static string GenerateData(int max)
        {
            Random random = new Random();
            var sb = new StringBuilder(max);
            for (int index = 0; index < max; ++index)
                sb.Append(random.Next(0, 10));

            return sb.ToString();
        }
    }
}
