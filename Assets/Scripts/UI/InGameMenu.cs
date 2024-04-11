using UnityEngine;

public class InGameMenu : MonoBehaviour
{
    [SerializeField]
    private PanelBase panelExit = null;
    
    private void Update()
    {
        if (Input.GetButtonUp("Cancel"))
        {
            panelExit.gameObject.SetActive(true);            
            panelExit.ShowOrHide();
        }
    }
}
