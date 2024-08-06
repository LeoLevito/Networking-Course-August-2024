using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Chat : NetworkBehaviour
{
    [SerializeField] InputReader inputReader;

    [SerializeField] TextMeshProUGUI text;
    private void Start()
    {
        if(inputReader != null)
        {
            inputReader.SendEvent += OnSend;
        }
    }

    private void OnSend()
    {
        FixedString128Bytes message = new("Hello");
        SubmitMessageRPC(message);
    }
    [Rpc(SendTo.Server)] //rpc has to be reference types and not variables
    public void SubmitMessageRPC(FixedString128Bytes message)
    {
       UpdateMessageRPC(message);
    }

    [Rpc(SendTo.Everyone)]
    public void UpdateMessageRPC(FixedString128Bytes message)
    {
        text.text = message.ToString();
    }
}
