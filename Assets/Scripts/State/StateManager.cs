using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    // this class will communicate with the solver via the Master Controller

    private char[] tokenSet = new char[] { '1', '2', '3', '4', '5', '6', '7', '8' };

    // the board that will be accessed by the UI/Slice View
    public char[,,] BoardState { get; set; }

    // the number of conflicts for each cell. A value of -1 indicates a Given.
    public int[,,] BoardGivensAndConflicts { get; set; }

    private void Awake()
    {
        BoardState = new char[8, 8, 8];
        GenerateTestBoardState();

        DontDestroyOnLoad(gameObject);
    }

    void GenerateTestBoardState()
    {
        for (int i = 0; i < BoardState.GetLength(0); i++)
        {
            for (int j = 0; j < BoardState.GetLength(1); j++)
            {
                for (int k = 0; k < BoardState.GetLength(1); k++)
                {
                    BoardState[i, j, k] = '0';
                }
            }
        }

        BoardState[0, 0, 0] = '1';
        BoardState[0, 0, 7] = '2';
        BoardState[0, 7, 0] = '3';
        BoardState[0, 7, 7] = '4';
        BoardState[7, 0, 0] = '5';
        BoardState[7, 0, 7] = '6';
        BoardState[7, 7, 0] = '7';
        BoardState[7, 7, 7] = '8';
    }
}
