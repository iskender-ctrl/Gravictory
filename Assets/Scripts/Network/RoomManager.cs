using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance { get; private set; }

    private List<RoomInfo> availableRooms = new List<RoomInfo>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void CreateRoom(string roomName)
    {
        if (PhotonNetwork.IsConnected)
        {
            RoomOptions options = new RoomOptions { MaxPlayers = 4 };
            PhotonNetwork.CreateRoom(roomName, options, TypedLobby.Default);
        }
    }

    public void JoinRoom(string roomName)
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRoom(roomName);
        }
    }

    public void ListRooms()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        availableRooms = roomList;
        foreach (RoomInfo room in availableRooms)
        {
            Debug.Log($"Room: {room.Name}, Players: {room.PlayerCount}/{room.MaxPlayers}");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.Name}");
        SceneManager.LoadSceneAsync(1);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Room creation failed: {message}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Failed to join room: {message}");
    }

    public override void OnLeftLobby()
    {
        availableRooms.Clear();
    }
}