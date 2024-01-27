using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClientScene : MonoBehaviour
{
    [SerializeField] private Button TestAddButton;
    [SerializeField] private Button ManualIPJoinButton;
    [SerializeField] private InputField ManualIPInputField;
    [SerializeField] private GameObject ServerVBoxList;
    [SerializeField] private GameObject ServerBoxPrefab;
    [SerializeField] private NetworkController Network;

    // Start is called before the first frame update
    void Start()
    {
        //GameObject obj = Instantiate(ServerBoxPrefab);
        //obj.transform.SetParent(ServerVBoxList.transform, false);

        NetworkManager.Singleton.OnClientConnectedCallback += OnConnectedToServer;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnManualIPJoinButtonPressed()
    {
        string ip = ManualIPInputField.text;
        if (!string.IsNullOrWhiteSpace(ip))
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = ip;
            bool success = NetworkManager.Singleton.StartClient();
            if (success)
            {
                print($"Client: IP address {ip} valid, awaiting server...");
            }
            else
            {
                print("Client: Failed to connect using IP: " + ip);
            }
        }
        else
        {
            print("Client: Invalid IP: " + ip);
        }
    }
    public void OnSwitchToServerButtonPressed()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnConnectedToServer;
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadSceneAsync("Scenes/ServerScene");
    }

    private void OnConnectedToServer(ulong obj)
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnConnectedToServer;
        print($"Client: Connection to server established! Parameter: {obj}");
        //SceneManager.LoadScene("Scenes/ControllerScene");
    }
}
