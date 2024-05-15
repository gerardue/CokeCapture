using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class GameStateData
{
    public string UserId;
    public bool IsGameAvailable;
}

/// <summary>
/// This is responsible for the admin part
/// </summary>
public class MasterNetworkPun : MonoBehaviourPunCallbacks
{
    public static MasterNetworkPun Instance;
    
    [SerializeField]
    private bool m_isAwake;
    [SerializeField]
    private string m_userId = string.Empty;
    [SerializeField]
    private string m_roomName = string.Empty;
    
    [SerializeField]
    private List<UserData> m_playerIds;

    [Header("UI")]
    [SerializeField]
    private GameObject m_ui;
    [SerializeField]
    private TextMeshProUGUI m_score;
    [SerializeField]
    private TextMeshProUGUI m_userName;

    [Header("Timer")]
    [SerializeField]
    private float timerForPlayer = 5;
    [SerializeField]
    private TextMeshProUGUI m_timerText;
    [SerializeField]
    private TextMeshProUGUI m_gameTimer;

    [Header("Score Network")]
    [SerializeField]
    private ScoreNetworkHandler m_scoreNetwork;
    [SerializeField]
    private Image m_bottle;

    [Header("QR Controller")]
    [SerializeField]
    private QREncodeAdapter m_qrEncondeAdapter;
    
    // Internal data
    public bool m_isGameAvailable = true;

    public bool IsGameAvailable
    {
        get => m_isGameAvailable;
        set => m_isGameAvailable = value;
    }
    
    #region Unity Methods

    private void Start()
    {
        Instance = this;
        m_isAwake = GameController.Instance.userType == UserType.Master;
        if(!m_isAwake)
            return;

        PhotonNetwork.EnableCloseConnection = true; 
        m_ui.SetActive(true);
        GameController.Instance.userType = UserType.Master;
        m_roomName = GameConstData.ROOM_NAME;
        m_userId = RandomNetworkTool.GetRandomUser();
        m_playerIds = new List<UserData>();
        
        PhotonNetwork.NickName = m_userId;
        PhotonNetwork.AuthValues = new AuthenticationValues
        {
            UserId = m_userId
        };
        PhotonNetwork.ConnectUsingSettings();
        
        // m_finishGame.onClick.AddListener(FinishGame);
        m_scoreNetwork.OnScore = FillBottle;
    }

    #endregion

    #region Public Methods

    #region API Photon

    public override void OnConnected()
    {
        if(!m_isAwake)
            return;
        
        Debug.Log("Connected");
    }
    
    public override void OnConnectedToMaster()
    {
        if(!m_isAwake)
            return;
        
        PhotonNetwork.JoinLobby(); 
        Debug.Log("Connected to master");
    }

    public override void OnJoinedLobby()
    {
        if(!m_isAwake)
            return;
        
        CreateRoom();
    }

    public override void OnCreatedRoom()
    {
        if(!m_isAwake)
            return;
        
        Debug.Log("Room Created");
    }
    
