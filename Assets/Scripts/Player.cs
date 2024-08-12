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
    [SerializeField] Transform objectSpawnPosition;

    private int cameraZOffset = -50;
    private float rotationSpeed;
    private float acceleration;

    private float lastRotation;
    private float lastAcceleration;

    private float thrust; //(space)
    private float spin;
    private Rigidbody2D rb2d;

    private Camera mainCamera;

    NetworkVariable<float> health = new NetworkVariable<float>();
    private void Awake()
    {

        health.OnValueChanged += NotifyUI;
        rb2d = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    private void NotifyUI(float previousValue, float newValue)
    {
        
    }

    private void Start()
    {
        health.Value = 100;
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

    public void OnSend()
    {
        Debug.Log("AA");
    }

    private void OnMove(Vector2 input)
    {
        MoveRPC(input);
    }


    
    private void Update()
    {
        if (IsServer)
        {
            transform.position += transform.up * moveInput.Value.y * Time.deltaTime; //not sure if deltaTime can differ between client and server and if that has an effect on latency.
      
        }
        transform.Rotate(transform.rotation.x, transform.rotation.y, moveInput.Value.x * 100 * -Time.deltaTime); //Wait I'm confused, why does this transfer to the server if it's not in the if-statement? Is it because of the moveInput variable? 
    }

    [Rpc(SendTo.Server)] //DECORATOR, MAY BE DECORATOR PATTERN!!!
    private void SpawnRPC()
    {
        NetworkObject ob = Instantiate(objectToSpawn).GetComponent<NetworkObject>();
        ob.transform.position = objectSpawnPosition.transform.position;
        ob.transform.rotation = transform.rotation;
        ob.Spawn();
    }

    [Rpc(SendTo.Server)] //DECORATOR, MAY BE DECORATOR PATTERN!!!
    private void MoveRPC(Vector2 data)
    {
        moveInput.Value = data;
    }

    public void TakeDamage()
    {
        health.Value -= 15f;
        if(health.Value <= 0)
        {
            RespawnTest();
            GetComponent<NetworkObject>().Despawn();
        }
    }

    IEnumerator RespawnTest()
    {
        yield return new WaitForSeconds(5);
        Debug.Log("Man, debug.log is quite nice");
        yield return null;
    }// this has to be on the server, can't respawn if you're destroyed.
}
