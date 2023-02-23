-> main
=== main ===
Hallo Schwester.
Willst erfahren wie du dich höher und schneller bewegen kannst? # speaker: Kleine Schwester
* [Ja!]
    Na klar, deine Tips sind meistens hilfreich. # speaker: Ich
    -> choice_yes
* [Nö.]
    Lass mich, ich kann das alleine. # speaker: Ich
    -> choice_no
-> DONE
=== choice_yes ===
Ja, super! Es ist ganz einfach. Mit dem Controller "B" drücken und du explodierst vor Energie.
Vorher solltest du dir gedanken machen in welche Richtung du willst. *zwinkersmiley 
# speaker: Kleine Schwester
-> END
=== choice_no ===
Na gut... Frag mich einfach wenn du einen Tip haben willst.
# speaker: Kleine Schwester
* [Alles, klar.] 
Ciao. # speaker: Ich
    -> END
    