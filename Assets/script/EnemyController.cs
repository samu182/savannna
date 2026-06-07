using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    public float stepDistance = 1.0f;
    public float moveDuration = 0.2f; 

    [Header("1回ごとに上げる音量の大きさ（0.25なら4回で最大）")]
    public float volumeStep = 0.25f;  

    private AudioSource audioSource; // 👈 手動アタッチはやめて、コードで自動取得にします

    void Start()
    {
        // 💡 自分自身にくっついている AudioSource を確実に自動でつかまえるコマンド
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            // もし追加し忘れていたら赤文字で怒るようにします
            Debug.LogError($"[敵エラー] {gameObject.name} に 'Audio Source' コンポーネントが追加されていません！インスペクターの Add Component から追加してください。");
        }
        else
        {
            audioSource.loop = true;        // ループ再生
            audioSource.volume = 0f;        // 最初は音量0
            audioSource.playOnAwake = false;
            audioSource.Play();             // 再生スタート
        }
    }

    public void MoveCloser()
    {
        // 1. 敵を左に動かす
        float targetX = transform.position.x - stepDistance;
        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);

        LeanTween.move(gameObject, targetPosition, moveDuration)
            .setEaseOutQuad();

        // 2. 🎯 関数が呼び出されるたびに音量を直接プラスする
        if (audioSource != null)
        {
            audioSource.volume += volumeStep;

            // 1.0（最大値）を超えないように固定
            if (audioSource.volume > 1.0f)
            {
                audioSource.volume = 1.0f;
            }
        }

        // 💡 コンソールのログに、今の「実際の数値」をハッキリ表示させます
        Debug.Log($"[敵デバッグ] {gameObject.name} の MoveCloser が呼ばれました。現在の内部音量: {audioSource.volume}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ShowGameOver();
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}