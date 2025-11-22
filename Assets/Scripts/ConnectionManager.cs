using System.Net;
using System.Net.Sockets;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Transporting;
using TMPro;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private string sceneToLoad;

    public void StartHost()
    {
        if (networkManager == null)
        {
            Debug.LogError("NetworkManager not set.");
            return;
        }

        // Load scenes only after the server is fully started.
        networkManager.ServerManager.OnServerConnectionState += OnServerStateChanged;

        networkManager.ServerManager.StartConnection();
        networkManager.ClientManager.StartConnection();
    }

    private void OnServerStateChanged(ServerConnectionStateArgs args)
    {
        if (args.ConnectionState != LocalConnectionState.Started)
            return;

        networkManager.ServerManager.OnServerConnectionState -= OnServerStateChanged;

        if (!string.IsNullOrWhiteSpace(sceneToLoad))
        {
            var sld = new SceneLoadData(sceneToLoad)
            {
                ReplaceScenes = ReplaceOption.All
            };
            
            networkManager.SceneManager.LoadGlobalScenes(sld);
        }

        Debug.Log(GetLocalIPAddress());
    }

    public void StartClient()
    {
        var connectionIp = inputField.text?.Trim();

        if (string.IsNullOrWhiteSpace(connectionIp))
        {
            Debug.LogWarning("IP field can't be empty.");
            return;
        }

        networkManager.ClientManager.StartConnection(connectionIp);
    }

    public static string GetLocalIPAddress()
    {
        foreach (var ni in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (ni.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ni))
                return ni.ToString();
        }

        return "0.0.0.0";
    }
}
