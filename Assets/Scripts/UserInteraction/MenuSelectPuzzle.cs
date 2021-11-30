using UnityEngine;
using System.Collections;

public class MenuSelectPuzzle : MonoBehaviour
{
    private MasterController _mc;

    private void Start()
    {
        _mc = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
    }

    void RefreshMenu()
    {

    }
}
