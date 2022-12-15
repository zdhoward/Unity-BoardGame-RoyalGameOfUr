using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] int width, height;

    [SerializeField] UrTile tilePrefab;

    Camera mainCamera;

    Vector2[] squaresToNotspawn = { new Vector2(0, 4), new Vector2(0, 5), new Vector2(2, 4), new Vector2(2, 5) };

    Dictionary<Vector2, UrTile> tiles;
    public Dictionary<Vector2, UrTile> Tiles => tiles;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        tiles = new Dictionary<Vector2, UrTile>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool shouldSkipSpawning = false;
                foreach (Vector2 squareToNotSpawn in squaresToNotspawn)
                    if (x == squareToNotSpawn.x && y == squareToNotSpawn.y)
                        shouldSkipSpawning = true;

                if (shouldSkipSpawning)
                    continue;

                UrTile spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity, transform);
                spawnedTile.Setup(new Vector2(x, y));
                tiles.Add(new Vector2(x, y), spawnedTile);
            }
        }

        mainCamera.transform.position = new Vector3((float)width / 2f - 0.5f, (float)height / 2f - 0.5f, -10f);
    }

    public UrTile GetTileAtPosition(Vector2 tilePosition)
    {
        if (tiles.TryGetValue(tilePosition, out UrTile tile))
            return tile;

        return null;
    }
}
