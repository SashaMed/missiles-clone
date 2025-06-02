using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerSessionManager : SessionManagerBase
{

    private GameScreen gameScreen;

    public override void EndSession()
    {
        Model.CurrentState = SessionModel.SessionState.GameOver;
        SimpleNavigation.Instance.Push<GameOverScreen, GameOverScreenModel>(new GameOverScreenModel
        {
            SessionManager = this
        });
    }

    public override void EndSessionToMenu()
    {
        throw new System.NotImplementedException();
    }

    public override void RestartSession()
    {
        BasicLayer.Instance.FadeWithAction(() =>
        {
            Refresh();
        },
        duration: 1);
    }

    public override void StartSession()
    {
        gameScreen = SimpleNavigation.Instance.Push<GameScreen, GameScreenModel>(new GameScreenModel
        {
            SessionManager = this
        });

    }


}
