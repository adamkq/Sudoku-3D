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

    private GameObject _tokenMenuInstance;
    private BoardManager _bm;
    private CellController _selectedCell;
    private TokenButton[] _tokenButtons;
    private TokenMenuCloseButton _tmcb; // entire-screen button in the background that will close the menu when clicked
    private GameObject _duplicatedCell; // show this above the grayed-out canvas to highlight it to the user

    public void OpenMenuAtCell(GameObject cell)
    {
        // instantiate under BoardCanvas
        _selectedCell = cell.GetComponent<CellController>();
        _tokenMenuInstance = Instantiate(_tokenMenu, cell.transform.parent.parent);

        // duplicate the cell
        _duplicatedCell = Instantiate(cell, cell.transform.parent.parent);
        Button btn = _duplicatedCell.GetComponentInChildren<Button>();
        btn.interactable = false;

        // positioning
        RectTransform rt = _tokenMenuInstance.GetComponent<RectTransform>();
        Vector2 rtSize = rt.sizeDelta;
        Vector2 cellSize = cell.GetComponent<RectTransform>().sizeDelta;

        rt.position = new Vector3(
            Screen.width / 2,
            cell.transform.position.y + rtSize.y / 2f + cellSize.y / 2f,
            0);

        // buttons
        _tokenButtons = _tokenMenuInstance.GetComponentsInChildren<TokenButton>();
        foreach(var tb in _tokenButtons)
        {
            tb.tokenMenuManager = gameObject.GetComponent<TokenMenuManager>();
        }

        _tmcb = _tokenMenuInstance.GetComponentInChildren<TokenMenuCloseButton>();
        _tmcb.tokenMenuManager = gameObject.GetComponent<TokenMenuManager>();

        // colors
        IndicateConflictingTokens();
    }

    // token buttons will call this method
    public void MakeSelection(char cellValue)
    {
        GetBoardManager();
        _bm.SetCellValue(_selectedCell.CellIndex, cellValue);
        _selectedCell.UpdateCellValue();

        _bm.RefreshAllCellColors();
        CloseMenu();
    }

    // tapping on the blocking panel (outside the menu) will call this method
    public void CloseMenu()
    {
        Destroy(_duplicatedCell);
        Destroy(_tokenMenuInstance);
    }

    // Can set other background colors as well
    void IndicateConflictingTokens()
    {
        GetBoardManager();
        HashSet<char> validTokens = _bm.masterController.stateManager.GetValidTokensForCell(_selectedCell.CellIndex);

        foreach(var tb in _tokenButtons)
        {
            char chr;
            bool result = char.TryParse(tb.GetTokenValue(), out chr);
            if (result)
            {
                tb.SetBackgroundColor(_bm.GetCellColor(validTokens, chr, false));
            }
        }
    }

    void GetBoardManager()
    {
        _bm = gameObject.GetComponent<CellController>()._bm;
    }
}
