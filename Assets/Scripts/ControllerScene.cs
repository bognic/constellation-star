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

        NetworkClientController.Instance.OnDisconnected += OnDisconnected;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnExitButtonPressed()
    {
        Destroy(NetworkClientController.Instance.gameObject);
        SceneManager.LoadScene("Scenes/ClientScene");
    }

    private void OnControllerButtonPressed(int buttonIndex)
    {
        NetworkClientController.Instance.SendInputToServer(buttonIndex.ToString());
    }

    private void OnDataReceived()
    {

    }
    private void OnDisconnected()
    {
        print("heeeere");
        OnExitButtonPressed();
    }
}
