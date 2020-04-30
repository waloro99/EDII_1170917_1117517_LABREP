using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LABREPO_ED2.ClassLab1
{
    class BTree<Tkey, T> where Tkey : IComparable<Tkey> //specific type comparison to sort your instances
    {

        //-------------------------CLASS KEY AND VALUE TO INSERT----------------------
        private class Value : IEquatable<Value> //equality of instances
        {
            public Tkey key { get; set; }
            public T Value_Val { get; set; }

            // public int line;

            // method builder
            public Value(Tkey newKey, T newValue/*, int newLine*/)
            {
                key = newKey;
                Value_Val = newValue;
                //line = newLine;
            }

            // method Equals
            public bool Equals(Value other)
            {
                return key.Equals(other.key) && Value_Val.Equals(other.Value_Val);
            }

        } // end class Value


        //-------------------------- CLASS NODE  -------------------------------------------
        private class Node
        {
            int Grade; //Know grade the tree
            public LinkedList<Node> Sons { get; set; } //list the sons
            public LinkedList<Value> Entries { get; set; } // list the way*

            //Method builder
            public Node(int grade)
            {
                Sons = new LinkedList<Node>();
                Entries = new LinkedList<Value>();
                Grade = grade;
            }

            // -------------------------------- NODE FUNCTIONS ----------------------------

            //method ask if the node is leaf
            public bool IsLeaf()
            {
                return Sons.Count == 0; //true if you don't have children
            }

            //method that asks if the node is overloaded
            public bool IsOverLoaded()
            {
                return Entries.Count == (Grade); //true if the entries are equal to the grade of the tree
            }

        } // end class Node


        //--------------------------- VARIABLE B TREE -----------------------------
        private Node Root;
        public int Grade;
        public int Height;

        //method builder
        public BTree(int grade)
        {
            Grade = grade;
            Root = new Node(grade);
            Height = 1;
        }


        // ---------------------------- FUNCTIONS INSERT ----------------------------------
        public void Insert(Tkey Newkey, T NewValue/*, int newLine*/)
        {
            Value NewVal = new Value(Newkey, NewValue/*, newLine*/);

            if (!KeyIsAlreadyOnTree(Newkey, Root)) this.Insert(this.Root, NewVal, null);
        } //end method public insert

        private void Insert(Node SubTreeRoot, Value NewVal, Node nodoFather)
        {
            if (Root.Entries.Count == 0)//If is the first element
            {
                Root.Entries.AddLast(NewVal);
                return;
            }


            LinkedListNode<Value> NodeElementPointer = SubTreeRoot.Entries.First;
            LinkedListNode<Node> WayElementPointer = SubTreeRoot.Sons.First;

            this.SearchSpace(ref NodeElementPointer, ref WayElementPointer, NewVal);//changes the ref items until find the closest but smaller than new value items
            if ((WayElementPointer != null) &&
                (NewVal.key.CompareTo(NodeElementPointer.Value.key) > 0))
                WayElementPointer = WayElementPointer.Next;//Moves the Way element pointer to the lastone if the element is the biggest and there are sons

            if (SubTreeRoot.IsLeaf())
            {
                if (NodeElementPointer.Value.key.CompareTo(NewVal.key) > 0) SubTreeRoot.Entries.AddBefore(NodeElementPointer, NewVal);//Inserts the value in the corresponding Leaf
                else SubTreeRoot.Entries.AddAfter(NodeElementPointer, NewVal);//Inserts the value in the corresponding Leaf

            }
            else this.Insert(WayElementPointer.Value, NewVal, SubTreeRoot);//Searches in a recursive way, the leaf to insert

            if (SubTreeRoot.IsOverLoaded()) this.Balance(SubTreeRoot, nodoFather);
        } // end method insert

        private void SearchSpace(ref LinkedListNode<Value> TempListNode, ref LinkedListNode<Node> TemListWay, Value NewVal)
        {
            while (TempListNode.Value.key.CompareTo(NewVal.key) <= 0)//While new value be smaller do
            {
                if (TempListNode.Next != null) TempListNode = TempListNode.Next;
                else break;

                if (TemListWay != null)
                    if (TemListWay.Next != null) TemListWay = TemListWay.Next;
            }
        } //end method search space

        private void Balance(Node OverLoadedNode, Node Father)
        {
            if (Father == null)
            {
                Father = new Node(Grade);
                Root = Father;
            }

            LinkedList<Value> FirstHalf = new LinkedList<Value>();
            LinkedListNode<Value> MiddleValue = new LinkedListNode<Value>(null);
            LinkedList<Value> SecondHalf = new LinkedList<Value>();

            LinkedListNode<Value> NodeElementPointer = Father.Entries.First;
            LinkedListNode<Node> WayElementPointer = Father.Sons.First;


            LinkedList<Node> FirstHalfOfNodes = new LinkedList<Node>();
            LinkedList<Node> SecondHalfOfNodes = new LinkedList<Node>();

            this.DivideNode(ref FirstHalf, ref MiddleValue, ref SecondHalf, ref OverLoadedNode);
            if (Father.Entries.First != null)
            {
                this.SearchSpace(ref NodeElementPointer, ref WayElementPointer, MiddleValue.Value);
                if (NodeElementPointer.Value.key.CompareTo(MiddleValue.Value.key) > 0) Father.Entries.AddBefore(NodeElementPointer, MiddleValue.Value);
                else Father.Entries.AddAfter(NodeElementPointer, MiddleValue.Value);
                Father.Sons.AddAfter(WayElementPointer, new Node(Grade) { Entries = FirstHalf /*, Sons = FirstHalfOfNodes */});
                Father.Sons.AddAfter(WayElementPointer.Next, new Node(Grade) { Entries = SecondHalf /*, Sons = SecondHalfOfNodes */});

                LinkedList<Node> TempList = new LinkedList<Node>();
                //This cicle is to delete the empty overloaded node from Sons list references
                foreach (var item in Father.Sons)
                {
                    if (item != OverLoadedNode)
                    {
                        TempList.AddLast(item);
                    }
                }
                Father.Sons = TempList;

                if (OverLoadedNode.Sons.Count == Grade + 1)
                {
                    SplitSonsForRecursiveInsertion(OverLoadedNode, ref FirstHalfOfNodes, ref SecondHalfOfNodes);
                    WayElementPointer.Next.Value.Sons = FirstHalfOfNodes;
                    WayElementPointer.Next.Next.Value.Sons = SecondHalfOfNodes;
                }
            }
            else
            {
                Height++;
                Father.Entries.AddFirst(MiddleValue.Value);
                Father.Sons.AddLast(new Node(Grade) { Entries = FirstHalf });
                Father.Sons.AddLast(new Node(Grade) { Entries = SecondHalf });
                SplitSons(ref OverLoadedNode, ref Father);
            }
        } //End method Balance

        private void DivideNode(ref LinkedList<Value> FirstHalf, ref LinkedListNode<Value> MiddleValue, ref LinkedList<Value> SecondHalf, ref Node OverLoadedNode)
        {
            //depends on the degree to do the division of the node
            if (IsGradePair())
            {
                for (int i = 0; i < ((Grade - 1) / 2); i++)
                {
                    LinkedListNode<Value> TempNode = OverLoadedNode.Entries.First;
                    OverLoadedNode.Entries.RemoveFirst();
                    FirstHalf.AddLast(TempNode);
                }

                MiddleValue.Value = OverLoadedNode.Entries.First.Value;
                OverLoadedNode.Entries.RemoveFirst();

                for (int i = 0; i < (Grade / 2); i++)
                {
                    LinkedListNode<Value> TempNode = OverLoadedNode.Entries.First;
                    OverLoadedNode.Entries.RemoveFirst();
                    SecondHalf.AddLast(TempNode);
                }
            }
            else
            {
                for (int i = 0; i < (Grade / 2); i++)
                {
                    LinkedListNode<Value> TempNode = OverLoadedNode.Entries.First;
                    OverLoadedNode.Entries.RemoveFirst();
                    FirstHalf.AddLast(TempNode);
                }

                MiddleValue.Value = OverLoadedNode.Entries.First.Value;
                OverLoadedNode.Entries.RemoveFirst();

                for (int i = 0; i < (Grade / 2); i++)
                {
                    LinkedListNode<Value> TempNode = OverLoadedNode.Entries.First;
                    OverLoadedNode.Entries.RemoveFirst();
                    SecondHalf.AddLast(TempNode);
                }
            }
        }//end method divide node

        //Method for split the sons 
        private void SplitSons(ref Node OverLoadedNode, ref Node Father)
        {
            if (OverLoadedNode.Sons.Count > 0)//If overloaded node have sons
            {
                var TempFatherSon = Father.Sons.First;
                if (IsGradePair())
                {
                    for (int i = 0; i < (Grade / 2); i++)
                    {
                        var TempSon = OverLoadedNode.Sons.First;
                        OverLoadedNode.Sons.RemoveFirst();
                        TempFatherSon.Value.Sons.AddLast(TempSon);
                    }
                    TempFatherSon = TempFatherSon.Next;
                    for (int i = 0; i < ((Grade + 2) / 2); i++)
                    {
                        var TempSon = OverLoadedNode.Sons.First;
                        OverLoadedNode.Sons.RemoveFirst();
                        TempFatherSon.Value.Sons.AddLast(TempSon);
                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < (Grade / 2) + 1; j++)
                        {
                            var TempSon = OverLoadedNode.Sons.First;
                            OverLoadedNode.Sons.RemoveFirst();
                            TempFatherSon.Value.Sons.AddLast(TempSon);
                        }
                        TempFatherSon = TempFatherSon.Next;
                    }
                }
            } //else does nothing

        }//end method split sons
        private void SplitSonsForRecursiveInsertion(Node OverLoadedNode, ref LinkedList<Node> FirstHalfofSons, ref LinkedList<Node> SecondHalfofSons)
        {
            //depend the grade tree
            if (IsGradePair())
            {
                for (int i = 1; i <= (Grade / 2); i++)
                {
                    Node tempNode = OverLoadedNode.Sons.First.Value;
                    OverLoadedNode.Sons.RemoveFirst();
                    FirstHalfofSons.AddLast(tempNode);
                }

                for (int i = 1; i <= ((Grade + 2) / 2); i++)
                {
                    Node tempNode = OverLoadedNode.Sons.First.Value;
                    OverLoadedNode.Sons.RemoveFirst();
                    SecondHalfofSons.AddLast(tempNode);
                }
            }
            else
            {
                for (int i = 1; i <= ((Grade + 1) / 2); i++)
                {
                    Node tempNode = OverLoadedNode.Sons.First.Value;
                    OverLoadedNode.Sons.RemoveFirst();
                    FirstHalfofSons.AddLast(tempNode);
                }

                for (int i = 1; i <= ((Grade + 1) / 2); i++)
                {
                    Node tempNode = OverLoadedNode.Sons.First.Value;
                    OverLoadedNode.Sons.RemoveFirst();
                    SecondHalfofSons.AddLast(tempNode);
                }
            }
        }//end method split sons for recursive insertion

        // method to know if the grade of the tree is pair
        private bool IsGradePair()
        {
            return Grade % 2 == 0;
        }//end method grade pair the tree

        //------------------------------Search Functiom ------------------------------------------
        public T GetElement(Tkey ItemKey)
        {
            LinkedListNode<Value> ElementPointer = null;
            FindElemet(Root, ItemKey, ref ElementPointer);
            if (ElementPointer == null) return default;
            else return ElementPointer.Value.Value_Val;
        }//end method get element

        private void FindElemet(Node SubTreeRoot, Tkey ItemKey, ref LinkedListNode<Value> ElementPointer)
        {
            LinkedListNode<Value> ValTempLinkedListNode = SubTreeRoot.Entries.First;
            LinkedListNode<Node> SonTempLinkedListNode = SubTreeRoot.Sons.First;

            if (ValTempLinkedListNode.Value.key.CompareTo(ItemKey) > 0)
            {
                if (!SubTreeRoot.IsLeaf())
                    FindElemet(SubTreeRoot.Sons.First.Value, ItemKey, ref ElementPointer);//If is smaller than the first value
            }
            else if (SubTreeRoot.Entries.Last.Value.key.CompareTo(ItemKey) < 0)
            {
                if (!SubTreeRoot.IsLeaf())
                    FindElemet(SubTreeRoot.Sons.Last.Value, ItemKey, ref ElementPointer);//If is bigger than the last value
            }

            else//If is in the middle
            {
                for (int i = 0; i < SubTreeRoot.Entries.Count; i++)
                {
                    if (ElementPointer != null) break;

                    if (ValTempLinkedListNode.Value.key.CompareTo(ItemKey) == 0) ElementPointer = ValTempLinkedListNode;

                    else if ((ValTempLinkedListNode.Next.Value.key.CompareTo(ItemKey) > 0) && //If ItemKey is smaller than next value
                        (ValTempLinkedListNode.Value.key.CompareTo(ItemKey) < 0) &&//if ItemKey is bigger than actual value
                        !(SubTreeRoot.IsLeaf()))//If this is not a leaf
                    {
                        for (int j = 0; j <= i; j++) SonTempLinkedListNode = SonTempLinkedListNode.Next;
                        FindElemet(SonTempLinkedListNode.Value, ItemKey, ref ElementPointer);
                    }

                    else ValTempLinkedListNode = ValTempLinkedListNode.Next;
                }
            }
        }//end method find element

        //---------------------------- Other Function --------------------------------------------
        private bool KeyIsAlreadyOnTree(Tkey newKey, Node SubTreeRoot)
        {
            //var cont
            int cont = 0;

            foreach (var item in SubTreeRoot.Entries)
            {
                if (item.key.CompareTo(newKey) == 0) return true;
                //        "B"           "A"              /*if the new value is smaller*/
                else if (item.key.CompareTo(newKey) > 0)
                {
                    if (SubTreeRoot.Sons.Count > 0)
                    {
                        LinkedListNode<Node> NextSearch = SubTreeRoot.Sons.First;
                        for (int i = 0; i < cont; i++) NextSearch = NextSearch.Next;
                        return KeyIsAlreadyOnTree(newKey, NextSearch.Value);
                    }
                    else return false;
                }
                else /*If is bigger*/
                {
                    cont++;
                }
            }
            if (SubTreeRoot.Sons.Count > 0) return KeyIsAlreadyOnTree(newKey, SubTreeRoot.Sons.Last.Value);
            else return false;
        }//Untested


        //----Search---------------------------------------------------------------------------------
        public void InOrden(ref List<T> LL)
        {
            Internal_InOrden(Root, ref LL);
        }//end method InOrden

        private void Internal_InOrden(Node Root, ref List<T> LL)
        {
            LinkedListNode<Node> WayPointer = Root.Sons.First;
            LinkedListNode<Value> TempValuePointer = Root.Entries.First;

            if (WayPointer != null) Internal_InOrden(WayPointer.Value, ref LL);// Go to the last sheet and pick it up
            else
            {
                foreach (var item in Root.Entries) LL.Add(item.Value_Val);
                return;
            }

            while (WayPointer.Next != null)
            {
                LL.Add(TempValuePointer.Value.Value_Val);
                TempValuePointer = TempValuePointer.Next;
                WayPointer = WayPointer.Next;
                Internal_InOrden(WayPointer.Value, ref LL);
            }
        }//end method Internal_InOrden


    } //end class BTree
}
