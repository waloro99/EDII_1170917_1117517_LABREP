using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LABREPO_ED2.ClassLab1;

namespace LABREPO_ED2.Repository
{
        public class SodasDataBase : ISodasDataBase
        {
            //My Database
            BTree<String, Soda> myTree = new BTree<String, Soda>(5); //instance class btree

            //method return all sodas
            public List<Soda> GetSodas()
            {
                List<Soda> rtrnList = new List<Soda>();
                myTree.InOrden(ref rtrnList);
                return rtrnList;
            }

            //method add new soda since interfaz
            public void AddNewSoda(string SodaName, Soda newSoda)
            {
                myTree.Insert(SodaName, newSoda);
            }

            //method return soda since interfaz
            public Soda ReturnMySoda(string SodaName)
            {
                return myTree.GetElement(SodaName);
            }
        }

  
}
