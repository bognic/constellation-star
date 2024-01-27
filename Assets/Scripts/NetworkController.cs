using Unity.Netcode;

public class NetworkController : NetworkBehaviour
{
    public static NetworkController Instance;

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

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ClientSendPlayerInputRequest(string code)
    {
        print($"Owner {OwnerClientId}? {IsOwnedByServer} {IsOwner} {IsClient} {IsServer} {IsHost}"); // soooooooo why ALL false???
        //if (!IsOwner)
        //    return;

        print($"Client: {OwnerClientId} sending to server request code: {code}");
        ServerRequestPlayerInputServerRpc(code, new ServerRpcParams());
    }

    [ServerRpc]
    private void ServerRequestPlayerInputServerRpc(string code, ServerRpcParams serverRpcParams)
    {
        print($"Server: Player {serverRpcParams.Receive.SenderClientId} requests input code: {code}");
    }
}
