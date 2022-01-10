using UnityEngine;
using System.Collections.Generic;

public class MenuSelectPuzzle : MonoBehaviour
{
    private MasterController m_masterController;

    [SerializeField] private GameObject m_viewportContent; // all the menu options are children of this Gobj
    [SerializeField] private GameObject m_puzzleMenuButtonPrefab;

    public void MenuButtonClicked(int buttonIndex)
    {
        m_masterController.InitializePuzzle(m_masterController.GetAllPuzzles()[buttonIndex]);
        m_masterController.LoadScene("OrbitCubeViewScene");
    }

    private void Awake()
    {
        m_masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
    }

    private void OnEnable()
    {
        RefreshMenu();
    }

    // present the valid puzzles as selectable options in the scroll menu
    void RefreshMenu()
    {
        // scan all saved puzzles and generate a menu button for each one
        ClearMenu();
        List<PuzzleJSON> allPuzzlesJSON = m_masterController.GetAllPuzzles();

        foreach(var puzzleJson in allPuzzlesJSON)
        {
            GameObject puzzleOption = Instantiate(m_puzzleMenuButtonPrefab, m_viewportContent.transform);
            PuzzleMenuButton pmb = puzzleOption.GetComponent<PuzzleMenuButton>();
            pmb.SetTextOnButton(puzzleJson.puzzleTitle, GetDifficulty(puzzleJson.serializeBoardGivens), GetNumFilled(puzzleJson.serializeBoardState));
            pmb.MenuSelectPuzzle = gameObject.GetComponent<MenuSelectPuzzle>();
        }
    }

    private void ClearMenu()
    {
        foreach(Transform child in m_viewportContent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private string GetDifficulty(string serializedBoardGivens)
    {
        int numGivens = 0;

        foreach (char chr in serializedBoardGivens)
        {
            if (chr == '1') numGivens++;
        }

        // this assumes only 1 valid solution per puzzle, and not that the puzzle is "wide-open" i.e. with many solutions
        if (numGivens == 0) { return "Blank"; };
        if (numGivens < 100) { return "Hard"; };
        if (numGivens < 200) { return "Medium"; };
        return "Easy";
    }

    private int GetNumFilled(string serializedBoardState)
    {
        int numFilled = 0;

        foreach (char chr in serializedBoardState)
        {
            if (char.IsDigit(chr) && chr != '0') { numFilled++; };
        }

        return numFilled;
    }
}
