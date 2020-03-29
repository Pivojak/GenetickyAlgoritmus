using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesta
{
    class Cesta
    {
        /// <summary>
        /// Jedinečný identifikátor CESTY
        /// </summary>
        public int Id;

        /// <summary>
        /// Kolekce, která obsahuje samotných 35 měst
        /// </summary>
        public List<Mesto> seznamMest;

        /// <summary>
        /// Celková vzdálenost dané souslednosti měst (CESTY)
        /// </summary>
        public float Vzdalenost;

        /// <summary>
        /// Počáteční hodnota rozsahu na vážené ruletě
        /// </summary>
        public int MinRozsah;

        /// <summary>
        /// Koncová hodnota rozsahu na vážené ruletě
        /// </summary>
        public int MaxRozsah;

        /// <summary>
        /// Kontruktor CESTA
        /// </summary>
        /// <param name="list">Kolekce měst, která obsahuje tato CESTA</param>
        /// <param name="id">Identifikátor dané CESTY</param>
        public Cesta(List<Mesto> list,int id)
        {
            Id = id;
            seznamMest = new List<Mesto>();
            seznamMest.AddRange(list);
            
            for (int i = 0; i < 34; i++)
            {
                Mesto prvni = seznamMest[i];
                Mesto druhe = seznamMest[i + 1];
                // Ve vzdalenostech mam uz ulozene vzdalenosti od tohoto mesta ke vsem dalsim, a podle ID si vyberu druhe mesto
                Vzdalenost += prvni.Vzdalenost[druhe.Id];
            }
        }

        /// <summary>
        /// Přetížená metoda pro výpis
        /// </summary>
        /// <returns>Identifikátor města</returns>
        public override string ToString()
        {
            string s = "";
            foreach (Mesto m in seznamMest)
            {
                s += m.Id + "|";
            }

            return string.Format("\n\n{0}",s);
        }

    }
}
