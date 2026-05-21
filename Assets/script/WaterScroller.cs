using UnityEngine;

public class WaterScroller : MonoBehaviour
{
    public float moveSpeed = 5f;
    private PlayerController player;
    private EnemyController enemy;
    
    // すでに敵を動かしたかどうかを覚えるフラグ
    private bool hasTriggered = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) player = playerObj.GetComponent<PlayerController>();

        GameObject enemyObj = GameObject.Find("Enemy");
        if (enemyObj != null) enemy = enemyObj.GetComponent<EnemyController>();
    }

    void Update()
    {
        float currentSpeed = (player != null && player.IsInWater()) ? moveSpeed * 0.3f : moveSpeed;
        transform.Translate(Vector3.right * currentSpeed * Time.deltaTime);

        if (transform.position.x > 15f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // まだこの水たまりで敵を動かしていない場合だけ実行
            if (enemy != null && !hasTriggered)
            {
                enemy.MoveCloser();
                hasTriggered = true; // 「もう動かしたよ」と記録する
            }

            // Destroy(gameObject); // ←ここを消す（あるいは先頭に // を書いて無効化する）
        }
    }
}