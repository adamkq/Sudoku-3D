using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCubeUIController : MonoBehaviour
{
    private MasterController _mc;

    private void Start()
    {
        _mc = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
    }

    public void LoadMenuScene(string sceneName)
    {
        Debug.Log(sceneName);
        _mc.LoadScene(sceneName);
    }
}
