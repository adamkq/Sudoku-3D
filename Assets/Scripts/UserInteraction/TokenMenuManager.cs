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

    [SerializeField] private GameObject m_tokenMenu;
    [SerializeField] private BoardManager m_boardManager;

    private CellController m_selectedCell;
    private TokenButton[] m_tokenButtons;
    private TokenMenuCloseButton m_tmcb; // entire-screen button in the background that will close the menu when clicked

    public void OpenMenuAtCell(GameObject cell)
    {
        m_selectedCell = cell.GetComponent<CellController>();

        // positioning
        RectTransform rt = m_tokenMenu.GetComponent<RectTransform>();
        Vector2 rtSize = rt.sizeDelta;
        Vector2 cellSize = cell.GetComponent<RectTransform>().sizeDelta;

        rt.position = new Vector3(
            Screen.width / 2,
            cell.transform.position.y + rtSize.y / 2f + cellSize.y / 2f,
            0);

        // buttons; assign self as TMM
        m_tokenButtons = m_tokenMenu.GetComponentsInChildren<TokenButton>();
        foreach(var tb in m_tokenButtons)
        {
            tb.tokenMenuManager = gameObject.GetComponent<TokenMenuManager>();
        }

        m_tmcb = m_tokenMenu.GetComponentInChildren<TokenMenuCloseButton>();
        m_tmcb.tokenMenuManager = gameObject.GetComponent<TokenMenuManager>();

        // colors
        IndicateConflictingTokens();

        m_tokenMenu.SetActive(true);
    }

    // token buttons will call this method
    public void MakeSelection(char cellValue)
    {
        m_boardManager.SetCellValue(m_selectedCell.CellIndex, cellValue);
        m_selectedCell.UpdateCellValue();

        m_boardManager.RefreshAllCellColors();
        CloseMenu();
    }

    // tapping on the blocking panel (outside the menu) will call this method
    public void CloseMenu()
    {
        m_tokenMenu.SetActive(false);
    }

    private void Start()
    {
        m_tokenMenu.SetActive(false);
    }

    // Can set other background colors as well
    void IndicateConflictingTokens()
    {
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
}
