using UnityEngine;
using UnityEngine.SceneManagement;

public class MasterController : MonoBehaviour
{
    [SerializeField] private GameObject stateManagerPrefab;

    private GameObject stateManager;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        stateManager = Instantiate(stateManagerPrefab);
    }

    void Start()
    {
        LoadScene("MainMenu");
    }

    // All scene changes will be executed here
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
