using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    public float stepDistance = 1.0f;

    // ★【これが抜けていました！】移動にかける時間（秒）をここで合図しておきます
    public float moveDuration = 0.2f; 

    public void MoveCloser()
    {
        float targetX = transform.position.x - stepDistance;
        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);

        // 上で宣言した「moveDuration」がこれで正しく使えるようになります！
        LeanTween.move(gameObject, targetPosition, moveDuration)
            .setEaseOutQuad();

        Debug.Log("敵が迫ってきた！");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("捕まった！ゲームオーバー！");
            
            // ぶつかった相手（Player）から PlayerController スクリプトを探す
            PlayerController player = collision.GetComponent<PlayerController>();
            
            if (player != null)
            {
                // プレイヤーの ShowGameOver を実行する！
                player.ShowGameOver();
            }
            else
            {
                // もしプレイヤー側にスクリプトがなければ、とりあえずリスタート
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}