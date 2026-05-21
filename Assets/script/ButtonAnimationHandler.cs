using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // 👈 カーソル検知にこれが必要です！

// このスクリプトを使うには、同じオブジェクトに必ずImageコンポーネントが必要、という命令
[RequireComponent(typeof(Image))]
public class ButtonAnimationHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;

    [Header("大きさのアニメーション設定")]
    [SerializeField] private float highlightScale = 1.1f; // ホバー時の大きさ（1.1倍）
    [SerializeField] private float scaleTime = 0.1f; // 大きさがフワッと変わる時間

    void Awake()
    {
        // ゲーム開始時の大きさを覚えておく
        originalScale = transform.localScale;
    }

    // 👈 マウスカーソルがイラストの上に乗った瞬間に呼ばれる関数
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 大きさをフワッとhighlightScale（1.1倍）に変える
        LeanTween.scale(gameObject, originalScale * highlightScale, scaleTime)
            .setEaseOutQuad()
            .setIgnoreTimeScale(true); // ポーズ中（Time.timeScaleが0）でも動く魔法の命令
    }

    // 👈 マウスカーソルがイラストの上から外れた瞬間に呼ばれる関数
    public void OnPointerExit(PointerEventData eventData)
    {
        // 大きさを元の大きさにフワッと戻す
        LeanTween.scale(gameObject, originalScale, scaleTime)
            .setEaseOutQuad()
            .setIgnoreTimeScale(true);
    }
}