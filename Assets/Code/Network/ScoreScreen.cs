using TMPro;
using UnityEngine;


public class ScoreScreen : MonoBehaviour
{
    [Header("Score Screen")]
    [SerializeField]
    private GameObject m_scoreScreen;
    [SerializeField]
    private TextMeshProUGUI m_congratulation;
    [SerializeField]
    private TextMeshProUGUI m_finalScore;
    
    #region Public Methods

    public void Initialize(string userName, int score)
    {
        // Set Score Screen
        m_scoreScreen.SetActive(true);
        
        if(score >= GameConstData.TARGET_SCORE)
            m_congratulation.text = $"Felicitaciones {userName} \n Tu puntaje ha sido: {score}";
        else
            m_congratulation.text = $"Vuelve a intentarlo {userName} \n Tu puntaje ha sido: {score}";
    }

    public void Dispose()
    {
        
    }
    
    #endregion

    #region Private Methods

    

    #endregion
}
