using System;
using UnityEngine;

public class InGameOptionsMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject m_confirmExitDialog;
    [SerializeField] private GameObject m_confirmResetDialog;
    [SerializeField] private GameObject m_guidePage;

    private MasterController m_masterController;

    private void Start()
    {
        m_masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
    }

    public void CloseDialogOrPage()
    {
        // close any dialogs or pages opened by this script

    }

    public void OpenDialog(GameObject dialog)
    {
        dialog.SetActive(true);
    }

    public void ExitWithoutSaving()
    {
        m_masterController.LoadScene("MainMenu");
    }

    public void SaveAndExit()
    {

    }

    public void ResetBoard()
    {

    }

    public void ShowGuide()
    {

    }
}
