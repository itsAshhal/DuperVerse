using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class GoogleSheetsToJson : MonoBehaviour
{
    // URL to your PHP script that fetches Google Sheets data
    private string dataUrl = "https://devgene.live/FetchUsers.php";

    [System.Serializable]
    public class CustomerData
    {
        public string Email;
        public string CustomerID;
        public string FirstName;
        public string LastName;
        public string Orders;
        public string TotalSpent;
        public string LastOrderID;
    }

    [System.Serializable]
    public class CustomerDataList
    {
        public List<CustomerData> customers = new List<CustomerData>();
    }

    void Start()
    {
        StartCoroutine(FetchDataFromServer());
    }

    private IEnumerator FetchDataFromServer()
    {
        UnityWebRequest www = UnityWebRequest.Get(dataUrl);
        Debug.Log("Sending request to server...");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error while fetching data: " + www.error);
        }
        else
        {
            Debug.Log("Data fetched successfully.");
            ProcessData(www.downloadHandler.text);
        }
    }

    private void ProcessData(string jsonData)
    {
        Debug.Log("Processing data...");
        CustomerDataList customerDataList = JsonUtility.FromJson<CustomerDataList>(jsonData);
        if (customerDataList != null && customerDataList.customers.Count > 0)
        {
            Debug.Log("Data processed and loaded: " + JsonUtility.ToJson(customerDataList, true));
            // Optionally save to PlayerPrefs or handle data as needed
            PlayerPrefs.SetString("CustomerData", JsonUtility.ToJson(customerDataList, true));
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogError("No data found or data parsing error.");
        }
    }

    // Example method to load data from PlayerPrefs
    public void LoadCustomerData()
    {
        if (PlayerPrefs.HasKey("CustomerData"))
        {
            string json = PlayerPrefs.GetString("CustomerData");
            CustomerDataList loadedData = JsonUtility.FromJson<CustomerDataList>(json);
            Debug.Log("Loaded data from PlayerPrefs.");
            // Use loadedData as needed
        }
        else
        {
            Debug.LogError("No data found in PlayerPrefs.");
        }
    }
}
