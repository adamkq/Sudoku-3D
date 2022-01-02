using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCubeUIController : MonoBehaviour
{
    private MasterController m_masterController;

    private void Start()
    {
        m_masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
    }

    public void LoadMenuScene(string sceneName)
    {
        Debug.Log(sceneName);
        m_masterController.LoadScene(sceneName);
    }
}
