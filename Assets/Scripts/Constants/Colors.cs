using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colors : MonoBehaviour
{
    public static readonly Color CELL_NORMAL = Color.white;
    public static readonly Color CELL_CONFLICT = new Color(1f, 0.4f, 0.3f); // tomato
    public static readonly Color CELL_YELLOW = new Color(1f, 0.9f, 0f); // gold
    public static readonly Color CELL_GREEN = new Color(0.2f, 0.9f, 0.2f); // less-harsh green
    public static readonly Color CELL_GIVEN = new Color(0.5f, 0.8f, 0.9f); // sky blue
}
