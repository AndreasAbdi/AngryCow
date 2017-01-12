﻿using UnityEngine;
using System;
using System.Collections;
using System.Linq;

[System.Serializable]
public class Map
{
    [Range(1,100)]
    public int height;
    [Range(1, 100)]
    public int width;
    [Range(0, 1)]
    public float outlineSize;

    public Coord centerLocation;
    public string seed;
}

[System.Serializable]
public class Coord
{
    public int x;
    public int y;

    public Coord(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
}

public class MapGenerator : MonoBehaviour {
    public Map map;
    public Transform mapTile;
    public Transform tilePool;

    void Start () {
        GenerateMap();
	}

    void GenerateMap()
    {
        ClearTilePool();
        CreateMap();
    }

    void CreateMap()
    {
        (
        from x in Enumerable.Range(0, map.width)
        from y in Enumerable.Range(0, map.height)
        select new Coord(x, y)
        ).ToList()
        .Select(mapCoord => {
            Vector3 tilePosition = new Vector3(-map.width / 2 + 0.5f + mapCoord.x, 0, -map.height / 2 + 0.5f + mapCoord.y);
            return Instantiate(mapTile, tilePosition, Quaternion.identity) as GameObject;
        }).ToList()
        .ForEach(tile => {
            Transform tileTransform = tile.GetComponent<Transform>();
            tileTransform.localScale = Vector3.one * (1 - map.outlineSize);
            tileTransform.SetParent(tilePool);
        });
    }

    void ClearTilePool()
    {
        while (tilePool.childCount > 0)
        {
            DestroyImmediate(tilePool.GetChild(0));
        }
    }
}
