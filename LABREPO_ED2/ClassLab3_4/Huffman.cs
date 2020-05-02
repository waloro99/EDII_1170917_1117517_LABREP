using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace LABREPO_ED2.ClassLab3_4
{
    public class Huffman
    {
        //Internal Class
        internal class Node : IComparable<Node>
        {
            public char? Value { get; set; } //value the character
            public double Probability { get; set; } //frecuency / total character
            public Node RightSon { get; set; } //right son
            public Node LeftSon { get; set; } //left son

            //method builder
            public Node(char? v, double p)
            {
                Value = v;
                Probability = p;
            }

            //compare probability the two nodes
            public int CompareTo(Node other)
            {
                if (this.Probability > other.Probability) return 1;
                else if (this.Probability < other.Probability) return -1;
                else
                {
                    if ((int)this.Value > (int)other.Value) return 1;
                    else if ((int)this.Value > (int)other.Value) return -1;
                    else return 0;
                }
            }
        }
        //End internal class


        //------------------------------------------ COMPRESS -----------------------------------------------
        //Public Functions

        //method for compress file
        public void Compress
            (string rPath, string wPath)
        {
            int BitsToRead = 0;
            double charcount = 0.0;
            List<Node> CharactersProbabilites;
            using (FileStream FileToRead = new FileStream(rPath, FileMode.Open))
            using (BinaryReader BR = new BinaryReader(FileToRead))
            {
                CharactersProbabilites = GetProbabilities(BR, ref charcount)
                    .OrderBy(order => order.Probability).ThenBy(order => order.Value).ToList();
            }

            Dictionary<char, byte[]> PrefixCodes = Dic_Huffman(CharactersProbabilites.ToList());/*Ask why afther this method the list changes*/
            BitsToRead = CalculateBitsToRead(charcount, PrefixCodes, CharactersProbabilites);

            using (FileStream FileToRead = new FileStream(rPath, FileMode.Open))
            using (FileStream FileToWrite = new FileStream(wPath, FileMode.CreateNew))
            using (BinaryReader BR = new BinaryReader(FileToRead))
            using (BinaryWriter BW = new BinaryWriter(FileToWrite))
            {
                LookForPrefix(PrefixCodes);
                TranslateFile(BR, BW, PrefixCodes, CharactersProbabilites, BitsToRead, charcount);
            }
        }

        //method to know the value the rc,fc and pr
        public string GetFilesMetrics
            (string Name, string Original, string Compresed)
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
        }
        //END Public Functions


        //Private Functions compress

        //method to know probabilities
        private List<Node> GetProbabilities
            (BinaryReader br, ref double charcount)
        {
            int bufferSize = 1024; //1kb = heavy
            byte[] Buff; //buffer

            Dictionary<char, int> Characters = new Dictionary<char, int>(); //asociative array
            List<Node> RtrnLst = new List<Node>();

            //while used to read all the file
            while (br.BaseStream.Position != br.BaseStream.Length) //Returns the underlying sequence
            {
                Buff = br.ReadBytes(bufferSize);
                foreach (char item in Buff)
                {
                    if (Characters.ContainsKey(item)) Characters[item]++;
                    else Characters.Add(item, 1);
                    charcount++;
                }
            }

            //Fills the probability list
            foreach (var item in Characters) RtrnLst.Add(new Node(item.Key, item.Value / charcount));

            br.Close();
            return RtrnLst;
        }

        //method for  dictionary
        private Dictionary<char, byte[]> Dic_Huffman
            (List<Node> CharProb)
        {
            while (CharProb.Count > 1)//while there is more than one node on the list
            {
                Node Aux = new Node(null, CharProb[0].Probability + CharProb[1].Probability);
                Aux.LeftSon = CharProb[0];
                Aux.RightSon = CharProb[1];
                CharProb.RemoveAt(0);
                CharProb.RemoveAt(0);

                CharProb.Insert(0, Aux);
                CharProb = CharProb.OrderBy(order => order.Probability).ToList();
            }
            return GetPrefixCodes(CharProb[0]);
        }

        //method for get prefix codes
        private Dictionary<char, byte[]> GetPrefixCodes
           (Node Root)
        {
            Dictionary<char, byte[]> PCs = new Dictionary<char, byte[]>();
            PrefixCodTransversal(Root, ref PCs, new List<byte>());
            return PCs;
        }

        //method to know prefix code transversal
        private void PrefixCodTransversal
            (Node Root, ref Dictionary<char, byte[]> PCs, List<byte> PrefixCode)
        {

            if (Root.Value != null) PCs.Add((char)Root.Value, PrefixCode.ToArray());
            if (Root.LeftSon != null)
            {
                PrefixCode.Add(0);
                PrefixCodTransversal(Root.LeftSon, ref PCs, PrefixCode);
                PrefixCode.RemoveAt(PrefixCode.Count() - 1);
            }
            if (Root.RightSon != null)
            {
                PrefixCode.Add(1);
                PrefixCodTransversal(Root.RightSon, ref PCs, PrefixCode);
                PrefixCode.RemoveAt(PrefixCode.Count() - 1);
            }
        }

        //method for calculate bits to read
        private int CalculateBitsToRead
           (double charcount, Dictionary<char, byte[]> PrefixCodes, List<Node> CharactersProbabilites)
        {
            float bits = 0;
            foreach (var item in CharactersProbabilites) bits += (int)(charcount * item.Probability) * PrefixCodes[(char)item.Value].Count();
            return (int)bits;
        }

        //method for transalate file 
        private void TranslateFile
            (BinaryReader br, BinaryWriter bw, Dictionary<char, byte[]> PrefixCodes, List<Node> CharacterProbabilities, int BitsToRead, double charcount)
        {
            bw.Write(Encoding.Default.GetBytes(BitsToRead.ToString() + "," + charcount + "\r\n"));
            foreach (var item in CharacterProbabilities) bw.Write(Encoding.Default.GetBytes(item.Value + "," + (item.Probability * charcount) + ","));
            bw.Write(Encoding.Default.GetBytes("EOD"));

            int TEST_BitsCont = 0;
            char ActChar;
            string sbite = "";
            List<byte> binarybyte = new List<byte>();

            while (br.BaseStream.Position != br.BaseStream.Length)//while used to read all the file while writing the compressed file
            {
                ActChar = (char)br.ReadByte();
                foreach (var item in PrefixCodes[ActChar]) binarybyte.Add(item);

                while (binarybyte.Count() >= 8)
                {
                    TEST_BitsCont += 8;
                    byte[] NewCharacter = binarybyte.Take<byte>(8).ToArray();
                    foreach (var item in NewCharacter) sbite += (int)item;
                    bw.Write(Convert.ToByte(sbite, 2));
                    for (int i = 0; i < 8; i++) binarybyte.RemoveAt(0);
                    sbite = "";
                }//Writes the filled bytes
            }

            if (binarybyte.Count != 0)
            {
                while (binarybyte.Count < 8) binarybyte.Add(0);
                foreach (var item in binarybyte) sbite += (int)item;
                bw.Write(Convert.ToByte(sbite, 2));
            }
            br.Close();
            bw.Close();
        }

        //method look for prefix
        private void LookForPrefix
            (Dictionary<char, byte[]> DIC)
        {
            bool ItsOk = true;
            foreach (var item in DIC)
                foreach (var item2 in DIC)
                {
                    ItsOk = !CheckIfBIsPrefixOfA(item.Value.ToList(), item2.Value.ToList());
                    if (!ItsOk)
                    {
                        int THereisanerror = 1;
                    }
                }
        }

        //method to check if bits prefix 
        private bool CheckIfBIsPrefixOfA
            (List<byte> MotherList, List<byte> ChildList)
        {

            int MotherElements = MotherList.Count() - 1;
            int ChildElements = ChildList.Count() - 1;
            if (MotherElements < ChildElements) return false;
            bool Answer = true;

            for (int i = 0; i <= ChildElements; i++) if (!(MotherList[i] == ChildList[i] && Answer)) return false;
            return true;
        }
        //END Private Functions compress

        //----------------------------------------- UNCOMPRESS -----------------------------------------
        //method for uncompress file
        public string uncompress(string path)
        {
            string fullfile = "";
            Dictionary<char, Byte[]> dictionary = new Dictionary<char, Byte[]>();
            string text = "";
            using (var file = new FileStream(path, FileMode.Open))
            using (BinaryReader br = new BinaryReader(file))
            {
                List<Byte> code = new List<byte>();
                int bufferSize = 100;
                List<Byte> bufferStop = Encoding.ASCII.GetBytes("EOD").ToList<Byte>();
                List<Byte> Buff;
                List<Byte> tempTable = new List<Byte>();
                int count = 0;
                while (br.BaseStream.Position != br.BaseStream.Length)
                {
                    Buff = br.ReadBytes(bufferSize).ToList<Byte>();
                    tempTable.AddRange(Buff);
                    if (dictionary.Count == 0)
                    {
                        if (CheckEndDIccionary(tempTable, bufferStop, ref count))
                        {
                            GetProbabilities(tempTable.GetRange(0, count), ref dictionary);
                            tempTable.RemoveRange(0, count + 3);
                        }
                    }
                    if (dictionary.Count() > 0)
                    {
                        resetfile(dictionary, ref tempTable, ref text, ref code);
                        tempTable = new List<byte>();
                        fullfile += text;
                        text = "";
                    }
                }

            }

            return fullfile;
        }


        // Private Functions Uncompress
        //Method  to check end diccionary
        private bool CheckEndDIccionary(List<Byte> MotherList, List<Byte> ChildList, ref int count)
        {
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
        }

        //method get probabilities 
        private void GetProbabilities(List<Byte> table, ref Dictionary<char, Byte[]> dictionary)
        {
            int numByte = 0;
            int numCharacter = 0;
            getdata(ref table, ref numByte, ref numCharacter);
            List<Node> Frecuency = getFrecuency(ref table, numCharacter);
            dictionary = Dic_Huffman(Frecuency);
        }

        //method to get data
        private void getdata(ref List<Byte> table, ref int numByte, ref int numCharacter)
        {
            //remove the data of number of bytes and number of characters that the file originally contained
            char[] info = Encoding.Default.GetChars(table.ToArray());
            string data = "";
            for (int i = 0; i < info.Length; i++)
            {
                if (info[i] == '\n')
                {
                    char[] temporal = Encoding.Default.GetChars(table.GetRange(0, i + 1).ToArray());
                    for (int j = 0; j < temporal.Length - 2; j++)
                    {
                        data += Convert.ToString(temporal[j]);
                    }
                    table.RemoveRange(0, i + 1);
                    break;
                }
            }
            string[] metrics = data.Split(',');
            numByte = Convert.ToInt32(metrics[0]);
            numCharacter = Convert.ToInt32(metrics[1]);
        }

        //method to get frecuency
        private List<Node> getFrecuency(ref List<Byte> table, int numCharacter)
        {
            char[] Values = Encoding.Default.GetChars(table.ToArray());
            char key = default;
            List<double> frecuent = new List<double>();
            List<Node> value = new List<Node>();
            int counter = 1;
            for (int i = 0; i < Values.Count(); i++)
            {
                if (counter % 2 == 1)
                {
                    if (Values[i] != 44 || Values[i - 1] == 44 && Values[i + 1] == 44)
                        key = Values[i];
                    else
                        counter++;
                }
                else
                {
                    if (Values[i] != 44)
                        frecuent.Add(Convert.ToDouble(Convert.ToString(Values[i])));
                    else
                    {
                        string data = "";
                        foreach (var item in frecuent)
                        {
                            data += item;
                        }
                        double frecuents = Convert.ToDouble(data) / numCharacter;
                        Node obj = new Node(key, frecuents);
                        value.Add(obj);
                        key = default;
                        frecuent = new List<double>();
                        counter++;
                    }
                }
            }
            return value.OrderBy(order => order.Probability).ThenBy(order => order.Value).ToList();
        }

        //method for reset file
        private void resetfile(Dictionary<char, Byte[]> table, ref List<Byte> text, ref string data, ref List<Byte> code)
        {
            char[] arraychar;
            Byte[] prefix = default;
            int index = 0;
            foreach (var item in text)
            {
                string info = Convert.ToString(item, 2).PadLeft(8, '0');
                arraychar = info.ToCharArray();
                foreach (var item2 in arraychar)
                {
                    if (item2 == 0 || item2 == 48)
                        code.Add(0);
                    else
                        code.Add(1);
                }
            }
            for (int i = 0; i <= code.Count(); i++)
            {
                prefix = code.GetRange(index, i - index).ToArray();
                foreach (var item in table)
                {
                    if (prefix.SequenceEqual(item.Value))
                    {
                        data += item.Key;
                        index = i;
                        break;
                    }
                }
            }
            code = prefix.ToList();
        }

        //END PRIVATE FUNCTION UNCOMPRESS

    }

}
