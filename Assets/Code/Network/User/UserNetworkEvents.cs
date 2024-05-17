using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class UserNetworkEvents : MonoBehaviour, IOnEventCallback
{
    #region Unity Methods

    private void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    #endregion
    
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = (byte)photonEvent.Code;

        switch (eventCode)
        {
            case (byte) MsgType.GameState:
                // if(GameController.Instance.userType == UserType.Master)
                //     return;
                Debug.Log("Enter on event");
                object[] data = (object[])photonEvent.CustomData;
                UserNetworkPun.Instance.CanStartGame(data);
                break;
        }
    }
}
