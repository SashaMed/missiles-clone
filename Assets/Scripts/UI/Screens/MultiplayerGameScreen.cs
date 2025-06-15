using CoreKit.Runtime.Platform.UI.Basic;
using Mirror;
using TMPro;
using UnityEngine;

public class MultiplayerGameScreen : BasicScreen<ConnectScreenModel>
{

    public TextMeshProUGUI transportText;
    public TextMeshProUGUI serverText;
    public TextMeshProUGUI clientText;
    //public TMP_InputField portText;

    public UIStatesBool showStartButtons;
    public UIStates<ConnectionType> connectionStopStates;

    public enum ConnectionType
    {
        None ,
        Server,
        Host,
        Client
    }

    public void StartClient()
    {
        Model.NetworkManager.StartClient();
    }

    public void StartHost()
    {
        Model.NetworkManager.StartHost();
    }

    public void StartServer()
    {
        Model.NetworkManager.StartHost();
    }

    public void ClientReady()
    {
        if (NetworkClient.isConnected && !NetworkClient.ready)
        {
            // client ready
            NetworkClient.Ready();
            if (NetworkClient.localPlayer == null)
                NetworkClient.AddPlayer();
        }
    }

    public void StopHost()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            Model.NetworkManager.StopHost();

        }
    }

    public void StopClientOnHost()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            Model.NetworkManager.StopClient();
        }
    }

    public void StopClientOnClient()
    {
        if (NetworkServer.active)
        {
            Model.NetworkManager.StopServer();
        }
    }

    public void StopServer()
    {
        if (NetworkClient.isConnected)
        {
            Model.NetworkManager.StopClient();
        }
    }

    public void PortTextChange(string text)
    {
        var portText = text;
        if (Transport.active is PortTransport portTransport)
        {
            if (ushort.TryParse(portText.ToString(), out ushort port))
                portTransport.Port = port;
        }
    }

    public void AddressTextChange(string text)
    {
        Model.NetworkManager.networkAddress = text;
    }

    private void StatusLabels()
    {
        // host mode
        // display separately because this always confused people:
        //   Server: ...
        //   Client: ...
        if (NetworkServer.active && NetworkClient.active)
        {
            // host mode
            transportText.text = $"<b>Host</b>: running via {Transport.active}";
        }
        else if (NetworkServer.active)
        {
            // server only
            serverText.text =  $"<b>Server</b>: running via {Transport.active}";
        }
        else if (NetworkClient.isConnected)
        {
            // client only
            clientText.text = $"<b>Client</b>: connected to {Model.NetworkManager.networkAddress} via {Transport.active}";
        }
    }

    private void Update()
    {
        StatusLabels();
        showStartButtons.SetState(!NetworkClient.isConnected && !NetworkServer.active);
        var connectionType = ConnectionType.None;

        if (NetworkServer.active && NetworkClient.isConnected)
        {
            connectionType = ConnectionType.Host;
        }
        else if (NetworkClient.isConnected)
        {
            connectionType = ConnectionType.Client;
        }
        else if (NetworkServer.active)
        {
            connectionType = ConnectionType.Server;
        }

        connectionStopStates.SetState(connectionType);
    }

}


public class ConnectScreenModel
{
    public NetworkManager NetworkManager;
    public SessionManagerBase SessionManager { get; set; }
}