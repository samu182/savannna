using UnityEngine;

public class TitleUIWobbler : MonoBehaviour
{
    [Header("揺れの設定")]
    [SerializeField] private float moveRange = 15f;    // 👈 どれくらい大きく揺らすか（ピクセル単位）
    [SerializeField] private float moveTime = 3.5f;    // 👈 何秒かけて次の位置に動くか（大きいほどゆっくり）

    private Vector3 originalPosition;

    void Start()
    {
        // ゲーム開始時の、この画像の最初の位置を覚えておく
        originalPosition = transform.localPosition;

        // 最初の一歩（ゆらゆら運動のスタート）を呼び出す
        StartWobble();
    }

    void StartWobble()
    {
        // 現在の最初の位置を基準に、ランダムな方向へ少しズラした目標地点を決める
        float randomX = Random.Range(-moveRange, moveRange);
        float randomY = Random.Range(-moveRange, moveRange);
        Vector3 targetPosition = originalPosition + new Vector3(randomX, randomY, 0f);

        // LeanTweenを使って、目標地点までジワ〜ッと滑らかに移動させる
        LeanTween.moveLocal(gameObject, targetPosition, moveTime)
            .setEaseInOutSine() // 👈 これを使うことで、動き出しと止まり際がフワッと滑らかになります
            .setOnComplete(StartWobble); // 👈 移動が終わったら、もう一度この関数を呼び出して次の場所へ向かわせる（無限ループ）
    }
}