using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingPiece : MonoBehaviour
{
    [SerializeField] Players owner;
    Transform startPool;
    Transform endPool;

    int currentPositionIndex = -1;

    public bool inStartPool { get; private set; } = true;
    public bool inEndPool { get; private set; } = false;

    void Start()
    {
        if (owner == Players.White)
        {
            startPool = GameObject.FindGameObjectWithTag("WhitePieceStartPool").transform;
            endPool = GameObject.FindGameObjectWithTag("WhitePieceEndPool").transform;
        }
        else
        {
            startPool = GameObject.FindGameObjectWithTag("BlackPieceStartPool").transform;
            endPool = GameObject.FindGameObjectWithTag("BlackPieceEndPool").transform;
        }
    }

    public void SetCurrentPositionIndex(int index)
    {
        currentPositionIndex = index;
    }

    public int GetCurrentPositionIndex()
    {
        return currentPositionIndex;
    }

    public Players GetOwner()
    {
        return owner;
    }

    public void ReturnToStartPool()
    {
        currentPositionIndex = -1;
        transform.position = startPool.position;
    }

    public void SendToEndPool()
    {
        transform.position = endPool.position;
        inEndPool = true;
    }
}
