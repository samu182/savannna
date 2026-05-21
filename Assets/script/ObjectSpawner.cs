using UnityEngine;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
    [Header("障害物のプレハブをリストに入れてください")]
    public List<GameObject> obstaclePrefabs; 
    
    [Header("出現間隔の設定")]
    // 0.4f〜だとかなり激しくなるので、遊びながら調整してくださいね！
    public float minSpawnTime = 0.4f;
    public float maxSpawnTime = 4.5f;

    private float timer;
    private float nextSpawnTime;

    void Start()
    {
        SetNextSpawnTime();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= nextSpawnTime)
        {
            SpawnObstacle();
            timer = 0;
            SetNextSpawnTime();
        }
    }

    void SpawnObstacle()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Count == 0) return;

        int index = Random.Range(0, obstaclePrefabs.Count);
        GameObject selectedPrefab = obstaclePrefabs[index];

        // --- ★ここが修正ポイント！ ---
        // プレハブが持っている元々の位置（ズレ）を取得
        Vector3 prefabOffset = selectedPrefab.transform.position;
        // スポナーの座標 ＋ プレハブのズレ を計算
        Vector3 spawnPosition = transform.position + prefabOffset;

        // 計算した位置（spawnPosition）で生成
        GameObject obstacle = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        
        // --- サイズの調整（ここは維持） ---
        string prefabName = selectedPrefab.name;

        if (prefabName.Contains("水たまり"))
        {
            obstacle.transform.localScale = new Vector3(Random.Range(0.3f, 0.5f), 0.3f, 1f);
        }
        else if (prefabName.Contains("岩"))
        {
            float s = Random.Range(0.4f, 0.6f);
            obstacle.transform.localScale = new Vector3(s, s, 1f);
        }
        else if (prefabName.Contains("草"))
        {
            float s = Random.Range(0.3f, 0.4f);
            obstacle.transform.localScale = new Vector3(s, s, 1f);
        }
        else
        {
            float s = Random.Range(0.4f, 0.5f);
            obstacle.transform.localScale = new Vector3(s, s, 1f);
        }
    }

    void SetNextSpawnTime()
    {
        nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
    }
}