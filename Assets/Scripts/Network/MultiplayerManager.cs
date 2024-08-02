using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    private List<List<Player>> teams = new List<List<Player>>();
    private const int teamSize = 5;
    private void Start()
    {
        ConnectToPhoton();
    }
    public void StartGame()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    private void ConnectToPhoton()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 20 };
        PhotonNetwork.CreateRoom(null, roomOptions);
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError($"Disconnected from Photon: {cause}");
    }
    public override void OnCreatedRoom()
    {
        PhotonNetwork.LoadLevel("GravitySystem");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Yeni oyuncuyu takıma ekle
        AddPlayerToTeam(newPlayer);

        if (teams.Count * teamSize == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            // Takımlar oluşturuldu, oyun başlayabilir
            StartGameWithTeams();
        }
    }

    private void AddPlayerToTeam(Player player)
    {
        // Eğer mevcut takım yoksa, yeni bir takım oluştur
        if (teams.Count == 0 || teams[teams.Count - 1].Count >= teamSize)
        {
            teams.Add(new List<Player>()); // Yeni takım oluştur
        }

        teams[teams.Count - 1].Add(player); // Oyuncuyu mevcut takıma ekle
    }

    private void StartGameWithTeams()
    {
        // Takımları kullanarak oyunu başlatabilirsin
        Debug.Log("Takımlar oluşturuldu! Oyun başlıyor.");
        // Oyun başlatma kodunu buraya ekleyebilirsin
    }
}