﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Difficulty
{
    readonly int id;
    readonly string nom;
    readonly int roomLength;
    readonly double dmgPlayer;
    readonly double dmgMob;
    readonly int spawnMob;
    readonly int minSpawnWall;
    readonly int maxSpawnWall;
    readonly int minSpawnFood;
    readonly int maxSpawnFood;
    readonly double coefMoney;

    public int Id { get => id; }
    public string Nom { get => nom; }
    public int RoomLength { get => roomLength; }
    public double DmgPlayer { get => dmgPlayer; }
    public double DmgMob { get => dmgMob; }
    public int SpawnMob { get => spawnMob; }
    public int MinSpawnWall { get => minSpawnWall; }
    public int MaxSpawnWall { get => maxSpawnWall; }
    public int MinSpawnFood { get => minSpawnFood; }
    public int MaxSpawnFood { get => maxSpawnFood; }
    public double CoefMoney { get => coefMoney; }



    static public Difficulty selected;
    static public List<Difficulty> difficulties;

    public Difficulty(int Id, string Nom, int RoomLength, double DmgPlayer, double DmgMob, int SpawnMob, int MinSpawnWall, int MaxSpawnWall, int MinSpawnFood, int MaxSpawnFood, double CoefMoney)
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
