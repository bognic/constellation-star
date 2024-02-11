using System;
using TMPro;
using UnityEngine;

public class ServerBox : MonoBehaviour
{
    [SerializeField] private TMP_Text ServerText;

    public Action<string> OnJoinRequest { get; set; }

    public string _ip { get; private set; }
    private string _name;

    private float _time = 0;
    private float _timeoutInterval = 5;

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;
        if (_time > _timeoutInterval)
        {
            OnJoinRequest = null;
            Destroy(gameObject);
        }
    }

    public void Init(string ip, string name)
    {
        _ip = ip;
        _name = name;
        ServerText.text = _name;
    }
    public void Refresh()
    {
        _time = 0;
    }

    public void OnJoinButtonPressed()
    {
        if (OnJoinRequest is not null)
            OnJoinRequest.Invoke(_ip);
    }
}
