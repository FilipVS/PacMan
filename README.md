# PacMan
PacMan je projekt, který má za cíl vytvoření hry Pac-Man a level editoru k dané hře.
Status
	V momentální verzi je možné vyzkoušet menu systém.
	Pomocí něj se lze dostat ke hraní hry (tlačítka všech levelů však zatím směřují k jednomu placeholder levelu).
Ovládání
	Menu
		Mezi jednotlivými prvky se ovládá pomocí WSAD / šipkami
		Tlačítka se mačkají pomocí Enteru
		Do políček, která přijímají čísla, lze čísla vložit jakýmkoliv číselnou klávesou
		Pokud aplikace žádá text, lze použít celou klávesnici
	Hra
		Postava se ovládá pomocí WSAD / šipkami
	LevelEditor
		Pomocí WSAD lze ovládat kurzor na editované hrací ploše
		Pomocí šipek nahoru a dolů lze vybírat mezi umísťovanými objekty
		Pomocí klávesy escape lze vyvolat menu
	Zobrazení Menu, Hry i LevelEditoru lze refreshnout (například po změnení velikosti okna konzole) pomocí klávesy R.
		

Poznámky pro hraní hry
	Pro optimální vykreslování je nejlepší mít nastaveny barvy v konzoli na černé pozadí a bílý text

Poznámky pro tvoření levelů
	AI duchů funguje nejlépe v levelech, které jsou tvořeny uličkami o šířce 1
	Výherní zprávu dostane hráč pouze v levelech, které obsahují aspoň jeden coin
