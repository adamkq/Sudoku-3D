using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CellController : MonoBehaviour
{
    // Script will do multiple things:
    // 1. Allow other scripts to set component properties
    // 2. Show a popup menu when the cell is tapped
    // 3. Update the character in response to popup menu selection

    
    public char CellChar { get; set; }
    [SerializeField] private Button button;
    [SerializeField] private Image image;
    [SerializeField] private TokenMenuManager tokenMenu;
    [SerializeField] private int slice; // 0 or 1 depending on if it's the top or bottom of the screen

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
        buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

        masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();

        int siblingIndex = transform.GetSiblingIndex();
        SetCellIndex(new int[3] {siblingIndex % 8, siblingIndex / 8, slice});

        SetTint();

        if (masterController != null)
        {
            masterController.IncludeCellInList(this);
            UpdateCell();
        }
        else
        {
            Debug.LogError("Error: master controller not found");
        }
        
    }

    public void TaskOnClick()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ToggleCellIsGiven();
        }
        else
        {
            tokenMenu.OpenMenuAtCell(gameObject);
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
        UpdateCellValue(m_cellIndex);
        SetBackgroundColor();
    }

    public void UpdateCellValue(int[] cellIndex)
    {
        CellChar = masterController.stateManager.GetCellValue(cellIndex);
        
        if (buttonText != null)
        {
            buttonText.text = CellChar.ToString();
        }
    }

    private void SetBackgroundColor()
    {
        Color color;
        // ruleset for determining the color of the cell
        bool cellIsGiven = masterController.stateManager.GetCellIsGiven(m_cellIndex);

        HashSet<char> validTokens = masterController.solver.GetValidTokensForCell(m_cellIndex);

        // TODO if cell has only 1 valid option and is cleared, color it yellow
        // if cell is filled, and all of its row/col/subcube cells are filled, and it's valid, color it green
        if (cellIsGiven)
        {
            color = Colors.CELL_GIVEN;
        }
        else if (CellChar != ' ' && !validTokens.Contains(CellChar))
        {
            color = Colors.CELL_CONFLICT;
        }
        else
        {
            color = Colors.CELL_NORMAL;
        }

        if(image)
        {
            image.color = color;
        }
        
    }


    private void SetTint()
    {
        // set tint based on alternating subcubes of cell; this should help user navigate/understand the board
        int even = (CellIndex[0] - CellIndex[0] % 2) + (CellIndex[1] - CellIndex[1] % 2);
        Color tintColor = even % 4 == 0 ? new Color(1, 1, 1) : new Color(0.85f, 0.85f, 0.85f);

        ColorBlock colorBlock = button.colors;

        colorBlock.normalColor = tintColor;
        colorBlock.highlightedColor = tintColor;
        colorBlock.pressedColor = tintColor;
        colorBlock.selectedColor = tintColor;

        button.colors = colorBlock;
    }

    void ToggleCellIsGiven()
    {
        bool cellIsGiven = !masterController.stateManager.GetCellIsGiven(m_cellIndex);

        masterController.stateManager.SetCellIsGiven(m_cellIndex, cellIsGiven);

        SetBackgroundColor();
    }
}
