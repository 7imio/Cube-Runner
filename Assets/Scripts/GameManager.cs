using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Menu, Playing, GameOver }

    [Header("(debug) Current state")]
    public GameState state = GameState.Menu;

    [Header("UI Refs")]
    public GameObject pressSpaceUI;
    public GameObject gameUI;
    public GameObject gameOverUI;

    [Header("Player refs")]
    public GameObject hero;


    [Header("Enemies refs")]
    public float enemySpeed = 8f;
    public float speedIncreaseRate = .1f;
    public float baseEnemySpeed = 8f;

    [Header("Score")]
    public float score;
    public float bestScore;
    private const string BEST_KEY = "BEST_SCORE_SECONDS";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        bestScore = PlayerPrefs.GetFloat(BEST_KEY, 0f);
    }

    private void Start()
    {
        // menu on launch
        SetState(GameState.Menu);
    }

    private void Update()
    {
        if (CoreInput.Instance == null) return;

        switch (state)
        {
            case GameState.Menu:
                if (CoreInput.Instance.ConsumeStart())
                {
                    StartGame();
                }
                break;

            case GameState.Playing:
                score += Time.deltaTime;
                enemySpeed += speedIncreaseRate * Time.deltaTime;
                break;

            case GameState.GameOver:
                if (CoreInput.Instance.ConsumeRestart()) RestartToMenu();
                break;
        }
    }

    public void StartGame()
    {
        // reset enemy speed for new run
        enemySpeed = baseEnemySpeed;

        if (hero != null)
            hero.GetComponent<PlayerMovement>().ResetForNewRun();
        SetState(GameState.Playing);
    }

    public void GameOver()
    {
        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetFloat(BEST_KEY, bestScore);
            PlayerPrefs.Save();
        }

        SetState(GameState.GameOver);
    }
    public void RestartToMenu()
    {
        SetState(GameState.Menu);
        ResetScore();
        ResetSpawner();
        ClearAllObstacles();
        ClearHeroPosition();
        ClearFloorPosition();
    }

    private void ResetSpawner()
    {
        EnemySpawner spawner = GameObject.FindWithTag("Spawner").GetComponent<EnemySpawner>();
        if (spawner == null) return;
        spawner.ResetSpawnInterval();
    }

    private void ResetScore()
    {
        score = 0f;
    }

    private void ClearAllObstacles()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (var e in enemies)
        {
            if (e != null)
            {
                Destroy(e);
            }
        }
    }

    private void ClearHeroPosition()
    {
        if (!hero) return;
        hero.transform.position = hero.GetComponent<PlayerMovement>().initialPosision;
        var rb = hero.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = Vector3.zero;
    }

    private void ClearFloorPosition()
    {
        FloorFollower floorFollower = GameObject.FindWithTag("Floor").GetComponent<FloorFollower>();
        if (floorFollower == null) return;
        floorFollower.ResetFloor();

    }

    private void SetState(GameState newState)
    {
        state = newState;

        // UI
        if (pressSpaceUI) pressSpaceUI.SetActive(state == GameState.Menu);
        if (gameUI) gameUI.SetActive(state == GameState.Playing);
        if (gameOverUI) gameOverUI.SetActive(state == GameState.GameOver);

        // playerMovements
        if (hero != null)
        {
            var move = hero.GetComponent<PlayerMovement>();
            if (move != null)
            {
                move.enableMovement = (state == GameState.Playing);
            }
        }
    }

}
