using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_LoadingForWait : UI_Panel
{
    public TextMeshProUGUI loadingText;          // Reference to your loading text
    public float dotSpeed = 0.5f;     // Speed at which dots change
    public bool isLoading = false;
    private void Start()
    {
        ShowLoadingScreen();
    }
    public void ShowLoadingScreen()
    {
        isLoading = true;
        StartCoroutine(AnimateLoadingText());
        //StartCoroutine(DisableLoading(4));
    }
    private IEnumerator AnimateLoadingText()
    {
        string baseText = "Loading";
        int dotCount = 0;

        while (isLoading)
        {
            loadingText.text = baseText + new string('.', dotCount);
            dotCount = (dotCount + 1) % 4; // Cycle through 0, 1, 2, 3 dots
            yield return new WaitForSeconds(dotSpeed);
        }
    }
    private IEnumerator DisableLoading(float t)
    {
        yield return new WaitForSeconds(t);

    }
}
