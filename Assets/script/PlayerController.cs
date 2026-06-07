using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using unityroom.Api; // ★UnityroomApiClientを使うために必要

public class PlayerController : MonoBehaviour
{
    public GameObject gameOverPanel;
    
    [Header("オーディオ設定")]
    public AudioClip gaoSound;       // ★チーターの「ガオー！」の音
    public AudioClip jumpSound;      // ジャンプ音
    public AudioClip gameOverSound;  // ゲームオーバー音
    public AudioClip buttonClickSound; // ★ボタンを押した時の音
    private AudioSource audioSource;

    [Header("ジャンプ設定")]
    public float jumpForce = 12f;
    [SerializeField] private float jumpCooldown = 0.1f; // 連打・同時押し防止のクールタイム
    private float nextJumpTime = 0f;                    // 次にジャンプできる時間

    [Header("接地判定")]
    public Transform groundCheck;
    public float checkRadius = 0.3f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isInWater = false;

    [Header("クリア判定")]
    public float clearTimeThreshold = 100f; // 👈 インスペクターで変更できるようになります

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        
        if (isGrounded && !isInWater && Time.time >= nextJumpTime)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                TriggerJump();
                nextJumpTime = Time.time + jumpCooldown; 
            }
        }
    }

    private void TriggerJump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        if (audioSource != null && jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound);
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
        if (collision.gameObject.CompareTag("Water"))
        {
            ShowGameOver();
        }
    }

    public void ShowGameOver()
    {
        Debug.Log("[デバッグ] --- ShowGameOver が呼び出されました ---");

        if (audioSource != null && gaoSound != null)
        {
            audioSource.PlayOneShot(gaoSound);
        }

        // ① 【最優先】まずは本当の時間を TimerManager から取得する！
        float finalTime = 0f;
        TimerManager timer = FindAnyObjectByType<TimerManager>();
        
        if (timer != null)
        {
            finalTime = timer.StopTimer(); 
            Debug.Log($"[デバッグ] TimerManagerから取得した値: {finalTime}");
        }
        else
        {
            Debug.LogError("[デバッグ] エラー: TimerManager がシーン内に見つかりません！");
        }

        // ② 取得した時間（スコア）を Unityroom に送信する
        if (finalTime > 0f)
        {
            Debug.Log($"[デバッグ] Unityroomへスコア送信を開始します。送信値: {finalTime}");
            UnityroomApiClient.Instance.SendScore(1, finalTime, ScoreboardWriteMode.HighScoreDesc);
            Debug.Log($"[unityroom公式] スコア {finalTime} を送信完了メッセージを送りました。");
        }

        // ③ ★★★ ここでやっと判定！取得した本当の時間でクリアかチェックする ★★★
        if (finalTime >= clearTimeThreshold)
        {
            Debug.Log($"[クリア判定] 生存時間 {finalTime}秒 が目標を達成！クリア演出を開始します！");
            
            GameClearManager clearManager = FindAnyObjectByType<GameClearManager>();
            if (clearManager != null)
            {
                clearManager.StartClearSequence(this.gameObject);
            }
            else
            {
                Debug.LogError("[エラー] GameClearManager がシーン内に見つかりません！ヒエラルキーに作成されていますか？");
            }
            return; // 👈 10秒（または100秒）以上なら、ここで強制終了してゲームオーバー画面を防ぐ！
        }

        // ④ クリア条件を満たしていなかった時だけ、下のゲームオーバー処理が実行される
        Time.timeScale = 0f; 
        StartCoroutine(WaitAndShowGameOverPanel());
    }

    private IEnumerator WaitAndShowGameOverPanel()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        if (audioSource != null && gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            gameOverPanel.transform.localScale = Vector3.one;

            RectTransform rect = gameOverPanel.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = new Vector2(0f, 1200f);
                LeanTween.move(rect, Vector2.zero, 0.7f)
                    .setEaseOutBounce()
                    .setIgnoreTimeScale(true);
            }
        }
        
        FindAnyObjectByType<GameOverManager>().SetupGameOver();
    }

    public void BackToTitle()
    {
        Time.timeScale = 1f; 
        if (audioSource != null && buttonClickSound != null) 
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
        StartCoroutine(WaitAndBack());
    }

    IEnumerator WaitAndBack()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        SceneManager.LoadScene("StartMenu");
    }

    public void RetryGame()
    {
        Time.timeScale = 1f; 
        if (audioSource != null && buttonClickSound != null) 
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
        StartCoroutine(WaitAndRetry());
    }

    private IEnumerator WaitAndRetry()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}