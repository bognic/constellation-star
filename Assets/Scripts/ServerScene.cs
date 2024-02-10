using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerScene : MonoBehaviour
{
    [SerializeField] private TMP_Text IPText;
    [SerializeField] private GameObject PlayerVBoxList;
    [SerializeField] private PlayerBox PlayerBoxPrefab;

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

        NetworkServerController.Instance.OnClientConnected += UpdatePlayers;
        NetworkServerController.Instance.OnClientDisconnected += UpdatePlayers;
        NetworkServerController.Instance.OnClientMessageReceived += OnClientMessageReceived;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.GetComponent<PeerUDP>().Send(IPText.text);
    }

    public void OnSwitchToClientButtonPressed()
    {
        Destroy(NetworkServerController.Instance.gameObject);
        SceneManager.LoadScene("Scenes/ClientScene");
    }

    public void UpdatePlayers()
    {
        for (int i = 0; i < PlayerVBoxList.transform.childCount; i++)
        {
            int index = i;
            PlayerVBoxList.transform.GetChild(i).gameObject.GetComponent<PlayerBox>().KickRequest -= NetworkServerController.Instance.KickPlayer;
            Destroy(PlayerVBoxList.transform.GetChild(i).gameObject);
        }

        ushort[] ids = NetworkServerController.Instance.GetAllConnectedClientIDs();
        foreach (ushort id in ids)
        {
            PlayerBox box = Instantiate(PlayerBoxPrefab);
            box.InitPlayer(id, $"Player {id}");
            box.transform.SetParent(PlayerVBoxList.transform, false);
            box.KickRequest += NetworkServerController.Instance.KickPlayer;
        }
    }

    public void OnClientMessageReceived(ushort id, string data)
    {
        print("yaaas");
    }
}
