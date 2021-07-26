using UnityEngine;
using UnityEngine.SceneManagement;

public class MasterController : MonoBehaviour
{
    public StateManager stateManager;
    [SerializeField] private Solver solver;

    
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

    // *** The Rest of this file consists of Mediator Functions *** //

    public char GetCellValue(int[] cellIndex)
    {
        return stateManager.BoardState[cellIndex[0], cellIndex[1], cellIndex[2]];
    }

    public bool SetCellValue(int[] cellIndex, char cellValue)
    {
        stateManager.BoardState[cellIndex[0], cellIndex[1], cellIndex[2]] = cellValue;
        return true;
    }


}
