using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    // The "cell" game object contains a single digit. This script will
    // duplicate it 64 times in order to fill out the entire board.

    [SerializeField] private GameObject _cellContainer;
    [SerializeField] private GameObject _cell;

    // CellControllers can call this script to get the board state
    [SerializeField] private BoardManager _boardManager;

    // separate the board from the edge of the screen.
    [SerializeField] private int _pixelMarginFromScreenEdge = 50;

    private MasterController _mc;
    private Vector2Int screenDimensions;
    private Vector3 boardUpperLeftCorner;
    private int boardSideLength;

    private void Awake()
    {
        GetScreenDimensions();
        DetermineBoardSize();
        InstantiateAllCells();
    }

    void Start()
    {
        _mc = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
    }

    private void GetScreenDimensions()
    {
        screenDimensions = new Vector2Int(Screen.width, Screen.height);
        Debug.Log(screenDimensions);
    }

    private void DetermineBoardSize()
    {
        int corner = Mathf.Min(screenDimensions.x - _pixelMarginFromScreenEdge, screenDimensions.y - _pixelMarginFromScreenEdge) / 2;
        Vector3 offsetToCenter = new Vector3(screenDimensions.x / 2, screenDimensions.y / 2);

        if (corner < 10) Debug.LogWarning("Board size is too small.");

        boardUpperLeftCorner = new Vector3(-corner, corner) + offsetToCenter;
        boardSideLength = corner * 2;
    }

    private void InstantiateAllCells()
    {
        // instantiate starting from the upper left
        int cellSideLength = boardSideLength / 8;
        for (int i = 0; i < 64; i++)
        {
            GameObject cell = Instantiate(_cell, _cellContainer.transform);
            cell.transform.position = new Vector3(i % 8 * cellSideLength, -i / 8 * cellSideLength) + boardUpperLeftCorner + new Vector3(cellSideLength/2, -cellSideLength/2);
            cell.name = "Cell" + i;

            CellController cc = cell.GetComponent<CellController>();
            cc.SetRectTransformSize(cellSideLength, cellSideLength);
            cc._bm = _boardManager;
        }
    }
}
