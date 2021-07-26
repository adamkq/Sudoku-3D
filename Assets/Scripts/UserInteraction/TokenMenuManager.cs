using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardManager))]

public class TokenMenuManager : MonoBehaviour
{
    /*This class handles the menu that pops up and allows the user to select a token
     * It interacts with the BoardManager and with the currently selected cell
     * 1. Open Menu Prefab at a specific spot on screen
     * 2. Send selection to Board Manager so it can update the state
     * 3. Receive conflicting options (i.e. if a given number is already present in a subset)
     */

    [SerializeField] private GameObject _tokenMenu;

    private BoardManager _bm;
    private CellController _selectedCell;

    private void Start()
    {
        _bm = GetComponent<BoardManager>();
        _tokenMenu.SetActive(false);
    }

    public void OpenMenuAtCell(GameObject cell)
    {
        _selectedCell = cell.GetComponent<CellController>();
        RectTransform rt = cell.GetComponent<RectTransform>();
        Vector2 menuOffset = rt.sizeDelta;

        Vector3 menuPos = new Vector3(_tokenMenu.transform.position.x, cell.transform.position.y + menuOffset.y, _tokenMenu.transform.position.z);
        _tokenMenu.transform.position = menuPos;
        _tokenMenu.SetActive(true);
    }

    // token buttons will call this method
    public void MakeSelection(char cellValue)
    {
        _bm.SetCellValue(_selectedCell.CellIndex, cellValue);
        _selectedCell.UpdateCellValue();
        _tokenMenu.SetActive(false);
    }

    // if a token conflicts, set the background color of the button to red
    public void IndicateConflictingTokens(char[] conflictingToken)
    {

    }
}
