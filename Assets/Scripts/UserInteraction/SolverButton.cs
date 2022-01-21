using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SolverButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private BoardManager m_BoardManager;
    [SerializeField] private int m_maxNumberOfSolutions = 1;
    private MasterController m_masterController;
    private TextMeshProUGUI _buttonText;

    void Awake()
    {
        button.onClick.AddListener(TaskOnClick);
        _buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start()
    {
        m_masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
    }

    void TaskOnClick()
    {
        bool isSolved = m_masterController.solver.IsSolved(m_masterController.stateManager.BoardState);
        
        if (isSolved)
        {
            m_masterController.stateManager.ResetBoard();
        }
        else
        {
            int numSolutions = m_masterController.solver.SolveBacktrack(m_masterController.stateManager.BoardState, m_maxNumberOfSolutions);
            Debug.Log(numSolutions);
        }
        m_BoardManager.UpdateAllCells();
        isSolved = m_masterController.solver.IsSolved(m_masterController.stateManager.BoardState);

        if (!_buttonText) return;

        _buttonText.text = isSolved ? "Clear" : "Solve";
    }
}
