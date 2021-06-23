using UnityEngine;
using UnityEngine.SceneManagement;

public class MasterController : MonoBehaviour
{
    [SerializeField] private GameObject stateManagerPrefab;

    private GameObject stateManager;

    public string sliceTargetFaceName { get; set; }
    public string sliceDownwardFaceName { get; set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        stateManager = Instantiate(stateManagerPrefab);
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
}
