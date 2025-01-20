using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MasterController : MonoBehaviour
{
    public StateManager stateManager;
    public Solver solver;
    public PuzzleLoad puzzleLoad;
    public PuzzleSave puzzleSave;

    private List<CellController> cells = new List<CellController>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        LoadScene("MainMenu");
    }

    // All scene changes will be executed via this method
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void TransitionToSliceView()
    {
        LoadScene("SliceViewScene");
    }

    public List<PuzzleJSON> GetAllPuzzles()
    {
        return puzzleLoad.GetAllPuzzles();
    }

    public void InitializePuzzle(PuzzleJSON puzzleJSON)
    {
        // set the stateManager w/ the givens and the state.
        stateManager.InitializePuzzle(puzzleJSON);
        TransitionToSliceView();
    }

    public void InitializePuzzle()
    {
        // set the stateManager w/ the givens and the state. Default puzzle with all cells empty
        stateManager.InitializePuzzle(new PuzzleJSON());
        TransitionToSliceView();
    }

    public void IncludeCellInList(CellController cellController)
    {
        if (cellController != null)
        {
            cells.Add(cellController);
        }
    }

    public void RefreshAllCells()
    {
        foreach(CellController cell in cells)
        {
            cell.UpdateCell();
        }
    }
}
