using System;
using System.IO;
using System.Text;
using UnityEngine;

public class PuzzleSave : MonoBehaviour
{
    private string m_dirSavedPuzzles;
    [SerializeField] private MasterController _mc;
    [SerializeField] private string m_puzzleName;

    [Tooltip("Click this to save the current board state as a file w/ current name")]
    [SerializeField] private bool m_SaveButton;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        m_dirSavedPuzzles = Directories.SAVED_PUZZLES;
    }

    private void OnValidate()
    {
        if(m_SaveButton)
        {
            PuzzleJSON puzzleJSON = new PuzzleJSON();

            puzzleJSON.puzzleTitle = m_puzzleName;
            puzzleJSON.serializeBoardGivens = SerializeToString(_mc.stateManager.BoardGivens);
            puzzleJSON.serializeBoardState = SerializeToString(_mc.stateManager.BoardState);
            puzzleJSON.lastModified = DateTime.Now;

            string actualJSON = JsonUtility.ToJson(puzzleJSON);

            SaveData(m_puzzleName, actualJSON);
        }

        m_SaveButton = false;
    }

    public void SaveData(string fname, string actualJSON)
    {
        string fullpath = m_dirSavedPuzzles + fname;
        using (StreamWriter swriter = new StreamWriter(fullpath))
        {
            swriter.WriteLine(actualJSON);
        }
    }

    private string SerializeToString(int[,,] board)
    {
        StringBuilder sb = new StringBuilder("", board.GetLength(0) * board.GetLength(1) * board.GetLength(2));

        for(int ii = 0; ii < board.GetLength(0); ii++)
        {
            for (int jj = 0; jj < board.GetLength(1); jj++)
            {
                for (int kk = 0; kk < board.GetLength(2); kk++)
                {
                    sb.Append(board[ii, jj, kk].ToString());
                }
            }
        }
        return sb.ToString();
    }

    private string SerializeToString(char[,,] board)
    {
        StringBuilder sb = new StringBuilder("", board.GetLength(0) * board.GetLength(1) * board.GetLength(2));

        for (int ii = 0; ii < board.GetLength(0); ii++)
        {
            for (int jj = 0; jj < board.GetLength(1); jj++)
            {
                for (int kk = 0; kk < board.GetLength(2); kk++)
                {
                    sb.Append(board[ii, jj, kk]);
                }
            }
        }
        return sb.ToString();
    }
}
