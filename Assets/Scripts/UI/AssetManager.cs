using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class AssetManager : MonoBehaviour
{
    public static AssetManager Instance;
    public TMP_FontAsset fontAsset;
    public Font font;
    public int sizeDecrement=2;

    private void Awake()
    {
        if (Instance != this && Instance != null) Destroy(this);
        else Instance = this;
    }


    public void SetTexts()
    {
        TMP_Text[] allTMP = GameObject.FindObjectsOfType<TMP_Text>(includeInactive: true);

        foreach (var text in allTMP)
        {
            text.font = AssetManager.Instance.fontAsset;
        }

        Text[] alltexts = GameObject.FindObjectsOfType<Text>(includeInactive: true);

        foreach (var text in alltexts)
        {
            text.font = AssetManager.Instance.font;
            text.enabled = false;
            text.enabled = true;

            text.fontSize = text.fontSize - sizeDecrement;  
        }
    }


}
