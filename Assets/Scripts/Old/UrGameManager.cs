using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UrGameManager : MonoBehaviour
{
    public static event Action<Players> OnEndTurn;
    public static event Action<int> OnRoll;

    [SerializeField] PlayingPiece whitePiecePrefab;
    [SerializeField] PlayingPiece blackPiecePrefab;

    LayerMask tileLayerMask;
    LayerMask pieceLayerMask;

    Camera mainCamera;
    GridManager gridManager;

    PlayingPiece selectedPiece;
    Vector3 pieceScreenPoint;
    Vector3 pieceOffset;
    Vector3 pieceOriginalPosition;
    UrTile pieceOriginalTile;
    bool hasPieceSelected = false;

    UrTile selectedTile;

    List<Vector2> whitePath = new List<Vector2> {new Vector2(0,3), new Vector2(0,2), new Vector2(0,1), new Vector2(0,0),
    new Vector2(1,0), new Vector2(1,1), new Vector2(1, 2), new Vector2(1,3), new Vector2(1,4), new Vector2(1,5), new Vector2(1,6), new Vector2(1,7),
    new Vector2(0,7), new Vector2(0,6)};

    List<Vector2> blackPath = new List<Vector2> {new Vector2(2,3), new Vector2(2,2), new Vector2(2,1), new Vector2(2,0),
    new Vector2(1,0), new Vector2(1,1), new Vector2(1, 2), new Vector2(1,3), new Vector2(1,4), new Vector2(1,5), new Vector2(1,6), new Vector2(1,7),
    new Vector2(2,7), new Vector2(2,6)};

    List<Vector2> rollAgainTiles = new List<Vector2> { new Vector2(0, 0), new Vector2(0, 6), new Vector2(2, 0), new Vector2(2, 6) };
    Vector2 safeTile = new Vector2(1, 3);

    List<PlayingPiece> whitePieces = new List<PlayingPiece>();
    List<PlayingPiece> blackPieces = new List<PlayingPiece>();

    int numberOfPiecesPerPlayer = 5;

    Players currentPlayer = Players.White;

    int currentRoll = 0;

    bool hasRolled = false;

    #region MonoBehaviours
    void Awake()
    {
        mainCamera = Camera.main;
        gridManager = GameObject.FindGameObjectWithTag("GridManager").GetComponent<GridManager>();

        tileLayerMask = LayerMask.GetMask("Tile");
        pieceLayerMask = LayerMask.GetMask("Piece");
    }

    void Start()
    {
        SpawnPieces();
        OnEndTurn?.Invoke(currentPlayer);
    }

    void Update()
    {
        HandleHighlight();

        if (!hasRolled)
            return;

        HandleSelectedPieces();
    }
    #endregion

    #region Setup
    void SpawnPieces()
    {
        Transform whitePieceStartPool = GameObject.FindGameObjectWithTag("WhitePieceStartPool").transform;
        Transform blackPieceStartPool = GameObject.FindGameObjectWithTag("BlackPieceStartPool").transform;

        for (int i = 0; i < numberOfPiecesPerPlayer; i++)
            whitePieces.Add(Instantiate(whitePiecePrefab, whitePieceStartPool.position, Quaternion.identity));

        for (int i = 0; i < numberOfPiecesPerPlayer; i++)
            blackPieces.Add(Instantiate(blackPiecePrefab, blackPieceStartPool.position, Quaternion.identity));
    }
    #endregion

    #region CoreLoop
    void HandleHighlight()
    {
        void DeselectAll()
        {
            foreach (KeyValuePair<Vector2, UrTile> item in gridManager.Tiles.ToList())
            {
                item.Value.Deselect();
            }
            selectedTile = null;
        }

        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, tileLayerMask);

        if (hit.collider == null)
        {
            DeselectAll();
            return;
        }

        if (hit.collider.TryGetComponent<UrTile>(out UrTile tile))
        {
            DeselectAll();
            tile.Select();
            selectedTile = tile;
        }
    }

    void HandleSelectedPieces()
    {
        if (!hasPieceSelected)
        {
            if (Input.GetMouseButtonDown(0))
            {
                selectedPiece = GetPieceAtMousePosition();

                if (!SelectionIsValid())
                    return;

                SelectPieceAtMousePosition();
            }

            if (selectedPiece != null)
                hasPieceSelected = true;
        }

        if (hasPieceSelected)
        {
            // Handle Dragging
            if (Input.GetMouseButton(0))
            {
                StartDrag();
            }

            // Handle Mouse Button Up
            if (Input.GetMouseButtonUp(0))
            {
                HandleDrag();
            }
        }
    }
    #endregion

    #region SelectAndDrag
    void SelectPieceAtMousePosition()
    {
        pieceOriginalTile = GetTileAtMousePosition();
        pieceOriginalPosition = selectedPiece.transform.position;
        pieceOffset = selectedPiece.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, pieceScreenPoint.z));
    }

    void StartDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, pieceScreenPoint.z);
        Vector3 curPosition = mainCamera.ScreenToWorldPoint(curScreenPoint) + pieceOffset;
        selectedPiece.transform.position = curPosition;
    }

    void HandleDrag()
    {
        UrTile tile = GetTileAtMousePosition();

        // Handle endPool
        if (tile == null && GetPositionIndexOfTile(pieceOriginalTile) + currentRoll == whitePath.Count)
        {
            selectedPiece.SendToEndPool();
            pieceOriginalTile.SetPiece(null);

            EndTurn();
        }
        // Handle normal movement
        else if (tile != null && MoveIsValid(selectedPiece, tile, pieceOriginalTile))
        {
            selectedPiece.transform.position = tile.transform.position;
            selectedPiece.SetCurrentPositionIndex(GetPositionIndexOfTile(tile));
            if (pieceOriginalTile != null)
                pieceOriginalTile.SetPiece(null);
            if (tile.GetPiece() != null)
                tile.GetPiece().ReturnToStartPool();
            tile.SetPiece(selectedPiece);

            if (rollAgainTiles.Contains(tile.GetGridPosition()))
                EndTurn(false);
            else
                EndTurn();
        }
        else
        {
            selectedPiece.transform.position = pieceOriginalPosition;
        }

        hasPieceSelected = false;
        selectedPiece = null;
        pieceOriginalTile = null;
    }
    #endregion

    #region Validations
    bool SelectionIsValid()
    {
        if (selectedPiece == null)
            return false;

        if (selectedPiece.inEndPool)
        {
            selectedPiece = null;
            return false;
        }

        if (selectedPiece.GetOwner() != currentPlayer)
        {
            selectedPiece = null;
            return false;
        }

        return true;
    }

    bool MoveIsValid(PlayingPiece piece, UrTile targetTile, UrTile originalTile)
    {
        // Is dragged from pool to somewhere not on the board
        if (targetTile == null && originalTile == null)
            return false;

        // Is the target tile in player's path?
        if (currentPlayer == Players.White && !whitePath.Contains(targetTile.GetGridPosition()))
            return false;
        if (currentPlayer == Players.Black && !blackPath.Contains(targetTile.GetGridPosition()))
            return false;

        // Does the target tile have a piece?
        if (targetTile.GetPiece() != null && targetTile.GetPiece().GetOwner() == currentPlayer)
            return false;

        // Is the target position equal to currentPosition + roll?
        int pieceOriginalTilePathPositionIndex = -1;
        if (pieceOriginalTile != null)
            pieceOriginalTilePathPositionIndex = GetPositionIndexOfTile(pieceOriginalTile);
        if (GetPositionIndexOfTile(targetTile) != pieceOriginalTilePathPositionIndex + currentRoll)
            return false;

        // Is the target tile a safe tile with an enemy piece on it?
        if (targetTile.GetGridPosition() == safeTile && targetTile.GetPiece() != null)
            return false;


        return true;
    }

    bool CheckForWin()
    {
        if (currentPlayer == Players.White)
        {
            foreach (PlayingPiece piece in whitePieces)
                if (!piece.inEndPool)
                    return false;
        }
        else
        {
            foreach (PlayingPiece piece in blackPieces)
                if (!piece.inEndPool)
                    return false;
        }

        return true;
    }

    bool HasValidMoves()
    {
        if (currentPlayer == Players.White)
        {
            //for all white pieces that are not in endPool
            foreach (PlayingPiece piece in whitePieces)
            {
                var fromTile = GetTileAtPathPositionIndex(whitePath, piece.GetCurrentPositionIndex());
                var toTile = GetTileAtPathPositionIndex(whitePath, piece.GetCurrentPositionIndex() + currentRoll);
                if (MoveIsValid(piece, fromTile, toTile))
                {
                    return true;
                }
            }
        }
        else
        {
            //for all white pieces that are not in endPool
            foreach (PlayingPiece piece in blackPieces)
            {
                var fromTile = GetTileAtPathPositionIndex(blackPath, piece.GetCurrentPositionIndex());
                var toTile = GetTileAtPathPositionIndex(blackPath, piece.GetCurrentPositionIndex() + currentRoll);
                if (MoveIsValid(piece, fromTile, toTile))
                {
                    return true;
                }
            }
        }

        return false;
    }
    #endregion

    #region Helpers
    int GetPositionIndexOfTile(UrTile tile)
    {
        if (tile == null)
            return -1;

        Vector2 gridPosition = tile.GetGridPosition();
        int pathPositionIndex = -1;
        if (currentPlayer == Players.White)
            pathPositionIndex = whitePath.IndexOf(gridPosition);
        if (currentPlayer == Players.Black)
            pathPositionIndex = blackPath.IndexOf(gridPosition);
        return pathPositionIndex;
    }

    UrTile GetTileAtMousePosition()
    {
        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, tileLayerMask);
        if (hit.collider != null)
            return hit.collider.GetComponent<UrTile>();

        return null;
    }

    UrTile GetTileAtPathPositionIndex(List<Vector2> path, int index)
    {
        return gridManager.GetTileAtPosition(path[index]); ;
    }

    PlayingPiece GetPieceAtMousePosition()
    {
        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, pieceLayerMask);
        if (hit.collider != null)
            return hit.collider.GetComponent<PlayingPiece>();

        return null;
    }
    #endregion

    #region Public
    public void EndTurn(bool shouldChangePlayer = true)
    {
        if (CheckForWin())
            Debug.Log($"{currentPlayer} has won!");

        if (shouldChangePlayer)
        {
            if (currentPlayer == Players.White)
                currentPlayer = Players.Black;
            else if (currentPlayer == Players.Black)
                currentPlayer = Players.White;
        }

        hasRolled = false;

        OnEndTurn?.Invoke(currentPlayer);
    }

    public void Roll()
    {
        hasRolled = true;

        currentRoll = 0;
        for (int i = 0; i < 4; i++)
        {
            int roll = UnityEngine.Random.Range(0, 4);
            if (roll < 2)
            {
                currentRoll++;
            }
        }

        OnRoll?.Invoke(currentRoll);

        if (currentRoll == 0)//|| !HasValidMoves())
            EndTurn();
    }
    #endregion
}
