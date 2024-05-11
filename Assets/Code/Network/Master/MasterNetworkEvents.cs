using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MasterNetworkEvents : MonoBehaviour, IOnEventCallback
{
    [SerializeField]
    private MasterNetworkPun m_masterNetwork;
    
    private void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte evenCode = photonEvent.Code;

        switch (evenCode)
        {
            case (byte)MsgType.AddPlayerId:
                object[] userDataToAdd = (object[])photonEvent.CustomData;
                UserData data = new UserData()
                {
                    UserId = (string)userDataToAdd[0],
                    UserOwnData = (string)userDataToAdd[1]
                };
                m_masterNetwork.ReceivePlayerId(data);
                break;
            case (byte)MsgType.StartGame:
                byte isGameAvailable = (byte)photonEvent.CustomData;
                m_masterNetwork.ShowGame();
                // Receive player name in the custom data
                break;
        }
    }
}
