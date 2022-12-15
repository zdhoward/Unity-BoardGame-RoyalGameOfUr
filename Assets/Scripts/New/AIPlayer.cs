using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    public static AIPlayer Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one instance of AIPlayer in this scene!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void TakeTurn()
    {
        Dice.Instance.Roll();
        List<AIMove> aiMoves = GetValidMoves();
        AIMove bestMove = FindBestMove(aiMoves);
        TakeAction(bestMove);
    }

    List<AIMove> GetValidMoves()
    {
        Debug.Log("Getting Valid Moves");
        List<AIMove> validMoves = GameBoard.Instance.GetValidMoves(); //new List<AIMove>();
        //Debug.Log(validMoves.Count + " moves found");

        return validMoves;
    }

    AIMove FindBestMove(List<AIMove> aiMoves)
    {
        Debug.Log("Finding Best Move");
        AIMove bestMove = aiMoves[0];

        foreach (AIMove move in aiMoves)
        {
            if (move.GetValue() > bestMove.GetValue())
            {
                bestMove = move;
            }
        }

        return bestMove;
    }

    void TakeAction(AIMove bestMove)
    {
        Debug.Log("Taking Action");
        Debug.Log(bestMove);
    }
}

public struct AIMove
{
    Piece piece;
    Tile moveToTile;
    bool canMoveToEndPool;
    int value;

    public int GetValue() => value;

    public AIMove(Piece piece, Tile moveToTile, bool canMoveToEndPool = false)
    {
        this.piece = piece;
        this.moveToTile = moveToTile;
        this.canMoveToEndPool = canMoveToEndPool;
        this.value = 0;
        this.value = CalculateValue();
    }

    int CalculateValue()
    {
        int endPoolAndSafeValue = 1;
        int endPoolAndUnsafeValue = 9;

        int safeValue = 8;

        int rollAgainValue = 10;

        int normalValue = 5;

        //if (moveToTile == null && piece.GetPosi)
        if (moveToTile != null && moveToTile.GetTileType() == TileType.Safe)
            return safeValue;
        if (moveToTile != null && moveToTile.GetTileType() == TileType.RollAgain)
            return rollAgainValue;

        return normalValue;
    }

    public override string ToString()
    {
        string asString = "";
        asString += "Owner: " + piece.GetOwner() + " | ";
        //asString += "From: " + piece.GetCurrentGridPosition() + " | ";
        //asString += "To: " + moveToTile.GetGridPosition() + " | ";
        asString += "Can Move To End: " + canMoveToEndPool + " | ";
        asString += "MoveValue: " + value;
        return asString;
    }
}
