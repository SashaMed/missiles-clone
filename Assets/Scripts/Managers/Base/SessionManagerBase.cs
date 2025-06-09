using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SessionManagerBase : Entity<SessionModel>
{
    public bool IsActive => Model.CurrentState != SessionModel.SessionState.GameOver;

    [SerializeField] private GameObject _coreGO;
    protected CoreGameplayManagerBase CoreManager { get; private set; }

    private void CreateCore()
    {
        if (CoreManager != null) return; // Prevent multiple instantiations of the core manager
        var coreGO = Instantiate(_coreGO, Model.ManagersContentHolder);
        CoreManager = coreGO.GetComponent<CoreGameplayManagerBase>();
    }

    public override void Prepare()
    {
        base.Prepare();
        CreateCore();
        Model.CurrentState = SessionModel.SessionState.Waiting;
    }

    public abstract void RestartSession();

    public abstract void EndSessionToMenu();

    public abstract void EndSession();

    public abstract void StartSession();
    public abstract void KillSession();

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
        Model.CurrentState = SessionModel.SessionState.Running;
        CoreManager.SetModel(new GameCoreModel
        {
            SessionManager = this,
            Player = Model.Player,
            GameContentHolder = Model.GameContentHolder,
            ManagersContentHolder = Model.ManagersContentHolder
        });
        StartSession(); 
    }

}


public class SessionModel
{
    public enum SessionState { Waiting, Running, Paused, Win, GameOver }

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

    public Transform GameContentHolder;

    public Transform ManagersContentHolder;
}
