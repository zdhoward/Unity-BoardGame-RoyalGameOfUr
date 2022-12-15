using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    [SerializeField] bool stackDownwards = false;

    List<Piece> pieces = new List<Piece>();

    float pieceOffset = .25f;
    int stackDirection = 1;

    void Awake()
    {
        if (stackDownwards)
            stackDirection = -1;
    }

    public void AddPiece(Piece piece)
    {
        if (!pieces.Contains(piece))
            pieces.Add(piece);
        StackPieces();
    }

    public void RemovePiece(Piece piece)
    {
        pieces.Remove(piece);
        StackPieces();
    }

    public void StackPieces()
    {
        for (int i = 0; i < pieces.Count; i++)
        {

            Vector3 offsetPosition = new Vector3(0, pieceOffset * i * stackDirection, 0);
            pieces[i].transform.position = transform.position + offsetPosition;
            pieces[i].GetComponent<SpriteRenderer>().sortingOrder = i;
        }
    }
}
