using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio; // 👈 【重要】ミキサーを触るために追加！
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [Header("オーディオ設定")]
    [SerializeField] private AudioMixer mainMixer; // 👈 【新設】作ったMainMixerをここにドラッグする
    [SerializeField] private Slider bgmSlider;     // 👈 【新設】BGMSliderをここにドラッグする
    [SerializeField] private Slider seSlider;      // 👈 【新設】SESliderをここにドラッグする
    public AudioSource buttonAudioSource;

    [Header("UI設定")]
    public GameObject settingsPanel;

    void Awake()
    {
        // シーンごとに新しく作られる形にリセット
        Instance = this;
    }

    void Start()
    {
        // ゲーム起動時に、今のスライダーの値をミキサーに即座に反映させる
        if (bgmSlider != null) SetBGMVolume(bgmSlider.value);
        if (seSlider != null) SetSEVolume(seSlider.value);

        // スライダーを動かしたときに、リアルタイムで音量が変わるように命令を登録する
        if (bgmSlider != null) bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        if (seSlider != null) seSlider.onValueChanged.AddListener(SetSEVolume);
    }

    // 👈 【新設】BGMの音量を変更する関数
    public void SetBGMVolume(float volume)
    {
        if (mainMixer != null)
        {
            // Mixerの「BGMVolume」というパラメーターの値をスライダーの値に変える
            mainMixer.SetFloat("BGMVolume", volume);
        }
    }

    // 👈 【新設】効果音（SE）の音量を変更する関数
    public void SetSEVolume(float volume)
    {
        if (mainMixer != null)
        {
            // Mixerの「SEVolume」というパラメーターの値をスライダーの値に変える
            mainMixer.SetFloat("SEVolume", volume);
        }
    }

    // --------------------------------------------------
    // ボタンから呼び出す関数（ここから下は前のまま残しています！）
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
        yield return new WaitForSecondsRealtime(0.2f);

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // エディタでの再生停止
        #else
        Application.Quit(); // 実際のゲーム終了
        #endif
    }
}