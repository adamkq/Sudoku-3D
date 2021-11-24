using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SolverButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private BoardManager m_BoardManager;
    private MasterController _mc;
    private TextMeshProUGUI _buttonText;
    

    void Awake()
    {
        button.onClick.AddListener(TaskOnClick);
        _buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start()
    {
        _mc = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
    }

    void TaskOnClick()
    {
        bool isSolved = _mc.solver.IsSolved(_mc.stateManager.BoardState);
        Debug.Log(isSolved);

        if (isSolved)
        {
            _mc.stateManager.ResetBoard();
        }
        else
        {
            int numSolutions = _mc.solver.SolveBacktrack(_mc.stateManager.BoardState, 10);
            Debug.Log(numSolutions);
        }
        m_BoardManager.UpdateAllCells();
        isSolved = _mc.solver.IsSolved(_mc.stateManager.BoardState);

        if (!_buttonText) return;

        _buttonText.text = isSolved ? "Clear" : "Solve";
    }
}
