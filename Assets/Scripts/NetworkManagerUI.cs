using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] NetworkManager networkManager;
    [SerializeField] GameObject DropdownCanvas;
    public GameObject go;
    private void OnGUI()
    {
        if (GUILayout.Button("Host"))
        {
            networkManager.StartHost();
            InstantiateDropdownCanvas();
        }

        if (GUILayout.Button("Join"))
        {
            networkManager.StartClient();
            InstantiateDropdownCanvas();
        }

        if (GUILayout.Button("Quit"))
        {
            Application.Quit();
        }
    }

    private void InstantiateDropdownCanvas()
    {
        if (go == null)
        {
            go = Instantiate(DropdownCanvas, Vector3.zero, Quaternion.identity);
        }
    }
}
