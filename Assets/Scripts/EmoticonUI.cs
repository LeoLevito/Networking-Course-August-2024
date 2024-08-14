using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//using static System.Net.Mime.MediaTypeNames;

public class EmoticonUI : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Text emoticonText;
    [SerializeField] GameObject DropdownCanvas;
    Dropdown dropdown;
    NetworkManagerUI networkManagerUI;

    NetworkVariable<FixedString128Bytes> replicatedText = new NetworkVariable<FixedString128Bytes>();
    

    void Start()
    {
        if (IsLocalPlayer)
        {
            networkManagerUI = FindObjectOfType<NetworkManagerUI>();
            dropdown = networkManagerUI.go.GetComponentInChildren<Dropdown>();
            dropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(); }); //DELEGATE!
            OnDropdownValueChanged();//this is crazy, because player 2 is essentially 2 player objects at the same time, one local, one on the host, they get different DropdownCanvases! 
        }
        else
        {
            emoticonText.text = replicatedText.Value.ToString();
            
        }
    }

    private void OnDropdownValueChanged()
    {
        emoticonText.text = dropdown.options[dropdown.value].text;

        FixedString128Bytes message = new(emoticonText.text);
        OnDropdownValueChangedRPC(message);
    }
    [Rpc(SendTo.Server)]
    private void OnDropdownValueChangedRPC(FixedString128Bytes data) //okay how to do this?
    {
        OnDropdownValueReceivedRPC(data);
    }

    [Rpc(SendTo.Everyone)]
    private void OnDropdownValueReceivedRPC(FixedString128Bytes data) //okay how to do this?
    {
        if (!IsLocalPlayer)
        {
            emoticonText.text = data.ToString(); //IT FUCKING WORKS OMG!!! Only problem now is when you join it doesn't show anything on the host player...
        }
    }



    //private void OnSend()
    //{
    //    //  string text = "Hello";
    //    //  text.Length * 2; //check length and then adapt FixedString64Bytes or 128Bytes or similar. One char is apparently 2 bytes 
    //    //  text.ToCharArray().Length; //or this. But it's easier to just limit input chat box to a specific amount of characters.

    //    FixedString128Bytes message = new("Hello");
    //    SubmitMessageRPC(message);
    //}

    //[Rpc(SendTo.Server)] //rpc has to be reference types and not variables
    //public void SubmitMessageRPC(FixedString128Bytes message)
    //{
    //    UpdateMessageRPC(message);
    //}

    //[Rpc(SendTo.Everyone)]
    //public void UpdateMessageRPC(FixedString128Bytes message)
    //{
    //    text.text = message.ToString();
    //}
}
