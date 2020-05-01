using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LABREPO_ED2.ClassLab3_4
{
    public class FileCompress
    {
        //Type Stack 
        public string NameFileOriginal { get; set; }
        public string PathNameFile { get; set; }
        public float RazonCompress { get; set; }
        public float FactorCompress { get; set; }
        public float ReductionPorcent { get; set; }
        public string Algorithm { get; set; }


        //method builder
        public FileCompress(string nfo, string pnf, float rc, float fc, float rp, string alg)
        {
            NameFileOriginal = nfo;
            PathNameFile = pnf;
            RazonCompress = rc;
            FactorCompress = fc;
            ReductionPorcent = rp;
            Algorithm = alg;
        }

    }
}
