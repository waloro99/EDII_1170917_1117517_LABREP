using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LABREPO_ED2.Repository
{
    interface IMethod
    {

        //conexion method in class

        void CipherCesar(string rPath, string wPath, string key);

        void CipherZigZag(string rPath, string wPath, int key);

        void CipherRutaS(string rPath, string wPath, int rows);

        void CipherRutaV(string rPath, string wPath, int rows);

        void DecipherCesar(string rPath, string wPath, string key);

        void DecipherZigZag(string rPath, string wPath, int key);

        void DecipherRutaS(string rPath, string wPath, int key);

        void DecipherRutaV(string rPath, string wPath, int key);

    }
}
