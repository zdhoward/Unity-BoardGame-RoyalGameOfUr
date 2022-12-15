using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrTile : MonoBehaviour
{
    [SerializeField] LayerMask tileLayerMask;

    [SerializeField] Sprite normalTile1;
    [SerializeField] Sprite normalTile2;
    [SerializeField] Sprite normalTile3;
    [SerializeField] Sprite normalTile4;
    [SerializeField] Sprite normalTile5;
    [SerializeField] Sprite specialTile;

    Camera mainCamera;

    SpriteRenderer spriteRenderer;

    Vector2 gridPosition;

    GameObject highlight;

    Vector2[] normalTile1Positions = { new Vector2(0, 3), new Vector2(0, 1), new Vector2(1, 6), new Vector2(2, 3), new Vector2(2, 1) };
    Vector2[] normalTile2Positions = { new Vector2(0, 2), new Vector2(1, 1), new Vector2(1, 4), new Vector2(1, 7), new Vector2(2, 2) };
    Vector2[] normalTile3Positions = { new Vector2(1, 0) };
    Vector2[] normalTile4Positions = { new Vector2(1, 2), new Vector2(1, 5) };
    Vector2[] normalTile5Positions = { new Vector2(0, 7), new Vector2(2, 7) };
    Vector2[] specialTilePositions = { new Vector2(0, 0), new Vector2(0, 6), new Vector2(1, 3), new Vector2(2, 0), new Vector2(2, 6) };

    PlayingPiece currentPiece = null;

    void Awake()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        highlight = transform.Find("Highlight").gameObject;
    }

    public void Setup(Vector2 gridPosition)
    {
        name = $"Tile: {gridPosition.x}, {gridPosition.y}";
        this.gridPosition = gridPosition;
        ApplySprite();
    }

    void ApplySprite()
    {
        foreach (Vector2 tilePosition in normalTile1Positions)
        {
            if (gridPosition == tilePosition)
            {
                spriteRenderer.sprite = normalTile1;
                return;
            }
        }
        foreach (Vector2 tilePosition in normalTile2Positions)
        {
            if (gridPosition == tilePosition)
            {
                spriteRenderer.sprite = normalTile2;
                return;
            }
        }
        foreach (Vector2 tilePosition in normalTile3Positions)
        {
            if (gridPosition == tilePosition)
            {
                spriteRenderer.sprite = normalTile3;
                return;
            }
        }
        foreach (Vector2 tilePosition in normalTile4Positions)
        {
            if (gridPosition == tilePosition)
            {
                spriteRenderer.sprite = normalTile4;
                return;
            }
        }
        foreach (Vector2 tilePosition in normalTile5Positions)
        {
            if (gridPosition == tilePosition)
            {
                spriteRenderer.sprite = normalTile5;
                return;
            }
        }
        foreach (Vector2 tilePosition in specialTilePositions)
        {
            if (gridPosition == tilePosition)
            {
                spriteRenderer.sprite = specialTile;
                return;
            }
        }
    }

    public void Select()
    {
        if (!highlight.activeInHierarchy)
            highlight.SetActive(true);
    }

    public void Deselect()
    {
        if (highlight.activeInHierarchy)
            highlight.SetActive(false);
    }

    public void SetPiece(PlayingPiece piece)
    {
        currentPiece = piece;
    }

    public PlayingPiece GetPiece()
    {
        if (currentPiece != null)
            return currentPiece;

        return null;
    }

    public Vector2 GetGridPosition()
    {
        return gridPosition;
    }
}
