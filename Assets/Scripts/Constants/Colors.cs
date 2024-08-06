using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colors : MonoBehaviour
{
    // cell background colors
    public static readonly Color CELL_NORMAL = Color.white; // refers to color of text on white background
    public static readonly Color CELL_SELECTED = new Color(1f, 0.9f, 0f); // gold
    public static readonly Color CELL_IN_SET = new Color(0.5f, 0.8f, 0.9f); // sky blue

    // text colors
    public static readonly Color TEXT_NORMAL = Color.black;
    public static readonly Color TEXT_CONFLICT = new Color(1f, 0.4f, 0.3f); // tomato
    public static readonly Color TEXT_GIVEN = new Color(0.2f, 0.9f, 0.2f); // less-harsh green
    
}
