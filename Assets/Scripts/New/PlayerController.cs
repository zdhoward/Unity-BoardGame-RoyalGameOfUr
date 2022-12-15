using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameBoard gameBoard;

    [SerializeField] LayerMask tileLayerMask;
    [SerializeField] LayerMask pieceLayerMask;

    Camera mainCamera;

    // Drag vars
    Piece selectedPiece;
    Vector3 dragScreenPoint;
    Vector3 dragOffset;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        HighlightTileOnHover();

        HandleDragging();
    }

    #region Debug
    void HighlightTileOnHover()
    {
        Tile tile = GetTileAtMousePosition();

        if (tile == null)
        {
            gameBoard.ClearHighlights();
            return;
        }

        gameBoard.HighlightTileOnHover(tile);
    }
    #endregion

    #region Helpers
    Tile GetTileAtMousePosition()
    {
        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, tileLayerMask);

        if (hit.collider == null)
            return null;

        if (hit.collider.TryGetComponent<Tile>(out Tile tile))
        {
            return tile;
        }

        return null;
    }

    Piece GetPieceAtMousePosition()
    {
        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, pieceLayerMask);

        if (hit.collider == null)
            return null;

        if (hit.collider.TryGetComponent<Piece>(out Piece piece))
        {
            return piece;
        }

        return null;
    }
    #endregion

    #region DragPieces
    void HandleDragging()
    {
        if (Input.GetMouseButtonDown(0))
            StartDrag();
        else if (Input.GetMouseButtonUp(0))
            StopDrag();
        else if (Input.GetMouseButton(0))
            ContinueDrag();
    }

    void StartDrag()
    {
        if (!Dice.Instance.HasRolled())
            return;

        selectedPiece = GetPieceAtMousePosition();

        if (selectedPiece == null)
            return;

        if (selectedPiece.isInEndPool)
            return;

        if (selectedPiece.GetOwner() != GameManager.Instance.GetCurrentPlayerTurn())
        {
            selectedPiece = null;
            return;
        }

        dragOffset = selectedPiece.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragScreenPoint.z));
    }

    void StopDrag()
    {
        if (selectedPiece == null)
            return;

        Tile tile = GetTileAtMousePosition();

        // Handle moving to EndPool
        if (gameBoard.GetPositionIndexOfTile(selectedPiece.GetCurrentTile()) + Dice.Instance.GetCurrentRoll() == gameBoard.GetPathLength() && tile == null)
        {
            selectedPiece.SendToEndPool();
            gameBoard.EndTurn();
        }
        else if (tile == null)
        {
            Tile fromTile = selectedPiece.GetCurrentTile();
            if (fromTile == null)
                selectedPiece.ReturnToStartPool();
            else
                gameBoard.CancelMove(selectedPiece);
        }
        else
        {
            if (gameBoard.IsValidMove(selectedPiece, tile))
                gameBoard.MovePiece(selectedPiece, tile);
            else
                gameBoard.CancelMove(selectedPiece);

        }
        ResetDrag();
    }

    void ContinueDrag()
    {
        if (selectedPiece == null)
            return;

        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragScreenPoint.z);
        Vector3 curPosition = mainCamera.ScreenToWorldPoint(curScreenPoint) + dragOffset;
        selectedPiece.transform.position = curPosition;
    }

    void ResetDrag()
    {
        selectedPiece = null;
    }
    #endregion
}
