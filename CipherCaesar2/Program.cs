using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CipherCaesar2.Class;

namespace CipherCaesar2
{
    class Program
    {
        static void Main(string[] args)
        {
            //Get two data p and q for cipher RSA
            int n = 0;
            int e = 0;
            string cesar = "";

            //get values
            Console.WriteLine("Cifre la clave de caesar luego regresar a la API");
            Console.WriteLine("Palabra que se cifra de caesar: ");
            cesar = Console.ReadLine();
            Console.WriteLine("Llave publica valor D: ");
            n = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Llave publica valor E: ");
            e = Convert.ToInt32(Console.ReadLine());

            string publicKey = @"publickey.txt";//save publickey
            string cipher = @"cipher.txt";
            string cesarPath = @"caesar.txt";
            using (StreamWriter writer = new StreamWriter(cesarPath))
            {
                writer.WriteLine(cesar);
            }
            using (StreamWriter writer = new StreamWriter(publicKey))
            {
                writer.WriteLine(n + "," + e);
            }

            RSA rsa = new RSA();
            rsa.Encode(cesarPath, cipher, publicKey);

            Console.WriteLine("Finalizado correctamente");
            Console.ReadKey();

        }
    }
}
