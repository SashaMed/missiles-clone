using CoreKit.Runtime.Platform.UI.Basic;

public class PauseScreen : BasicScreen<PauseScreenModel>
{
    public void OnResumeClick()
    {
        if (Model != null && Model.SessionManager != null)
        {
            Model.SessionManager.ResumeSession();

        }
    }

    public void OnGoToMenuClick()
    {
        if (Model != null && Model.SessionManager != null)
        {
            Model.SessionManager.EndSessionToMenu();
        }
    }
}

public class PauseScreenModel 
{
    public SessionManagerBase SessionManager { get; set; }
}