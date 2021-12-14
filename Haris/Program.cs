using System;
using System.Threading;

namespace Haris
{
    class Program
    {
        static void Main(string[] args)
        {

            var test = new NonBlockingList<int>();
            Runable run = new Runable();
            run.Start(test, 5);
            run.Runthreds();
            PrintQueueForm(test); 
            int help=93;
            Thread.Sleep(1000);
            test.Delete(help);
            Console.WriteLine($"{help} was removed");
            PrintQueueForm(test);
        }

        private static void PrintQueueForm(NonBlockingList<int> test)
        {
            Console.WriteLine("________________OUTPUT_______________");
            var elem = test.Head.Next;
            for (int i = 0; elem != null; i++)
            {
                Console.WriteLine($"Elem number {i} has {elem.Data}");
                elem = elem.Next;
            }
            Console.WriteLine("________________OUTPUT_______________");
        }
       
    }
}
