using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LABREPO_ED2.ClassLab5;

namespace LABREPO_ED2.Repository
{
    public class Method : IMethod
    {

        Cesar objC = new Cesar();
        Zigzag objZ = new Zigzag();
        RutaEspiral objR = new RutaEspiral();

        // CIPHER

        //add method for cesar
        public void CipherCesar(string rPath, string wPath, string key)
        {
            objC.Encode(rPath, wPath, key);
        }

        //add method for zigzag
        public void CipherZigZag(string rPath, string wPath, int key)
        {
            objZ.Encode(rPath, wPath, key);
        }

        //add method for ruta spiral
        public void CipherRutaS(string rPath, string wPath, int rows)
        {
            objR.Spiral(rPath, wPath, rows);
        }

        //add method for ruta vertical
        public void CipherRutaV(string rPath, string wPath, int rows)
        {
            objR.Vertical(rPath, wPath, rows);
        }

        //END CIPHER

        //DECIPHER

        //add method for cesar
        public void DecipherCesar(string rPath, string wPath, string key)
        {
            objC.Decode(rPath, wPath, key);
        }

        //add method for zigzag
        public void DecipherZigZag(string rPath, string wPath, int key)
        {
            objZ.Decode(rPath, wPath, key);
        }

        //add method for ruta spiral
        public void DecipherRutaS(string rPath, string wPath, int key)
        {
            objR.DecryptSpiral(rPath, wPath, key);
        }

        //add method for ruta vertical
        public void DecipherRutaV(string rPath, string wPath, int key)
        {
            objR.DecryptVertical(rPath, wPath, key);
        }



        //END DECIPHER
    }
}
