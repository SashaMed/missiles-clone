using CoreKit.Runtime.Platform.UI.Basic;
using System;

public class GameOverScreen : BasicScreen<GameOverScreenModel>
{

    public void OnMenuClick()
    {
        Close();
        Model.SessionManager.EndSessionToMenu();
    }

    public void OnPlayClick()
    {
        Close();
        Model.SessionManager.RestartSession();
    }
}

public class GameOverScreenModel 
{
    public SessionManagerBase SessionManager { get; set; }
}