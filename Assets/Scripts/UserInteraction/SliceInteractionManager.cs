using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles UI changes between cells and digit-buttons, such as highlighting or disabling
/// </summary>
public class SliceInteractionManager : MonoBehaviour
{
    private MasterController _mc { get; set; }
    private CellController[] m_cells;
    private int m_selected = 0; // default digit

    [SerializeField] private GameObject m_slicesParent;

    void Awake()
    {
        _mc = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();

        m_cells = m_slicesParent.GetComponentsInChildren<CellController>(true);

        foreach(var cell in m_cells)
        {
            cell._sim = this;
        }
    }

    public void DeselectAllCells()
    {
        foreach(CellController _cell in m_cells)
        {
            _cell.SetSelected(false);
        }
    }

    public void DeselectAllCellsExcept(CellController cell)
    {
        foreach(CellController _cell in m_cells)
        {
            if (_cell != cell)
            {
                _cell.SetSelected(false);
            }
        }
    }

    /// <summary>
    /// This represents an attempt by the player to modify the state of the board.
    /// Will apply the selected digit to the selected cell(s), then deselect.
    /// </summary>
    public void ModifyCell(int digit)
    {
        char chr = digit != 0 ? (char)(digit + 48) : ' '; // ASCII conversion
        Debug.Log(chr);
        Debug.Log(m_cells.Length);
        foreach(var cell in m_cells)
        {            
            if(cell.IsSelected)
            {
                _mc.stateManager.SetCellValue(cell.CellIndex, chr);
            }
        }

        DeselectAllCells();
        _mc.RefreshAllCells();
    }

    public void OnClickUndo()
    {
        // step-back the state of the board
    }

    public void OnClickRedo()
    {
        // step-forward the state of the board
    }

    public void OnClickPencilErase()
    {
        // when this button is clicked, activate a mode that, instead of modifying the main text of a cell, modifies the notes instead
    }

    public void RegenerateNotes()
    {
        // for each cell, enable the notes that could be part of the solution.
    }

}
