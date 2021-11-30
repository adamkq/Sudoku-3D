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

    public List<PuzzleJSON> AllPuzzles { get { return m_allPuzzles; } }

    private void OnValidate()
    {
        if (m_ScanFiles)
        {
            m_allPuzzles = new List<PuzzleJSON>();
            m_numberOfFiles = ScanAllPuzzles();
        }
        m_ScanFiles = false;
    }

    public List<PuzzleJSON> GetAllPuzzles()
    {
        ScanAllPuzzles();
        return m_allPuzzles;
    }

    public int ScanAllPuzzles()
    {
        int numberOfFiles = 0;

        foreach(var jsonfile in Directory.GetFiles(Directories.SAVED_PUZZLES))
        {
            numberOfFiles += 1;
            try
            {
                using (StreamReader sr = new StreamReader(jsonfile))
                {
                    string str = sr.ReadToEnd();
                    PuzzleJSON puzzleJSON = DeserializeJSON(str);
                    m_allPuzzles.Add(puzzleJSON);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogFormat("Problem reading file: {0}", jsonfile);
                throw ex;
            }
        }

        return numberOfFiles;
    }

    private PuzzleJSON DeserializeJSON(string str)
    {
        return JsonUtility.FromJson<PuzzleJSON>(str);
    }
}
