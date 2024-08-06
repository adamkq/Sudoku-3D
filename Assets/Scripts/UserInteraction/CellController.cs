using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class CellController : MonoBehaviour
{
    // Script will do multiple things:
    // 1. Allow other scripts to set component properties
    // 2. Show a popup menu when the cell is tapped
    // 3. Update the character in response to popup menu selection

    
    public char CellChar { get; set; }
    [SerializeField] private Button m_button;
    [SerializeField] private Image m_background;
    [SerializeField] private int slice; // 0 or 1 depending on if it's the top or bottom of the screen

    [SerializeField] private GameObject[] m_notes;

    private MasterController masterController { get; set; }
    private int[] m_cellIndex;

    public int[] CellIndex
    {
        get { return (int[])m_cellIndex.Clone(); } // returns a copy of the array
        private set { m_cellIndex = value; }
    } 

    private TextMeshProUGUI buttonText;

    void Awake()
    {
        buttonText = m_button.GetComponentInChildren<TextMeshProUGUI>();

        masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();

        int siblingIndex = transform.GetSiblingIndex();
        SetCellIndex(new int[3] {siblingIndex % 8, siblingIndex / 8, slice});

        if (masterController != null)
        {
            masterController.IncludeCellInList(this);
            UpdateCell();
        }
        else
        {
            Debug.LogError("Error: master controller not found");
        }

        UpdateNotes(new HashSet<char>());
    }

    public void TaskOnClick()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ToggleCellIsGiven();
        }
        else
        {
            Debug.LogFormat("Cell clicked at [{0}, {1}, {2}]", m_cellIndex[0], m_cellIndex[1], m_cellIndex[2]);
            // TODO make this toggle
            SetBackgroundColor(Colors.CELL_SELECTED);
        }
    }

    private void SetCellIndex(int[] cellIndex)
    {
        if (cellIndex.Length != 3)
        {
            Debug.LogError("Invalid dimensions of cell index");
            return;
        }

        foreach(int index in cellIndex)
        {
            if (index < 0 || index > 7)
            {
                Debug.LogErrorFormat("Invalid values in cell index: {0}, {1}, {2}", cellIndex[0], cellIndex[1], cellIndex[2]);
                return;
            }
        }

        m_cellIndex = cellIndex;
    }

    public void UpdateCell()
    {
        // also mark conflicted cell
        UpdateCellValue(m_cellIndex);

        HashSet<char> validTokens = masterController.solver.GetValidTokensForCell(m_cellIndex);

        SetMainTextColor(CellChar != ' ' && !validTokens.Contains(CellChar) ? Colors.TEXT_CONFLICT : Colors.TEXT_NORMAL);
    }

    public void UpdateCellValue(int[] cellIndex)
    {
        CellChar = masterController.stateManager.GetCellValue(cellIndex);
        
        if (buttonText != null)
        {
            buttonText.text = CellChar.ToString();
        }
    }

    private void SetMainTextColor(Color color)
    {
        if (buttonText != null)
        {
            buttonText.color = color;
        }   
    }

    private void SetBackgroundColor(Color color)
    {
        if(m_background)
        {
            m_background.color = color;
        }
    }

    private void CheckRelatedAndSetBackgroundColor()
    {
        // set background color if this cell is related to the selected cell. This illustrates related sets to the user
    }

    void ToggleCellIsGiven()
    {
        bool cellIsGiven = !masterController.stateManager.GetCellIsGiven(m_cellIndex);

        masterController.stateManager.SetCellIsGiven(m_cellIndex, cellIsGiven);

        Color _color = cellIsGiven ? Colors.TEXT_GIVEN : Colors.TEXT_NORMAL;
        SetMainTextColor(_color);
    }

    void ShowNote(GameObject note, bool isDisplayed)
    {
        TextMeshProUGUI tmpUGUI = note.GetComponentInChildren<TextMeshProUGUI>();

        if (tmpUGUI)
        {
            tmpUGUI.enabled = isDisplayed;
        }
        else
        {
            Debug.LogWarning("tmpUGUI not found on cell");
        }
        
    }

    public void UpdateNotes(HashSet<char> tokens)
    {
        // show the notes that correspond with valid tokens. If not present, then hide that token
        ShowNote(m_notes[0], tokens.Contains('1'));
        ShowNote(m_notes[1], tokens.Contains('2'));
        ShowNote(m_notes[2], tokens.Contains('3'));
        ShowNote(m_notes[3], tokens.Contains('4'));
        ShowNote(m_notes[4], tokens.Contains('5'));
        ShowNote(m_notes[5], tokens.Contains('6'));
        ShowNote(m_notes[6], tokens.Contains('7'));
        ShowNote(m_notes[7], tokens.Contains('8'));
    }
}
