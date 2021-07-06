using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // This class manages navigation between slices, and updates the 3d index of each cell

    [SerializeField] private GameObject _cellContainer;

    private CellController[] _cells;
    private MasterController _mc;

    // these variables can be used to determine the slice to display
    private string _targetFaceName = string.Empty;
    private string _downwardFaceName = string.Empty;
    private int _depth = 0;
    private int[] _upperLeftCellIndex;

    void Start()
    {
        _cells = _cellContainer.GetComponentsInChildren<CellController>();

        _mc = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        AssignMCToCells(_mc);

        _targetFaceName = _mc.sliceTargetFaceName;
        _downwardFaceName = _mc.sliceDownwardFaceName;

        Debug.LogFormat("{0}, {1}", _targetFaceName, _downwardFaceName);

        // iterate over the target cube face
        SetNominalIndexesForCells();

        // rotate depending on the downward face
        RotateCellIndexesCW();
    }

    private void AssignMCToCells(MasterController mc)
    {
        foreach(var cell in _cells)
        {
            cell.mc = this._mc;
        }
    }

    void SetNominalIndexesForCells()
    {
        // Assume a given downward face. For Back, Front, Left, and Right faces, assume that the Bottom face is downward.
        // For Top face, assume that the Back face is downward.
        // For Bottom face, assume that the Front face is downward.
        int index = 0;
        foreach(var cell in _cells)
        {
            switch (_targetFaceName)
            {
                case "CubeFaceBack":
                    cell.SetCellIndex(new int[] { index / 8, index % 8, _depth });
                    break;
                case "CubeFaceFront":
                    cell.SetCellIndex(new int[] { index / 8, 7 - (index % 8), 7 - _depth });
                    break;
                case "CubeFaceLeft":
                    cell.SetCellIndex(new int[] { index / 8, _depth, 7 - (index % 8) });
                    break;
                case "CubeFaceRight":
                    cell.SetCellIndex(new int[] { index / 8, 7 - _depth, index % 8 });
                    break;
                case "CubeFaceBottom":
                    cell.SetCellIndex(new int[] { 7 - _depth, index % 8, index / 8 });
                    break;
                case "CubeFaceTop":
                    cell.SetCellIndex(new int[] { _depth, index % 8, 7 - (index / 8) });
                    break;
                default:
                    Debug.LogError("Cube Face name not found.");
                    break;
            }
            index += 1;
        }
    }

    void RotateCellIndexesCW(int rotationsCW = 1)
    {
        int[] swap;
        int row, col;
        for(int i = 0; i < rotationsCW; i++)
        {
            for (row = 0; row < 4; row++)
            {
                for (col = row; col < 6 - row; col++)
                {
                    swap = _cells[row * 8 + col].CellIndex;
                    // todo
                }
            }
        }
    }
}
