using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    private MasterController m_masterController;

    [SerializeField] private GameObject m_MenuParentObj;
    
    void Start()
    {
        m_masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        OnClickSetChildOnlyActive(0);
    }

    public void MenuLoadScene(string sceneName)
    {
        m_masterController.LoadScene(sceneName);
    }

    // set one menu active and disable all others
    public void OnClickSetChildOnlyActive(int child)
    {
        for (int i = 0; i < m_MenuParentObj.transform.childCount; i++)
        {
            GameObject menu = m_MenuParentObj.transform.GetChild(i).gameObject;

            if (i == child)
            {
                menu.SetActive(true);
            }
            else
            {
                menu.SetActive(false);
            }
        }
    }
}
