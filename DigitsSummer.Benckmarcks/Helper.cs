using System;
using System.Collections.Generic;
using System.IO;
using DigitsSummer.Tests;

namespace DigitsSummer.Benchmarks
{
    public static class GlobalSetupHelper
    {
        public static Dictionary<string, string> GenerateDataFilesIfDoesNotExist(params string[] @params)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            foreach (string param in @params)
            {
                switch (param)
                {
                    case "1M":
                        GenerateDataFileIfDoesNotExist("data1M.txt", 1_000_000);
                        ret.Add(param, "data1M.txt");
                        break;
                    case "10M":
                        GenerateDataFileIfDoesNotExist("data10M.txt", 10_000_000);
                        ret.Add(param, "data10M.txt");
                        break;
                    case "100M":
                        GenerateDataFileIfDoesNotExist("data100M.txt", 100_000_000);
                        ret.Add(param, "data100M.txt");
                        break;
                    case "1G":
                        GenerateDataFileIfDoesNotExist("data1G.txt", 1_000_000_000);
                        ret.Add(param, "data1G.txt");
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return ret;
        }

        private static void GenerateDataFileIfDoesNotExist(string fileName, int max)
        {
            if (!File.Exists(fileName))
            {
                var text = Helper.GenerateData(max);
                File.WriteAllText(fileName, text);
            }
        }
    }
}
