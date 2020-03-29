using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mesta
{
    /// <summary>
    /// Třída, která obsahuje kompletní řešení pro genetický algoritmus. Stačí vytvořit instanci, jelikož vše potřebné se umístěné v konstruktoru
    /// </summary>
    class GenetickyAlgoritmus
    {
        // Pole ve kterém jsou uloženy vzdálenosti jednotlivých měst, získaných ze souboru EXCEL
        private float[,] mapaVzdalenosti;
        // Kolekce pro uchování 35 měst
        private List<Mesto> mesta;
        // Kolekce pro uložení jedné generace algoritmu
        public List<Cesta> Generace { get; set; }
        // Proměnná ve které je uložená poslední souslednost 35 měst, v tomto programu nazývané jako CESTA
        private Cesta posledniCesta;
        // Proměnná pro uložení instance random
        private Random random;
        // Identifikátor cesty
        private int IdCesta = 0;
        // Identifikátor generace
        private int IdGenerace = 0;
        private Cesta nejlepsiCesta;
        // Počet potomků v dané generaci, myšleno počet CEST
        private const int PocetPotomku = 500;

        // Konstruktor ve kterém je vše potřebné zakomponováno, proto postačí vytvoření instance třídy
        public GenetickyAlgoritmus()
        {
            mapaVzdalenosti = new float[35, 35];
            mesta = new List<Mesto>();
            Generace = new List<Cesta>();
            random = new Random();
            // 
            string[] nazvy = DefinujMapuVzdalenosti();
            // Definuji si zakladni seznam mest, podle puvodniho poradi
            for(int i = 0; i < 35; i++)
            {
                mesta.Add(new Mesto(i, nazvy[i], SpocitejVzdalenosti(i)));
            }
            // Vytvoření základní generace, se kterou se dále prováději kroky mutace, křížení a ohodnocení 
            VytvorZakladniGeneraci(PocetPotomku);
            // Pocet vytvorenych generaci 
            VytvorNovouGeneraci(1000);
            Console.WriteLine(" ------------------------------ Nejlepsi nalezena cesta ------------------------------ ");
            int ind = 0;
            // Zobrazení nalezeného nejlepšího řešení
            foreach (Mesto m in nejlepsiCesta.seznamMest)
            {
                ind++;
                Console.Write("|" + m.Nazev + "\t\t" + m.Id + "|");
                if (ind % 4 == 0)
                    Console.Write("\n");
            }
            Console.Write("\n Vzdalenost teto cesty je: {0} km", nejlepsiCesta.Vzdalenost);

            // Vypis vsech prvku v posledni generaci, pozdeji smazat. Pouze pro ucely testovani
            foreach(Cesta c in Generace)
            {
                Cesta puvodni = c;
                int pocetShody = 0;
                foreach(Cesta ces in Generace)
                {
                    if (c.Vzdalenost == ces.Vzdalenost)
                        pocetShody++;
                }
                Console.WriteLine("{0} {1}", pocetShody, puvodni.Vzdalenost);
                
            }
        }

        /// <summary>
        /// Metoda, která združuje všechny komponenty genetického algoritmu.
        /// </summary>
        /// <param name="pocet">Počet generací který chci vytvořit</param>
        public void VytvorNovouGeneraci(int pocet)
        {
            // Poměr genů od rodičů. Mění se průběžně, aby nebyl po celou dobu programu nastaven stabilně na nějakou hodnotu
            int[] deleniRodicu = { 5, 10, 15, 20, 25 };
            int indexPomeru = 0;
            for (int i = 0; i < pocet; i++)
            {
                // Fitnes funkce
                VypocitejKvalitu();
                // Křížení na základě poměru výše
                VytvorPotomky(deleniRodicu[indexPomeru],35-deleniRodicu[indexPomeru]);
                Mutace();
                if(i%10 == 0 && i > 0)
                {
                    indexPomeru++;
                    if (indexPomeru == 5)
                        indexPomeru = 0;
                }
            }
        }

        /// <summary>
        /// Náhodná mutace členů generace.
        /// </summary>
        public void Mutace()
        {
            int cisloPrvku = 0;
            // Projdu všechny Cesty v generaci
            foreach (Cesta c in Generace)
            {
                for (int i = 0; i < 35; i++)
                {
                    cisloPrvku++;
                    // Zbytek po dělení 400 je roven 0, tedy jedná se o 400 prvek (Město) 
                    // Jedná se o 0.5 % měst. Děleno 400, nikoliv 200 protože při mutaci měním dva prvky, mutovaný zaměňuji s jiným
                    if (cisloPrvku%400 == 0)
                    {
                        // Náhodně vygenerovaná hodnota města od 0 do 34, jedná se o město, kterým bude zaměněn 400. prvek
                        int indexNove = random.Next(35);
                        // Kontrola zda mutace i náhodné číslo neukazují na ten samý prvek
                        if (indexNove == i)
                        {
                            // Generuji do té doby, dokud se nejedná o odlišné ID od mutovaného
                            while (true)
                            {
                                indexNove = random.Next(35);
                                if (indexNove != i)
                                    break;
                            }
                        }
                        Mesto zaloha = c.seznamMest[i];
                        Mesto nove = c.seznamMest[indexNove];

                        // Prohození měst
                        c.seznamMest[i] = nove;
                        c.seznamMest[indexNove] = zaloha;
                    }
                    
                }

            }
            // Zobrazení inforamací do konzole
            VykresliInfo(IdGenerace, 2);
        }
        /// <summary>
        /// Metoda, která generuje nové potomky (novou generaci), myšleno KŘÍŽENÍ v genetickém algoritmu
        /// </summary>
        /// <param name="velikostZacatku">Velikost genu od rodiče 1</param>
        /// <param name="velikostKonce">Velikost genu od rodiče 2</param>
        public void VytvorPotomky(int velikostZacatku, int velikostKonce)
        {
            IdCesta = 0;
            List<Cesta> zaloha = Generace;
            Generace = new List<Cesta>();
            for(int i = 0; i < zaloha.Count- 1; i += 2)
            {
                // Vytvoření nové kolekce pro uložení potomků 1
                List<Mesto> novyPotomekA = new List<Mesto>();
                // Vytvoření nové kolekce pro uložení potomků 2
                List<Mesto> novyPotomekB = new List<Mesto>();
                // Například pro poměr rodičA 15 : rodičB 20
                // Nový potomek dostane celou cestu od rodiče A
                novyPotomekA.AddRange(zaloha[i].seznamMest);
                // Odeberu zadní část CESTY z rodičeA a zůstane prvních 15 měst od rodičeA
                novyPotomekA.RemoveRange(velikostZacatku, velikostKonce);
                // Přidám celý rozsah rodičeB
                novyPotomekA.AddRange(zaloha[i + 1].seznamMest);
                // Z přidané části rodičeB odeberu přední část a zůstane 35 měst, které tvoří nového potomka na základě kombinace rodičů
                novyPotomekA.RemoveRange(velikostZacatku, velikostZacatku);

                // Stejný postup jako výše, akorát v opačném poměru, protože tento potomek bude dědit více od prvního rodiče (20 : 15 např.)
                novyPotomekB.AddRange(zaloha[i + 1].seznamMest);
                novyPotomekB.RemoveRange(velikostZacatku, velikostKonce);
                novyPotomekB.AddRange(zaloha[i].seznamMest);
                novyPotomekB.RemoveRange(velikostZacatku, velikostZacatku);

                List<int> duplicitaA = new List<int>();
                List<int> duplicitaB = new List<int>();
                List<int> duplicitaA_hodnota= new List<int>();
                List<int> duplicitaB_hodnota = new List<int>();
                int shodnost = 0;
                int indexPrvniDuplicita = 0;
                // Procházím potomka zda neobsahuje nějaké MESTO vícekrát
                // Křížením vznikají duplicity, které je nutné odstranit
                for(int l = 0; l < 35; l++)
                {
                    for(int k = 0; k < 35; k++)
                    {
                        // Procházím všechna města (0 až 34) v potomkuA(CESTA) a porovnávám je hodnotami l, které je 0,1,2,3,4 atd
                        // Pokud naleznu dané ID mezi městy, uložím si pozici. Jakmile najdu podruhé dané ID, pozici získanou jako první si uložím
                        // do duplicit, aby bylo poté možno duplicity zaměnit mezi dvěma potomky
                        if (l == novyPotomekA[k].Id)
                        {
                            shodnost++;

                            // Nalezena první shoda, uložení pozice města
                            if (shodnost == 1)
                                indexPrvniDuplicita = k;
                            // Nalezena druhá shoda, jedná se o duplicitu, uložení pozice do duplicit pro další úpravy
                            if(shodnost == 2)
                            {
                                // Pozice v poli duplicitiního města
                                duplicitaA.Add(indexPrvniDuplicita);
                                shodnost = 0;
                                indexPrvniDuplicita = 0;

                                duplicitaA_hodnota.Add(novyPotomekA[indexPrvniDuplicita].Id);
                                break;
                            }
                        }
                    }
                    shodnost = 0;
                    indexPrvniDuplicita = 0;
                }
                // Stejná kontrola jako u potomkaA popsána výše
                for (int l = 0; l < 35; l++)
                {
                    for (int k = 0; k < 35; k++)
                    {
                        if (l == novyPotomekB[k].Id)
                        {
                            shodnost++;
                            if (shodnost == 1)
                                indexPrvniDuplicita = k;
                            if (shodnost == 2)
                            {
                                duplicitaB.Add(indexPrvniDuplicita);
                                shodnost = 0;
                                indexPrvniDuplicita = 0;

                                duplicitaB_hodnota.Add(novyPotomekB[indexPrvniDuplicita].Id);
                                break;
                            }
                        }
                    }
                    shodnost = 0;
                    indexPrvniDuplicita = 0;
                }
                // Zálohování kolekce
                List<Mesto> zalohaDuplicita = new List<Mesto>();
                // Město na pozici, kterou udává Duplicita je uloženo do kolekce zalohaDuplicita, aby bylo možné tyto města umístit poté do
                // druhého potomka
                for(int p = 0; p < duplicitaA.Count; p++)
                {
                    zalohaDuplicita.Add(novyPotomekA[duplicitaA[p]]);
                }
                
                // Na pozici duplicity v A uložím město které se nachází na pozici duplicityB
                for (int k = 0; k < duplicitaA.Count; k++)
                {
                    novyPotomekA[duplicitaA[k]] = novyPotomekB[duplicitaB[k]];
                }
                // Ze zálohy jsou uloženy města do potomkaB
                for (int k = 0; k < zalohaDuplicita.Count; k++)
                {
                    novyPotomekB[duplicitaB[k]] = zalohaDuplicita[k];
                }
                // Do kolekce Generace uložím 1. potomka a zvýším IDpotomka
                Generace.Add(new Cesta(novyPotomekA, IdCesta));
                IdCesta++;
                // Do kolekce Generace uložím 2. potomka a zvýším IDpotomka
                Generace.Add(new Cesta(novyPotomekB, IdCesta));
                IdCesta++;
            }
            // Zvýším id generace
            IdGenerace++;
            // Zobrazím inforamce o právě probíhaném kroku do konzole
            VykresliInfo(IdGenerace,1);
        }
        /// <summary>
        /// Fitnes funkce pro genetický algoritmus. Ohodnotím potomky a poté roztočím váženou ruletu, která preferuje jedince s kratší vzdáleností.
        /// </summary>
        public void VypocitejKvalitu()
        {
            IdCesta = 0;
            double CelkovaVzdalenost = 0;
            // Instance randomu, pro náhodný výběr
            Random random = new Random();
            List<Cesta> UpravenaGenerace = new List<Cesta>();
            // Projdu všechny potomky a sečtu jejich vzdálenosti, abych měl celkovou vzdálenost všech potomků pro vytvoření poměru
            foreach(Cesta c in Generace)
                CelkovaVzdalenost += (1/c.Vzdalenost);
            // Proměnná do která se postupně zvyšuje, tak aby bylo možné stanovit spodní hranici daného potomka na ruletě
            double Vzdalenost = 0;
            // Projdu všechny potomky a každému spočítám spodní a horní hranici. Tedy úsek, který zabírá z celkové vzdálenosti
            foreach (Cesta c in Generace)
            {
                // Násobek je pouze pro to, aby byla dosažena dostatečná velikost každého prvku
                if (Vzdalenost != 0)
                    c.MinRozsah = (int)Math.Floor((Vzdalenost / CelkovaVzdalenost) * 5000);
                else
                    c.MinRozsah = 0;
                // 1/vzdálenost, abych dosáhl toho, že nejlepší potomci (nejkratší vzdálenosti) zabírají více než ty s větší vzdáleností
                Vzdalenost += (1/c.Vzdalenost);
                c.MaxRozsah = (int)Math.Floor((Vzdalenost / CelkovaVzdalenost) * 5000);
            }
            // Náhodně ,,trefuji,, rozsahy, dokud netrefím stejný počet potomků jako měla původní generace
            while (UpravenaGenerace.Count < PocetPotomku)
            {
                int hodnota = random.Next(5001);

                for (int i = 0; i < PocetPotomku; i++)
                {
                    Cesta aktualniCesta = Generace[i];
                    if (hodnota > aktualniCesta.MinRozsah && hodnota < aktualniCesta.MaxRozsah)
                    {
                        UpravenaGenerace.Add(new Cesta(aktualniCesta.seznamMest, IdCesta));
                        IdCesta++;
                        break;
                    }
                }
            }
            // Předám referenci na novou kolekci původní kolekci
            Generace = UpravenaGenerace;
            // Zobrazím informace do konzole
            VykresliInfo(IdGenerace,0);
        }
        /// <summary>
        /// Metoda, která vytvoří základní generaci CEST
        /// </summary>
        /// <param name="pocet">Počet CEST v počáteční i dalších generacích</param>
        public void VytvorZakladniGeneraci(int pocet)
        {
            for (int j = 0; j < pocet; j++)
            {
                // Vytvarim novy seznam mest, podle pozedavku. Tedy novou cestu, ktera bude obsahovat 35 mest
                List<Mesto> novaCesta = new List<Mesto>();
                List<int> vyberMest = new List<int>();
                // Pouze seznam abych vytvarel nova mesta a nebyli na ceste nektera mesta vicekrat
                for (int i = 0; i < 35; i++)
                    vyberMest.Add(i);
                for (int i = 0; i < 35; i++)
                {
                    // Vyberu ze zbyvajicich moznosti nejake ID a podle toho vytvorim dane mesto
                    int pomocna = vyberMest[random.Next(vyberMest.Count)];
                    novaCesta.Add(new Mesto(mesta[pomocna].Id, mesta[pomocna].Nazev, SpocitejVzdalenosti(mesta[pomocna].Id)));
                    // Smazu vybrane ID, protoze by se mi opakovali mesta
                    vyberMest.RemoveAt(vyberMest.IndexOf(pomocna));
                }
                // Přidám novou CESTU do generace a zvýším IDcesty
                posledniCesta = new Cesta(novaCesta, IdCesta);
                IdCesta++;
                Generace.Add(posledniCesta);
                
            }

            VykresliInfo(0,1);
        }

        /// <summary>
        /// Metoda, která vykreslí základní informace o právě probíhané události.
        /// </summary>
        /// <param name="generace">Číslo generace</param>
        /// <param name="vstup">0 - kvalita, 1 - nova generace, 2 - mutace</param>
        private void VykresliInfo(int generace,byte vstup)
        {
            int soucetIDmest = 0;
            // Smažu konzoli
            Console.Clear();
            // Pouze podmínka, jaká hlavička se bude v konzoli vykreslovat, aby bylo vidět jaká operace probíhá
            if (vstup == 0)
                Console.WriteLine("************************|    Kvalita   |*************************");
            else if(vstup == 1)
                Console.WriteLine("************************|    NovaGEN   |*************************");
            else if(vstup == 2)
                Console.WriteLine("************************|    MUTACE    |*************************");

            double cesta = 0;
            
            // Výpočet celkové vzdálenosti všech CEST v generaci
            foreach (Cesta c in Generace)
                cesta += c.Vzdalenost;
            // Seřazení kolekce Generace, vzestupně
            IEnumerable<Cesta> serazene = Generace.OrderBy(a => a.Vzdalenost);
            // Uložení nejvyšší vzdálenosti potomka - CESTY
            float TOP = serazene.Last().Vzdalenost;
            // Uložení nejnižší vzdálenosti potomka - CESTY
            float DOWN = serazene.First().Vzdalenost;
            // Vyčtení nejlepší CESTY
            Cesta novaNejvyssi = serazene.First();
            if (nejlepsiCesta == null)
                nejlepsiCesta = novaNejvyssi;
            if (novaNejvyssi.Vzdalenost < nejlepsiCesta.Vzdalenost)
                nejlepsiCesta = novaNejvyssi;

            int pocetNejvyssich = 0;
            int pocetNejnizsich = 0;
            // Výpočet počtu nejlepších jedinců a současně nejhorších, pro kontrolu koncentrace nejlepších a nejhorších
            foreach (Cesta c in Generace)
            {
                if (c.Vzdalenost == TOP)
                    pocetNejvyssich++;
                else if (c.Vzdalenost == DOWN)
                    pocetNejnizsich++;
            }
            // Součet ID měst, pouze kontrola pro objevení duplicity města v kolekci
            foreach(Cesta c in Generace)
            {
                foreach (Mesto m in c.seznamMest)
                    soucetIDmest += m.Id;
            }

            int hraniceSpodniHorni = Generace.Count / 5;
            double spodniSkupina = 0;
            double horniSkupina = 0;
            int inc = 0;
            // Nalezení CEST, které spadají do horních a spodních 20 % rozsahu aktuální generace
            foreach (Cesta c in serazene)
            {
                inc++;
                spodniSkupina += c.Vzdalenost;
                if (inc > hraniceSpodniHorni)
                    break;
            }
            inc = 0;
            foreach (Cesta c in serazene)
            {
                inc++;
                if (inc > Generace.Count - hraniceSpodniHorni)
                {
                    horniSkupina += c.Vzdalenost;
                }
            }
            // Výpočet průměru horní a spodních 20 % jedinců
            spodniSkupina = spodniSkupina / (double)hraniceSpodniHorni;
            spodniSkupina = Math.Floor(spodniSkupina);
            horniSkupina = horniSkupina / (double)hraniceSpodniHorni;
            horniSkupina = Math.Floor(horniSkupina);

            Console.WriteLine("------------------------|  GENERACE {5}  |------------------------\n" +
                              "     -- Celkova vzdalenost teto generace je:\t{0}\n" +
                              "     -- Nejvyssi vzdalenost je:\t\t\t{1}\n" +
                              "     -- A kolekce jich obsahuje:\t\t{2}\n" +
                              "     -- A jejich PRUMER je:\t\t\t{7}\n\n\n" +
                              "     -- Nejnizsi vzdalenost je:\t\t\t{3}\n" +
                              "     -- A kolekce jich obsahuje:\t\t{4}\n"+
                              "     -- A jejich PRUMER je:\t\t\t{8}\n" +
                              "     -- Soucet mest u potomku je:\t\t{6}\n"
                              , cesta, serazene.Last().Vzdalenost, pocetNejvyssich, serazene.First().Vzdalenost,pocetNejnizsich
                              ,generace,soucetIDmest,horniSkupina,spodniSkupina);
            // Uspání vlákna pro čitelnost výpisu
            Thread.Sleep(50);

        }
        /// <summary>
        /// Metoda, která uloží souřadnice všech měst do pole 35 x 35
        /// </summary>
        /// <returns>Pole názvu měst ve string</returns>
        public string[] DefinujMapuVzdalenosti()
        {
            string[] nazvyMest = new string[35];
            // Cesta k excel souboru, který obsahuje vzdálenosti atd.
            string cesta = @"d:\Distance.xlsx";
            // Using, který získá jednotlivé vzdálenosti z excelu
            using (var stream = File.Open(cesta, FileMode.Open, FileAccess.Read))
            {
                using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                {
                    DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = false }
                    });
                    DataTableCollection table = result.Tables;

                    foreach (DataTable a in table)
                    {
                        for (int i = 1; i < 36; i++)
                        {
                            for (int j = 1; j < 36; j++)
                            {
                                // Uložení do proměnné data samotné hodnoty dané buňky v tabulce
                                var data = a.Rows[i][j];
                                string textData = data.ToString();
                                // Parsování získaného stringu na float, aby bylo do pole uloženo číslo, se kterým se dále pracuje
                                float.TryParse(textData, out float cisloData);
                                // Uložení do základní matice vzdáleností
                                mapaVzdalenosti[j - 1, i - 1] = cisloData;
                            }
                            nazvyMest[i - 1] = a.Rows[i][0].ToString();
                        }
                    }
                }
            }
            return nazvyMest;
        }

        /// <summary>
        /// Metoda, která spočítá vzdálenosti daného města od všech ostatních.
        /// </summary>
        /// <param name="id">Identifikátor města</param>
        /// <returns>Pole vzdáleností daného města a všech ostatních</returns>
        private float[] SpocitejVzdalenosti(int id)
        {
            float[] vzdalenosti = new float[35];
            for(int i = 0; i < 35; i++)
            {
                // Nejprve beru info ze sloupce, protoze mesta maji vztaznost jak ve sloupci tak v radku
                if (i < id)
                    vzdalenosti[i] = mapaVzdalenosti[id, i];
                // Pokud jsem narazil na ID mesta, tak dosadim 0, protoze vzdalenost je 0
                else if (i == id)
                    vzdalenosti[i] = 0;
                // Pokud je i vetsi nez ID, tak zacnu cerpat vztaznou vzdalenost z radku
                else
                    vzdalenosti[i] = mapaVzdalenosti[i, id];
            }
            return vzdalenosti;
        }
    }
}
