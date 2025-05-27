
using System;
using UnityEngine;


namespace ConsentService
{
    public abstract class ConsentListenerBehaviour : MonoBehaviour
    {

        [SerializeField] bool onlyConsent;

        private Action initFunction;
        public ConsentServiceData consent { get; private set; }


        protected bool InitByConsentOption(Action initFunction)
        {
            if (onlyConsent)
            {
                SetInitFunction(initFunction);
            }
            else
            {
                Debug.Log($"{this} direct INIT");

                initFunction();

                return true;
            }

            return false;
        }

        protected void SetInitFunction(Action init)
        {
            initFunction = init;

            TryToInit();

        }

        protected virtual void OnGetConsent(ConsentServiceData consentData)
        {
            consent = consentData;

            TryToInit();

        }

        private void TryToInit()
        {

            Debug.Log($"{this} consent func={initFunction != null}, consent={consent != null}");

            if (consent != null && initFunction != null)
            {
                Debug.Log($"{this} CONSENT INIT");
                initFunction.Invoke();
                initFunction = null;

            }
        }


        public static event Action OnConsentUpdated;
        public static ConsentServiceData Actual = new() { ad = false, analytics = false };

        public static void SendConsentToAll(ConsentServiceData consent)
        {
            Actual = consent;
            OnConsentUpdated?.Invoke();

            FindObjectsByType<ConsentListenerBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID)
                .ForEach(l =>
                {
                    try
                    {
                        l.OnGetConsent(consent);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Analytics Exception: {ex.Message}\n{ex.StackTrace}");
                    }
                });
        }
    }


    public class ConsentServiceData
    {
        public bool ad;
        public bool analytics;
    }


    public abstract class SingletonConsentListenerBehaviour<T> : ConsentListenerBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else
                Debug.LogError($"Singleton '{typeof(T).Name}' trying initialise twice!");
        }
    }
}
