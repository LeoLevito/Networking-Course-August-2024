using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EmoticonUI : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Text emoticonText;
    [SerializeField] GameObject DropdownCanvas;
    Dropdown dropdown;
    NetworkManagerUI networkManagerUI;

    NetworkVariable<FixedString128Bytes> replicatedText = new NetworkVariable<FixedString128Bytes>("");

    void Start()
    {
        emoticonText.text = replicatedText.Value.ToString(); 
        if (IsLocalPlayer)
        {
            networkManagerUI = FindObjectOfType<NetworkManagerUI>();
            dropdown = networkManagerUI.go.GetComponentInChildren<Dropdown>();
            dropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(); }); //DELEGATE!
            OnDropdownValueChanged();//this is crazy, because player 2 is essentially 2 player objects at the same time, one local, one on the host, they get different DropdownCanvases! 
        }
        else
        {
            replicatedText.OnValueChanged += UpdateText;
        }
    }

    private void UpdateText(FixedString128Bytes previousValue, FixedString128Bytes newValue)
    {
        emoticonText.text = newValue.Value;
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
        replicatedText.Value = data;
    }
}
