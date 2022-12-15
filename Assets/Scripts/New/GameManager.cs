using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMode
{
    LocalMultiplayer,
    SinglePlayer,
    RemoteMultiplayer,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameBoard gameBoard;

    PlayerTeam currentPlayerTurn = PlayerTeam.White;

    public PlayerTeam GetCurrentPlayerTurn() => currentPlayerTurn;

    public void SetCurrentPlayerTurn(PlayerTeam currentPlayerTurn)
    {
        this.currentPlayerTurn = currentPlayerTurn;
        // if (currentPlayerTurn == PlayerTeam.Black)
        // {
        //     AIPlayer.Instance.TakeTurn();
        // }
    }

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one instance of GameManager in this scene!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        gameBoard.Setup();
    }
}
