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

    void Start()
    {
        _cells = _cellContainer.GetComponentsInChildren<CellController>();

        _mc = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        AssignMCToCells(_mc);

        _targetFaceName = _mc.sliceTargetFaceName;
        _downwardFaceName = _mc.sliceDownwardFaceName;

        Debug.LogFormat("{0}, {1}", _targetFaceName, _downwardFaceName);

        SetIndexesForAllCells();
        UpdateAllCells();
    }

    private void AssignMCToCells(MasterController mc)
    {
        foreach(var cell in _cells)
        {
            cell.mc = this._mc;
        }
    }

    void SetIndexesForAllCells()
    {
        // Assume a given downward face. For Back, Front, Left, and Right faces, assume that the Bottom face is downward.
        // For Top face, assume that the Back face is downward.
        // For Bottom face, assume that the Front face is downward.
        int index = 0;

        // iterate over the target cube face
        foreach (var cell in _cells)
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

        // rotate depending on the downward face
        switch (_targetFaceName)
        {
            case "CubeFaceBack":
                if (_downwardFaceName == "CubeFaceLeft") RotateCellIndexesCW(3);
                else if (_downwardFaceName == "CubeFaceTop") RotateCellIndexesCW(2);
                else if (_downwardFaceName == "CubeFaceRight") RotateCellIndexesCW(1);
                break;
            case "CubeFaceFront":
                if (_downwardFaceName == "CubeFaceRight") RotateCellIndexesCW(3);
                else if (_downwardFaceName == "CubeFaceTop") RotateCellIndexesCW(2);
                else if (_downwardFaceName == "CubeFaceLeft") RotateCellIndexesCW(1);
                break;
            case "CubeFaceLeft":
                if (_downwardFaceName == "CubeFaceFront") RotateCellIndexesCW(3);
                else if (_downwardFaceName == "CubeFaceTop") RotateCellIndexesCW(2);
                else if (_downwardFaceName == "CubeFaceBack") RotateCellIndexesCW(1);
                break;
            case "CubeFaceRight":
                if (_downwardFaceName == "CubeFaceBack") RotateCellIndexesCW(3);
                else if (_downwardFaceName == "CubeFaceTop") RotateCellIndexesCW(2);
                else if (_downwardFaceName == "CubeFaceFront") RotateCellIndexesCW(1);
                break;
            case "CubeFaceBottom":
                if (_downwardFaceName == "CubeFaceLeft") RotateCellIndexesCW(3);
                else if (_downwardFaceName == "CubeFaceBack") RotateCellIndexesCW(2);
                else if (_downwardFaceName == "CubeFaceRight") RotateCellIndexesCW(1);
                break;
            case "CubeFaceTop":
                if (_downwardFaceName == "CubeFaceLeft") RotateCellIndexesCW(3);
                else if (_downwardFaceName == "CubeFaceFront") RotateCellIndexesCW(2);
                else if (_downwardFaceName == "CubeFaceRight") RotateCellIndexesCW(1);
                break;
            default:
                Debug.LogError("Cube Downward Face name not found.");
                break;
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
                    // rotating a 2d array using 1d indexing, fun...
                    swap = _cells[row * 8 + col].CellIndex;
                    _cells[row * 8 + col].CellIndex = _cells[(7 - col) * 8 + row].CellIndex;
                    _cells[(7 - col) * 8 + row].CellIndex = _cells[63 - (row * 8 + col)].CellIndex;
                    _cells[63 - (row * 8 + col)].CellIndex = _cells[63 - ((7 - col) * 8 + row)].CellIndex;
                    _cells[63 - ((7 - col) * 8 + row)].CellIndex = swap;
                }
            }
        }
    }

    void UpdateAllCells()
    {
        foreach(var cell in _cells)
        {
            cell.UpdateCellValue(cell.CellIndex);
        }
    }

    public void IncrementDepthAndUpdateCells(int depthDelta)
    {
        _depth = Mathf.Clamp(_depth + depthDelta, 0, 7);
        SetIndexesForAllCells();
        UpdateAllCells();
    }

    public void LoadOrbitCubeViewScene()
    {
        _mc.LoadScene("OrbitCubeViewScene");
    }
}
