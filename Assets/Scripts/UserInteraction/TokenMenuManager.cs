using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private TokenMenuCloseButton _tmcb;

    public void OpenMenuAtCell(GameObject cell)
    {
        // reset if open
        if (_tokenMenuInstance != null) Destroy(_tokenMenuInstance);

        // instantiate under BoardCanvas
        _selectedCell = cell.GetComponent<CellController>();
        _tokenMenuInstance = Instantiate(_tokenMenu, cell.transform.parent.parent);

        // positioning
        RectTransform rt = _tokenMenuInstance.GetComponent<RectTransform>();
        Vector2 rtSize = rt.sizeDelta;
        Vector2 cellSize = cell.GetComponent<RectTransform>().sizeDelta;

        rt.position = new Vector3(
            //cell.transform.position.x + rtSize.x / 2f + cellSize.x / 2f,
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
        _bm = gameObject.GetComponent<CellController>()._bm;
        _bm.SetCellValue(_selectedCell.CellIndex, cellValue);
        _selectedCell.UpdateCellValue();

        _bm.RefreshAllCellColors();
        CloseMenu();
    }

    // tapping on the blocking panel (outside the menu) will call this method
    public void CloseMenu()
    {
        Destroy(_tokenMenuInstance);
    }

    // Can set other background colors as well
    void IndicateConflictingTokens()
    {
        _bm = gameObject.GetComponent<CellController>()._bm;
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
}
