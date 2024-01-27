using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControllerSceneUI : MonoBehaviour
{
    [SerializeField] private GameObject ControllerHBox;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < ControllerHBox.transform.childCount; i++)
        {
            int index = i;
            Button button = ControllerHBox.transform.GetChild(i).gameObject.GetComponent<Button>();
            button.onClick.AddListener(() => OnControllerButtonPressed(index));
        }

        NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnectedFromServer;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnExitButtonPressed()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnDisconnectedFromServer;
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Scenes/ClientScene");
    }

    public void OnControllerButtonPressed(int buttonIndex)
    {
        NetworkController.Instance.ClientSendPlayerInputRequest(buttonIndex.ToString());
    }

    private void OnDisconnectedFromServer(ulong obj)
    {
        print($"Disconnected from server! Parameter: {obj}");
        OnExitButtonPressed();
    }
}
