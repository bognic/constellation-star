using UnityEngine;
using UnityEngine.SceneManagement;

public class BootScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadSceneAsync("Scenes/ClientScene");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
