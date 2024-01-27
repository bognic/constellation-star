using System.Net;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerScene : MonoBehaviour
{
    [SerializeField] private GameObject PlayerVBoxList;
    [SerializeField] private GameObject PlayerBoxPrefab;
    [SerializeField] private TMP_Text IPText;
    [SerializeField] private NetworkController Network;

    // Start is called before the first frame update
    void Start()
    {
        IPText.text = string.Empty;
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                IPText.text = "IP: " + ip.ToString();
            }
        }
        if (string.IsNullOrWhiteSpace(IPText.text))
            IPText.text = "IP unknown!";

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        //NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = IPText.text;
        NetworkManager.Singleton.StartServer();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSwitchToClientButtonPressed()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadSceneAsync("Scenes/ClientScene");
    }

    private void OnClientConnected(ulong obj)
    {
        print($"Server: Client connected to server. Parameter: {obj}");
        GameObject gobj = Instantiate(PlayerBoxPrefab);
        gobj.transform.SetParent(PlayerVBoxList.transform, false);
    }

    private void OnClientDisconnected(ulong obj)
    {
        print($"Server: Client disconnected from server. Parameter: {obj}");
        // remove player box
    }
}
