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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnManualIPJoinButtonPressed()
    {
        string ip = ManualIPInputField.text;
        bool success = NetworkClientController.Instance.TryConnectToIP(ip);
        if (success)
        {
            SceneManager.LoadScene("Scenes/ControllerScene");
        }
    }
    public void OnSwitchToServerButtonPressed()
    {
        Destroy(NetworkClientController.Instance.gameObject);
        SceneManager.LoadSceneAsync("Scenes/ServerScene");
    }
}
