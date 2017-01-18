using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour {
    public Wave[] waves;
    public string seed;

    public Transform enemy;
    public MapGenerator mapGenerator;

    float currentTimeToSpawn;
    int enemiesToSpawn;

    System.Random pseudoRandom;
    List<Wave> waveList;

	void Start () {
        waveList = new List<Wave>(waves);
        if (!ShouldTerminate())
        {
            SetSpawnVariables();
            pseudoRandom = new System.Random(seed.GetHashCode());
        }
    }

    void FixedUpdate()
    {
        if(ShouldTerminate())
        {
            return;
        }

        if(ShouldGoToNextWave())
        {
            waveList.Remove(waveList[0]);
            if(ShouldTerminate())
            {
                return;
            }
            SetSpawnVariables();
        }
        if(waveList[0].infiniteWave)
        {
            HandleInfiniteWave();
            return;
        }
        HandleNormalWave();
    }

    void SetSpawnVariables()
    {
        currentTimeToSpawn = waveList[0].timeBetweenSpawns;
        enemiesToSpawn = waveList[0].numberOfEnemies;
    }

    bool ShouldTerminate()
    {
        return waveList.Count <= 0;
    }

    bool ShouldGoToNextWave()
    {
        return enemiesToSpawn == 0 && !waveList[0].infiniteWave;
    }

    void HandleNormalWave()
    {
        currentTimeToSpawn -= Time.deltaTime;
        if (currentTimeToSpawn <= 0 && enemiesToSpawn > 0)
        {
            enemiesToSpawn--;
            PerformSpawnIteration();
        }
    }

    void HandleInfiniteWave()
    {
        currentTimeToSpawn -= Time.deltaTime;
        if (currentTimeToSpawn <= 0)
        {
            PerformSpawnIteration();
        }
    }

    void PerformSpawnIteration()
    {
        currentTimeToSpawn = waveList[0].timeBetweenSpawns;
        SpawnEnemyAtRandomPointOnMap();
    }

    void SpawnEnemyAtRandomPointOnMap()
    {
        GameMap currentMap = mapGenerator.maps[mapGenerator.currentMapIndex];
        Coord targetPosition = null;

        do
        {
            int x = pseudoRandom.Next(0, currentMap.width);
            int y = pseudoRandom.Next(0, currentMap.height);
            targetPosition = new Coord(x, y);
            if (!mapGenerator.Blocked(targetPosition))
            {
                mapGenerator.SpawnAt(targetPosition, enemy, enemy.transform.localScale.y, false);
            }
        }
        while (mapGenerator.Blocked(targetPosition));
    }
}

