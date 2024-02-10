using DarkRift;
using DarkRift.Server;
using System;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

public class NetworkServerController : MonoBehaviour
{
    public static NetworkServerController Instance;

    private const string _serverConfigFilePath = "Assets/Config/server.config";
    private MainThreadRunner _runner;
    private DarkRiftServer _server;

    public Action OnClientConnected { get; set; }
    public Action OnClientDisconnected { get; set; }
    public Action<ushort, string> OnClientMessageReceived { get; set; }

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
    void Start()
    {
        _runner = transform.GetComponent<MainThreadRunner>();

        ServerSpawnData serverSpawnData = ServerSpawnData.CreateFromXml(_serverConfigFilePath, null);
        _server = new DarkRiftServer(serverSpawnData);
        _server.ClientManager.ClientConnected += ClientManager_ClientConnected;
        _server.ClientManager.ClientDisconnected += ClientManager_ClientDisonnected;
        _server.StartServer();

        print("DarkRift server initialized.");
    }
    void OnDestroy()
    {
        _server.Dispose();
        print("DarkRift server disposed.");
    }

    private void ClientManager_ClientConnected(object sender, ClientConnectedEventArgs e)
    {
        try
        {
            e.Client.MessageReceived += Client_MessageReceived;
            print($"Client {e.Client.ID} connected! Initializing player...");

            _runner.RunOnMainThread.Enqueue(() =>
            {
                if (OnClientConnected is not null)
                    OnClientConnected.Invoke();
            });
        }
        catch (Exception ex)
        {
            print($"Server: {nameof(ClientManager_ClientConnected)} exception");
            print(ex);
        }
    }
    private void ClientManager_ClientDisonnected(object sender, ClientDisconnectedEventArgs e)
    {
        try
        {
            print($"Client {e.Client.ID} disconnected!");
            e.Client.MessageReceived -= Client_MessageReceived;
            _runner.RunOnMainThread.Enqueue(() =>
            {
                if (OnClientDisconnected is not null)
                    OnClientDisconnected.Invoke();
            });
        }
        catch (Exception ex)
        {
            print($"Server: {nameof(ClientManager_ClientDisonnected)} exception");
            print(ex);
        }
    }
    private void Client_MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        try
        {
            using Message message = e.GetMessage();
            using DarkRiftReader reader = message.GetReader();
            string messageStr = reader.ReadString();
            print($"Server: Received message from client {e.Client.ID}: {messageStr}");
            _runner.RunOnMainThread.Enqueue(() =>
            {
                if (OnClientMessageReceived is not null)
                    OnClientMessageReceived.Invoke(e.Client.ID, messageStr);
            });
        }
        catch (Exception ex)
        {
            print($"Server: {nameof(Client_MessageReceived)} exception");
            print(ex);
        }
    }

    public ushort[] GetAllConnectedClientIDs()
        => _server.ClientManager.GetAllClients().Select(x => x.ID).ToArray();

    public void SendMessageToClient(ushort id, string message)
    {
        using DarkRiftWriter writer = DarkRiftWriter.Create();
        writer.Write(message);
        using Message netMessage = Message.Create(0, writer);
        _server.ClientManager.GetClient(id).SendMessage(netMessage, SendMode.Reliable);
        print($"Server: Sent message to client {id}: {message}");
    }

    public void KickPlayer(ushort id)
    {
        ushort[] ids = GetAllConnectedClientIDs();
        if (ids.Contains(id))
        {
            bool result = _server.ClientManager.GetClient(id).Disconnect();
            if (result)
            {
                print($"Server: Kicked player {id}");
            }
            else
            {
                print($"Server: Could not kick player {id}");
            }
        }
        else
        {
            print($"Server: Could not kick player {id}, unknown ID");
        }
    }
}
