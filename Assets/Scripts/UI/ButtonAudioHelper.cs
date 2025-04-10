using FinishOne.GeneralUtilities.Audio;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hooks up a Unity Button or Toggle to play a sound clip when it is clicked
/// </summary>
public class ButtonAudioHelper : MonoBehaviour
{
    [SerializeField] private AudioConfigSO audioConfig;

    private AudioPlayRequester audioPlayRequester;

    public AudioConfigSO AudioConfig => audioConfig;
    public bool Muted { get; set; }

    private void Awake()
    {
        audioPlayRequester = GetComponent<AudioPlayRequester>();

        if (TryGetComponent(out Button button))
        {
            button.onClick.AddListener(Click);
        }
        else if (TryGetComponent(out Toggle toggle))
        {
            toggle.onValueChanged.AddListener((val) => Click());
        }
        else
        {
            Debug.LogError($"{nameof(ButtonAudioHelper)} is not attached to a Button or Toggle component.", gameObject);
        }
    }
    
    public void SetAudio(AudioConfigSO newAudio)
    {
        audioConfig = newAudio;
    }

    private void Click()
    {
        if (!Muted)
        {
            audioPlayRequester.Request(audioConfig);
        }
    }
}