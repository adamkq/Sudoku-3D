using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCubeUIController : MonoBehaviour
{
    private MasterController mc;

    private void Start()
    {
        mc = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
    }

    public void LoadMenuScene(string sceneName)
    {
        Debug.Log(sceneName);
        mc.LoadScene(sceneName);
    }
}
