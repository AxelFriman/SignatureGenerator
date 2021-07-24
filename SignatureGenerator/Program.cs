using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SignatureGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"theFile.txt";
            int blockSize = 11;
            int blockId = 0;
            using (SHA256 mySHA256 = SHA256.Create())
            using (FileStream fstream = File.OpenRead($"{path}"))
            {
                while ((blockId + 1) * blockSize <= fstream.Length)
                {
                    byte[] block = new byte[blockSize];
                    fstream.Read(block, 0, blockSize);
                    byte[] hash = mySHA256.ComputeHash(block);
                    Console.WriteLine($"{blockId} - {BytesToString(hash)} - {fstream.Position}");
                    blockId++;
                }
            }
            using (StreamReader file = new StreamReader(path))
            {
                Console.WriteLine(file.ReadLine());
            }
        }

        static string BytesToString(byte[] bytes)
        {
            StringBuilder str = new();
            foreach (var b in bytes)
            {
                str.Append(String.Format("{0:X2}", b));
            }
            return str.ToString();
        }
    }
}
