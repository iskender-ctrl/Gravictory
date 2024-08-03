using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BotAI : MonoBehaviourPun, IPunObservable
{
    public float moveSpeed = 3.0f;
    public float detectionRange = 10.0f;

    private Vector3 moveDirection;
    private Transform targetPlayer;
    private float timer;
    private float changeDirectionTime = 2.0f;

    private void Start()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }

        timer = changeDirectionTime;
        ChangeDirection();
    }

    private void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }

        DetectPlayer();
        if (targetPlayer != null)
        {
            FollowPlayer();
        }
        else
        {
            Wander();
        }
    }

    private void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                targetPlayer = hit.transform;
                break;
            }
        }
    }

    private void FollowPlayer()
    {
        Vector3 direction = (targetPlayer.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
    }

    private void Wander()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ChangeDirection();
            timer = changeDirectionTime;
        }

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    private void ChangeDirection()
    {
        float angle = Random.Range(0, 360);
        moveDirection = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Veri gönderme
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // Veri alma
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }
}