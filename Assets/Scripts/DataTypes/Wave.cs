using System;
using UnityEngine;

[System.Serializable]
public struct Wave {
    [Range(0, 100000)]
    public float timeBetweenSpawns;
    [Range(0, 100000)]
    public int numberOfEnemies;
    public bool infiniteWave;

    Wave(int _numberOfEnemies, float _timeBetweenSpawns, bool _infiniteWave)
    {
        timeBetweenSpawns = _timeBetweenSpawns;
        numberOfEnemies = _numberOfEnemies;
        infiniteWave = _infiniteWave;
    }
}
