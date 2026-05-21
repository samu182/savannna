using UnityEngine;

public class RightSlideLooper : MonoBehaviour
{
    [Header("移動速度（大きいほど速い）")]
    public float scrollSpeed = 2.0f;

    [Header("ループ設定")]
    public float startX = -15f; // 左端（ここから現れる）
    public float endX = 15f;   // 右端（ここまで行ったら戻る）

    // ★ プレイヤーの情報を入れる変数
    private PlayerController player;

    void Start()
    {
        // ★ 画面内からプレイヤーを自動で見つけておく
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.GetComponent<PlayerController>();
        }
    }

    void Update()
    {
        // ★ プレイヤーが水の中にいたら、小石や草の速度も30%（0.3倍）にする
        // 水の中にいない時は、そのままの速度（scrollSpeed）で動く
        float currentSpeed = (player != null && player.IsInWater()) ? scrollSpeed * 0.3f : scrollSpeed;

        // 1. 右方向へ移動 (scrollSpeed から currentSpeed に変更)
        transform.Translate(Vector2.right * currentSpeed * Time.deltaTime);

        // 2. 指定した右端（endX）を超えたら、左端（startX）へワープ
        if (transform.position.x >= endX)
        {
            Vector3 pos = transform.position;
            pos.x = startX;
            transform.position = pos;
        }
    }
}