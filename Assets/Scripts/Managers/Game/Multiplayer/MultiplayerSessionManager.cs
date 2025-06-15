using Mirror;
using UnityEngine;

public class MultiplayerSessionManager : SessionManagerBase
{
    [Header("Network bootstrap")]
    [SerializeField] private NetworkManager networkManagerPrefab;

    public NetworkManager NetworkManager => _net;

    private NetworkManager _net;
    private MultiplayerGameScreen gameScreen;

    public override void Prepare()
    {
        base.Prepare();

        if (NetworkManager.singleton == null)
        {
            var go = Instantiate(networkManagerPrefab.gameObject);     
            _net = go.GetComponent<NetworkManager>();

        }
        else
        {
            _net = NetworkManager.singleton;
        }


        Model.CurrentState = SessionModel.SessionState.Waiting;
    }

    public override void StartSession()
    {
        //if (_net.mode == NetworkManagerMode.Offline)
        //    CoreManager.Refresh();
        gameScreen = SimpleNavigation.Instance.Push<MultiplayerGameScreen, ConnectScreenModel>(new ConnectScreenModel
        {
            SessionManager = this,
            NetworkManager = _net
        });
    }

    public override void RestartSession() {}

    public override void EndSessionToMenu() {}

    public override void EndSession() {}

    public override void KillSession() {}


    //public void StartHost()
    //{
    //    if (_net.mode != NetworkManagerMode.Offline) return;
    //    _net.StartHost();
    //    ListenForConnections();
    //}

    //public void StartClient(string address)
    //{
    //    if (_net.mode != NetworkManagerMode.Offline) return;
    //    _net.networkAddress = string.IsNullOrWhiteSpace(address) ? "localhost" : address;
    //    _net.StartClient();
    //    ListenForConnections();
    //}

    //public void StopHostOrClient()
    //{
    //    if (_net.mode == NetworkManagerMode.Host ||
    //        _net.mode == NetworkManagerMode.ServerOnly)
    //        _net.StopHost();
    //    else if (_net.mode == NetworkManagerMode.ClientOnly)
    //        _net.StopClient();

    //    Model.CurrentState = SessionModel.SessionState.Waiting;
    //}


    //void ListenForConnections()
    //{
    //    //_net.OnServerAddPlayer = OnServerAddPlayer;
    //    //_net.OnClientConnect = OnClientConnected;
    //    //_net.OnClientDisconnect = OnClientDisconnected;
    //}


    //void OnServerAddPlayer(NetworkConnectionToClient conn)
    //{
    //    //var spawn = Vector3.zero;
    //    //var player = Instantiate(Net.playerPrefab, spawn, Quaternion.identity);
    //    //NetworkServer.AddPlayerForConnection(conn, player);

    //    //// У хоста Core запускаем сразу
    //    //if (conn == NetworkServer.localConnection)
    //    //    CoreManager.Refresh();
    //}


    //void OnClientConnected(NetworkConnection conn)
    //{
        
    //}


    //void OnClientDisconnected(NetworkConnection conn)
    //{
    //    CoreManager.KillCoreLoop();
    //    StopHostOrClient();
    //}

}
