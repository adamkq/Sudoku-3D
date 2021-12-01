using UnityEngine;
using System.Collections.Generic;

public class MenuSelectPuzzle : MonoBehaviour
{
    private MasterController _mc;

    private void Start()
    {
        _mc = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
    }

    // make sure all puzzles have valid data
    void ValidatePuzzles()
    {
        List<PuzzleJSON> puzzleJSONs = _mc.GetAllPuzzles();

        foreach(var puzzleJSON in puzzleJSONs)
        {
            if (!ValidateBoardGivens(puzzleJSON.serializeBoardGivens) ||
                !ValidateBoardState(puzzleJSON.serializeBoardState))
            {
                continue;
            }

            // for now just debug to console: the number of givens and the total number of filled-in cells
        }
    }

    // present the valid puzzles as selectable options in the scroll menu
    void RefreshMenu()
    {

    }

    private bool ValidateBoardGivens(string boardGivens)
    {
        if (boardGivens.Length != 512) return false;

        foreach(char c in boardGivens)
        {
            if (c != '0' && c != '1') return false;
        }

        return true;
    }

    private bool ValidateBoardState(string boardState)
    {
        if (boardState.Length != 512) return false;

        foreach (char c in boardState)
        {
            if (c != ' '  && !char.IsDigit(c) || c == '9') return false;
        }

        return true;
    }
}
