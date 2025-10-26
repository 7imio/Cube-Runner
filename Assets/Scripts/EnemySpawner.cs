using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Instance Prefab")]
    public GameObject enemyPrefab;

    [Header("Spawn zone")]
    public float spawnZStart = 50f;
    public float spawnXRange = 4f;
    public float spawnY = .5f;

    [Header("Timing")]
    public float spawnInterval = 1.5f;
    private float _timer;

    [Header("Cleaning")]
    public float despawnZ = -5f;

    public Transform hero;

    private void Start()
    {
        // retrieving hero object
        if (!hero)
        {
            hero = GameObject.FindWithTag("Player")?.transform;
        }
    }
    
    private void Update()
    {
        // if not playing, not spawning
        if (GameManager.Instance == null || GameManager.Instance.state != GameManager.GameState.Playing) return;

        _timer += Time.deltaTime;
        if (_timer >= spawnInterval)
        {
            _timer = 0f;
            SpawnEnemy();
        }
        // basic cleanup
        CleanupEnemies();

    }

    private void SpawnEnemy()
    {
        if (!enemyPrefab || !hero) return;

        float randomX = Random.Range(-spawnXRange, spawnXRange);
        float posZ = (hero.position.z + spawnZStart);

        Vector3 spawnPosition = new Vector3(randomX, spawnY, posZ);

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    private void CleanupEnemies()
    {

        if (!hero) return;
        if (GameManager.Instance == null) return;

        float despawnPlane = hero.transform.position.z + despawnZ;

        var enemies = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (var e in enemies)
        {
            if (e.transform.position.z < despawnPlane) Destroy(e);
        }
    }


}
