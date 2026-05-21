using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public GameObject gameOverPanel;
    
    [Header("オーディオ設定")]
    public AudioClip jumpSound;      // ジャンプ音
    public AudioClip gameOverSound;  // ゲームオーバー音
    public AudioClip buttonClickSound; // ★ボタンを押した時の音
    private AudioSource audioSource;

    [Header("ジャンプ設定")]
    public float jumpForce = 12f;

    [Header("接地判定")]
    public Transform groundCheck;
    public float checkRadius = 0.3f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isInWater = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // 転倒防止
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // 自分のオブジェクトについているAudioSourceを準備
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // 地面についているかチェック
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        
        // スペースキーでジャンプ
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isInWater)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            // ジャンプ音を鳴らす
            if (audioSource != null && jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }
    }

    public bool IsInWater() => isInWater;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Water")) isInWater = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Water")) isInWater = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 水（障害物）に当たったらゲームオーバー
        if (collision.gameObject.CompareTag("Water"))
        {
            ShowGameOver();
        }
    }

public void ShowGameOver()
{
    // ★【追加】画面にいるTimerManagerを見つけて、タイマーをストップさせる！
    FindObjectOfType<TimerManager>().StopTimer();

    // ★【ここが超重要！】捕まったその瞬間に、即座にゲームの時間をピタッと止める！
    Time.timeScale = 0f; 

    // 時間を止めた状態で、裏で「少し待ってから画面を出す処理」を起動する
    StartCoroutine(WaitAndShowGameOverPanel());
}

private IEnumerator WaitAndShowGameOverPanel()
{
    // ★【注意】Time.timeScale = 0f でゲームの時間が止まっているため、
    // 普通の「WaitForSeconds」だと無限に待つことになってしまいます。
    // そのため、現実世界のリアルな時間を計る「WaitForSecondsRealtime」に書き換えます！
    yield return new WaitForSecondsRealtime(0.5f);

    // ゲームオーバー音を鳴らす
    if (audioSource != null && gameOverSound != null)
    {
        audioSource.PlayOneShot(gameOverSound);
    }

    if (gameOverPanel != null)
    {
        // まずゲームオーバーイラストをアクティブ（表示）にする
        gameOverPanel.SetActive(true);

        // 大きさは1のまま固定
        gameOverPanel.transform.localScale = Vector3.one;

        // RectTransformを取得して位置をアニメーション
        RectTransform rect = gameOverPanel.GetComponent<RectTransform>();
        if (rect != null)
        {
            // 画面のすぐ上（Y座標: 1000）に配置
            rect.anchoredPosition = new Vector2(0f, 1200f);

            // 0.7秒かけて、上から中央にガッシャーンと落とす！
            LeanTween.move(rect, Vector2.zero, 0.7f)
                .setEaseOutBounce()
                .setIgnoreTimeScale(true); // 時間が止まっていても動く魔法の命令
        }
    }
    
    // メッセージやタイムのセットアップ（文字の流し込みなど）
    FindObjectOfType<GameOverManager>().SetupGameOver();
}
    public void BackToTitle()
    {
        // 1. まず時間を動かす
        Time.timeScale = 1f; 

        // 2. ボタンの音（buttonClickSound）を鳴らす
        if (audioSource != null && buttonClickSound != null) 
        {
            audioSource.PlayOneShot(buttonClickSound);
        }

        // 3. 少し待ってからシーンを切り替える（WaitAndBackを呼ぶ）
        StartCoroutine(WaitAndBack());
    }

    IEnumerator WaitAndBack()
    {
        // 0.2秒待つ（音を聞かせるため）
        yield return new WaitForSecondsRealtime(0.2f);
        
        SceneManager.LoadScene("StartMenu");
    }
    // --- 📄 ここから新しく一番下に追加するコード ---

    // リトライボタンが押された時に実行するメソッド
    public void RetryGame()
    {
        // 1. まず時間を通常（1）に戻す
        Time.timeScale = 1f; 

        // 2. ボタンのクリック音を鳴らす
        if (audioSource != null && buttonClickSound != null) 
        {
            audioSource.PlayOneShot(buttonClickSound);
        }

        // 3. 音を響かせるために、少し待ってから再起動するコルーチンを開始
        StartCoroutine(WaitAndRetry());
    }

    // 音を聞かせるために一瞬だけ待ってからリトライする処理
    private IEnumerator WaitAndRetry()
    {
        // 0.2秒待つ（タイトルに戻る処理と同じタメを作る）
        yield return new WaitForSecondsRealtime(0.2f);
        
        // いま遊んでいるシーンの名前を取得して、最初から読み直す！
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}