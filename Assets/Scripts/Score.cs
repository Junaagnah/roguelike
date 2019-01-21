using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score
{
    readonly int idUser;
    readonly string pseudoUser;
    readonly int scoreUser;
    private int niveauPerso;
    private int forcePerso;
    private int argentPerso;
    private int tours;
    private int monstres;
    private int boss;

    public int IdUser { get => idUser; }
    public string PseudoUser { get => pseudoUser; }
    public int ScoreUser { get => scoreUser; }

    public int NiveauPerso { get => niveauPerso; set => niveauPerso = value; }
    public int ForcePerso { get => forcePerso; set => forcePerso = value; }
    public int ArgentPerso { get => argentPerso; set => argentPerso = value; }
    public int Tours { get => tours; set => tours = value; }
    public int Monstres { get => monstres; set => monstres = value; }
    public int Boss { get => boss; set => boss = value; }

    static public int idUserTemp;

    public Score(int Id)
    {
        idUser = Id;
    }

    public Score(string PseudoUser, int Score)
    {
        pseudoUser = PseudoUser;
        scoreUser = Score;
    }

    public void SaveGame(int niveau, int force, int argent, int tours, int monstres, int boss)
    {
        NiveauPerso = niveau;
        ForcePerso = force;
        ArgentPerso = argent;
        Tours = tours;
        Monstres = monstres;
        Boss = boss;
    }
}
