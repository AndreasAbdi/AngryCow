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
    [HideInInspector]
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

    public static bool operator==(Coord c1, Coord c2)
    {
        return c1.x == c2.x && c1.y == c2.y;
    }

    public static bool operator!=(Coord c1, Coord c2)
    {
        return !(c1 == c2);
    }
}

public class MapGenerator : MonoBehaviour {
    [Range(0,1)]
    public float obstaclePercent;
    public Transform obstaclePrefab;
    public Map map;
    public float tileSize;
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
        bool[,] obstacleMap = new bool[map.width, map.height];
        map.centerLocation = new Coord(map.width / 2, map.height / 2);

        int obstacleCount = (int) (map.width * map.height * obstaclePercent);
        int currentObstacleCount = 0;
        Enumerable
            .Range(0, obstacleCount)
            .Select(
                (obstacleNumber) =>{
                Coord nextCoord = shuffledCoords.Dequeue();
                shuffledCoords.Enqueue(nextCoord);
                return nextCoord;
            })
            .ToList()
            .ForEach(
                (position) => {
                currentObstacleCount++;
                obstacleMap[position.x, position.y] = true;
                if(position != map.centerLocation && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
                {
                    Transform obstacle = Instantiate(obstaclePrefab, CoordToVector3(position) + Vector3.up *tileSize* 0.5f, Quaternion.identity) as Transform;
                    obstacle.parent = objectPool;
                    obstacle.localScale = Vector3.one * (1 - map.outlineSize) * tileSize;
                }
                    else
                {
                    obstacleMap[position.x, position.y] = false;
                    currentObstacleCount--;
                }
            });
    }
    
    Vector3 CoordToVector3(Coord coord)
    {
        return new Vector3(-map.width / 2 + 0.5f + coord.x, 0, -map.height / 2 + 0.5f + coord.y) * tileSize;
    }

    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(map.centerLocation);
        mapFlags[map.centerLocation.x, map.centerLocation.y] = true;

        int accessibleTileCount = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            (
            //generate all neighbors of tile
            from x in Enumerable.Range(-1, 3)
            from y in Enumerable.Range(-1, 3)
            select new Coord(x + tile.x, y + tile.y)
            ).Where((neighbor) =>
            //only vertical/horizontal neighbors
                neighbor.x == tile.x || neighbor.y == tile.y
            ).Where((neighbor) =>
            //not crossing the map borders in x
                neighbor.x >= 0 && neighbor.x < obstacleMap.GetLength(0)
            ).Where((neighbor) =>
            //not crossing the map borders in y
                neighbor.y >= 0 && neighbor.y < obstacleMap.GetLength(1)
            ).Where((neighbor) =>
            //has yet to be marked
                !mapFlags[neighbor.x, neighbor.y] && !obstacleMap[neighbor.x, neighbor.y]
            ).ToList()
            .ForEach((neighbor) => {
                mapFlags[neighbor.x, neighbor.y] = true;
                queue.Enqueue(neighbor);
                accessibleTileCount++;
            });
        }

        int targetAccessibleTileCount = (int)(map.width * map.height - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }

    void CreateMap()
    {
        (
        from x in Enumerable.Range(0, map.width)
        from y in Enumerable.Range(0, map.height)
        select new Coord(x, y)
        ).ToList()
        .Select(mapCoord => {
            Vector3 tilePosition = CoordToVector3(mapCoord);
            return Instantiate(mapTile, tilePosition, Quaternion.identity) as GameObject;
        }).ToList()
        .ForEach(tile => {
            Transform tileTransform = tile.GetComponent<Transform>();
            tileTransform.localScale = Vector3.one * (1 - map.outlineSize) * tileSize;
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
