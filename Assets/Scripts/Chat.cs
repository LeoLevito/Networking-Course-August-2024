using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
      //  string text = "Hello";
      //  text.Length * 2; //check length and then adapt FixedString64Bytes or 128Bytes or similar. One char is apparently 2 bytes 
      //  text.ToCharArray().Length; //or this. But it's easier to just limit input chat box to a specific amount of characters.

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
