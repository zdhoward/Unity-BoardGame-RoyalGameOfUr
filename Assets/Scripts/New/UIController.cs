using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI notificationLabelPrefab;

    [SerializeField] TextMeshProUGUI currentTurnLabel;
    [SerializeField] TextMeshProUGUI currentRollLabel;

    Transform notificationSpawnPosition;

    RectTransform winScreen;
    TextMeshProUGUI winScreenLabel;

    Button rollButton;

    void Awake()
    {
        winScreen = transform.Find("WinScreen").GetComponent<RectTransform>();
        winScreenLabel = winScreen.GetComponentInChildren<TextMeshProUGUI>();

        //currentTurnLabel = transform.Find("CurrentTurnLabel").GetComponent<TextMeshProUGUI>();
        //currentRollLabel = transform.Find("CurrentRollLabel").GetComponent<TextMeshProUGUI>();

        notificationSpawnPosition = transform.Find("NotificationSpawnPosition");

        rollButton = transform.Find("RollButton").GetComponent<Button>();
    }

    void OnEnable()
    {
        GameBoard.OnEndTurn += GameBoard_OnEndTurn;
        Dice.OnRoll += Dice_OnRoll;
        GameBoard.OnPieceCaptured += GameBoard_OnPieceCaptured;
        GameBoard.OnRollAgain += GameBoard_OnRollAgain;
        GameBoard.OnHasNoValidMoves += GameBoard_OnHasNoValidMoves;
        GameBoard.OnMoveBlockedBySafeTile += GameBoard_OnMoveBlockedBySafeTile;
        GameBoard.OnPlayerWin += GameBoard_OnPlayerWin;
        Piece.OnPieceMovedToEndPool += Piece_OnMovedToEndPool;
    }

    void OnDisable()
    {
        GameBoard.OnEndTurn -= GameBoard_OnEndTurn;
        Dice.OnRoll -= Dice_OnRoll;
        GameBoard.OnPieceCaptured -= GameBoard_OnPieceCaptured;
        GameBoard.OnRollAgain -= GameBoard_OnRollAgain;
        GameBoard.OnHasNoValidMoves -= GameBoard_OnHasNoValidMoves;
        GameBoard.OnMoveBlockedBySafeTile -= GameBoard_OnMoveBlockedBySafeTile;
        GameBoard.OnPlayerWin -= GameBoard_OnPlayerWin;
        Piece.OnPieceMovedToEndPool -= Piece_OnMovedToEndPool;
    }

    void ShowWinScreen(string labelText)
    {
        winScreenLabel.text = labelText;
        winScreen.LeanMoveLocalY(0, 1f).setEaseOutElastic();
    }

    #region ButtonLogic
    public void Button_ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

    #region Callbacks
    void GameBoard_OnEndTurn(PlayerTeam player)
    {
        currentTurnLabel.text = $"{player}'s Turn";
        currentRollLabel.fontSize = 50;
        currentRollLabel.text = $"Awaiting Roll";
        rollButton.interactable = true;
    }

    void Dice_OnRoll(int roll)
    {
        currentRollLabel.fontSize = 100;
        currentRollLabel.text = $"{roll}";
        rollButton.interactable = false;

        NotificationSystem.Instance.QueueNotification($"{GameManager.Instance.GetCurrentPlayerTurn()} rolled a {roll}");

        if (roll == 0)
            NotificationSystem.Instance.QueueNotification($"{GameManager.Instance.GetCurrentPlayerTurn()} misses their turn");
    }

    void GameBoard_OnPieceCaptured(PlayerTeam player)
    {
        NotificationSystem.Instance.QueueNotification($"{player} captured a piece");
    }

    void GameBoard_OnRollAgain(PlayerTeam player)
    {
        NotificationSystem.Instance.QueueNotification($"{player} gets to roll again");
    }

    void GameBoard_OnHasNoValidMoves(PlayerTeam player)
    {
        NotificationSystem.Instance.QueueNotification($"{player} has no valid moves, misses turn");
    }

    void GameBoard_OnMoveBlockedBySafeTile(PlayerTeam player)
    {
        NotificationSystem.Instance.QueueNotification($"{player}'s move blocked by safe tile");
    }

    void GameBoard_OnPlayerWin(PlayerTeam player)
    {
        NotificationSystem.Instance.QueueNotification($"{player} has won!");
        ShowWinScreen($"{player} Wins!");
    }

    void Piece_OnMovedToEndPool(PlayerTeam player)
    {
        NotificationSystem.Instance.QueueNotification($"{player} moved a piece to the end pool");
    }
    #endregion
}
