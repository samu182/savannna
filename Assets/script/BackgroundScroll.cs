using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [Header("移動速度（大きいほど速い）")]
    public float scrollSpeed = 2.0f;

    [Header("ループ設定")]
    public float startX = -15f; // 左端（ここから現れる）
    public float endX = 15f;   // 右端（ここまで行ったら戻る）

    void Update()
    {
        // 1. 右方向へ移動
        transform.Translate(Vector2.right * scrollSpeed * Time.deltaTime);

        // 2. 指定した右端（endX）を超えたら、左端（startX）へワープ
        if (transform.position.x >= endX)
        {
            Vector3 pos = transform.position;
            pos.x = startX;
            transform.position = pos;
        }
    }
}