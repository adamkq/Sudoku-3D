using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    private MasterController m_masterController;

    [SerializeField] private GameObject m_menuStart;
    [SerializeField] private GameObject m_menuSelectPuzzle;
    
    void Start()
    {
        m_masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        m_menuSelectPuzzle.SetActive(false);
    }

    public void MenuLoadScene(string sceneName)
    {
        m_masterController.LoadScene(sceneName);
    }

    public void OnClickGoToMenuSelectPuzzle()
    {
        m_menuSelectPuzzle.SetActive(true);
        m_menuStart.SetActive(false);
    }
}
