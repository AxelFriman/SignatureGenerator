using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace SignatureGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = args[0];
            int blockSize = 0;
            int blockId = 0;
            try
            {
                if (!File.Exists(path))
                {
                    throw new ArgumentException("File not found");
                }
                if (!Int32.TryParse(args[1], out blockSize) || blockSize == 0)
                {
                    throw new ArgumentException("Wrong block size.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
                return;
            }

            List<Thread> threads = new();

            using (FileStream fstream = File.OpenRead(path))
            {
                while ((blockId + 1) * (long)blockSize <= fstream.Length)
                {
                    var block = new BlockOfBytes(blockId, new byte[blockSize]);
                    fstream.Read(block.Data, 0, blockSize);

                    Thread myThread = new Thread(new ParameterizedThreadStart(Signature.AddSignature)); 

                    threads.Add(myThread);
                    myThread.Start(block);
                    blockId++;
                }

                if (threads != null)
                {
                    foreach (Thread thread in threads)
                    {
                        thread.Join();
                    }
                }
            }
            Signature.PrintSignature();
        }
    }
    public record BlockOfBytes(int Id, byte[] Data);
    public class Signature
    {
        public static ConcurrentDictionary<int, string> Sign { get; set; } = new();
        public static void PrintSignature()
        {
            foreach (var block in Sign)
            {
                Console.WriteLine($"{block.Key} - {block.Value}");
            }

        }
        public static void AddSignature(object blockObj)
        {
            try
            {
                BlockOfBytes block = (BlockOfBytes)blockObj;
                byte[] hash = new byte[1];
                using (SHA256 mySHA256 = SHA256.Create())
                {
                    hash = mySHA256.ComputeHash(block.Data);
                }
                int id = block.Id;
                block = null;
                Sign.TryAdd(id, BitConverter.ToString(hash).Replace("-", ""));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }
        }
    }
}
