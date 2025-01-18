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
    private CellController[] m_cellsUpper;
    private CellController[] m_cellsLower;
    private int m_selected = 0; // default digit

    [SerializeField] private GameObject m_sliceUpper;
    [SerializeField] private GameObject m_sliceLower;

    void Awake()
    {
        _mc = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();

        m_cellsUpper = m_sliceUpper.GetComponentsInChildren<CellController>(true);
        m_cellsLower = m_sliceLower.GetComponentsInChildren<CellController>(true);

        foreach(var cell in m_cellsUpper)
        {
            cell.sim = this;
        }

        foreach(var cell in m_cellsLower)
        {
            cell.sim = this;
        }
    }

    /// <summary>
    /// This represents an attempt by the player to modify the state of the board.
    /// </summary>
    public void ModifyCell(int digit)
    {
        // TODO; change values of all selected cells to the digit in question, then de-select cells
        // if digit = 0, then clear cells
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
