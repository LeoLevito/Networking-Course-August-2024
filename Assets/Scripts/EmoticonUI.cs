using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EmoticonUI : NetworkBehaviour
{
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
            dropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(); }); //Delegate
            OnDropdownValueChanged();
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
    private void OnDropdownValueChangedRPC(FixedString128Bytes data)
    {
        replicatedText.Value = data;
    }
}
