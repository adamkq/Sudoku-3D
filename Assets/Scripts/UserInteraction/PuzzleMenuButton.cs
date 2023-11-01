using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PuzzleMenuButton : MonoBehaviour
{
    public MenuSelectPuzzle MenuSelectPuzzle;

    public void OnPuzzleSelected()
    {
        // Buttons are ordered in menu as in Unity Hierarchy.
        MenuSelectPuzzle.MenuButtonClicked(gameObject.transform.GetSiblingIndex());
    }
}
