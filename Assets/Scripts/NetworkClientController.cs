using DarkRift;
using DarkRift.Client;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class NetworkClientController : MonoBehaviour
{
    public static NetworkClientController Instance;

    private MainThreadRunner _runner;
    private DarkRiftClient _client;

    public Action OnDisconnected { get; set; }
    public Action<string> OnMessageReceived { get; set; }

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        _runner = transform.GetComponent<MainThreadRunner>();

        _client = new DarkRiftClient();
        _client.MessageReceived += Client_MessageReceived;
        _client.Disconnected += Client_Disconnected;
        print("DarkRift client initialized.");
    }

    // Update is called once per frame
    void Update()
    {
        //var ip = new IPEndPoint(IPAddress.Any, 0);
        //byte[] bytes = _udpClient.Receive(ref ip);
        //if (bytes != null)
        //{
        //    print(bytes);
        //}
    }
    private void OnDestroy()
    {
        _client.Disconnect();
        _client.Dispose();
        print("DarkRift client disposed.");
    }

    public bool TryConnectToIP(string ip)
    {
        if (IPAddress.TryParse(ip, out IPAddress address))
        {
            try
            {
                print($"IP {ip} valid. Trying to connect...");
                _client.Connect(address, tcpPort: PeerUDP.TCPPort, udpPort: PeerUDP.UDPPort, noDelay: true);
                print("Connected to server!");
                return true;
            }
            catch (Exception ex)
            {
                print("Failed to connect!");
                print(ex);
            }
        }
        else
        {
            print("Client: Invalid IP " + ip);
        }

        return false;
    }

    public bool SendInputToServer(string code)
    {
        print($"Client: Sending server message: {code}");
        using DarkRiftWriter writer = DarkRiftWriter.Create();
        writer.Write(code);
        using Message secretMessage = Message.Create(1337, writer);
        bool success = _client.SendMessage(secretMessage, SendMode.Reliable);
        if (!success)
        {
            print($"Client: Failed to send message: {code}");
            return false;
        }
        return true;
    }

    private void Client_MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        try
        {
            using Message message = e.GetMessage();
            using DarkRiftReader reader = message.GetReader();
            string messageStr = reader.ReadString();
            print($"Client: Received server message: {messageStr}");
            _runner.RunOnMainThread.Enqueue(() =>
            {
                if (OnMessageReceived is not null)
                    OnMessageReceived.Invoke(messageStr);
            });
        }
        catch (Exception ex)
        {
            print($"Client: {nameof(Client_MessageReceived)} exception");
            print(ex);
        }
    }
    private void Client_Disconnected(object sender, DisconnectedEventArgs e)
    {
        try
        {
            print("Client: Disconnected from server! Reason: " + e.Error);
            _runner.RunOnMainThread.Enqueue(() =>
            {
                if (OnDisconnected is not null)
                    OnDisconnected.Invoke();
            });
        }
        catch (Exception ex)
        {
            print($"Client: {nameof(Client_Disconnected)} exception");
            print(ex);
        }
    }
}
