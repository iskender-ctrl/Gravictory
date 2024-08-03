using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    public static List<PlayerInfo> AllPlayers = new List<PlayerInfo>();
    private List<List<PlayerInfo>> teams = new List<List<PlayerInfo>>();
    private const int teamSize = 5;
    private const int maxPlayers = 10;
    private const float searchDuration = 15f; // 1 dakika

    public GameObject searchingPopup;
    private Coroutine searchCoroutine;

    private void Start()
    {
        ConnectToPhoton();
    }

    public void OnPlayButtonClicked()
    {
        // Oyun arama pop-up'ını göster
        searchingPopup.SetActive(true);

        // Oyun arama işlemini başlat
        StartGame();

        // Zamanlayıcı başlat
        searchCoroutine = StartCoroutine(SearchTimeoutCoroutine());
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
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = maxPlayers };
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

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.NickName} entered the room");

        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayers)
        {
            Debug.Log("Maximum oyuncu sayısına ulaşıldı.");
            if (searchCoroutine != null)
            {
                StopCoroutine(searchCoroutine);
            }
            CreateTeams();
            if (ShouldStartGame())
            {
                StartGameWithTeams();
            }
        }
    }

    private IEnumerator SearchTimeoutCoroutine()
    {
        yield return new WaitForSeconds(searchDuration);

        if (PhotonNetwork.CurrentRoom.PlayerCount < maxPlayers)
        {
            AddBotsToTeams();
        }

        CreateTeams();
    }

    private void CreateTeams()
    {
        teams.Clear(); // Mevcut takımları temizle
        AllPlayers.Clear();

        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            Player player = PhotonNetwork.PlayerList[i];
            PlayerInfo playerInfo = new PlayerInfo(player);
            AddPlayerToTeam(new PlayerInfo(player));
            AllPlayers.Add(playerInfo);
        }

        // Eğer maksimum oyuncu sayısına ulaşıldıysa veya süre dolduysa botları ekleyelim
        if (PhotonNetwork.CurrentRoom.PlayerCount < maxPlayers)
        {
            AddBotsToTeams();
        }

        // Takımları yazdır
        PrintTeams();

        // Eğer oyun başlatma kriterleri karşılanıyorsa oyunu başlat
        if (ShouldStartGame())
        {
            StartGameWithTeams();
        }
    }

    private bool ShouldStartGame()
    {
        return teams.Count >= 2;
    }

    private void AddPlayerToTeam(PlayerInfo playerInfo)
    {
        if (teams.Count == 0 || teams[teams.Count - 1].Count >= teamSize)
        {
            teams.Add(new List<PlayerInfo>());
        }

        teams[teams.Count - 1].Add(playerInfo);
    }

    private void AddBotsToTeams()
    {
        // Botları takımlara ekle
        int totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        int botsToAdd = maxPlayers - totalPlayers;

        for (int i = 0; i < botsToAdd; i++)
        {
            BotPlayer bot = new BotPlayer($"Bot_{i + 1}");
            PlayerInfo botInfo = new PlayerInfo(bot);
            AddPlayerToTeam(new PlayerInfo(bot));
            AllPlayers.Add(botInfo);
        }
    }

    private void PrintTeams()
    {
        for (int i = 0; i < teams.Count; i++)
        {
            Debug.Log($"Team {i + 1}:");
            foreach (var playerInfo in teams[i])
            {
                if (playerInfo.IsBot)
                {
                    Debug.Log($"  Bot: {playerInfo.Bot.NickName}");
                }
                else
                {
                    Debug.Log($"  Player: {playerInfo.Player.NickName}");
                }
            }
        }
    }

    private void StartGameWithTeams()
    {
        Debug.Log("Teams are created! Game is starting.");
        searchingPopup.SetActive(false); // Oyun arama pop-up'ını gizle

        // Oyun başlatma kodu buraya eklenmeli
        PhotonNetwork.LoadLevel("InPlanet");
    }
}

public class PlayerInfo
{
    public Player Player { get; private set; }
    public BotPlayer Bot { get; private set; }
    public bool IsBot { get; private set; }

    public PlayerInfo(Player player)
    {
        Player = player;
        IsBot = false;
    }

    public PlayerInfo(BotPlayer bot)
    {
        Bot = bot;
        IsBot = true;
    }
}

public class BotPlayer
{
    public string NickName { get; private set; }

    public BotPlayer(string nickName)
    {
        NickName = nickName;
    }
}