using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Loading : UI_Panel
{

    public Slider loadingBarSlider;
    public float loadingSpeed = 0.5f;
    private float targetProgress = 0f;
    public bool loading = false;

    private void OnEnable()
    {

    }
    void Start()
    {
        SetProgress(1f);
        loadingBarSlider.value = 0;
    }


    void Update()
    {

        if (loadingBarSlider.value < targetProgress)
        {
            loadingBarSlider.value += loadingSpeed * Time.deltaTime;
            //Debug.LogError("gg");
        }
        else
        {
            loading = false;
            UI_Manager.Instance.OpenPanel(typeof(UI_Login));
        }
    }

    public void SetProgress(float progress)
    {
        targetProgress = progress;
    }

}
