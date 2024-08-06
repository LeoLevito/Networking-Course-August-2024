using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] InputReader inputReader;

    NetworkVariable<Vector2> moveInput = new NetworkVariable<Vector2>(); //will be replicated across the network. on every copy of an object. writePerm Owner allows owner to change varable.
    [SerializeField] GameObject objectToSpawn;

    private int cameraZOffset = -50;
    private float rotationSpeed;
    private float acceleration;

    private float lastRotation;
    private float lastAcceleration;

    private float thrust; //(space)
    private float spin;
    private Rigidbody2D rb2d;

    private Camera mainCamera;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        if (inputReader != null && IsLocalPlayer) //localplayer check prevents two clients trying to write to the same player or something. Can also check if server and whatnot.
        {
            inputReader.MoveEvent += OnMove;
            inputReader.ShootEvent += SpawnRPC;
            inputReader.SendEvent += OnSend;
        }



    }

    //private void LateUpdate()
    //{
    //    if (IsLocalPlayer)
    //    {
    //        Vector3 currentPosition = transform.position;
    //        currentPosition.z = cameraZOffset;
    //        mainCamera.transform.position = currentPosition;
    //    }
    //}

    private void OnSend()
    {
        Debug.Log("AA");
    }

    private void OnMove(Vector2 input)
    {
        MoveRPC(input);
    }


    
    private void Update()
    {
        if(IsServer)
        {
            transform.position += (Vector3)moveInput.Value * 3 * Time.deltaTime;
        }
    }

    [Rpc(SendTo.Server)] //DECORATOR, MAY BE DECORATOR PATTERN!!!
    private void SpawnRPC()
    {
       NetworkObject ob = Instantiate(objectToSpawn).GetComponent<NetworkObject>();
        ob.Spawn();
    }

    [Rpc(SendTo.Server)] //DECORATOR, MAY BE DECORATOR PATTERN!!!
    private void MoveRPC(Vector2 data)
    {
        moveInput.Value = data;
    }
}
