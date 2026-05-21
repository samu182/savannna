using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [Header("オーディオ設定")]
    public AudioSource buttonAudioSource;

    [Header("UI設定")]
    public GameObject settingsPanel;
    public Slider volumeSlider;

    void Awake()
    {
        // シーンごとに新しく作られる形にリセット
        Instance = this;
    }

    void Start()
    {
        // 音量スライダーが設定されている場合のみイベントを登録
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            volumeSlider.value = AudioListener.volume;
        }
    }

    // --------------------------------------------------
    // ボタンから呼び出す関数（ここが消えていた可能性があります）
    // --------------------------------------------------

    // スタートボタン用
    public void StartGame()
    {
        if (buttonAudioSource != null) buttonAudioSource.Play();
        StartCoroutine(WaitAndStart());
    }

    IEnumerator WaitAndStart()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        SceneManager.LoadScene("savanna");
    }

    // オプションボタン用
    public void OpenSettings()
    {
        if (buttonAudioSource != null) buttonAudioSource.Play(); // 音を鳴らす
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true); // パネルを表示する
        }
        else
        {
            Debug.LogError("【エラー】settingsPanel がインスペクターで設定されていません！");
        }
    }

    // 設定画面の閉じる（×）ボタン用
    public void CloseSettings()
    {
        if (buttonAudioSource != null) buttonAudioSource.Play(); // 音を鳴らす
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false); // パネルを非表示にする
        }
    }

// イグジットボタンを押したときに呼ばれる関数
    public void QuitGame()
    {
        if (buttonAudioSource != null) buttonAudioSource.Play(); // 音を鳴らす
        
        // 音が鳴り終わるまで待ってから終了するコルーチンを実行！
        StartCoroutine(WaitAndQuit()); 
    }

    // 0.2秒待ってからゲームを終了するお留守番コード
    IEnumerator WaitAndQuit()
    {
        // 0.2秒だけ待つ（クリック音が鳴り終わる時間）
        yield return new WaitForSecondsRealtime(0.2f);

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // エディタでの再生停止
        #else
        Application.Quit(); // 実際のゲーム終了
        #endif
    }

    // スライダー用
    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
    }
}