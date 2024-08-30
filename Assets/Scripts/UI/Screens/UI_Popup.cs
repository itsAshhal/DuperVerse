using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_Popup : UI_Panel
{
    [SerializeField] TextMeshProUGUI _Text;
    public Button okBtn;
    void Start()
    {
        okBtn.onClick.AddListener(OnClick_Ok);
    }
    public void OnClick_Ok()
    {
        SoundManager.Instance.PlayButtonSound(0);
        UI_Manager.Instance.CloseLastOpenedPanel();
    }
    public void SetText(string _txt)
    {
        _Text.text = _txt;
    }
}
