using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TokenMenuCloseButton : MonoBehaviour
{
    [SerializeField] private Button button;
    public TokenMenuManager tokenMenuManager { get; set; }

    public void CloseMenu()
    {
        tokenMenuManager.CloseMenu();
    }
}
