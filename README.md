# PacMan
PacMan je projekt, který má za cíl vytvoření hry Pac-Man a level editoru k dané hře.
Stav projektu
	Projket by měl být plnně funkční, je ve stadiu přidávání vedlejších funkcí, opravování chyb a zpřesňování kódu-
Ovládání
	Menu
		Mezi jednotlivými prvky se ovládá pomocí WSAD / šipkami
		Pro zvolení tlačítka se používá Enter
		Do políček, která přijímají čísla, lze čísla vložit jakoukoliv číselnou klávesou
		Pokud aplikace žádá text, lze použít celou klávesnici
	Hra
		Postava se ovládá pomocí WSAD / šipkami
		Pomocí klávesy Escape se uživatel může dostat do menu
	LevelEditor
		Pomocí WSAD lze ovládat kurzor na editované hrací ploše
		Pomocí šipek nahoru a dolů lze vybírat mezi umísťovanými objekty
		Pomocí klávesy escape lze vyvolat menu
	Zobrazení Menu, Hry i LevelEditoru lze refreshnout (například po změnení velikosti okna konzole) pomocí klávesy R.
		

Poznámky pro hraní hry
	Pro optimální vykreslování je nejlepší mít nastaveny barvy v konzoli na černé pozadí a bílý text a hrát v nezvětšené konzoli

Poznámky pro tvoření levelů
	AI duchů funguje nejlépe v levelech, které jsou tvořeny uličkami o šířce 1
	Výherní zprávu dostane hráč pouze v levelech, které obsahují aspoň jeden coin
