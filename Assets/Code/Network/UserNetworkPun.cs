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
    private bool m_isAwake;
    [SerializeField]
    private string m_userId = string.Empty;
    [SerializeField]
    private string m_roomName;
    
    [Header("UI")]
    [SerializeField]
    private GameObject m_ui;
    [SerializeField]
    private TMP_InputField m_inputField;
    [SerializeField]
    private TMP_InputField m_userNameInputField;
    [SerializeField]
    private Button m_joinRoom;
    [SerializeField]
    private TextMeshProUGUI m_tempDebug;

    [Header("Other UI")]
    [SerializeField]
    private GameObject m_waitingSign;
    [SerializeField]
    private Button m_startButton;
    [SerializeField]
    private Button m_finishButton;

    public string UserId => m_userId;
    
    #region Unity Methods

    private void Start()
    {
        Instance = this;
        m_isAwake = GameController.Instance.userType == UserType.User;
        
        if(!m_isAwake)
            return;

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
        
        // Join Room
        m_joinRoom.onClick.AddListener(() =>
        {
            var room = m_inputField.text; 
            Debug.Log(room + " name");
            JoinRoom(room);
        });
        
        m_startButton.onClick.AddListener(StartGameEventNetwork);
        m_finishButton.onClick.AddListener(FinishGame);
        m_waitingSign.SetActive(true);
    }

    // private void OnApplicationQuit()
    // {
    //     if(!m_isAwake)
    //         return;
    //     
    //     LeftRoom();
    // }

    #endregion

    #region Public Methods

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

    public override void OnJoinedRoom()
    {
        if(!m_isAwake)
            return;

        byte eventCode = (byte)MsgType.AddPlayerId;
        object[] data = new object[]
        {
            m_userId, 
            m_userNameInputField.text
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
        m_tempDebug.text = "Fila llena, intenta mas tarde";
    }

    public override void OnLeftRoom()
    {
        if(!m_isAwake)
            return;
        
        Debug.Log("Room Left");
    }

    public void JoinRoom(string room)
    {
        PhotonNetwork.JoinRoom(room);
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
        
        Debug.Log("Can start game");
        
        if (isGameAvailable && m_userId == userId)
        {
            Debug.Log("it can Init Game");
            m_tempDebug.text += "You can init game";
            // Disable text that says "Waiting your turn"
            m_waitingSign.SetActive(false);
            m_startButton.gameObject.SetActive(true);
            // Then the player must to press button "Start Game"
            // Start Game
        }
        else
        {
            Debug.Log("please wait your turn");
            m_tempDebug.text += "Please wait your turn";
        }
    }
    
    #endregion

    #region Private Methods

    private void StartGameEventNetwork()
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

    [ContextMenu("Finish Game")]
    private void FinishGame()
    {
        // Stop game and avoid that user continues playing
        PhotonNetwork.Disconnect();
    }

    #endregion
}