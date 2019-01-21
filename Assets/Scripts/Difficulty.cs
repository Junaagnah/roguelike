using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Difficulty
{
    readonly int id;
    readonly string nom;
    readonly int nbSalles;
    readonly int dmgPlayer;
    readonly int dmgMob;
    readonly int spawnMob;
    readonly int spawnPotions;

    public int Id { get => id; }
    public string Nom { get => nom; }
    public int NbSalles { get => nbSalles; }
    public int DmgPlayer { get => dmgPlayer; }
    public int DmgMob { get => dmgMob; }
    public int SpawnMob { get => spawnMob; }
    public int SpawnPotions { get => spawnPotions; }

    static public Difficulty selected;
    static public List<Difficulty> difficulties;

    public Difficulty(int Id, string Nom, int NbSalles, int DmgPlayer, int DmgMob, int SpawnMob, int SpawnPotions)
    {
        id = Id;
        nom = Nom;
        nbSalles = NbSalles;
        dmgPlayer = DmgPlayer;
        dmgMob = DmgMob;
        spawnMob = SpawnMob;
        spawnPotions = SpawnPotions;
    }
}
