﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    private int columns;
    private int rows;
    private Count wallCount = new Count(Difficulty.selected.MinSpawnWall, Difficulty.selected.MaxSpawnWall);
    private Count foodCount = new Count(Difficulty.selected.MinSpawnFood, Difficulty.selected.MaxSpawnFood);
    public GameObject exit;
    public GameObject boss;
    public GameObject food;
    public GameObject mob;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();
    private List<Vector3> gridPositionsBoss = new List<Vector3>();
    public List<Vector3> occupedPositionList = new List<Vector3>();
    private List<Vector3> positionToInstantiate = new List<Vector3>();



    void InitializeList()
    {
        gridPositions.Clear();

        for (int x = 2; x < columns - 2; x++)
        {
            for (int y = 2; y < rows - 2; y++)
            {
                if (x % 10 != 0 && y % 10 != 0)
                {
                    gridPositions.Add(new Vector3(x, y, 0f));
                    if (GameManager.instance.level % 10 == 0)
                    {
                        gridPositionsBoss.Add(new Vector3(x, y, 0f));
                    }
                }
            }
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        for (int x = 0; x < columns ; x++)
        {
            for (int y = 0; y < rows ; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                if (x != 0 && y != 0 && x != columns -1 && y != rows -1)
                {
                    if ((x % 10 == 0 || y % 10 == 0) && ((x % 5 != 0) || (y % 5 != 0)) || (x % 10 == 0 && y % 10 == 0))
                    {
                        toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                    }
                }
                else if (x == 0 || y == 0 || x == columns -1 || y == rows -1)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }

                

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);

        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int level)
    {
        if (level % 10 == 0)
        {
            columns = 11;
            rows = 11;
            BoardSetup();
            InitializeList();
            LayoutObjectAtRandom(foodTiles, 1, 3);
            Instantiate(boss, new Vector3(5, 5, 0f), Quaternion.identity);

        }
        else
        {
            columns = Difficulty.selected.RoomLength;
            rows = Difficulty.selected.RoomLength;

            BoardSetup();
            InitializeList();
            LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
            LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);

            int enemyCount = (int)Mathf.Log(level, 2f) + Difficulty.selected.SpawnMob;
            LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

            InstantiateExit();
        }
    }

    public void BossInvokeMob()
    {
        positionToInstantiate = new List<Vector3>(gridPositionsBoss);

        foreach (Enemy mob in GameManager.instance.enemies)
        {
            positionToInstantiate.Remove(mob.transform.position);
        }
        positionToInstantiate.Remove(GameManager.instance.playerPosition);

        Instantiate(mob, positionToInstantiate[Random.Range(0, positionToInstantiate.Count)], Quaternion.identity);
    }

public void FoodInvoke()
    {
        Instantiate(foodTiles[0], gridPositionsBoss[Random.Range(0, gridPositionsBoss.Count)], Quaternion.identity);
    }

    public void InstantiateExit()
    {
        Instantiate(exit, new Vector3(columns - 2, rows - 2, 0f), Quaternion.identity);
    }
}
