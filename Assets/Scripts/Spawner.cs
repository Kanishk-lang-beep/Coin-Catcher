using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public GameObject bombPrefab;
    public float spawnInterval = 1f;
    public float minInterval = 0.35f;
    [Range(0f, 1f)] public float bombChance = 0.25f;

    float timer;
    Camera cam;

    void Start() { cam = Camera.main; }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Spawn();
            timer = spawnInterval;
            spawnInterval = Mathf.Max(minInterval, spawnInterval - 0.02f);
        }
    }

    void Spawn()
    {
        float halfW = cam.orthographicSize * cam.aspect - 0.5f;
        float x = Random.Range(-halfW, halfW);
        float y = cam.orthographicSize + 1f;
        var prefab = Random.value < bombChance ? bombPrefab : coinPrefab;
        Instantiate(prefab, new Vector3(x, y, 0f), Quaternion.identity);
    }
}