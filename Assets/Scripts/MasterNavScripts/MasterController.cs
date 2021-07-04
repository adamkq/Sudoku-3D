using UnityEngine;
using UnityEngine.SceneManagement;

public class MasterController : MonoBehaviour
{
    [SerializeField] private GameObject stateManagerPrefab;

    private GameObject stateManager;

    public string sliceTargetFaceName { get; set; }
    public string sliceDownwardFaceName { get; set; }
    private StateManager sm;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        stateManager = Instantiate(stateManagerPrefab);
        sm = stateManager.GetComponent<StateManager>();
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

    public char GetCellValue(int[] cellIndex)
    {
        return sm.BoardState[cellIndex[0], cellIndex[1], cellIndex[2]];
    }
}
