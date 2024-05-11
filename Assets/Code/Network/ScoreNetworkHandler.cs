using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreNetworkHandler : MonoBehaviour
{
    [SerializeField]
    private Button m_score;
    [SerializeField]
    private TextMeshProUGUI m_scoreText;

    [SerializeField]
    private PhotonView m_photonView;

    private int m_scoreUser = 0;
    private int m_currentScore = 0;
    
    #region Unity Methods

    private void Awake()
    {
        if(m_score != null)
            m_score.onClick.AddListener(ScoreAction);
    }

    #endregion

    #region Public Methods

    public void ScoreAction()
    {
        m_currentScore++;
        m_photonView.RPC("SendScore", RpcTarget.All, m_currentScore);
    }
    
    [PunRPC]
    public void SendScore(int playerScore)
    {
        m_scoreUser = playerScore;
        Score();
    }

    public void Score()
    {
        m_scoreText.text = m_scoreUser.ToString();
    }

    #endregion
}
