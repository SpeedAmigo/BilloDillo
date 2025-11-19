using System.Net;
using System.Net.Sockets;
using FishNet.Managing;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private string sceneToLoad;

    public void StartHost()
    {
        networkManager.ServerManager.StartConnection();
        networkManager.ClientManager.StartConnection();
        
        SceneManager.LoadScene(sceneToLoad);

        string ipAdress = GetLocalIPAddress();
        Debug.Log(ipAdress);
    }

    public void StartClient()
    {
        string connectionIp = inputField.text;

        if (connectionIp == string.Empty)
        {
            Debug.LogWarning("field can't be empty");
            return;
        }
        
        networkManager.ClientManager.StartConnection(connectionIp);
        
        SceneManager.LoadScene(sceneToLoad);
    }
    
    public static string GetLocalIPAddress()
    {
        foreach (var ni in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (ni.AddressFamily == AddressFamily.InterNetwork &&
                !IPAddress.IsLoopback(ni))
            {
                return ni.ToString();
            }
        }

        return "0.0.0.0";
    }
}
