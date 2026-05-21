using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameOverManager : MonoBehaviour
{
    [Header("表示するテキストUI")]
    public TextMeshProUGUI gameOverText;

    [Header("ゲームオーバーメッセージ（30種類入力してください）")]
    [TextArea(1, 3)]
    public List<string> deathMessages = new List<string>();

    // ゲームオーバー時に他のスクリプトからこれを呼ぶ
    public void SetupGameOver()
    {
        if (deathMessages.Count > 0 && gameOverText != null)
        {
            // 30種類の中からランダムに1つ選ぶ
            int randomIndex = Random.Range(0, deathMessages.Count);
            
            // UIに表示する
            gameOverText.text = deathMessages[randomIndex];
        }
        else
        {
            Debug.LogWarning("メッセージが設定されていないか、TextUIが空です。");
        }
    }
}