using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;

namespace LABREPO_ED2.ClassLab5
{

    public class Zigzag
    {

        //PUBLIC FUNCTION

        public void Encode(string rPath, string wPath, int key)
        {
            string[] ZigZag;
            using (FileStream Rfile = new FileStream(rPath, FileMode.Open))
            using (BinaryReader BR = new BinaryReader(Rfile))
            {
                ZigZag = EText(BR, key);
            }
            using (FileStream Wfile = new FileStream(wPath, FileMode.Create))
            using (BinaryWriter BW = new BinaryWriter(Wfile))
            {
                WCText(ref ZigZag, BW, key);
            }
        }// End method for encode the algorithm zig zag

        public void Decode(string rPath, string wPath, int key)
        {
            int TotalChars = File.ReadAllText(rPath).Length;
            string[] ZigZag;
            using (FileStream Rfile = new FileStream(rPath, FileMode.Open))
            using (BinaryReader BR = new BinaryReader(Rfile))
            {
                ZigZag = DText(BR, key, TotalChars);
            }
            using (FileStream Wfile = new FileStream(wPath, FileMode.Create))
            using (BinaryWriter BW = new BinaryWriter(Wfile))
            {
                WDText(ref ZigZag, BW, key);
            }
        }//End method for decode the algorithm zig zag


        //END PUBLIC FUNCTIONS


        //PRIVATE FUNCTIONS

        //FUNCTION FOR ENCODE

        private string[] EText(BinaryReader br, int key)
        {
            string[] ZigZag = new string[key];
            ZigZag.Initialize();

            while (br.BaseStream.Position != br.BaseStream.Length)
            {
                int current = 0;
                while (br.BaseStream.Position != br.BaseStream.Length && current < key - 1)
                {
                    ZigZag[current] += br.ReadChar();
                    current++;
                }

                while (br.BaseStream.Position != br.BaseStream.Length && current > 0)
                {
                    ZigZag[current] += br.ReadChar();
                    current--;
                }
            }
            return ZigZag;
        }//End method for encrypted text for encode

        private void WCText(ref string[] ZigZag, BinaryWriter bw, int Key)
        {
            for (int i = 0; i < Key; i++) if (ZigZag[i] != null) bw.Write(Encoding.UTF8.GetBytes(ZigZag[i]));
        }//End method for write cyphed text in encode


        //FUNCTION FOR DECODE

        private string[] DText(BinaryReader br, int key, int TotalChars)
        {
            string[] ZigZag = new string[key];
            ZigZag.Initialize();

            int n = (int)Math.Ceiling((double)((TotalChars - 1.0) / (2 * (key - 1))) + 1);
            int MissedChars = (int)(n + (key - 2) * (2 * (n - 1)) + (n - 1) - TotalChars);


            //For Top Line || If Missing Spaces != 0 |=>| Fill Top - 1, else fill Top
            if (MissedChars != 0) ZigZag[0] = GetString(br.ReadChars(n - 1));
            else ZigZag[0] = GetString(br.ReadChars(n));


            //If the key its bigger than two, and missing characters >= 2 |=>| Fill second - 1
            if (key > 2)
            {
                if (MissedChars >= 2) ZigZag[1] = GetString(br.ReadChars(2 * n - 3));
                else ZigZag[1] = GetString(br.ReadChars(2 * n - 2));
            }

            //If the key its bigger than three, check if "i" line needs to be Fiiled (complete || complete - 1 || complete - 2) 
            if (key > 3)
            {
                for (int i = 2; i < key - 1; i++)
                {
                    //This if valuates if i need to skip characters or not
                    if ((key - MissedChars < 0) || MissedChars >= (i + 1))//If its negative that means that in all lines have to skip one or two || If missed chars in last column are more that current line
                    {
                        //This valuates if i need to skip 1 or two /*If true |=>| two, else |=>| one*/
                        if (MissedChars >= (key + (key - (i + 1)))) ZigZag[i] = GetString(br.ReadChars(2 * n - 4));
                        else ZigZag[i] = GetString(br.ReadChars(2 * n - 3));
                    }
                    else ZigZag[i] = GetString(br.ReadChars(2 * n - 2));
                }
            }


            //For Bottom Line, If MissingSpaces < Key |=>| Fill Last, Else FIll Last - 1
            if (MissedChars > key) ZigZag[key - 1] = GetString(br.ReadChars(n - 2));
            else ZigZag[key - 1] = GetString(br.ReadChars(n - 1));

            return ZigZag;
        }//End method for decrypted text the algorithm zig zag

        private string GetString(char[] CharArray)
        {
            string strg = "";
            foreach (var item in CharArray) strg += item;
            return strg;
        }//End method for get string the array type char

        private void WDText(ref string[] ZigZag, BinaryWriter bw, int key)
        {
            int current = 0;
            while (ZigZag[current].Length != 0)
            {
                current = 0;
                while (ZigZag[current].Length != 0 && current < key - 1)
                {
                    char TEST_CurrentByte = (ZigZag[current][0]);
                    bw.Write((byte)(ZigZag[current][0]));
                    ZigZag[current] = ZigZag[current].Substring(1);
                    current++;
                }

                while (ZigZag[current].Length != 0 && current > 0)
                {
                    char TEST_CurrentByte = (ZigZag[current][0]);
                    bw.Write((byte)(ZigZag[current][0]));
                    ZigZag[current] = ZigZag[current].Substring(1);
                    current--;
                }
            }
        }//End method for write decrypted text 


        //END PRIVATE FUNCTIONS
    }

}
