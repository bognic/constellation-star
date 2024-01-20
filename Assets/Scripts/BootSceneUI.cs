using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BootScene : MonoBehaviour
{
    public Button TestAddButton;
    public Button ManualIPJoinButton;
    public InputField ManualIPInputField;
    public GameObject ServerVBoxList;
    public GameObject ServerBoxPrefab;

    // Start is called before the first frame update
    void Start()
    {
        //GameObject obj = Instantiate(ServerBoxPrefab);
        //obj.transform.SetParent(ServerVBoxList.transform, false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnManualIPJoinButtonPressed()
    {
        print(ManualIPInputField.text);
    }
    public void OnSwitchHostButtonPressed()
    {
        SceneManager.LoadSceneAsync("Scenes/HostScene");
    }
}
