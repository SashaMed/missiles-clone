using CoreKit.Runtime.Platform.UI.Basic;

public class GameScreen : BasicScreen<GameScreenModel>
{

}


public class GameScreenModel
{
    public SessionManagerBase SessionManager { get; set; }
}