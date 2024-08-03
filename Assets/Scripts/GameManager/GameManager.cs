using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public GameObject botPrefab;
    public List<Transform> spawnPoints;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //SpawnBots();
        }

        SpawnPlayer();
    }

    private void SpawnBots()
    {
        if (MultiplayerManager.AllPlayers == null || MultiplayerManager.AllPlayers.Count == 0)
        {
            Debug.LogError("No players to spawn.");
            return;
        }

        for (int i = 0; i < MultiplayerManager.AllPlayers.Count; i++)
        {
            var playerInfo = MultiplayerManager.AllPlayers[i];
            Transform spawnPoint = spawnPoints[i % spawnPoints.Count];

            if (playerInfo.IsBot)
            {
                var bot = PhotonNetwork.Instantiate(botPrefab.name, spawnPoint.position, spawnPoint.rotation);
                bot.AddComponent<BotAI>();
            }
        }
    }

    private void SpawnPlayer()
    {
        Transform spawnPoint = spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber % spawnPoints.Count];
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }

    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            SpawnPlayer();
        }
    }
}