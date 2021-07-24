﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

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
                    Thread myThread = new Thread(new ParameterizedThreadStart((Id) =>
                    {
                        byte[] block = new byte[blockSize];
                        fstream.ReadAsync(block, 0, blockSize);
                        byte[] hash = mySHA256.ComputeHash(block);
                        Console.WriteLine($"{Id} - {BytesToString(hash)}");
                    }));
                    myThread.Start(blockId);
                    blockId++;
                }
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
