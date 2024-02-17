using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClientScene : MonoBehaviour
{
    [SerializeField] private Button TestAddButton;
    [SerializeField] private Button ManualIPJoinButton;
    [SerializeField] private InputField ManualIPInputField;
    [SerializeField] private GameObject ServerVBoxList;
    [SerializeField] private ServerBox ServerBoxPrefab;
    [SerializeField] private GameObject ConnectionPanel;
    [SerializeField] private TMP_Text StatusText;
    [SerializeField] private Slider AnimateSlider;

    private MainThreadRunner _runner;

    private bool _animateSliderReverse = false;

    // Start is called before the first frame update
    void Start()
    {
        _runner = transform.GetComponent<MainThreadRunner>();
        transform.GetComponent<PeerUDP>().OnUDPDataReceived += OnUDPDataReceived;
        NetworkClientController.Instance.OnControllerLayoutParsed += OnControllerLayoutParsed;
        NetworkClientController.Instance.OnControllerLayoutParseFailed += OnControllerLayoutParseFailed;
    }

    void Update()
    {
        if (ConnectionPanel.activeSelf)
        {
            AnimateSlider.value = _animateSliderReverse ? AnimateSlider.value - Time.deltaTime : AnimateSlider.value + Time.deltaTime;
            if (_animateSliderReverse && AnimateSlider.value <= 0)
                _animateSliderReverse = false;
            else if (!_animateSliderReverse && AnimateSlider.value >= 1)
                _animateSliderReverse = true;
        }
    }

    private void OnUDPDataReceived(string serverIP, string message)
    {
        try
        {
            _runner.RunOnMainThread.Enqueue(() =>
            {
                for (int i = 0; i < ServerVBoxList.transform.childCount; i++)
                {
                    int index = i;
                    ServerBox box = ServerVBoxList.transform.GetChild(i).gameObject.GetComponent<ServerBox>();
                    if (box._ip == serverIP)
                    {
                        box.Refresh();
                        return;
                    }
                }

                ServerBox serverBox = Instantiate(ServerBoxPrefab);
                serverBox.Init(serverIP, "Server " + message);
                serverBox.transform.SetParent(ServerVBoxList.transform, false);
                serverBox.OnJoinRequest += OnJoinRequest;
                print($"Client: Added server {serverIP} name {message} to list");
            });
        }
        catch (Exception ex)
        {
            print("UDP exception");
            print(ex);
        }
    }
    private void OnJoinRequest(string ip)
    {
        TryToJoinServer(ip);
    }

    public void OnManualIPJoinButtonPressed()
    {
        TryToJoinServer(ManualIPInputField.text);

    }
    public void OnSwitchToServerButtonPressed()
    {
        Destroy(NetworkClientController.Instance.gameObject);
        SceneManager.LoadScene("Scenes/ServerScene");
    }

    private void TryToJoinServer(string ip)
    {
        AnimateSlider.value = 0;
        StatusText.text = $"Trying to connect to {ip}...";
        ConnectionPanel.SetActive(true);
        JoinRoutine(ip);
    }

    private async void JoinRoutine(string ip)
    {
        bool success = await NetworkClientController.Instance.TryConnectToIPAsync(ip);
        if (success)
        {
            _runner.RunOnMainThread.Enqueue(() =>
            {
                StatusText.text = "Connected! Downloading controller layout...";
            });
        }
        else
        {
            _runner.RunOnMainThread.Enqueue(() =>
            {
                ConnectionPanel.SetActive(false);
            });
        }
    }
    private void OnControllerLayoutParsed()
    {
        SceneManager.LoadSceneAsync("Scenes/ControllerScene");
    }
    private void OnControllerLayoutParseFailed()
    {
        ConnectionPanel.SetActive(false);
    }
}
