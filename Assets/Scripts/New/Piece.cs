using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public static event Action<PlayerTeam> OnPieceMovedToEndPool;

    [SerializeField] PlayerTeam owner;

    int currentPathPositionIndex = -1;
    Vector2 currentGridPosition;
    Tile currentTile;

    Pool startPool;
    Pool endPool;

    public bool isInEndPool { get; private set; } = false;
    public Pool GetStartPool() => startPool;
    public Pool GetEndPool() => endPool;
    public Vector2 GetCurrentGridPosition() => currentGridPosition;
    public int GetCurrentPathPositionIndex() => currentPathPositionIndex;

    public void Setup(Pool startPool, Pool endPool)
    {
        this.startPool = startPool;
        this.endPool = endPool;
    }

    public Tile GetCurrentTile()
    {
        return currentTile;
    }

    public void SetCurrentTile(Tile tile)
    {
        currentTile = tile;
    }

    public void ReturnToStartPool()
    {
        if (currentTile != null)
            currentTile.ClearPiece();

        currentTile = null;
        currentPathPositionIndex = -1;

        //transform.position = startPool.transform.position;
        startPool.AddPiece(this);
    }

    public void SendToEndPool()
    {
        currentTile.ClearPiece();

        currentTile = null;
        currentPathPositionIndex = -1;

        isInEndPool = true;

        //transform.position = endPool.transform.position;
        endPool.AddPiece(this);

        OnPieceMovedToEndPool?.Invoke(owner);
    }

    public PlayerTeam GetOwner()
    {
        return owner;
    }
}
