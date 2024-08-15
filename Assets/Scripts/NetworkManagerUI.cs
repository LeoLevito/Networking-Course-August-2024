using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] NetworkManager networkManager;
    [SerializeField] GameObject DropdownCanvas;

    [HideInInspector] public GameObject go;
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

        GUI.Label(new Rect(0, 100, 100, 100),(1 / Time.deltaTime).ToString("0") + " fps");
    }

    private void InstantiateDropdownCanvas()
    {
        if (go == null)
        {
            go = Instantiate(DropdownCanvas, Vector3.zero, Quaternion.identity);
        }
    }

    private void Start() //further latency reductions
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 300;
    }
}
