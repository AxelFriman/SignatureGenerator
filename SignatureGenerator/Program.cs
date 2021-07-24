using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace SignatureGenerator
{
    class TPL
    {
        public int id;
        public byte[] block;
    }
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
                    Thread myThread = new Thread(new ParameterizedThreadStart((tpl) =>
                    {
                        //byte[] hash = mySHA256.ComputeHash(block);
                        TPL temp = (TPL)tpl;
                        Console.WriteLine($"{temp.id} - {BytesToString(temp.block)}");
                    }));
                    myThread.Start(new TPL() { id = blockId, block = block});
                    blockId++;
                }
            }
            Console.WriteLine();
        }
        static object locker = new object();

        static string BytesToString(byte[] bytes)
        {
            lock (locker) {
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
