using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PuzzleMenuButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_textTitle;
    [SerializeField] private TextMeshProUGUI m_textDetails;

    public MenuSelectPuzzle MenuSelectPuzzle;

    public void SetTextOnButton(string puzzleTitle, string puzzleDifficulty, int cellsFilled)
    {
        m_textTitle.text = puzzleTitle;
        m_textDetails.text = puzzleDifficulty + ", " + cellsFilled + "/512 cells filled";
    }

    public void OnPuzzleSelected()
    {
        // Buttons are ordered in menu as in Unity Hierarchy. MSP assigned during instantiation.
        MenuSelectPuzzle.MenuButtonClicked(gameObject.transform.GetSiblingIndex());
    }
}
