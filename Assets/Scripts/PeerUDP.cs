using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class PeerUDP : MonoBehaviour
{
    public const int TCPPort = 4296;
    public const int UDPPort = 4297;
    public const string BroadcastIP = "255.255.255.255";

    public Action<string> OnUDPDataReceived { get; set; }

    private UdpClient _mainUdpClient;
    private IAsyncResult _udpResult;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init()
    {
        _mainUdpClient = new UdpClient(UDPPort);
        _udpResult = _mainUdpClient.BeginReceive(GetDataInReceive, new());
    }
    public void Send(string message)
    {
        UdpClient client = new UdpClient();
        IPEndPoint ip = new IPEndPoint(IPAddress.Parse(BroadcastIP), UDPPort);
        byte[] bytes = Encoding.ASCII.GetBytes(message);
        client.Send(bytes, bytes.Length, ip);
        client.Close();
        print($"UDP: Sent message {message}");
    }
    public void GetDataInReceive(IAsyncResult ar)
    {
        IPEndPoint ip = new IPEndPoint(IPAddress.Any, UDPPort);
        byte[] bytes = _mainUdpClient.EndReceive(ar, ref ip);
        string message = Encoding.ASCII.GetString(bytes);
        _udpResult = _mainUdpClient.BeginReceive(GetDataInReceive, new());
        print($"UDP: Received message {message}");
        if (OnUDPDataReceived is not null)
            OnUDPDataReceived.Invoke(message);
    }
    
}
