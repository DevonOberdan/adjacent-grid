using FinishOne.GeneralUtilities.Audio;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hooks up a Unity Button or Toggle to play a sound clip when it is clicked
/// </summary>
public class ButtonAudioHelper : MonoBehaviour
{
    [SerializeField] private AudioConfigSO audioConfig;

    private Button button;
    private Toggle toggle;
    private AudioPlayRequester audioPlayRequester;

    public AudioConfigSO AudioConfig => audioConfig;
    public bool Muted { get; set; }

    private void Awake()
    {
        button = GetComponent<Button>();
        toggle = GetComponent<Toggle>();

        audioPlayRequester = GetComponent<AudioPlayRequester>();

        if (button != null)
        {
            button.onClick.AddListener(Click);
        }
        else if (toggle != null)
        {
            toggle.onValueChanged.AddListener((val) => Click());
        }
        else
        {
            Debug.LogError($"{nameof(ButtonAudioHelper)} is not attached to a Button of Toggle component.", gameObject);
        }
    }
    
    public void SetAudio(AudioConfigSO newAudio) => audioConfig = newAudio;

    private void Click()
    {
        if (!Muted)
        {
            audioPlayRequester.Request(audioConfig);
        }
    }
}