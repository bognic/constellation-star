using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class PeerUDP : MonoBehaviour
{
    public const int TCPPort = 4296;
    public const int UDPPort = 4297;
    public const int UDPBroadcastPort = 5000;
    public const string BroadcastIP = "255.255.255.255";
    [SerializeField] private float _broadcastInterval = 3;

    public Action<string, string> OnUDPDataReceived { get; set; }

    private UdpClient _mainUdpClient;
    private IAsyncResult _udpResult;
    private bool _server = false;
    private string _broadcastMessage = string.Empty;
    private float _broadcastTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        _mainUdpClient = new UdpClient(UDPBroadcastPort);
        _udpResult = _mainUdpClient.BeginReceive(GetDataInReceive, new());
        print("UDP peer initialized.");
    }
    // Update is called once per frame
    void Update()
    {
        if (_server)
        {
            _broadcastTime += Time.deltaTime;
            if (_broadcastTime >= _broadcastInterval)
            {
                Send();
                _broadcastTime = 0;
            }
        }
    }
    void OnDestroy()
    {
        _mainUdpClient.Close();
        _udpResult = null;
        print("UDP peer disposed.");
    }

    public void InitAsServer(string broadcastMessage)
    {
        _broadcastMessage = broadcastMessage;
        _server = true;
        print("UDP peer initialized as server.");
    }
    public void Send()
    {
        if (!string.IsNullOrWhiteSpace(_broadcastMessage))
        {
            UdpClient client = new UdpClient();
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(BroadcastIP), UDPBroadcastPort);
            byte[] bytes = Encoding.ASCII.GetBytes(_broadcastMessage);
            client.Send(bytes, bytes.Length, ip);
            client.Close();
            print($"UDP: Sent message {_broadcastMessage}");
        }
    }
    public void GetDataInReceive(IAsyncResult ar)
    {
        IPEndPoint ip = new IPEndPoint(IPAddress.Any, UDPBroadcastPort);
        byte[] bytes = _mainUdpClient.EndReceive(ar, ref ip);
        string message = Encoding.ASCII.GetString(bytes);
        _udpResult = _mainUdpClient.BeginReceive(GetDataInReceive, new());
        print($"UDP: Received from IP {ip.Address} message {message}");
        if (OnUDPDataReceived is not null)
            OnUDPDataReceived.Invoke(ip.Address.ToString(), message);
    }

}
