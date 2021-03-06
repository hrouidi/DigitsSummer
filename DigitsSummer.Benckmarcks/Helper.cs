﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DigitsSummer.Benchmarks
{
    public static class GlobalSetupHelper
    {
        private const string _dataDirectory = @"D:\WorkSpace\Perso\DigitsSummer\Data";
        public static Dictionary<string, string> GenerateDataFilesIfDoesNotExist(params string[] @params)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            foreach (string param in @params)
            {
                switch (param)
                {
                    case "1M":
                        GenerateDataFileIfDoesNotExist("data1M.txt", 1_000_000);
                        ret.Add(param, Path.Combine(_dataDirectory, "data1M.txt"));
                        break;
                    case "10M":
                        GenerateDataFileIfDoesNotExist("data10M.txt", 10_000_000);
                        ret.Add(param, Path.Combine(_dataDirectory, "data10M.txt"));
                        break;
                    case "100M":
                        GenerateDataFileIfDoesNotExist("data100M.txt", 100_000_000);
                        ret.Add(param, Path.Combine(_dataDirectory, "data100M.txt"));
                        break;
                    case "1G":
                        GenerateDataFileIfDoesNotExist("data1G.txt", 1_000_000_000);
                        ret.Add(param, Path.Combine(_dataDirectory, "data1G.txt"));
                        break;
                    case "2G":
                        GenerateDataFileIfDoesNotExist("data2G.txt", 2_000_000_000);
                        ret.Add(param, Path.Combine(_dataDirectory, "data2G.txt"));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return ret;
        }

        private static void GenerateDataFileIfDoesNotExist(string fileName, int max)
        {
            var path = Path.Combine(_dataDirectory, fileName);
            if (!File.Exists(path))
            {
                Console.WriteLine($"Generating data file @ : {path}");
                using TextWriter writeFile = new StreamWriter(path);
                foreach (var text in GenerateData(max))
                    writeFile.Write(text);
                writeFile.Flush();
                writeFile.Close();
            }
        }

        public static IEnumerable<string> GenerateData(int max)
        {
            const int bufferSize = 1024 * 16;
            Random random = new Random();
            var sb = new StringBuilder(max);
            for (int index = 0; index < max; ++index)
            {
                sb.Append(random.Next(0, 10));
                if (index % bufferSize == 0)
                {
                    var ret = sb.ToString();
                    sb.Clear();
                    yield return ret;
                }
            }
            if (sb.Length > 0)
                yield return sb.ToString();

        }

        public static string GenerateDataAsString(int max)
        {
            Random random = new ();
            var sb = new StringBuilder(max);
            for (int index = 0; index < max; ++index)
                sb.Append(random.Next(0, 10));
            return sb.ToString();
        }
    }
}
