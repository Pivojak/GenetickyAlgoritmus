using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesta
{
    class Mesto
    {
        /// <summary>
        ///  Jedinečný identifikátor daného města
        /// </summary>
        public int Id { get; set; } 

        /// <summary>
        /// Název konkrétního města pro lepší přehlednost při testování
        /// </summary>
        public string Nazev { get; set; }

        /// <summary>
        /// Kolekce, která obsahuje vzdálenosti tohoto města od všech ostatních
        /// </summary>
        public List<float> Vzdalenost { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="id">Identifikátor města</param>
        /// <param name="nazev">Název města</param>
        /// <param name="vzdalenosti">Kolekce všech vzdáleností</param>
        public Mesto(int id, string nazev, float[] vzdalenosti)
        {
            Id = id;
            Nazev = nazev;
            Vzdalenost = new List<float>();
            foreach(float f in vzdalenosti)
            {
                Vzdalenost.Add(f);
            }
        }

        public Mesto()
        {

        }
    }
}
