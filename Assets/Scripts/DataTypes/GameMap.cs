using UnityEngine;

[System.Serializable]
public class GameMap
{
    [Range(1, 100)]
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
