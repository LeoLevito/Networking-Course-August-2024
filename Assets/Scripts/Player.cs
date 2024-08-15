using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    [SerializeField] InputReader inputReader;

    NetworkVariable<Vector2> moveInput = new NetworkVariable<Vector2>(); //will be replicated across the network. on every copy of an object. writePerm Owner allows owner to change varable.
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] Transform objectSpawnPosition;
    [SerializeField] Canvas emoticonCanvas;
    NetworkVariable<float> health = new NetworkVariable<float>(100);
    bool isDead = false;

    private void Start()
    {
        if (inputReader != null && IsLocalPlayer) //localplayer check prevents two clients trying to write to the same player or something. Can also check if server and whatnot.
        {
            inputReader.MoveEvent += OnMove;
            inputReader.ShootEvent += SpawnRPC;
        }
        emoticonCanvas.transform.SetParent(null, true);
        emoticonCanvas.transform.rotation = Quaternion.identity;
    }

    private void OnMove(Vector2 input)
    {
        MoveRPC(input);
    }

    private void Update()
    {
        if (!isDead)
        {
            if (IsServer)
            {
                transform.position += transform.up * moveInput.Value.y * Time.deltaTime; //not sure if deltaTime can differ between client and server and if that has an effect on latency.
            }
            transform.Rotate(transform.rotation.x, transform.rotation.y, moveInput.Value.x * 100 * -Time.deltaTime); //Wait I'm confused, why does this transfer to the server if it's not in the if-statement? Is it because of the moveInput variable? 
            emoticonCanvas.transform.position = objectSpawnPosition.transform.position;
        }
    }

    [Rpc(SendTo.Server)] //DECORATOR, MAY BE DECORATOR PATTERN!!!
    private void SpawnRPC()
    {
        NetworkObject ob = Instantiate(objectToSpawn).GetComponent<NetworkObject>();
        ob.transform.position = objectSpawnPosition.transform.position;
        ob.transform.rotation = transform.rotation;
        ob.Spawn(true);
    }

    [Rpc(SendTo.Server)] //DECORATOR, MAY BE DECORATOR PATTERN!!!
    private void MoveRPC(Vector2 data)
    {
        moveInput.Value = data;
    }

    public void TakeDamage()
    {
        health.Value -= 15f;
        Debug.Log("Damage taken");
        if (health.Value <= 0)
        {
            HidePlayerRPC(false);
            isDead = true;
            StartCoroutine(RespawnTimer());
        }
    }

    [Rpc(SendTo.Everyone)]
    private void HidePlayerRPC(bool state)
    {
        foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.enabled = state;
        }
        emoticonCanvas.GetComponentInChildren<Text>().enabled = state;
        GetComponent<CapsuleCollider2D>().enabled = state;
    }

    IEnumerator RespawnTimer()
    {
        yield return new WaitForSeconds(5);
        HidePlayerRPC(true);
        health.Value = 100;
        isDead = false;
    }

    new private void OnDestroy()
    {
        Destroy(emoticonCanvas.gameObject);
    }
}
