using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    /* This class:
     * 1. manages navigation between slices, and updates the 3d index of each cell
     * 2. mediates between user selection, state update, and cell value update
     * 3. colors the background of all cells in a slice in order to provide hints to the user
    */

    [SerializeField] private GameObject m_cellContainer;
    [SerializeField] private TokenMenuManager m_tokenMenuManager;

    private CellController[] m_cells;
    public MasterController masterController {get; set;}

    // these variables can be used to determine the slice to display
    private string _targetFaceName = string.Empty;
    private string _downwardFaceName = string.Empty;
    private int _depth = 0;

    public void UpdateAllCells()
    {
        foreach(var cell in m_cells)
        {
            cell.UpdateCellValue(cell.CellIndex);
        }
        RefreshAllCellColors();
    }

    public void IncrementDepthAndUpdateCells(int depthDelta)
    {
        _depth = Mathf.Clamp(_depth + depthDelta, 0, 7);
        SetIndexesForAllCells();
        UpdateAllCells();
        RefreshAllCellColors();
    }

    public void LoadOrbitCubeViewScene()
    {
        masterController.LoadScene("OrbitCubeViewScene");
    }

    // called by token selection menu
    public void SetCellValue(int[] cellIndex, char cellValue)
    {
        masterController.stateManager.SetCellValue(cellIndex, cellValue);
        RefreshAllCellColors();
    }

    public Color GetCellColor(HashSet<char> validTokens, char cellChar, bool isGiven = false)
    {
        if (!masterController.stateManager.TokenSet.Contains(cellChar))
        {
            return Colors.CELL_NORMAL; // get any space/empty chars
        }
        else if (isGiven)
        {
            return Colors.CELL_GIVEN;
        }
        else if (!validTokens.Contains(cellChar))
        {
            return Colors.CELL_CONFLICT;
        }
        else if (validTokens.Count == 1)
        {
            return Colors.CELL_GREEN;
        }
        else if (validTokens.Count < 4)
        {
            return Colors.CELL_YELLOW;
        }
        else
        {
            return Colors.CELL_NORMAL;
        }
    }

    public void RefreshAllCellColors()
    {
        foreach (var cell in m_cells)
        {
            HashSet<char> validTokens = masterController.stateManager.GetValidTokensForCell(cell.CellIndex);

            cell.SetBackgroundColor(GetCellColor(validTokens, cell.CellChar, masterController.stateManager.IsGiven(cell.CellIndex)));
        }
    }

    public void OpenTokenMenuAtCell(GameObject cell)
    {
        m_tokenMenuManager.OpenMenuAtCell(cell);
    }

    private void Awake()
    {
        m_cells = m_cellContainer.GetComponentsInChildren<CellController>();

        masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        AssignMCToCells();

        _targetFaceName = masterController.sliceTargetFaceName;
        _downwardFaceName = masterController.sliceDownwardFaceName;

        Debug.LogFormat("{0}, {1}", _targetFaceName, _downwardFaceName);

        SetIndexesForAllCells();
        UpdateAllCells();
        RefreshAllCellColors();
    }

    private void AssignMCToCells()
    {
        foreach (var cell in m_cells)
        {
            cell.MasterController = masterController;
        }
    }

    private void SetIndexesForAllCells()
    {
        // Assume a given downward face. For Back, Front, Left, and Right faces, assume that the Bottom face is downward.
        // For Top face, assume that the Back face is downward.
        // For Bottom face, assume that the Front face is downward.
        int index = 0;

        // iterate over the target cube face
        foreach (var cell in m_cells)
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

    private void RotateCellIndexesCW(int rotationsCW = 1)
    {
        int[] swap;
        int row, col;
        for (int i = 0; i < rotationsCW; i++)
        {
            for (row = 0; row < 4; row++)
            {
                for (col = row; col < 7 - row; col++)
                {
                    // rotating a 2d array using 1d indexing
                    swap = m_cells[row * 8 + col].CellIndex;
                    m_cells[row * 8 + col].CellIndex = m_cells[(7 - col) * 8 + row].CellIndex;
                    m_cells[(7 - col) * 8 + row].CellIndex = m_cells[63 - (row * 8 + col)].CellIndex;
                    m_cells[63 - (row * 8 + col)].CellIndex = m_cells[63 - ((7 - col) * 8 + row)].CellIndex;
                    m_cells[63 - ((7 - col) * 8 + row)].CellIndex = swap;
                }
            }
        }
    }
}
