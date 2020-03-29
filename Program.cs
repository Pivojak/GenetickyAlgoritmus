using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace Mesta
{
    class Program
    {
        static void Main(string[] args)
        {
            // Vytvoření instance
            //  - vše potřebné je uvnitř třídy GenetickýAlgoritmus
            GenetickyAlgoritmus genetickyAlgoritmus = new GenetickyAlgoritmus();

            // Čekání na stisk libovolného tlačítka po vykonání celého programu
            Console.ReadKey();
        }
    }
}
