using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class OpenBuyButtonPage : MonoBehaviour
{
    // URL of your GitHub Pages site where the Shopify Buy Button is hosted
    private string buyButtonUrl = "https://alihaidermta.github.io/duperverse_buttons/";
    private string googleSheetUrl = "https://docs.google.com/spreadsheets/d/e/2PACX-1vSa4yoI-suuYbYhVbPwvYcEtdUlXFfoBIHq9JSJOI-XEcDI3IBx-rht4F-OkRB2Ru0cKtbvQIoYDm1g/pub?output=csv"; // Replace with your Google Sheet URL returning JSON
    private string orderStatusComplete = "Order Complete";
    private float checkInterval = 5f;
    private float timeout = 120f;
    public TMP_Text orderStatusText; // Reference to your TMP Text component


    public static OpenBuyButtonPage Instance;
    private void Awake()
    {
        if (Instance != this && Instance != null) Destroy(this);
        else Instance = this;
    }

    void Start()
    {
        //orderStatusText = GetComponent<TMP_Text>(); // Ensure TMP Text component is attached to the same GameObject
    }

    /// <summary>
    /// Opens the WebRequest on any browser.
    /// </summary>
    /// <param name="productId">Send a unique ID of your card</param>
    /// <param name="cardBuyURL">Send the URL related to the card as well</param>
    public void OpenBuyButton(string productId, string cardBuyURL)
    {
        Debug.Log($"Checkout called");
        PlayerPrefs.SetString("OrderCheckout", productId);
        PlayerPrefs.Save();
        Debug.Log($"Card URL sent is {cardBuyURL}");
        Application.OpenURL(cardBuyURL);
        StartCoroutine(CheckOrderStatus(productId, cardBuyURL));
    }

    private IEnumerator CheckOrderStatus(string productId, string cardBuyURL)
    {
        Debug.Log($"Checkout coroutine called");
       // orderStatusText.text = "Waiting for Order Completion";
        float elapsedTime = 0f;

        while (elapsedTime < timeout)
        {
            UnityWebRequest request = UnityWebRequest.Get(googleSheetUrl);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResult = request.downloadHandler.text;
                if (jsonResult.Contains(productId))
                {
                    //orderStatusText.text = orderStatusComplete;
                    yield break;
                }
            }
            else
            {
                Debug.LogError("Error fetching Google Sheet data: " + request.error);
            }

            elapsedTime += checkInterval;
            yield return new WaitForSeconds(checkInterval);
        }

        // Timeout handling
        //orderStatusText.text = "Order Timeout";
        Debug.LogError("Order check timed out.");
    }
}
