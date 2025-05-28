using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SessionManagerBase : Entity<SessionModel>
{
    [SerializeField] private GameObject _coreGO;
    protected CoreGameplayManagerBase CoreManager { get; private set; }

    public override void InitialPrepare()
    {
        base.InitialPrepare();
        CreateCore();
    }

    private void CreateCore()
    {
        var coreGO = Instantiate(_coreGO, transform);
        CoreManager = coreGO.GetComponent<CoreGameplayManagerBase>();
    }

    public override void Prepare()
    {
        base.Prepare();
        Model.CurrentState = SessionModel.SessionState.Waiting;
    }

    public abstract void RestartSession();

    public abstract void EndSessionToMenu();

    public abstract void EndSession();

    public abstract void StartSession();

    public virtual void PauseSession()
    {
        if (Model.CurrentState != SessionModel.SessionState.Running) return;
        Model.CurrentState = SessionModel.SessionState.Paused;
        Time.timeScale = 0f; // Pause the game
    }

    public virtual void ResumeSession()
    {
        if (Model.CurrentState != SessionModel.SessionState.Paused) return;
        Model.CurrentState = SessionModel.SessionState.Running;
        Time.timeScale = 1f; // Resume the game
    }

    public override void Refresh()
    {
        CoreManager.SetModel(new GameCoreModel
        {
            SessionManager = this,
            Player = Model.Player
        });
        StartSession(); 
    }

}


public class SessionModel
{
    public enum SessionState { Waiting, Running, Paused, Win, Lose }

    private SessionState _state = SessionState.Waiting;
    public SessionState CurrentState
    {
        get => _state;
        set
        {
            if (_state == value) return;
            _state = value;
            OnStateChanged?.Invoke(_state);
        }
    }
    public PlayerController Player { get; set; }

    public event Action<SessionState> OnStateChanged;
}
