using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public enum GameState
{
    Waiting,
    Playing,
    Finished
}

public class UserGameManager : MonoBehaviour
{
    [Header("Register UI")]
    [SerializeField]
    private TMP_InputField m_userName;
    [SerializeField]
    private TMP_InputField m_userEmail;
    [SerializeField]
    private TMP_InputField m_userNumberPhone;
    [SerializeField]
    private Button m_registerButton;

    [Header("Popups")]
    [SerializeField]
    private GameObject m_joinFailed;
    [SerializeField]
    private GameObject m_waitTurn;
    [SerializeField]
    private GameObject m_roomLeft;

    [Header("Other UI")]
    [SerializeField]
    private Button m_startGameButton;
    
    [Header("Game UI")]
    public TMP_Text scoreText;
    public TMP_Text scoreTextFin;
    public TMP_Text timerText;
    public float gameTimeInSeconds = GameConstData.GAME_DURATION;
    public GameObject[] targetPrefabs;
    public Transform[] spawnPoints;
    public float initialSpawnDelay = 1f;
    public float spawnFrequencyDecreaseRate = 0.1f;
    public GameObject registro;
    public GameObject finalizar;
    public Image botellaLoading;

    [Header("Network")]
    [SerializeField]
    private UserNetworkPun m_userNetwork;
    [SerializeField]
    private ScoreNetworkHandler m_scoreNetwork;
    
    private float timer;
    private int score = 0;
    private GameState state = GameState.Waiting;
    private float currentSpawnDelay;
    private float currentSpawnFrequency;

    private List<GameObject> m_popups;

    public GameState GameState => state; 

    private void Start()
    {
        if(GameController.Instance.userType != UserType.User)
            return;
        
        SetNetworkEvents();
        registro.SetActive(true); // Activar el panel de registro al inicio
        finalizar.SetActive(false); // Desactivar el panel final al inicio
    }

    public void StartGame()
    {
        state = GameState.Playing;
        
        // Send network event
        m_userNetwork.StartGameEventNetwork();

        timer = gameTimeInSeconds;
        currentSpawnDelay = initialSpawnDelay;
        currentSpawnFrequency = initialSpawnDelay;
        UpdateUI();
        StartCoroutine(SpawnTargets());
        m_startGameButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (state == GameState.Playing)
        {
            timer -= Time.deltaTime;
            UpdateUI();

            if (timer <= 0f)
            {
                timer = 0f; // Asegurarse de que el tiempo no sea negativo
                state = GameState.Finished;
                GameOver();
            }
        }
    }

    private void UpdateUI()
    {
        timerText.text = Mathf.CeilToInt(timer).ToString();
        scoreTextFin.text = score.ToString();
        // scoreText.text = score.ToString();
        // botellaLoading.fillAmount = (float)score / 100f;
    }


    public void IncrementScore()
    {
        if (state == GameState.Playing)
        {
            // score++;
            // UpdateUI();
            m_scoreNetwork.UpdateScore(state);
        }
    }

    private void GameOver()
    {
        m_userNetwork.FinishGame();
        finalizar.SetActive(true); // Activar el panel final cuando el juego termine
        Time.timeScale = 0f; // Pausar el juego al finalizar
    }

    private IEnumerator SpawnTargets()
    {
        yield return new WaitForSeconds(currentSpawnDelay);

        while (state == GameState.Playing)
        {
            GameObject randomPrefab = targetPrefabs[Random.Range(0, targetPrefabs.Length)];
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            Instantiate(randomPrefab, randomSpawnPoint.position, Quaternion.identity);

            currentSpawnFrequency -= spawnFrequencyDecreaseRate;
            if (currentSpawnFrequency < 0.1f)
            {
                currentSpawnFrequency = 0.1f;
            }

            yield return new WaitForSeconds(currentSpawnFrequency);
        }
    }

    #region Network Functions

    private void RegisterUser()
    {
        string userData = $"Nombre: {m_userName.text} Email: {m_userEmail.text} Telefono: {m_userNumberPhone.text}";
        Debug.Log(userData);
        m_userNetwork.JoinRoom(userData);
    }

    private void SetNetworkEvents()
    {
        // Buttons
        m_registerButton.onClick.AddListener(RegisterUser);
        m_startGameButton.onClick.AddListener(StartGame);
        
        // Events
        m_userNetwork.OnJoinFailed = EnableJoinFailedPopUp;
        m_userNetwork.OnWaitTurn = EnableWaitTurnPopUp;
        m_userNetwork.OnReadyToPlay = ReadyToPlay;
        m_userNetwork.OnRoomLeft = EnableRoomLeft;
        
        // Add popups
        m_popups = new List<GameObject>();
        m_popups.Add(m_waitTurn);
        m_popups.Add(m_joinFailed);
        m_popups.Add(m_roomLeft);
    }

    private void ReadyToPlay()
    {
        DisablePopups();
        m_startGameButton.gameObject.SetActive(true);
    }

    private void SetUpPopUps()
    {
        m_popups.Add(m_waitTurn);
        m_popups.Add(m_joinFailed);
    }

    private void EnableWaitTurnPopUp()
    {
        registro.SetActive(false); // Desactivar el panel de registro al iniciar el juego
        finalizar.SetActive(false); // Asegurarse de que el panel final esté desactivado al iniciar el juego
        EnablePopUp(m_waitTurn);
    }

    private void EnableJoinFailedPopUp()
    {
        EnablePopUp(m_joinFailed);
    }

    private void EnableRoomLeft()
    {
        m_startGameButton.gameObject.SetActive(false);
        EnablePopUp(m_roomLeft);
    }

    private void EnablePopUp(GameObject popUp)
    {
        DisablePopups();
        popUp.SetActive(true);
    }
    
    private void DisablePopups()
    {
        foreach (var popup in m_popups) 
            popup.SetActive(false);
    }

    #endregion
}