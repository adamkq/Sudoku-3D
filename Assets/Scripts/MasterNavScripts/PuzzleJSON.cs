using System;
using UnityEngine;

public class PuzzleJSON
{
    public string puzzleTitle;
    public string serializeBoardGivens = new string('0', 128);
    public string serializeBoardState = new string('0', 128);
    public DateTime lastModified;
}
