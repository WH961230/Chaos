using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkManager : Mirror.NetworkManager {
    public override void OnServerConnect(NetworkConnectionToClient conn) {
        base.OnServerConnect(conn);
        Debug.Log("OnServerConnect");
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn) {
        base.OnServerDisconnect(conn);
        Debug.Log("OnServerDisconnect");
    }

    public override void OnStartHost() {
        base.OnStartHost();
        Debug.Log("OnStartHost");
    }

    public override void OnStartServer() {
        base.OnStartServer();
        Debug.Log("OnStartServer");
    }

    public override void OnStartClient() {
        base.OnStartClient();
        Debug.Log("OnStartClient");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn) {
        base.OnServerAddPlayer(conn);
        Debug.Log("OnServerAddPlayer");
    }
}