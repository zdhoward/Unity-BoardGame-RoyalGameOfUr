using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameBoard : MonoBehaviour
{
    public static GameBoard Instance;

    public static event Action<PlayerTeam> OnEndTurn;
    public static event Action<PlayerTeam> OnPieceCaptured;
    public static event Action<PlayerTeam> OnRollAgain;
    public static event Action<PlayerTeam> OnHasNoValidMoves;
    public static event Action<PlayerTeam> OnMoveBlockedBySafeTile;
    public static event Action<PlayerTeam> OnPlayerWin;

    [SerializeField] Tile tilePrefab;
    [SerializeField] Piece whitePiecePrefab;
    [SerializeField] Piece blackPiecePrefab;

    [SerializeField] Pool whiteStartPool;
    [SerializeField] Pool whiteEndPool;
    [SerializeField] Pool blackStartPool;
    [SerializeField] Pool blackEndPool;

    Camera mainCamera;

    int width = 3;
    int height = 8;

    int numberOfPiecesPerPlayer = 5;

    Vector2[] squaresToNotspawn = { new Vector2(0, 4), new Vector2(0, 5), new Vector2(2, 4), new Vector2(2, 5) };

    Dictionary<Vector2, Tile> tiles;

    List<Piece> whitePieces = new List<Piece>();
    List<Piece> blackPieces = new List<Piece>();

    List<Vector2> whitePath = new List<Vector2> {new Vector2(0,3), new Vector2(0,2), new Vector2(0,1), new Vector2(0,0),
    new Vector2(1,0), new Vector2(1,1), new Vector2(1, 2), new Vector2(1,3), new Vector2(1,4), new Vector2(1,5), new Vector2(1,6), new Vector2(1,7),
    new Vector2(0,7), new Vector2(0,6)};

    List<Vector2> blackPath = new List<Vector2> {new Vector2(2,3), new Vector2(2,2), new Vector2(2,1), new Vector2(2,0),
    new Vector2(1,0), new Vector2(1,1), new Vector2(1, 2), new Vector2(1,3), new Vector2(1,4), new Vector2(1,5), new Vector2(1,6), new Vector2(1,7),
    new Vector2(2,7), new Vector2(2,6)};

    #region Setup
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one instance of GameBoard in this scene!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Setup()
    {
        mainCamera = Camera.main;

        SetupTiles();
        SetupPieces();

        OnEndTurn?.Invoke(GameManager.Instance.GetCurrentPlayerTurn());
    }

    void SetupTiles()
    {
        tiles = new Dictionary<Vector2, Tile>();

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

                Tile spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity, transform);
                spawnedTile.Setup(new Vector2(x, y));
                tiles.Add(new Vector2(x, y), spawnedTile);
            }
        }

        mainCamera.transform.position = new Vector3((float)width / 2f - 0.5f, (float)height / 2f - 0.5f, -10f);
    }

    void SetupPieces()
    {
        for (int i = 0; i < numberOfPiecesPerPlayer; i++)
        {
            Piece piece = Instantiate(whitePiecePrefab, whiteStartPool.transform.position, Quaternion.identity);
            piece.Setup(whiteStartPool, whiteEndPool);
            whitePieces.Add(piece);

            whiteStartPool.AddPiece(piece);
        }
        whiteStartPool.StackPieces();

        for (int i = 0; i < numberOfPiecesPerPlayer; i++)
        {
            Piece piece = Instantiate(blackPiecePrefab, blackStartPool.transform.position, Quaternion.identity);
            piece.Setup(blackStartPool, blackEndPool);
            blackPieces.Add(piece);

            blackStartPool.AddPiece(piece);
        }
        blackStartPool.StackPieces();
    }
    #endregion

    #region Validations
    public bool IsValidMove(Piece piece, Tile targetTile)
    {
        // Is dragged from pool to somewhere not on the board
        if (targetTile == null && piece.GetCurrentTile() == null)
            return false;

        // Does the current roll take the selected piece to the end pool?
        if (targetTile == null && GetPositionIndexOfTile(piece.GetCurrentTile()) + Dice.Instance.GetCurrentRoll() == whitePath.Count)
            return true;

        // Is the target tile in player's path?
        if (targetTile != null && GameManager.Instance.GetCurrentPlayerTurn() == PlayerTeam.White && !whitePath.Contains(targetTile.GetGridPosition()))
            return false;
        if (targetTile != null && GameManager.Instance.GetCurrentPlayerTurn() == PlayerTeam.Black && !blackPath.Contains(targetTile.GetGridPosition()))
            return false;

        // Does the target tile have a piece?
        if (targetTile != null && targetTile.GetPiece() != null && targetTile.GetPiece().GetOwner() == GameManager.Instance.GetCurrentPlayerTurn())
            return false;

        // Is the target position equal to currentPosition + roll?
        int pieceOriginalTilePathPositionIndex = -1;
        if (piece.GetCurrentTile() != null)
            pieceOriginalTilePathPositionIndex = GetPositionIndexOfTile(piece.GetCurrentTile());
        if (GetPositionIndexOfTile(targetTile) != pieceOriginalTilePathPositionIndex + Dice.Instance.GetCurrentRoll())
            return false;

        // Is the target tile a safe tile with an enemy piece on it?
        if (targetTile.GetTileType() == TileType.Safe && targetTile.HasPiece())
        {
            OnMoveBlockedBySafeTile?.Invoke(GameManager.Instance.GetCurrentPlayerTurn());
            return false;
        }

        return true;
    }

    public bool HasValidMoves()
    {
        if (GameManager.Instance.GetCurrentPlayerTurn() == PlayerTeam.White)
        {
            //for all white pieces that are not in endPool
            foreach (Piece piece in whitePieces)
            {
                Tile targetTile = GetTileAtPositionIndex(whitePath, GetPositionIndexOfTile(piece.GetCurrentTile()) + Dice.Instance.GetCurrentRoll());
                if (IsValidMove(piece, targetTile))
                {
                    return true;
                }
            }
        }
        else
        {
            //for all white pieces that are not in endPool
            foreach (Piece piece in blackPieces)
            {
                Tile targetTile = GetTileAtPositionIndex(blackPath, GetPositionIndexOfTile(piece.GetCurrentTile()) + Dice.Instance.GetCurrentRoll());
                if (IsValidMove(piece, targetTile))
                {
                    return true;
                }
            }
        }

        OnHasNoValidMoves?.Invoke(GameManager.Instance.GetCurrentPlayerTurn());
        return false;
    }

    public List<AIMove> GetValidMoves()
    {
        List<AIMove> validMoves = new List<AIMove>();

        if (GameManager.Instance.GetCurrentPlayerTurn() == PlayerTeam.White)
        {
            //for all white pieces that are not in endPool
            foreach (Piece piece in whitePieces)
            {
                Tile targetTile = GetTileAtPositionIndex(whitePath, GetPositionIndexOfTile(piece.GetCurrentTile()) + Dice.Instance.GetCurrentRoll());
                if (IsValidMove(piece, targetTile))
                {
                    bool canMoveToEndPool = false;
                    if (targetTile == null)
                        canMoveToEndPool = true;
                    validMoves.Add(new AIMove(piece, targetTile, canMoveToEndPool));
                    //return true;
                }
            }
        }
        else
        {
            //for all white pieces that are not in endPool
            foreach (Piece piece in blackPieces)
            {
                Tile targetTile = GetTileAtPositionIndex(blackPath, GetPositionIndexOfTile(piece.GetCurrentTile()) + Dice.Instance.GetCurrentRoll());
                if (IsValidMove(piece, targetTile))
                {
                    bool canMoveToEndPool = false;
                    if (targetTile == null)
                        canMoveToEndPool = true;
                    validMoves.Add(new AIMove(piece, targetTile, canMoveToEndPool));
                    //return true;
                }
            }
        }

        return validMoves;
    }
    #endregion

    #region Actions
    public void MovePiece(Piece piece, Tile tile)
    {
        Tile currentTile = piece.GetCurrentTile();

        // Handle if the tile has an enemy piece on the tile
        Piece tileCurrentPiece = tile.GetPiece();
        if (tile.HasPiece() && tileCurrentPiece.GetOwner() != GameManager.Instance.GetCurrentPlayerTurn())
        {
            tileCurrentPiece.ReturnToStartPool();
            OnPieceCaptured?.Invoke(GameManager.Instance.GetCurrentPlayerTurn());
        }

        if (currentTile != null)
            currentTile.ClearPiece();
        else
            piece.GetStartPool().RemovePiece(piece);

        tile.SetPiece(piece);
        piece.SetCurrentTile(tile);
        piece.transform.position = tile.transform.position;

        if (tile.GetTileType() == TileType.RollAgain)
        {
            OnRollAgain?.Invoke(GameManager.Instance.GetCurrentPlayerTurn());
            EndTurn(false);
        }
        else
            EndTurn();
    }

    public void CancelMove(Piece piece)
    {
        Tile tile = piece.GetCurrentTile();
        if (tile == null)
            piece.ReturnToStartPool();
        else
            piece.transform.position = tile.transform.position;
    }
    #endregion

    #region Debug
    public void HighlightTileOnHover(Tile tile)
    {
        ClearHighlights();

        tile.ActivateHighlight();
    }

    public void ClearHighlights()
    {
        foreach (KeyValuePair<Vector2, Tile> item in tiles.ToList())
        {
            item.Value.DeactivateHighlight();
        }
    }
    #endregion

    public void EndTurn(bool shouldChangePlayer = true)
    {
        if (CheckForWin())
            OnPlayerWin?.Invoke(GameManager.Instance.GetCurrentPlayerTurn());

        if (shouldChangePlayer)
        {
            if (GameManager.Instance.GetCurrentPlayerTurn() == PlayerTeam.White)
                GameManager.Instance.SetCurrentPlayerTurn(PlayerTeam.Black);
            else if (GameManager.Instance.GetCurrentPlayerTurn() == PlayerTeam.Black)
                GameManager.Instance.SetCurrentPlayerTurn(PlayerTeam.White);
        }

        Dice.Instance.ResetRoll();

        OnEndTurn?.Invoke(GameManager.Instance.GetCurrentPlayerTurn());
    }

    bool CheckForWin()
    {
        if (GameManager.Instance.GetCurrentPlayerTurn() == PlayerTeam.White)
        {
            foreach (Piece piece in whitePieces)
                if (!piece.isInEndPool)
                    return false;
        }
        else
        {
            foreach (Piece piece in blackPieces)
                if (!piece.isInEndPool)
                    return false;
        }

        return true;
    }

    public int GetPositionIndexOfTile(Tile tile)
    {
        if (tile == null)
            return -1;

        Vector2 gridPosition = tile.GetGridPosition();
        int pathPositionIndex = -1;
        if (GameManager.Instance.GetCurrentPlayerTurn() == PlayerTeam.White)
            pathPositionIndex = whitePath.IndexOf(gridPosition);
        if (GameManager.Instance.GetCurrentPlayerTurn() == PlayerTeam.Black)
            pathPositionIndex = blackPath.IndexOf(gridPosition);
        return pathPositionIndex;
    }
    public int GetPathLength()
    {
        return whitePath.Count;
    }

    Tile GetTileAtPositionIndex(List<Vector2> path, int index)
    {
        if (index < path.Count)
            if (tiles.TryGetValue(path[index], out Tile tile))
                return tile;

        return null;
    }
}
