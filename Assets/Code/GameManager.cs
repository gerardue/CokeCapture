using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameState
{
    Waiting,
    Playing,
    Finished
}

public class GameManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text scoreTextFin;
    public TMP_Text timerText;
    public float gameTimeInSeconds = 60f;
    public GameObject[] targetPrefabs;
    public Transform[] spawnPoints;
    public float initialSpawnDelay = 1f;
    public float spawnFrequencyDecreaseRate = 0.1f;
    public GameObject registro;
    public GameObject finalizar;
    public Image botellaLoading;
    private float timer;
    private int score = 0;
    private GameState state = GameState.Waiting;
    private float currentSpawnDelay;
    private float currentSpawnFrequency;

    private void Start()
    {
        registro.SetActive(true); // Activar el panel de registro al inicio
        finalizar.SetActive(false); // Desactivar el panel final al inicio
    }

    public void StartGame()
    {
        state = GameState.Playing;
        registro.SetActive(false); // Desactivar el panel de registro al iniciar el juego
        finalizar.SetActive(false); // Asegurarse de que el panel final esté desactivado al iniciar el juego

        timer = gameTimeInSeconds;
        currentSpawnDelay = initialSpawnDelay;
        currentSpawnFrequency = initialSpawnDelay;
        UpdateUI();
        StartCoroutine(SpawnTargets());
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
        scoreText.text = score.ToString();
        scoreTextFin.text = score.ToString();
        botellaLoading.fillAmount = (float)score / 100f;
    }


    public void IncrementScore()
    {
        if (state == GameState.Playing)
        {
            score++;
            UpdateUI();
        }
    }

    private void GameOver()
    {
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
}