using System;
using UnityEngine;

public class GameBase : MonoBehaviourSingleton<GameBase>
{

    //public class BaseSettings
    //{

    //    public class Lang
    //    {
    //        public readonly string Value;
    //        public readonly string Title;

    //        public Lang(string value, string title)
    //        {
    //            Value = value;
    //            Title = title;
    //        }
    //    }

    //    public virtual Lang[] GetAvailableLanguages() => new Lang[] { new("", "<i>Default</i>") };


    //    protected PrefsValue<bool> soundPrefs = new PrefsValue<bool>("OptionSounds", true);
    //    protected PrefsValue<bool> vibratePrefs = new PrefsValue<bool>("OptionVibrate", true);
    //    protected PrefsValue<string> langPrefs = new PrefsValue<string>("OptionLang", "");

    //    public virtual bool soundsValue { get => soundPrefs.Value; set => soundPrefs.Value = value; }

    //    public virtual bool vibrateValue { get => vibratePrefs.Value; set => vibratePrefs.Value = value; }

    //    public virtual string langValue { get => langPrefs.Value; set => langPrefs.Value = value; }

    //}

    //public BaseSettings Settings => (settingsInstance ?? (settingsInstance = CreateSettingsInstance()));

    //private BaseSettings settingsInstance;
    //protected virtual BaseSettings CreateSettingsInstance() => new BaseSettings();
    //public virtual IFirebaseRemoteConfig GetRemoteConfigInsctance() => null;

    protected PrefsValue<int> session = new PrefsValue<int>("_session_index", 0);
    protected PrefsValue<DateTime> sessionEndTime = new PrefsValue<DateTime>("_session_end_time");
    protected PrefsValue<DateTime> sessionStartTime = new PrefsValue<DateTime>("_session_start_time");

    protected PrefsValue<DateTime> installTime = new PrefsValue<DateTime>("_install_time");

    protected long sessionTimeout = 5 * 60 * 1000; // 5 minutes session timeout
    protected UpdateTimeout sessionCheckTimer = 5f; // 5 seconds session refresh timeout

    public int SessionIndex => session.Value; // (1... ) 

    public DateTime InstallTime => installTime.Value;
    public TimeSpan SessionTime => (DateTime.UtcNow - sessionStartTime.Value);

    public TimeSpan PreviosSessionTime { get; private set; }
    public TimeSpan TimeFromPreviosSessionEnd { get; private set; }

    /// <summary>
    /// In Seconds
    /// </summary>
    public float SessionLongTime => (float)(DateTime.UtcNow - sessionStartTime.Value).TotalSeconds;

    protected virtual void Start()
    {
        OnAppResume();
    }

    protected virtual void Update()
    {
        if (sessionCheckTimer)
            sessionEndTime.Value = DateTime.UtcNow;
    }

    protected void CheckSessionRefresh()
    {
        if (session.Value <= 0)
        {
            FirstSession();
            StartSession(); // First Session

            return;
        }

        if ((DateTime.UtcNow - sessionEndTime.Value).TotalMilliseconds > sessionTimeout)
        {
            StartSession();
        }
    }

    protected virtual void StartSession()
    {
        if (!installTime.HasKey())
        {
            FirstSession();
        }
        else
        {
            TimeFromPreviosSessionEnd = (DateTime.UtcNow - sessionEndTime.Value);
            PreviosSessionTime = (sessionEndTime.Value - sessionStartTime.Value);
        }


        session.Value += 1;

        sessionStartTime.Value = DateTime.UtcNow;
        sessionEndTime.Value = DateTime.UtcNow;

        Debug.Log($"[NEW SESSION ({session.Value})]");

        OnSessionStart();
    }

    protected virtual void FirstSession()
    {
        installTime.Value = DateTime.UtcNow;
    }

    protected virtual void OnAppResume()
    {
        CheckSessionRefresh();
    }

    protected virtual void OnAppSuspend() { }

    protected virtual void OnSessionStart() { }



    private void OnApplicationPause(bool pause) // Not lounched on startup on device
    {
        if (!pause)
            OnAppResume();
        else
            OnAppSuspend();

    }
    private void OnApplicationQuit() => OnApplicationPause(true);


}