using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoyalGameOfUr : MonoBehaviour
{
    // GameBoard
    //// Vars
    ////// Tile[] allTiles
    ////// Piece[] whitePieces
    ////// Piece[] blackPieces
    ////// List<Vector2> whitePath
    ////// List<Vector2> blackPath
    ////// Transform whiteStartPool
    ////// Transform whiteEndPool
    ////// Transform blackStartPool
    ////// Transform blackEndPool
    //// Setup
    ////// SpawnTiles()
    ////// ApplySpritesToTiles()
    //// Helpers
    ////// GetTileAtGridPosition(Vector2 gridPosition)
    ////// GetTileAtMousePosition()
    ////// ActivateHighlightAtGridPosition(Vector2 gridPosition)
    ////// DeactivateAllHighlights()
    //// Actions
    ////// TryMovePiece(Piece piece, Vector2 targetPosition)
    //// Validations
    ////// IsValidMove(Piece piece, Vector2 targetPosition)

    // Tile
    //// Vars
    ////// Sprite sprite
    ////// Piece currentPiece
    ////// Vector2 gridPosition
    ////// bool isRollAgainTile
    ////// bool isSafeTile
    //// Setup
    ////// Setup(Sprite sprite, Vector2 gridPosition)
    //// Helpers
    ////// HasPiece()
    ////// GetPiece()
    ////// GetGridPosition()

    // Piece
    //// Vars
    ////// Tile currentTile
}
