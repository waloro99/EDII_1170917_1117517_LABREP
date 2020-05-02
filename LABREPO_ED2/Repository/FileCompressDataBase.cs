using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LABREPO_ED2.Repository;
using LABREPO_ED2.ClassLab3_4;

namespace LABREPO_ED2.Repository
{
    public class FileCompressDataBase : IFileComprassDataBase
    {
        //My Database
        Stack<FileCompress> lifo = new Stack<FileCompress>();

        public Stack<FileCompress> GetFiles()
        {
            return lifo;
        }

        //method add new soda since interfaz
        public void AddNewFile(string nfo, string pnf, float rc, float fc, float rp, string alg)
        {
            FileCompress n_compress = new FileCompress(nfo, pnf, rc, fc, rp, alg);
            lifo.Push(n_compress);
        }
    }
}
