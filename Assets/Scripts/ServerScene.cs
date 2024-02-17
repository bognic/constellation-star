using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerScene : MonoBehaviour
{
    [SerializeField] private TMP_Text IPText;
    [SerializeField] private TMP_Text PlayerCountText;
    [SerializeField] private GameObject PlayerVBoxList;
    [SerializeField] private PlayerBox PlayerBoxPrefab;

    private string _ip = string.Empty;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            _ip = Dns.GetHostEntry(string.Empty).AddressList.FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
            if (!string.IsNullOrWhiteSpace(_ip))
                IPText.text = "IP: " + _ip;
        }
        catch (Exception ex)
        {
            print(ex);
        }
        if (string.IsNullOrWhiteSpace(IPText.text))
            IPText.text = "IP: Unknown!";

        NetworkServerController.Instance.OnClientConnected += OnClientConnected;
        NetworkServerController.Instance.OnClientDisconnected += UpdatePlayers;
        NetworkServerController.Instance.OnClientMessageReceived += OnClientMessageReceived;
        NetworkServerController.Instance.InitUDP(_ip);
    }

    private void OnClientConnected(ushort id)
    {
        NetworkServerController.Instance.SendMessageToClient(id, "init_controller");
        UpdatePlayers();
    }
    private void OnClientMessageReceived(ushort id, string data)
    {
        // parse data
        Dictionary<string, int> dict = new Dictionary<string, int>()
        {
            { "0", 0 },
            { "1", 1 },
            { "2", 2 },
            { "3", 3 }
        };
        print($"Server: Client {id} requests input code: {dict.GetValueOrDefault(data, -1)}");
    }
    private void UpdatePlayers()
    {
        for (int i = 0; i < PlayerVBoxList.transform.childCount; i++)
        {
            int index = i;
            PlayerVBoxList.transform.GetChild(i).gameObject.GetComponent<PlayerBox>().KickRequest -= NetworkServerController.Instance.KickPlayer;
            Destroy(PlayerVBoxList.transform.GetChild(i).gameObject);
        }

        ushort[] ids = NetworkServerController.Instance.GetAllConnectedClientIDs();
        PlayerCountText.text = $"Connected players ({ids.Length}):";
        foreach (ushort id in ids)
        {
            PlayerBox box = Instantiate(PlayerBoxPrefab);
            box.InitPlayer(id, $"Player {id}");
            box.transform.SetParent(PlayerVBoxList.transform, false);
            box.KickRequest += NetworkServerController.Instance.KickPlayer;
        }
    }

    public void OnSwitchToClientButtonPressed()
    {
        Destroy(NetworkServerController.Instance.gameObject);
        SceneManager.LoadScene("Scenes/ClientScene");
    }
}
