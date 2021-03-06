using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MasterController : MonoBehaviour
{
    public StateManager stateManager;
    public Solver solver;
    public PuzzleLoad puzzleLoad;
    public PuzzleSave puzzleSave;
    
    public string sliceTargetFaceName { get; set; }
    public string sliceDownwardFaceName { get; set; }

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

    public void TransitionToSliceView(string targetFaceName, string downwardFaceName)
    {
        sliceTargetFaceName = targetFaceName;
        sliceDownwardFaceName = downwardFaceName;
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
        TransitionToSliceView("CubeFaceBack", "CubeFaceBottom");
    }
}
