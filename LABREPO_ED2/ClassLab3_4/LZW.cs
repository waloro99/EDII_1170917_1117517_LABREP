using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace LABREPO_ED2.ClassLab3_4
{

    public class LZW
    {
        //Public functions 

        public void Compress(string rPath, string wPath)
        {
            /*Dictionary<Char[], int>*/
            var Characters = new Dictionary<string, int>();

            /*First, reads all the text to Get the possibles chars*/
            using (FileStream FSR = new FileStream(rPath, FileMode.Open))
            using (BinaryReader BR = new BinaryReader(FSR))
            {
                Characters = GetFileCharacters(BR);
            }


            /*Second, reads again while compressing*/
            using (FileStream FSR = new FileStream(rPath, FileMode.Open))
            using (BinaryReader BR = new BinaryReader(FSR))
            using (FileStream FSW = new FileStream(wPath, FileMode.Create))
            using (BinaryWriter BW = new BinaryWriter(FSW))
            {
                CompressA(BR, BW, Characters);
            }
        }//end method compress

        public void Uncompress(string rPath, string wPath)
        {
            string text = "";
            List<byte> tempo = new List<byte>();
            int count = 0;
            Dictionary<int, string> diccionary = new Dictionary<int, string>();
            int index = 0;

            using (var wfile = new FileStream(wPath, FileMode.OpenOrCreate))
            using (var bw = new BinaryWriter(wfile))
            using (var rfile = new FileStream(rPath, FileMode.Open))
            using (BinaryReader br = new BinaryReader(rfile))
            {
                int bufferSize = 1;
                byte[] Buff = new byte[bufferSize];

                //------- creating dictionary ---------------------
                while (br.BaseStream.Position != br.BaseStream.Length)
                {
                    Buff = br.ReadBytes(bufferSize);
                    tempo.AddRange(Buff.ToList<byte>());
                    //read while diccionari isn't create
                    if (diccionary.Count == 0)
                    {
                        if (CEDI(tempo, ref count))
                        {
                            diccionary = CreateDictionary(tempo.GetRange(0, count));
                            index = diccionary.Count + 1;
                            tempo.RemoveRange(0, count + 3);
                            break;
                        }
                    }
                }

                List<int> presente = new List<int>();
                List<int> aux = new List<int>() { Convert.ToInt32(br.ReadByte()) };
                //------- redoing the text ---------------------
                while (br.BaseStream.Position != br.BaseStream.Length)
                {
                    while (br.BaseStream.Position != br.BaseStream.Length && diccionary.ContainsKey(getNumber(aux)))
                    {
                        aux.Add(Convert.ToInt32(br.ReadByte()));
                    }
                    if (!diccionary.ContainsKey(getNumber(aux)))
                    {
                        presente = aux.GetRange(0, aux.Count - 1);
                        string value = diccionary[getNumber(presente)];
                        if (diccionary.ContainsKey(aux.Last())) value += diccionary[aux.Last()][0];
                        else value += value[value.Length - 1];
                        diccionary.Add(index, value);

                        bw.Write(Encoding.Default.GetBytes(diccionary[getNumber(presente)]));
                    }
                    else
                    {
                        bw.Write(Encoding.Default.GetBytes(diccionary[getNumber(presente)]));
                    }
                    aux = new List<int>() { aux.Last() };
                    index++;
                }
                //writes the last characters
                bw.Write(Encoding.Default.GetBytes(diccionary[diccionary.Count]));
            }

        } //End method decompress

        public string GetFilesMetrics(string Name, string Original, string Compresed)
        {
            string Metrics = Name.Replace(".txt", "") + '|';
            double RC, FC, PR;
            using (FileStream OR = new FileStream(Original, FileMode.Open))
            using (FileStream CM = new FileStream(Compresed, FileMode.Open))
            {
                RC = Math.Round(CM.Length / (double)OR.Length, 3);
                FC = Math.Round(OR.Length / (double)CM.Length, 3);
                PR = Math.Round(((1 - RC) * 100), 2);
            }
            Metrics += RC.ToString() + "|" + FC.ToString() + "|" + PR.ToString();
            return Metrics;
        } //End method for get file metrics

        //-------------------------------------------------------END PUBLIC FUNCTIONS

        // PRIVATE FUNCTIONS
        private Dictionary<string, int> GetFileCharacters(BinaryReader br)
        {
            string NextByte;
            List<byte> cmpByte = new List<byte>() { 0 };
            Dictionary<string, int> Characters = new Dictionary<string, int>();
            Characters.Add("An entry to inicialize the values", 0);

            //while used to read all the file and get all the characters
            while (br.BaseStream.Position != br.BaseStream.Length)
            {
                /*Its used .ReadBytes(1) instead of readbyte because its necessary to have an array in the paramether*/
                NextByte = (char)br.ReadByte() + "";
                if (!Characters.ContainsKey(NextByte)) Characters.Add(NextByte, Characters.Last().Value + 1);
            }

            Characters.Remove("An entry to inicialize the values");
            return Characters;
        } //End method Get File characters


        private void CompressA(BinaryReader BR, BinaryWriter BW, Dictionary<string, int> Dic)
        {
            /*Writes just the original dictionay on the compressed file*/
            foreach (var item in Dic)
            {
                BW.Write(Convert.ToByte(item.Value));
                BW.Write(Encoding.Default.GetBytes("," + item.Key + ","));
            }
            BW.Write(Encoding.Default.GetBytes("EOD"));

            string newDicValue = Encoding.Default.GetString(BR.ReadBytes(1));
            while (BR.BaseStream.Position != BR.BaseStream.Length)
            {
                //While the string of bytes is still present on the dictionary, string += next char
                while (Dic.ContainsKey(newDicValue) && (BR.BaseStream.Position != BR.BaseStream.Length)) newDicValue += (char)BR.ReadByte();

                //Adds to the dictionary the new value with the new string readed
                if (!Dic.ContainsKey(newDicValue)) Dic.Add(newDicValue, Dic.Count + 1);

                /*Writes the contained bytelist*/
                int TST_CheckValToWrite = Dic[newDicValue.Remove(newDicValue.Length - 1)];
                BW.Write(GBFI(Dic[newDicValue.Remove(newDicValue.Length - 1)]));

                newDicValue = Convert.ToString(newDicValue[newDicValue.Length - 1]);
            }
        } //End method for compress algorithm


        private byte[] GBFI(int number)
        {
            string IntInBinary = "";
            var rtrnLst = new List<byte>();
            if (number <= 255)
            {
                rtrnLst.Add((byte)number);
            }
            else
            {
                IntInBinary = Convert.ToString(number, 2);
                int bytesToFill = IntInBinary.Length % 8;
                string Fill = "";
                for (int i = 0; i < 8 - bytesToFill; i++) Fill += 0;
                IntInBinary = Fill + IntInBinary;

                int iterations = IntInBinary.Length / 8;
                for (int i = 0; i < iterations; i++)
                {
                    string newbyte = "";
                    for (int j = 0; j < 8; j++) newbyte += IntInBinary[(i * 8) + j];
                    rtrnLst.Add(Convert.ToByte(newbyte, 2));
                }

            }
            return rtrnLst.ToArray();
        }// End method get bytes from int

        private static bool CEDI(List<byte> MotherList, ref int count)
        {
            List<byte> ChildList = Encoding.ASCII.GetBytes("EOD").ToList<byte>();
            int MotherElements = MotherList.Count() - 1;
            int ChildElements = ChildList.Count() - 1;
            bool Answer = false;
            for (int i = 0; i <= MotherElements; i++)
            {
                for (int j = 0; j <= ChildElements; j++)
                {
                    if (i + ChildElements > MotherElements)
                        return false;
                    if (MotherList[i + j] == ChildList[j])
                    {
                        Answer = true;
                        if (MotherList[i + j] == ChildList[2] && Answer == true)
                            count = i;
                    }
                    else
                    {
                        Answer = false;
                        break;
                    }
                    if (j == ChildElements)
                        return Answer;
                }
            }
            return true;
        }//End method check end diccionary

        private static Dictionary<int, string> CreateDictionary(List<byte> table)
        {
            Dictionary<int, string> diccionary = new Dictionary<int, string>();
            char[] Values = Encoding.Default.GetChars(table.ToArray());
            int counter = 1;
            int key = default;
            char value = default;

            for (int i = 0; i < Values.Count(); i++)
            {
                if (counter % 2 == 1)
                {
                    if (Values[i] != 44 || Values[i - 1] == 44 && Values[i + 1] == 44) key = Convert.ToByte(Values[i]);
                    else counter++;
                }
                else
                {
                    if (Values[i] != 44 || Values[i - 1] == 44 && Values[i + 1] == 44) value = Values[i];
                    else { diccionary.Add(key, Convert.ToString(value)); counter++; }
                }
            }

            Dictionary<int, string> TST_newDic = new Dictionary<int, string>();
            foreach (var item in diccionary) TST_newDic.Add(item.Key, item.Value);
            return diccionary;
        } // End method create diccionary

        private static int getNumber(List<int> values)
        {
            byte[] numbers = new byte[values.Count];
            for (int i = 0; i < values.Count; i++)
            {
                numbers[i] = Convert.ToByte(values[i]);
            }
            string strg = "";
            foreach (var item in numbers)
            {
                string temp = Convert.ToString(item, 2).PadLeft(8, '0');
                strg += temp;
            }
            int num = Convert.ToInt32(strg, 2);
            return num;
        }// End method get number

        //-------------------------------------------------------------------END PRIVATE FUNCTIONS


    }

}
