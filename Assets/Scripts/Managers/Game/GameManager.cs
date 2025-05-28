using ConsentService;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public class GameManager : GameBase
{
    public new static GameManager Instance => (GameManager)GameBase.Instance;

    //[Header("Configs")]
    ////public LevelsConfig levels;

    //[Header("Boosters")]
    ////public ProgressRewardsConfig progressRewardsConfig;

    //[Space]
    //public BoosterLootItemConfig boosterPillar;
    //public BoosterLootItemConfig boosterSpirit;
    //public BoosterLootItemConfig boosterUndo;

    //[Header("Prefabs")]
    //public ResourceView resourceViewPrefab;

    // [Header("Economics")]
    // public int finishLevelCoins;
    // public int boosterPrice; 
    // public int boosterItems;

    //[Header("Audio")]
    //[SerializeField] private AudioMixer _audioMixer;
    //[SerializeField] private string _soundEffectsVolume;
    //[SerializeField] private string _musicEffectsVolume;
    //[Space]
    //[SerializeField] private AudioHitInfo _gameMusicEnd;
    //[SerializeField] public AudioHitInfo skipAdAudio;

    //// public LevelInfoModel tempLevel;
    //[Header("Unsorted")]
    //public AdManager.InterstitialAdsConfig interstitialAdsConfig;
    //public TutorialsConfig tutorialsConfig;
    //[SerializeField] private bool rewardedEnabled;

//    public override IFirebaseRemoteConfig GetRemoteConfigInsctance() =>
//#if USE_FIREBASE
//        RemoteConfigManager.Instance;
//#else
//        null;
//#endif



    protected override void Awake()
    {
        base.Awake();
    }

    protected async override void Start()
    {
        base.Start();
#if !UNITY_EDITOR
        Application.targetFrameRate = 60;
#endif

        using (SimpleNavigation.Instance.Push<BlockerScreen, BlockerIntent>(
            new BlockerIntent { State = BlockerScreen.TypeState.Full }))
        {

            var (adConcent, analyticsConcent) = await WaitForConsent().Task;

            Debug.Log($"conserns: adConcent - {adConcent}, analyticsConcent - {analyticsConcent}");

            ConsentListenerBehaviour.SendConsentToAll(new ConsentServiceData
            {
                ad = adConcent,
                analytics = analyticsConcent
            });


#if USE_FIREBASE
            await BaseFirebaseManager.WaitConfigReady();
            
            if (GameManager.Instance.SessionIndex <= 1)
            {
                await BaseFirebaseManager.WaitConfigActual(5f); 
            }
#endif

        }

        await Task.Delay(1); // Wait to all modules initialisation

        SimpleNavigation.Instance.Push<MainMenuScreen, EmptyModel>();
    }


    private static TaskCompletionSource<(bool, bool)> WaitForConsent()
    {
        var tcs = new TaskCompletionSource<(bool, bool)>();
        tcs.SetResult((true, true));
        //TinySauce.SubscribeOnInitFinishedEvent((adConcent, analyticsConcent) =>
        //{
        //   tcs.SetResult((adConcent, analyticsConcent));
        //});
        return tcs;
    }


    protected async override void OnSessionStart()
    {

        //{
        //    await BaseFirebaseManager.WaitConfigActual();

        //    RemoteConfigManager.Balance.simplifyer?.OnStartSession((float)TimeFromPreviosSessionEnd.TotalHours);
        //}
    }

}
