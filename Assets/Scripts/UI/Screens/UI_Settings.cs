using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Settings : UI_Panel
{
    [Header("UI Elements")]
    public Slider soundSlider;
    public Slider musicSlider;
    public Toggle soundToggle;
    public Toggle musicToggle;

    [Header("Audio Sources")]
    public AudioSource soundAudioSource;
    public AudioSource musicAudioSource;

    public Button backBtn;
    public Button audioBtn;
    public Button notificationBtn;

    public GameObject audioPanel;
    public GameObject notificationPanel;
    

    private void Start()
    {
        soundAudioSource = SoundManager.Instance.buttonSource;
        musicAudioSource = SoundManager.Instance.musicSource;
        // Load saved settings or set default values
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1.0f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        //soundToggle.isOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
        //musicToggle.isOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;

        Init();
    }
    public void Init()
    {
        ApplySettings();
        OnClick_AudioBtn();
    }
    private void OnEnable()
    {
        backBtn.onClick.AddListener(() => Back());
        audioBtn.onClick.AddListener(OnClick_AudioBtn);
        notificationBtn.onClick.AddListener(OnClick_NoficationBtn);

    }
    public void OnSoundSliderChanged()
    {
        soundAudioSource.volume = soundSlider.value;
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
    }

    public void OnMusicSliderChanged()
    {
        musicAudioSource.volume = musicSlider.value;
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
    }

    public void OnSoundToggleChanged()
    {
        soundAudioSource.mute = !soundToggle.isOn;
        PlayerPrefs.SetInt("SoundOn", soundToggle.isOn ? 1 : 0);
    }

    public void OnMusicToggleChanged()
    {
        musicAudioSource.mute = !musicToggle.isOn;
        PlayerPrefs.SetInt("MusicOn", musicToggle.isOn ? 1 : 0);
    }

    private void ApplySettings()
    {
        soundAudioSource.volume = soundSlider.value;
        //musicAudioSource.volume = musicSlider.value;

        //soundAudioSource.mute = !soundToggle.isOn;
        //musicAudioSource.mute = !musicToggle.isOn;
    }

    private void OnDisable()
    {
        // Save settings when the settings menu is closed or the game is exited
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        //PlayerPrefs.SetInt("SoundOn", soundToggle.isOn ? 1 : 0);
        //PlayerPrefs.SetInt("MusicOn", musicToggle.isOn ? 1 : 0);
    }
    
    public void Back()
    {
        SoundManager.Instance.PlayButtonSound(0);
        ApplySettings();
        UI_Manager.Instance.CloseLastOpenedPanel();
    }
    public void OnClick_AudioBtn()
    {
        SoundManager.Instance.PlayButtonSound(0);
        EventSystem.current.SetSelectedGameObject(audioBtn.gameObject);
        audioPanel.SetActive(true);
        notificationPanel.SetActive(false);
    }
    public void OnClick_NoficationBtn()
    {
        SoundManager.Instance.PlayButtonSound(0);
        audioPanel.SetActive(false);
        notificationPanel.SetActive(true);
    }
}
