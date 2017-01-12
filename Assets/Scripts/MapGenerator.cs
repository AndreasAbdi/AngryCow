using UnityEngine;
using System.Collections;

[System.Serializable]
public class Map
{
    [Range(1,100)]
    public int height;
    [Range(1, 100)]
    public int width;
    public GameObject mapTile;
    public Coord CenterLocation;

    public string seed;
}

[System.Serializable]
public class Coord
{
    public int x;
    public int y;
}

public class MapGenerator : MonoBehaviour {
    public Map map;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
