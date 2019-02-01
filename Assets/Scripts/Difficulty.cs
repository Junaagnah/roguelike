using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cette classe permet de stocker les différentes difficultés
public class Difficulty
{
    readonly int id;
    readonly string nom;
    readonly int roomLength;
    readonly decimal dmgPlayer;
    readonly decimal dmgMob;
    readonly int spawnMob;
    readonly int minSpawnWall;
    readonly int maxSpawnWall;
    readonly int minSpawnFood;
    readonly int maxSpawnFood;
    readonly decimal coefMoney;

    public int Id { get => id; }
    public string Nom { get => nom; }
    public int RoomLength { get => roomLength; }
    public decimal DmgPlayer { get => dmgPlayer; }
    public decimal DmgMob { get => dmgMob; }
    public int SpawnMob { get => spawnMob; }
    public int MinSpawnWall { get => minSpawnWall; }
    public int MaxSpawnWall { get => maxSpawnWall; }
    public int MinSpawnFood { get => minSpawnFood; }
    public int MaxSpawnFood { get => maxSpawnFood; }
    public decimal CoefMoney { get => coefMoney; }



    static public Difficulty selected;
    static public List<Difficulty> difficulties;

    public Difficulty(int Id, string Nom, int RoomLength, decimal DmgPlayer, decimal DmgMob, int SpawnMob, int MinSpawnWall, int MaxSpawnWall, int MinSpawnFood, int MaxSpawnFood, decimal CoefMoney)
    {
        id = Id;
        nom = Nom;
        roomLength = RoomLength;
        dmgPlayer = DmgPlayer;
        dmgMob = DmgMob;
        spawnMob = SpawnMob;
        minSpawnWall = MinSpawnWall;
        maxSpawnWall = MaxSpawnWall;
        minSpawnFood = MinSpawnFood;
        maxSpawnFood = MaxSpawnFood;
        coefMoney = CoefMoney;
    }
}
