using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is an all-in-one that does the following:
/// 1) Receive clicks on the digits menu and highlight/select that cell
/// 2) Receive clicks on action items and update cells accordingly
/// </summary>
public class SliceInteractionManager : MonoBehaviour
{
    private MasterController m_masterController { get; set; }
    private CellController[] m_cellsUpper;
    private CellController[] m_cellsLower;
    private int m_selected = 0; // default digit

    [SerializeField] private GameObject m_sliceUpper;
    [SerializeField] private GameObject m_sliceLower;

    void Awake()
    {
        m_masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();

        m_cellsUpper = m_sliceUpper.GetComponentsInChildren<CellController>(true);
        Debug.Log($"{m_cellsUpper.Length}");
        m_cellsLower = m_sliceLower.GetComponentsInChildren<CellController>(true);
        Debug.Log($"{m_cellsLower.Length}");
    }

    /// <summary>
    /// This represents an attempt by the player to modify the state of the board.
    /// It is called by the cell controller when a cell in one of the slices is clicked on.
    /// </summary>
    public void SelectCell(CellController cellController)
    {
        if (m_selected != 0)
        {
            m_masterController.stateManager.SetCellValue(cellController.CellIndex, (char)m_selected);
        }
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
