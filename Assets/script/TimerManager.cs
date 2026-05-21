using UnityEngine;
using TMPro; // 👈 TextMeshProを使うためにこれが必要です！

public class TimerManager : MonoBehaviour
{
    [Header("UI設定")]
    [SerializeField] private TextMeshProUGUI timerText; // 👈 画面のTimerTextを入れる枠

    private float elapsedTime = 0f; // 生き残った時間を入れる箱
    private bool isGamePlaying = true; // ゲーム中かどうかのフラグ

    void Start()
    {
        elapsedTime = 0f;
        isGamePlaying = true;
    }

    void Update()
    {
        // ゲームプレイ中（時間が動いているとき）だけ、タイマーを進める
        if (isGamePlaying && Time.timeScale > 0f)
        {
            // 前のフレームからの経過時間をプラスしていく
            elapsedTime += Time.deltaTime;

            // 画面の文字を書き換える（例: "12.34" のように小数第2位まで表示）
            if (timerText != null)
            {
                timerText.text = elapsedTime.ToString("F2");
            }
        }
    }

    // 👈 ゲームオーバー時にタイマーを止めて、最終時間を取得するための関数
    public float StopTimer()
    {
        isGamePlaying = false;
        return elapsedTime;
    }
}