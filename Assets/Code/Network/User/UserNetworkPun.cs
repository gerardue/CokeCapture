using System;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// This is responsible for the users part
/// </summary>
public class UserNetworkPun : MonoBehaviourPunCallbacks
{
    public static UserNetworkPun Instance;
    
    [SerializeField]
    private string m_userId = string.Empty;
    [SerializeField]
    private string m_roomName;
    
    [Header("UI")]
    [SerializeField]
    private GameObject m_ui;
    [SerializeField]
    private GameObject m_waitingSignal;

    [SerializeField]
    private TextMeshProUGUI m_debug;

    private string m_userData;
    
    private bool m_isAwake;

    private string roomName;
    
    
    // Events
    public Action OnJoinFailed;
    public Action OnWaitTurn;
    public Action OnReadyToPlay;
    public Action OnRoomLeft; 
    
    public string UserId => m_userId;
    
    #region Unity Methods

    private void Start()
    {
        Instance = this;
        m_isAwake = GameController.Instance.userType == UserType.User;
        
        if(!m_isAwake)
            return;

        #if UNITY_WEBGL && !UNITY_EDITOR
        
        Debug.Log(Application.absoluteURL);
        string data = Application.absoluteURL.Split("?"[0])[1];
        roomName = data.Split("=")[1];
        Debug.Log($"data {data}, room {roomName}");

        // m_debug.text += Application.absoluteURL;
        // m_debug.text += $" data {data}, room {roomName}";
        
        #endif

        m_waitingSignal.SetActive(true);
        
        PhotonNetwork.EnableCloseConnection = true; 
        m_ui.SetActive(true);
        GameController.Instance.userType = UserType.User; 
        m_roomName = GameConstData.ROOM_NAME;
        m_userId = RandomNetworkTool.GetRandomUser();
        PhotonNetwork.NickName = m_userId;
        PhotonNetwork.AuthValues = new AuthenticationValues
        {
            UserId = m_userId
        };
        PhotonNetwork.ConnectUsingSettings();
        
        // Get URL Data
        
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
        m_waitingSignal.SetActive(false);
    }

    public override void OnErrorInfo(ErrorInfo errorInfo)
    {
        base.OnErrorInfo(errorInfo);
        m_debug.text += $"\n {errorInfo.Info}";
    }

    public override void OnJoinedRoom()
    {
        if(!m_isAwake)
            return;

        byte eventCode = (byte)MsgType.AddPlayerId;
        object[] data = new object[]
        {
            m_userId, 
            m_userData
        };
        
        RaiseEventOptions options = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.MasterClient
        };
        
        PhotonNetwork.RaiseEvent(eventCode, data, options, SendOptions.SendReliable);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if(!m_isAwake)
            return;
        
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log($"{returnCode} {message}");
        m_debug.text += $"\n joinroom err {message}";
        OnJoinFailed?.Invoke();
    }

    public override void OnLeftRoom()
    {
        if(!m_isAwake)
            return;
        
        Debug.Log("Room Left");
        OnRoomLeft?.Invoke();
    }

    #endregion

    public void JoinRoom(string userData)
    {
        m_userData = userData;
        PhotonNetwork.JoinRoom(roomName);
    }

    [ContextMenu("Players amount")]
    public void AmountPlayer()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount + " players amount");
    }

    public void CanStartGame(object[] data)
    {
        bool isGameAvailable = (bool)data[0];
        string userId = (string)data[1];
        
        if (isGameAvailable && m_userId == userId)
        {
            OnReadyToPlay?.Invoke();
            return;
        }
        // else
        // {
        //     OnWaitTurn?.Invoke();
        //     Debug.Log("Waiting turn");
        // }
    }
    
    public void StartGameEventNetwork()
    {
        // Disable button "StartGame" & start game
        
        byte data = 1;
        byte eventCode = (byte)MsgType.StartGame;
        RaiseEventOptions options = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.MasterClient
        };
        
        PhotonNetwork.RaiseEvent(eventCode, data, options, SendOptions.SendReliable);
    }
    
    public void FinishGame()
    {
        // Stop game and avoid that user continues playing
        PhotonNetwork.Disconnect();
    }
    
    #endregion

    #region Private Methods



    #endregion
}