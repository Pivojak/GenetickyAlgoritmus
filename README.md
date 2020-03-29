# GenetickyAlgoritmus
V rámci studia na UPCE jsem měl vyhotovit problematiku obchodního cestujícího pomocí Genetického algoritmu. Tento algoritmus se snaží
napodobit evoluci, kdy každý nový prvek vznikne spojením informace rodičů a počet prvků daného druhu je odvislý od kvality, tedy v tomto 
případě jak blízko je řešení problému. Současně zde probíhá malá míra mutace, kdy se náhodně zamění jeden z prvků za jiný. 
******************************************************************************************************************************************
# Rozbor problému
Problematika projektu se jmenuje obchodní cestující, kdy algoritmus musí nalézt co nejkratší cestu mezi 35 městy, tak aby navštívil každé
město, ale v žádném z nich nebyl dvakrát. Program je postaven na třech třídách. První je město, což znázorňuje jedno město na cestě. Další
je Cesta, jež sdružuje města po 35 a tvoří tedy jednoho potomka generace. Veškeré operace poté řídí třída GenetickyAlgoritmus, která obsahuje základní logiku pro výkon tohoto algoritmu. Program je podrobně rozepsán v souboru Report.pdf. Výstupem programu je konzole, která zobrazuje informaci o čísle generace, celkové vzdálenosti generace, nejvyšší vzdálenosti, počet nejlepších a nejhorších potomků a nejkratší vzdálenost. Vzhled této konzole je ukázán na obrázku GenetickyAlgoritmus_Run.png. Pro funkčnost projektu je třeba vytvořit excel soubor, který bude obsahovat právě 35 měst a vzdálenosti mezi všemi městy. 
******************************************************************************************************************************************
# Kompletní informace naleznete v souboru Report.pdf
