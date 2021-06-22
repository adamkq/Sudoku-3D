using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    private MasterController mc;

    void Start()
    {
        mc = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
    }

    public void MenuLoadScene(string sceneName)
    {
        mc.LoadScene(sceneName);
    }
}
