using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class PuzzleLoad : MonoBehaviour
{
    // This class scans the Saved Puzzles folder and returns a JSON object for each puzzle it finds.
    // Another menu class can then select the specific puzzle to load.

    private List<PuzzleJSON> m_allPuzzles;

    [SerializeField] private bool m_ScanFiles;

    [Tooltip("Debug use only. Do not enter values here.")]
    [SerializeField] private int m_numberOfFiles;

    private void OnValidate()
    {
        if (m_ScanFiles)
        {
            OnEnable();
        }
        m_ScanFiles = false;
    }

    private void OnEnable()
    {
        m_allPuzzles = ScanAllPuzzles();
        m_numberOfFiles = m_allPuzzles.Count;
    }

    public List<PuzzleJSON> GetAllPuzzles()
    {
        return ScanAllPuzzles();
    }

    private List<PuzzleJSON> ScanAllPuzzles()
    {
        List<PuzzleJSON> ap = new List<PuzzleJSON>();

        foreach (var jsonfile in Directory.GetFiles(Directories.SAVED_PUZZLES))
        {
            try
            {
                using (StreamReader sr = new StreamReader(jsonfile))
                {
                    string str = sr.ReadToEnd();
                    PuzzleJSON puzzleJSON = DeserializeJSON(str);
                    ap.Add(puzzleJSON);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogFormat("Problem reading file: {0}", jsonfile);
                throw ex;
            }
        }

        return ap;
    }

    private PuzzleJSON DeserializeJSON(string str)
    {
        return JsonUtility.FromJson<PuzzleJSON>(str);
    }

    // make sure all puzzles have valid data
    void ValidatePuzzles()
    {
        List<PuzzleJSON> puzzleJSONs = GetAllPuzzles();

        foreach (var puzzleJSON in puzzleJSONs)
        {
            if (!ValidateBoardGivens(puzzleJSON.serializeBoardGivens) ||
                !ValidateBoardState(puzzleJSON.serializeBoardState))
            {
                continue;
            }
        }
    }

    private bool ValidateBoardGivens(string boardGivens)
    {
        if (boardGivens.Length != 512) return false;

        foreach (char c in boardGivens)
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
            if (c != ' ' && !char.IsDigit(c) || c == '9') return false;
        }

        return true;
    }
}
