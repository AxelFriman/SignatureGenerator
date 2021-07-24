using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace SignatureGenerator
{
    class BlockOfBytes
    {
        public int id;
        public byte[] data;
    }
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"theFile.txt";
            int blockSize = 100000000;
            int blockId = 0;
            SortedDictionary<int, string> signature = new();
            using (SHA256 mySHA256 = SHA256.Create())
            using (FileStream fstream = File.OpenRead($"{path}"))
            {
                while ((blockId + 1) * blockSize <= fstream.Length)
                {
                    var block = new BlockOfBytes()
                    {
                        id = blockId,
                        data = new byte[blockSize]
                    };
                    fstream.Read(block.data, 0, blockSize);
                    Thread myThread = new Thread(new ParameterizedThreadStart((blockObj) =>
                    {
                        BlockOfBytes block = (BlockOfBytes)blockObj;
                        byte[] hash = mySHA256.ComputeHash(block.data);
                        signature.Add(block.id, BytesToString(hash));
                        //Console.WriteLine($"{block.id} - {BytesToString(block.data)} - {BytesToString(hash)}");
                    }));
                    myThread.Start(block);
                    blockId++;
                }
                Thread.Sleep(5000);
            }
            foreach (var block in signature)
            {
                Console.WriteLine($"{block.Key} - {block.Value}");
            }
        }
        static object locker = new object();

        static string BytesToString(byte[] bytes)
        {
            lock (locker)
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
}
