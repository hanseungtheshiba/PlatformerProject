using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelGameOver : PanelBase
{
    protected override void OnClickOK()
    {
        base.OnClickOK();
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    protected override void OnClickCancel()
    {
        base.OnClickCancel();
        Application.Quit();
    }
}
