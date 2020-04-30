using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LABREPO_ED2.ClassLab1;

namespace LABREPO_ED2.Repository
{

    interface ISodasDataBase
    {
        //conexion the method in class
        List<Soda> GetSodas();

        //conexion the method in class
        void AddNewSoda(string SodaName, Soda newSoda);

        //conexion the method in class
        Soda ReturnMySoda(string SodaName);
    }

}
