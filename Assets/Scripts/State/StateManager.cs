using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    private MasterController _mc;
    public HashSet<char> TokenSet { get; set; }

    // the board that will be accessed by the UI/Slice View
    public char[,,] BoardState { get; set; }

    // A value of 1 indicates a Given.
    public int[,,] BoardGivens { get; set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        BoardState = new char[8, 8, 8];
        BoardGivens = new int[8, 8, 8];

        ResetBoard();

        // the set of symbols that the user can put into each cell; does not include the empty option
        TokenSet = new HashSet<char>() { '1', '2', '3', '4', '5', '6', '7', '8' };
    }

    private void Start()
    {
        _mc = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
    }

    public char GetCellValue(int[] cellIndex)
    {
        return BoardState[cellIndex[0], cellIndex[1], cellIndex[2]];
    }

    public void SetCellValue(int[] cellIndex, char cellValue)
    {
        BoardState[cellIndex[0], cellIndex[1], cellIndex[2]] = cellValue;
    }

    public void ResetBoard()
    {
        GenerateTestBoardState();
        LoadBoardGivens();
    }

    void GenerateTestBoardState()
    {
        for (int i = 0; i < BoardState.GetLength(0); i++)
        {
            for (int j = 0; j < BoardState.GetLength(1); j++)
            {
                for (int k = 0; k < BoardState.GetLength(1); k++)
                {
                    BoardState[i, j, k] = ' ';
                }
            }
        }

        BoardState[0, 0, 0] = '1';
    }

    void LoadBoardGivens()
    {
        for (int i = 0; i < BoardGivens.GetLength(0); i++)
        {
            for (int j = 0; j < BoardGivens.GetLength(1); j++)
            {
                for (int k = 0; k < BoardGivens.GetLength(2); k++)
                {
                    BoardGivens[i, j, k] = 0;
                }
            }
        }

        BoardGivens[0, 0, 0] = 1;
    }

    internal HashSet<char> GetValidTokensForCell(int[] cellIndex)
    {
        return _mc.solver.GetValidTokensForCell(BoardState, cellIndex);
    }

    public bool IsGiven(int[] cellIndex)
    {
        return BoardGivens[cellIndex[0], cellIndex[1], cellIndex[2]] == 1;
    }
}
