using System;
using TMPro;
using UnityEngine;

public class PlayerBox : MonoBehaviour
{
    [SerializeField] private TMP_Text PlayerText;

    public Action<ushort> KickRequest { get; set; }

    private ushort _id;
    private string _name;

    public void InitPlayer(ushort id, string name)
    {
        _id = id;
        _name = name;
        PlayerText.text = name;
    }
    public void RequestKickPlayer()
    {
        if (KickRequest is not null)
            KickRequest.Invoke(_id);
    }
}
