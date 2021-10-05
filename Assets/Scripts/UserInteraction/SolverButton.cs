using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SolverButton : MonoBehaviour
{
    [SerializeField] private Button button;
    private MasterController _mc;

    void Awake()
    {
        button.onClick.AddListener(TaskOnClick);
    }

    void Start()
    {
        _mc = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
    }

    void TaskOnClick()
    {
        int numSolutions = _mc.solver.SolveBacktrack(_mc.stateManager.BoardState, true);
        Debug.Log(numSolutions);
    }
}
