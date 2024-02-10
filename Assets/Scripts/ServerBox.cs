using TMPro;
using UnityEngine;

public class ServerBox : MonoBehaviour
{
    [SerializeField] private TMP_Text ServerText;

    private string _ip;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitServer(string ip)
    {
        _ip = ip;
        ServerText.text = _ip;
    }
}
