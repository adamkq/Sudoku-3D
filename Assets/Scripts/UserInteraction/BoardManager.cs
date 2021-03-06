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

    public void Start()
    {
        Solver solver = GameObject.Find("Solver").GetComponent<Solver>();
        solver.BoardManager = gameObject.GetComponent<BoardManager>();
    }
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
        _depth = Mathf.Clamp(_depth + depthDelta, 0, 1);
        SetIndexesForAllCells();
        UpdateAllCells();
        RefreshAllCellTints();
    }

    public void ExitToMainMenu()
    {
        masterController.LoadScene("MainMenu");
    }

    // called by token selection menu
    public void SetCellValue(int[] cellIndex, char cellValue)
    {
        masterController.stateManager.SetCellValue(cellIndex, cellValue);
        RefreshAllCellColors();
    }

    public Color GetCellColor(HashSet<char> validTokens, char cellChar, bool isGiven = false)
    {
        if (isGiven)
        {
            return Colors.CELL_GIVEN;
        }
        else if (!validTokens.Contains(cellChar) && cellChar != ' ' || validTokens.Count == 0)
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

        IncrementDepthAndUpdateCells(0);
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
        // In Sandwich Sudoku, only the Back Face is needed here
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
    }

    // Not needed for Sandwich Sudoku refactor, but may be a handy reference for other projects
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

    private void RefreshAllCellTints()
    {
        // set tint based on alternating subcubes of cell; this should help user navigate/understand the board
        foreach (var cell in m_cells)
        {
            int even = (cell.CellIndex[0] - cell.CellIndex[0] % 2) +
                       (cell.CellIndex[1] - cell.CellIndex[1] % 2) +
                       (cell.CellIndex[2] - cell.CellIndex[2] % 2);

            Color tintColor = even % 4 == 0 ? new Color(1, 1, 1) : new Color(0.85f, 0.85f, 0.85f);

            cell.SetTint(tintColor);
        }
    }
}