    public override void OnJoinedRoom()
    {
        if(!m_isAwake)
            return;
        
        Debug.Log($"Room Joined, Amount players {PhotonNetwork.CurrentRoom.PlayerCount}");
    }
    
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if(!m_isAwake)
            return;
        
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log($"{returnCode} {message}");
    }

    #endregion

    [ContextMenu("Amount Players")]
    public void PrinAMountPlayers()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount + " player count");
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Debug.Log(player.UserId);
        }
    }

    public void RemovePlayer(string userId)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Debug.Log(player.UserId);
            if (player.UserId == userId)
            {
                Debug.Log("Close Conection");
                PhotonNetwork.CloseConnection(player);
            }
        }
    }
    
    /// <summary>
    /// Room creation
    /// </summary>
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            MaxPlayers = 5,
            PublishUserId = true
        };
        m_roomName += m_userId;
        string url = $"{GameConstData.URL}?room={m_roomName}";
        PhotonNetwork.CreateRoom(m_roomName ,roomOptions);
        m_qrEncondeAdapter.Encode(url);
        Debug.Log($"Room Craeted {url}");
    }
    
    /// <summary>
    /// Every player connected to room are going to be register and added to a list.
    /// If it is unique player on the room, the game will begin.
    /// </summary>
    public void ReceivePlayerId(UserData userData)
    {
        m_playerIds.Add(userData);
        
        // if is game available and is the first player on file can start game
        if(m_isGameAvailable && m_playerIds.IndexOf(userData) == 0)
            SendGameStateToUser(userData);
    }
    
    /// <summary>
    /// Start and show the principal mechanic of bottle on master client side
    /// </summary>
    public void ShowGame()
    {
        // Stop Timer
        StopAllCoroutines();
        m_timerText.gameObject.SetActive(false);
        
        // Show bottle
        m_isGameAvailable = false;
        
        // Start game timer
        StartCoroutine(GameTimer());
    }
    
    /// <summary>
    /// Finish game on master client, this should stops after a few seconds
    /// </summary>
    public void FinishGame()
    {
        // Get data to save on local file as .cvs
        SaveDataOnLocalStorage(m_playerIds[0].UserOwnData); 
        
        m_scoreNetwork.ResetScore();
        m_isGameAvailable = true;
        
        StopAllCoroutines();
        
        // Get player data and save its name and its score
        m_gameTimer.gameObject.SetActive(false);
        NextPlayer();
    }
    
    #endregion

    #region Private Methods
    
    private void NextPlayer()
    {
        // Remove current player
        m_playerIds.RemoveAt(0);

        if (m_playerIds.Count > 0)
        {
            m_isGameAvailable = true;
            SendGameStateToUser(m_playerIds[0]);
            // Show a timer to wait the player
        }
        else
        {
            m_userName.text = "";
        }
    }
    
    private void SendGameStateToUser(UserData userData)
    {
        // Prepare Data
        object[] data = new object[]
        {
            m_isGameAvailable,
            userData.UserId
        };
        
        // Send event
        byte eventCode = (byte)MsgType.GameState;
        RaiseEventOptions options = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.Others
        };
        
        PhotonNetwork.RaiseEvent(eventCode, data, options, SendOptions.SendReliable);
        
        // Start Timer
        StartCoroutine(TimerToPlayerStartGame());
        m_isGameAvailable = false;
        string userName = GetName(userData.UserOwnData);
        m_userName.text = $"Es el turno de {userName}";
    }

    private void FillBottle(float value)
    {
        m_bottle.fillAmount = value;
    }
    
    #region Timers

    private IEnumerator TimerToPlayerStartGame()
    {
        float totalTime = timerForPlayer;
        float elapsedTime = 0;
        
        m_timerText.gameObject.SetActive(true);
        
        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            m_timerText.text = elapsedTime.ToString("F0"); 
            yield return null;
        }

        RemovePlayer(m_playerIds[0].UserId);
        
        m_isGameAvailable = true; 
        m_timerText.gameObject.SetActive(false);
        NextPlayer();
    }
    
    private IEnumerator GameTimer()
    {
        float totalTime = GameConstData.GAME_DURATION;
        float elapsedTime = 0;
        
        m_gameTimer.gameObject.SetActive(true);
        
        while (totalTime >= 0)
        {
            totalTime -= Time.deltaTime;
            m_timerText.text = totalTime.ToString("F0"); 
            yield return null;
        }
        
        m_gameTimer.gameObject.SetActive(false);
        
        FinishGame();
    }
    
    #endregion
    
    #region Save Data (csv)

    /// <summary>
    /// Save player data on local data base (csv file)
    /// </summary>
    private void SaveDataOnLocalStorage(string userData)
    {
        int score = m_scoreNetwork.GetScore;
        string userName = GetName(userData);
        string userEmail = GetEmail(userData);
        string userPhone = GetPhone(userData);
        string rowData = $"{userName}, {userEmail}, {userPhone}, {score}";
        ConvertToCSV.RecordData(GameConstData.HEADER_ROW, rowData);
    }
    
    private string GetName(string userData)
    {
        int indexStartWord = userData.IndexOf("Nombre:") + "Nombre:".Length;
        int indexFinishWord = userData.IndexOf("Email:");
        string word = userData.Substring(indexStartWord, indexFinishWord - indexStartWord).Trim();
        return word;
    }
    
    private string GetEmail(string userData)
    {
        int indexStartWord = userData.IndexOf("Email:") + "Email:".Length;
        int indexFinishWord = userData.IndexOf("Telefono:");
        string word = userData.Substring(indexStartWord, indexFinishWord - indexStartWord).Trim();
        return word;
    }
    
    private string GetPhone(string userData)
    {
        int indexStartWord = userData.IndexOf("Telefono:") + "Telefono:".Length;
        int indexFinishWord = userData.Length;
        string word = userData.Substring(indexStartWord, indexFinishWord - indexStartWord).Trim();
        return word;
    }

    #endregion
    
    #endregion
}