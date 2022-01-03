using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TokenMenuManager : MonoBehaviour
{
    /*This class handles the menu that pops up and allows the user to select a token
     * It interacts with the BoardManager and with the currently selected cell
     * 1. Open Menu Prefab at a specific spot on screen
     * 2. Send selection to Board Manager so it can update the state
     * 3. Receive conflicting options (i.e. if a given number is already present in a subset)
     */

    [SerializeField] private GameObject _tokenMenu;

    private GameObject m_tokenMenuInstance;
    private BoardManager m_boardManager;
    private CellController m_selectedCell;
    private TokenButton[] m_tokenButtons;
    private TokenMenuCloseButton m_tmcb; // entire-screen button in the background that will close the menu when clicked
    private GameObject m_duplicatedCell; // show this above the grayed-out canvas to highlight it to the user

    public void OpenMenuAtCell(GameObject cell)
    {
        // instantiate under BoardCanvas
        m_selectedCell = cell.GetComponent<CellController>();
        m_tokenMenuInstance = Instantiate(_tokenMenu, cell.transform.parent.parent);

        // duplicate the cell
        m_duplicatedCell = Instantiate(cell, cell.transform.parent.parent);
        Button btn = m_duplicatedCell.GetComponentInChildren<Button>();
        btn.interactable = false;

        // positioning
        RectTransform rt = m_tokenMenuInstance.GetComponent<RectTransform>();
        Vector2 rtSize = rt.sizeDelta;
        Vector2 cellSize = cell.GetComponent<RectTransform>().sizeDelta;

        rt.position = new Vector3(
            Screen.width / 2,
            cell.transform.position.y + rtSize.y / 2f + cellSize.y / 2f,
            0);

        // buttons
        m_tokenButtons = m_tokenMenuInstance.GetComponentsInChildren<TokenButton>();
        foreach(var tb in m_tokenButtons)
        {
            tb.tokenMenuManager = gameObject.GetComponent<TokenMenuManager>();
        }

        m_tmcb = m_tokenMenuInstance.GetComponentInChildren<TokenMenuCloseButton>();
        m_tmcb.tokenMenuManager = gameObject.GetComponent<TokenMenuManager>();

        // colors
        IndicateConflictingTokens();
    }

    // token buttons will call this method
    public void MakeSelection(char cellValue)
    {
        GetBoardManager();
        m_boardManager.SetCellValue(m_selectedCell.CellIndex, cellValue);
        m_selectedCell.UpdateCellValue();

        m_boardManager.RefreshAllCellColors();
        CloseMenu();
    }

    // tapping on the blocking panel (outside the menu) will call this method
    public void CloseMenu()
    {
        Destroy(m_duplicatedCell);
        Destroy(m_tokenMenuInstance);
    }

    // Can set other background colors as well
    void IndicateConflictingTokens()
    {
        GetBoardManager();
        HashSet<char> validTokens = m_boardManager.masterController.stateManager.GetValidTokensForCell(m_selectedCell.CellIndex);

        foreach(var tb in m_tokenButtons)
        {
            char chr;
            bool result = char.TryParse(tb.GetTokenValue(), out chr);
            if (result)
            {
                tb.SetBackgroundColor(m_boardManager.GetCellColor(validTokens, chr, false));
            }
        }
    }

    void GetBoardManager()
    {
        m_boardManager = gameObject.GetComponent<CellController>().BoardManager;
    }
}
