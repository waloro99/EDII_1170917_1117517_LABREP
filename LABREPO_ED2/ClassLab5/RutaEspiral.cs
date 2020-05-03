using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace LABREPO_ED2.ClassLab5
{
    public class RutaEspiral
    {
        //PUBLIC FUNCTIONS

        public void Spiral(string rpath, string wpath, int rows)
        {
            int columns = 0;
            char[,] matrizc;
            using (var file = new FileStream(wpath, FileMode.OpenOrCreate))
            {
                using (var writer = new BinaryWriter(file))
                {
                    matrizc = fell(rpath, rows, ref columns);
                    int i, k = 0, l = 0;
                    while (k < rows && l < columns)
                    {
                        for (i = l; i < columns; ++i)
                        {
                            writer.Write(matrizc[k, i]);
                        }
                        k++;
                        for (i = k; i < rows; ++i)
                        {
                            writer.Write(matrizc[i, columns - 1]);
                        }
                        columns--;
                        if (k < rows)
                        {
                            for (i = columns - 1; i >= l; --i)
                            {
                                writer.Write(matrizc[rows - 1, i]);
                            }
                            rows--;
                        }
                        if (l < columns)
                        {
                            for (i = rows - 1; i >= k; --i)
                            {
                                writer.Write(matrizc[i, l]);
                            }
                            l++;
                        }
                    }
                }
            }
        }//End method for spiral

        public void Vertical(string rpath, string wpath, int rows)
        {
            int columns = 0;
            using (var file = new FileStream(wpath, FileMode.OpenOrCreate))
            {
                using (var writer = new BinaryWriter(file))
                {
                    char[,] matrizc = fell(rpath, rows, ref columns);
                    int i, k = 0, l = 0;
                    int cnt = 0;
                    int total = rows * columns;
                    while (k < rows && l < columns)
                    {
                        if (cnt == total)
                            break;
                        for (i = k; i < rows; ++i)
                        {
                            writer.Write(matrizc[i, l]);
                        }
                        l++;
                        if (cnt == total)
                            break;
                        for (i = l; i < columns; ++i)
                        {
                            writer.Write(matrizc[rows - 1, i]);
                        }
                        rows--;
                        if (cnt == total)
                            break;
                        if (k < rows)
                        {
                            for (i = rows - 1; i >= k; --i)
                            {
                                writer.Write(matrizc[i, columns - 1]);
                            }
                            columns--;
                        }
                        if (cnt == total)
                            break;
                        if (l < columns)
                        {
                            for (i = columns - 1; i >= l; --i)
                            {
                                writer.Write(matrizc[k, i]);
                            }
                            k++;
                        }
                    }
                }
            }
        }//End method for vertical

        public void DecryptSpiral(string rPath, string wPath, int key)
        {
            using (var file = new FileStream(@wPath, FileMode.OpenOrCreate))
            {
                using (var writer = new BinaryWriter(file))
                {
                    char[,] data = FVertical(rPath, key);//FSpiral
                    int rows = data.Length / key;
                    int columns = key;
                    for (int i = 0; i < columns; i++)
                    {
                        for (int j = 0; j < rows; j++)
                        {
                            if (data[j, i] != '°')
                            {
                                writer.Write(data[j, i]);
                            }
                        }
                    }
                }
            }
        }//End method for decrypt spiral


        public void DecryptVertical(string rPath, string wPath, int key)
        {
            using (var file = new FileStream(@wPath, FileMode.OpenOrCreate))
            {
                using (var writer = new BinaryWriter(file))
                {
                    char[,] data = FSpiral(rPath, key); //FVertical
                    int rows = data.Length / key;
                    int columns = key;
                    for (int i = 0; i < columns; i++)
                    {
                        for (int j = 0; j < rows; j++)
                        {
                            if (data[j, i] != '°')
                            {
                                writer.Write(data[j, i]);
                            }
                        }
                    }
                }
            }
        }//End method for decrypt vertical


        //END PUBLIC FUNCTIONS


        //PRIVATE FUNCTIONS

        private char[,] fell(string rPath, int rows, ref int columns)
        {
            char[,] text;
            FileInfo n1 = new FileInfo(rPath);
            int aux = Convert.ToInt32(n1.Length);
            using (var file = new FileStream(rPath, FileMode.Open))
            using (BinaryReader br = new BinaryReader(file))
            {
                columns = Convert.ToInt32(Math.Ceiling((double)file.Length / rows));
                text = new char[rows, columns];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (br.BaseStream.Position != br.BaseStream.Length)
                        {
                            text[i, j] = br.ReadChar();
                        }
                        else
                        {
                            text[i, j] = '°';
                        }
                    }

                }
            }
            return text;
        }//End method for fell the array


        private char[,] FVertical(string rPath, int rows)
        {
            char[,] mander;
            int columns;
            using (var file = new FileStream(@rPath, FileMode.Open))
            using (BinaryReader br = new BinaryReader(file))
            {
                columns = rows;
                rows = Convert.ToInt32(Math.Floor((double)file.Length / rows));
                mander = new char[rows, columns];
                int i, k = 0, l = 0;
                int cnt = 0;
                int total = rows * columns;
                while (k < rows && l < columns)
                {
                    if (cnt == total)
                        break;
                    for (i = k; i < rows; ++i)
                    {
                        if (br.BaseStream.Position != br.BaseStream.Length)
                        {
                            mander[i, l] = br.ReadChar();
                        }
                    }
                    l++;
                    if (cnt == total)
                        break;
                    for (i = l; i < columns; ++i)
                    {
                        if (br.BaseStream.Position != br.BaseStream.Length)
                        {
                            mander[rows - 1, i] = br.ReadChar();
                        }
                    }
                    rows--;
                    if (cnt == total)
                        break;
                    if (k < rows)
                    {
                        for (i = rows - 1; i >= k; --i)
                        {
                            if (br.BaseStream.Position != br.BaseStream.Length)
                            {
                                mander[i, columns - 1] = br.ReadChar();
                            }

                        }
                        columns--;
                    }
                    if (cnt == total)
                        break;
                    if (l < columns)
                    {
                        for (i = columns - 1; i >= l; --i)
                        {
                            if (br.BaseStream.Position != br.BaseStream.Length)
                            {
                                mander[k, i] = br.ReadChar();
                            }
                        }
                        k++;
                    }
                }

            }
            return mander;
        }//End method for fell vertical in array

        private char[,] FSpiral(string rPath, int rows)
        {
            char[,] mander;
            int columns;
            using (var file = new FileStream(@rPath, FileMode.Open))
            using (BinaryReader br = new BinaryReader(file))
            {
                columns = rows;
                rows = Convert.ToInt32(Math.Floor((double)file.Length / rows));
                mander = new char[rows, columns];
                int i, k = 0, l = 0;
                while (k < rows && l < columns)
                {
                    for (i = l; i < columns; ++i)
                    {
                        if (br.BaseStream.Position != br.BaseStream.Length)
                        {
                            mander[k, i] = br.ReadChar();
                        }
                    }
                    k++;
                    for (i = k; i < rows; ++i)
                    {
                        if (br.BaseStream.Position != br.BaseStream.Length)
                            mander[i, columns - 1] = br.ReadChar();
                    }
                    columns--;
                    if (k < rows)
                    {
                        for (i = columns - 1; i >= l; --i)
                        {
                            if (br.BaseStream.Position != br.BaseStream.Length)
                                mander[rows - 1, i] = br.ReadChar();
                        }
                        rows--;
                    }
                    if (l < columns)
                    {
                        for (i = rows - 1; i >= k; --i)
                        {
                            if (br.BaseStream.Position != br.BaseStream.Length)
                                mander[i, l] = br.ReadChar();
                        }
                        l++;
                    }
                }

            }
            return mander;
        }//End method for feel spiral in array

        //END PRIVATE FUNCTIONS
    }

}
