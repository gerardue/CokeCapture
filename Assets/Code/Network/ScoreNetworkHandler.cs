using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreNetworkHandler : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_scoreText;
    
    [SerializeField]
    private PhotonView m_photonView;
    
    private int m_scoreUser = 0;
    private int m_currentScore = 0;
    
    private GameState m_gameState;

    public int GetScore => m_scoreUser; 
    
    public Action<float> OnScore; 
    
    #region Unity Methods

    // private void Awake()
    // {
    //     if(m_score != null)
    //         m_score.onClick.AddListener(UpdateScore);
    // }

    #endregion

    #region Public Methods

    public void Iniatialize(TextMeshProUGUI text)
    {
        m_scoreText = text;
    }
    
    public void UpdateScore(GameState gameState)
    {
        if (gameState == GameState.Playing || GameController.Instance.userType == UserType.Master)
        {
            m_currentScore++;
            SendScore(m_currentScore);
            m_photonView.RPC("SendScore", RpcTarget.MasterClient, m_currentScore);
        }
    }

    public void ResetScore()
    {
        m_currentScore = 0;
        m_scoreUser = 0;
        OnScore?.Invoke(0);
        if(m_scoreText != null)
            m_scoreText.gameObject.SetActive(false);
    }

    [PunRPC]
    public void SendScore(int playerScore)
    {
        m_scoreUser = playerScore;
        Score();
        OnScore?.Invoke((float)m_scoreUser/100);
    }

    public void Score()
    {
        if(m_scoreText != null)
            m_scoreText.text = m_scoreUser.ToString();
    }

    #endregion
}
