using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostScene : MonoBehaviour
{
    public TMP_Text IPText;

    // Start is called before the first frame update
    void Start()
    {
        IPText.text = string.Empty;
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                IPText.text = "IP: " + ip.ToString();
            }
        }
        if (string.IsNullOrWhiteSpace(IPText.text))
            IPText.text = "IP: Unknown!";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSwitchHostButtonPressed()
    {
        SceneManager.LoadSceneAsync("Scenes/BootScene");
    }
}
