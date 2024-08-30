using PlayFab.AdminModels;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_OwnedCard : MonoBehaviour
{
    public string cardName;
    public string cardCategory;
    public Sprite cardFront;
    public Sprite cardBack;
    public Image cardLowerBackground;

    // stats values
    public TMP_Text strengthText;
    public TMP_Text speedText;
    public TMP_Text fightText;
    public TMP_Text intelligenceText;
    public TMP_Text staminaText;
    public TMP_Text strangeText;

    // bar (progress)
    public Image strengthBar;
    public Image speedBar;
    public Image fightBar;
    public Image intelligenceBar;
    public Image staminaBar;
    public Image strangeBar;


    public void SetCard(string cardName,
    string cardCategory,
    Sprite cardFront,
    float strength,
    float speed,
    float intelligence,
    float stamina,
    float strange,
    float fight,
    Sprite lowerBackground
     )
    {
        this.cardName = cardName;
        this.cardCategory = cardCategory;
        this.cardFront = cardFront;

        this.strengthText.text = strength.ToString();
        this.speedText.text = speed.ToString();
        this.intelligenceText.text = intelligence.ToString();
        this.fightText.text = fight.ToString();
        this.staminaText.text = stamina.ToString();
        this.strangeText.text = strange.ToString();

        // Update the sprite of the image
        this.GetComponent<Image>().sprite = this.cardFront;

        // update the lower part as well
        this.cardLowerBackground.sprite = lowerBackground;

        // Set the scale of the bars based on the float values
        SetBarScale(strengthBar, strength);
        SetBarScale(speedBar, speed);
        SetBarScale(intelligenceBar, intelligence);
        SetBarScale(staminaBar, stamina);
        SetBarScale(strangeBar, strange);
        SetBarScale(fightBar, fight);
    }

    private void SetBarScale(Image bar, float value)
    {
        // Assuming the maximum value of the scale is 1, and it scales linearly with the float value
        // Adjust the scale factor as per your requirement
        float scaleX = value / 100f; // This divides the value by 10 to get a scale factor; adjust if necessary
        bar.transform.localScale = new Vector3(scaleX, bar.transform.localScale.y, bar.transform.localScale.z);
    }
}
