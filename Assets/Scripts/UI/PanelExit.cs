using UnityEngine;

public class PanelExit : PanelBase
{
    protected override void OnClickOK()
    {
        base.OnClickOK();
        Application.Quit();
    }
}
