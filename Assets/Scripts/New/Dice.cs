using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public static Dice Instance;

    public static event Action<int> OnRoll;

    List<int> dice = new List<int>() { 0, 0, 0, 0 };

    int currentRoll = 0;

    bool hasRolled = false;

    public int GetCurrentRoll() => currentRoll;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one instance of Dice in this scene");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !hasRolled)
            Roll();
    }

    public void Roll()
    {
        hasRolled = true;

        currentRoll = 0;

        for (int i = 0; i < dice.Count; i++)
        {
            dice[i] = UnityEngine.Random.Range(0, 4);
            if (dice[i] < 2)
                currentRoll++;
        }

        OnRoll?.Invoke(currentRoll);

        if (currentRoll == 0 || !GameBoard.Instance.HasValidMoves())
            GameBoard.Instance.EndTurn();
    }

    public bool HasRolled()
    {
        return hasRolled;
    }

    public void ResetRoll()
    {
        hasRolled = false;
        currentRoll = 0;
        dice = new List<int>() { 0, 0, 0, 0 };
    }
}
