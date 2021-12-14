using System;
using System.Collections.Generic;
using System.Threading;

namespace Haris
{
    public class NonBlockingList<T> where T : IComparable<T>
    {
        public class Node<T>
        {
            public T Data;
            public Node<T> Next;
            public Node(T data)
            {
                Data = data;
            }
        }

        public Node<T> Head;
        private Node<T> Tail;
        private Node<T> Temp;

        public NonBlockingList() {
            Head = new Node<T>(default);
            Tail = new Node<T>(default);
        }

        public void Push(T data)
        {
            var NewNode = new Node<T>(data);
            var LeftNode = new Node<T>(default);
            var RightNode = new Node<T>(default);
            do
            {              
                RightNode = Search(data, ref LeftNode);
                if (RightNode != null && RightNode.Data.CompareTo(data) == 0)
                    return;
                NewNode.Next = RightNode;               
                if (CAS(ref LeftNode.Next, NewNode, RightNode))
                    return;
            } while (true);
        }

        private  Node<T> Search(T val, ref  Node<T> leftNode)
        {
            var RightNode = new Node<T>(default);
            var leftNode_next = new Node<T>(default);
            do
            {
                 var t = Head;
                var tNext = Head.Next;
                do
                {
                    var isMark = t.Data.CompareTo(val);
                    if (!(isMark >= 0))
                    {
                        leftNode = t;
                        leftNode_next = leftNode.Next;
                    }
                    else {
                        break;
                    }                    
               
                    t = leftNode_next;
                    if (t == Tail) break;                   


                } while (leftNode_next != null);
                RightNode = t;

                if (leftNode == null && CAS(ref RightNode, Head, Head))
                {
                    return RightNode;
                }

                if (leftNode.Next == RightNode)
                {
                    return RightNode;
                }

                if (CAS(ref leftNode.Next, RightNode, leftNode_next))
                {
                    return RightNode;
                }
            } while (true);
        }

        public void Delete(T data)
        {
            try
            {
                var RightNode = new Node<T>(default);
                var LeftNode = new Node<T>(default);
                var RightNodeNext = new Node<T>(default);

                RightNode = Search(data, ref LeftNode);
                if (RightNode.Data.CompareTo(data) != 0)
                    return;
                RightNodeNext = RightNode;
                if (LeftNode == null && RightNode.Data.CompareTo(data) == 0)
                {
                    do
                    {
                        Temp = RightNode;
                    } while (!CAS(ref RightNode, RightNode.Next, Temp));
                    Head = RightNode;
                    return;
                }
                do
                {
                    Temp = RightNode;
                } while (!CAS(ref RightNode, RightNode.Next, Temp));
                LeftNode.Next = RightNode;
                if (RightNode.Next == null)
                {
                    Tail = LeftNode;
                }
            }catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }
        }  

     
        private bool CAS(ref  Node<T> compare, Node<T> swapVal,  Node<T> compareVal)
        {
            return Interlocked.CompareExchange< Node<T>>(ref compare, swapVal, compareVal) == compareVal;
        }

    }
}