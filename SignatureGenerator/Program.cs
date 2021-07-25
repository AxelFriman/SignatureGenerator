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
            //string path = @"theFile1.txt";
            string path = @"F:/theFileL.txt";
            int blockSize = 100_000_000;
            int blockId = 0;
            List<Thread> threads = new List<Thread>();
            
            using (FileStream fstream = File.OpenRead($"{path}"))
            {
                while ((blockId + 1) * (long) blockSize <= fstream.Length)
                {
                    Console.WriteLine($"[{blockId}/{fstream.Length/blockSize}]");
                    Console.WriteLine($"({blockId}+1)*{blockSize} ({(blockId + 1) * blockSize}) <= {fstream.Length}");
                    var block = new BlockOfBytes(blockId, new byte[blockSize]);
                    fstream.Read(block.data, 0, blockSize);

                    Thread myThread = new Thread(new ParameterizedThreadStart(new Signature().AddSignature));

                    threads.Add(myThread);
                    myThread.Start(block);
                    blockId++;
                }
                Console.WriteLine("DONE");

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
    public record BlockOfBytes(int id, byte[] data);
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
        public void AddSignature(object blockObj)
        {
            BlockOfBytes block = (BlockOfBytes)blockObj;
            byte[] hash = new byte[1];
            using (SHA256 mySHA256 = SHA256.Create())
            {
                hash = mySHA256.ComputeHash(block.data);
            }
            int id = block.id;
            block = null; 
            Sign.TryAdd(id, BitConverter.ToString(hash).Replace("-", ""));
        }
    }
}
