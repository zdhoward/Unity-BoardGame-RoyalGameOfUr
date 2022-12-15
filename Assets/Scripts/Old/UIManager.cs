using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    TextMeshProUGUI currentTurnLabel;
    TextMeshProUGUI currentRollLabel;

    Button rollButton;

    void Awake()
    {
        currentTurnLabel = transform.Find("CurrentTurnLabel").GetComponent<TextMeshProUGUI>();
        currentRollLabel = transform.Find("CurrentRollLabel").GetComponent<TextMeshProUGUI>();

        rollButton = transform.Find("RollButton").GetComponent<Button>();
    }

    void OnEnable()
    {
        UrGameManager.OnEndTurn += GameManager_OnEndTurn;
        UrGameManager.OnRoll += GameManager_OnRoll;
    }

    void OnDisable()
    {
        UrGameManager.OnEndTurn -= GameManager_OnEndTurn;
        UrGameManager.OnRoll -= GameManager_OnRoll;
    }

    void GameManager_OnEndTurn(Players player)
    {
        currentTurnLabel.text = $"Current Turn: {player}";
        currentRollLabel.text = $"Current Roll: Awaiting Roll...";
        rollButton.interactable = true;

    }

    void GameManager_OnRoll(int roll)
    {
        currentRollLabel.text = $"Current Roll: {roll}";
        rollButton.interactable = false;
    }
}
