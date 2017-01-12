using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
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
    [Range(0,100)]
    public int obstacleCount;
    public Transform obstaclePrefab;
    public Map map;
    public Transform mapTile;
    public Transform objectPool;


    void Start () {
        GenerateMap();
	}

    public void GenerateMap()
    {
        ClearTilePool();
        CreateMap();
        CreateObstacles();
    }

    void CreateObstacles()
    {
        Coord[] coords = (
        from x in Enumerable.Range(0, map.width)
        from y in Enumerable.Range(0, map.height)
        select new Coord(x, y)
        ).ToArray();

        Queue<Coord> shuffledCoords = new Queue<Coord>(Utility.ShuffleArray(coords, map.seed.GetHashCode()));

        Enumerable
            .Range(0, obstacleCount)
            .Select(
                (obstacleNumber) =>{
                    Coord nextCoord = shuffledCoords.Dequeue();
                    shuffledCoords.Enqueue(nextCoord);
                    return nextCoord;
            }).Select(
                (coord) =>
                {
                    return new Vector3(-map.width / 2 + 0.5f + coord.x, 0, -map.height / 2 + 0.5f + coord.y);
                }
            )
            .ToList()
            .ForEach(
                (position) => {
                    Transform obstacle = Instantiate(obstaclePrefab, position + Vector3.up * 0.5f, Quaternion.identity) as Transform;
                    obstacle.parent = objectPool;
            });
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
            tileTransform.SetParent(objectPool);
        });
    }

    void ClearTilePool()
    {
        while (objectPool.childCount != 0)
        {
            DestroyImmediate(objectPool.GetChild(0).gameObject);
        }
    }
}
