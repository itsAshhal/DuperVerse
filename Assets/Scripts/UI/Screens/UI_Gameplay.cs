using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_Gameplay : UI_Panel
{
    public TextMeshProUGUI dummyText;
    public Button pauseBtn;
    public Button BackBtn;
    void Start()
    {
        pauseBtn.onClick.AddListener(OnClick_Pause);
        BackBtn.onClick.AddListener(OnClick_Back);
    }
    public void OnClick_Pause()
    {

    }
    public void OnClick_Back()
    {
        UI_Manager.Instance.OpenPanel(typeof(UI_MainMenu));
    }
}
