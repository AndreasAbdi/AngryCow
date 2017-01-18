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

    [Range(0, 1)]
    public float obstaclePercent;

    public float minObstacleHeight;
    public float maxObstacleHeight;
    public Color foregroundColor;
    public Color backgroundColor;

    public string seed;

    [HideInInspector]
    public Coord centerLocation;
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
    public Map[] maps;
    public int currentMapIndex;
    [Range(0.001f, 100)]
    public float tileSize;
    //needs to be greater than all the current maps for nav meshes to work
    public Vector2 maxMapSize;

    public Transform obstaclePrefab;
    public GameObject mapTile;
    public Transform objectPool;

    //for controlling the navigation mesh. 
    public Transform navMeshFloor;
    public Transform navMeshMaskPrefab;


    //for the ground
    public BoxCollider floor;

    Map map;
    //obstacles are 00 at top left, and nn at bottom right
    bool[,] obstacleMap;

    void Start () {
        GenerateMap();
	}

    public void GenerateMap()
    {
        SetCurrentMap();
        ClearTilePool();
        SetupFloor();
        CreateMap();
        CreateObstacles();
        UpdateNavMesh();
    }

    public void SpawnAt(Coord spawnTarget, Transform gameObject, float objectHeight, bool blocking)
    {
        if (!obstacleMap[spawnTarget.x, spawnTarget.y])
        {
            SpawnObject(gameObject, spawnTarget, objectHeight);
            obstacleMap[spawnTarget.x, spawnTarget.y] = true && blocking;
        }
    }

    public void SpawnAt(Vector3 spawnTarget, Transform gameObject, float objectHeight, bool blocking)
    {
        SpawnAt(Vector3ToCoord(spawnTarget), gameObject, objectHeight, blocking);
    }

    public void ClearPosition(Vector3 position)
    {
        Coord toClear = Vector3ToCoord(position);
        obstacleMap[toClear.x, toClear.y] = false;
    }

    public bool Blocked(Coord position)
    {
        return obstacleMap[position.x, position.y];
    }

    public Vector3 CoordToVector3(Coord coord)
    {
        return new Vector3(-map.width / 2f + 0.5f + coord.x, 0, -map.height / 2f + 0.5f + coord.y) * tileSize;
    }

    void SetCurrentMap()
    {
        map = maps[currentMapIndex];
        obstacleMap = new bool[map.width, map.height];
    }

    void SetupFloor()
    {
        floor.center = new Vector3(0, -0.25f, 0);
        floor.size = new Vector3(map.width * tileSize, 0.5f, map.height * tileSize);
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
            return Instantiate(mapTile, tilePosition, Quaternion.identity);
        }).ToList()
        .ForEach(tile => {
            Transform tileTransform = tile.GetComponent<Transform>();
            tileTransform.localScale = Vector3.one * (1 - map.outlineSize) * tileSize;
            tileTransform.SetParent(objectPool);
        });
    }

    void CreateObstacles()
    {
        System.Random pseudoRandom = new System.Random(map.seed.GetHashCode());

        Coord[] coords = (
        from x in Enumerable.Range(0, map.width)
        from y in Enumerable.Range(0, map.height)
        select new Coord(x, y)
        ).ToArray();

        Queue<Coord> shuffledCoords = new Queue<Coord>(Utility.ShuffleArray(coords, map.seed.GetHashCode()));
        map.centerLocation = new Coord(map.width / 2, map.height / 2);

        int obstacleCount = (int)(map.width * map.height * map.obstaclePercent);
        int currentObstacleCount = 0;
        Enumerable
            .Range(0, obstacleCount)
            .Select(
                (obstacleNumber) => {
                    Coord nextCoord = shuffledCoords.Dequeue();
                    shuffledCoords.Enqueue(nextCoord);
                    return nextCoord;
                })
            .ToList()
            .ForEach(
                (position) => {
                    currentObstacleCount++;
                    if (!GenerateObstacle(position, (float)pseudoRandom.NextDouble(), currentObstacleCount))
                    {
                        currentObstacleCount--;
                    }
                });
    }

    bool GenerateObstacle(Coord position, float randomValue, int currentObstacleCount)
    {
        obstacleMap[position.x, position.y] = true;
        if (position != map.centerLocation && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
        {
            SpawnObstacle(position, randomValue);
            return true;
        }
        obstacleMap[position.x, position.y] = false;
        return false;
    }

    void SpawnObstacle(Coord position, float randomValue)
    {
        float obstacleHeight = Mathf.Lerp(map.minObstacleHeight, map.maxObstacleHeight, randomValue);

        Transform obstacle = SpawnObject(obstaclePrefab, position, obstacleHeight);
        Renderer obstacleRenderer = obstacle.GetComponentInChildren<Renderer>(); 
        Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);

        float colourPercent = position.y / (float)map.height;
        obstacleMaterial.color = Color.Lerp(map.foregroundColor, map.backgroundColor, colourPercent);
        obstacleRenderer.sharedMaterial = obstacleMaterial;
    }

    Transform SpawnObject(Transform objectToSpawn, Coord position, float obstacleHeight)
    {
        float targetScale = (1 - map.outlineSize) * tileSize;

        Vector3 upShift = Vector3.up * targetScale * obstacleHeight / 2f;

        Transform gameObject = Instantiate(objectToSpawn, CoordToVector3(position) + upShift, Quaternion.identity) as Transform;
        gameObject.parent = objectPool;
        gameObject.localScale = new Vector3(targetScale, targetScale * obstacleHeight, targetScale);

        return gameObject;
    }

    void UpdateNavMesh()
    {
        navMeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
        float positionScale = 1/(4f * tileSize);
        float verticalPositionScale = (map.width + maxMapSize.x) * positionScale;
        float horizontalPositionScale = (map.height + maxMapSize.y) * positionScale;
        Vector3 partialLocalScale = new Vector3((maxMapSize.x - map.width) / 2f, 1, map.height) * tileSize;
        Vector3 fillingLocalScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - map.height) / 2f) * tileSize;
        //left 
        CreateNavMeshMask(Vector3.left * verticalPositionScale, partialLocalScale);
        //right
        CreateNavMeshMask(Vector3.right * verticalPositionScale, partialLocalScale);
        //top
        CreateNavMeshMask(Vector3.forward * horizontalPositionScale, fillingLocalScale);
        //bottom
        CreateNavMeshMask(Vector3.back * horizontalPositionScale, fillingLocalScale);
    }

    void ClearTilePool()
    {
        while (objectPool.childCount != 0)
        {
            DestroyImmediate(objectPool.GetChild(0).gameObject);
        }
    }

    void CreateNavMeshMask(Vector3 position, Vector3 localScale)
    {
        Transform navMeshMask = Instantiate(navMeshMaskPrefab, position, Quaternion.identity) as Transform;
        navMeshMask.parent = objectPool;
        navMeshMask.localScale = localScale;
    }

    Coord Vector3ToCoord(Vector3 vector3)
    {
        int x = Mathf.RoundToInt(vector3.x / tileSize + (map.width - 1) / 2f);
        int y = Mathf.RoundToInt(vector3.z / tileSize + (map.height - 1) / 2f);
        x = Mathf.Clamp(x, 0, map.width - 1);
        y = Mathf.Clamp(y, 0, map.height - 1);
        return new Coord(x, y);
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

}
