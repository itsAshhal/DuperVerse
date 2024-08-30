using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine.Networking;

public class GoogleSheetsToJson : MonoBehaviour
{
    // Replace with your Google Sheets published CSV URL
    [SerializeField]
    private string googleSheetUrl = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQ5xLXw9Mq3DsTtCqPTlUxeHSVdfsonrLyPccLO-nfxZH8RUs7xRT2FuOH2Cd-F3IhMUvAommOYVX-i/pub?output=csv";

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

    public class CustomerDataList
    {
        public List<CustomerData> customers = new List<CustomerData>();
    }

    void Start()
    {
        StartCoroutine(FetchGoogleSheetData());
    }

    private IEnumerator FetchGoogleSheetData()
    {
        UnityWebRequest www = UnityWebRequest.Get(googleSheetUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            ProcessCSV(www.downloadHandler.text);
        }
    }

    private void ProcessCSV(string csvData)
    {
        string[] rows = csvData.Split('\n');
        CustomerDataList customerDataList = new CustomerDataList();

        for (int i = 1; i < rows.Length; i++)
        {
            string[] cells = rows[i].Split(',');

            if (cells.Length >= 7)
            {
                CustomerData customer = new CustomerData
                {
                    Email = cells[0],
                    CustomerID = cells[1],
                    FirstName = cells[2],
                    LastName = cells[3],
                    Orders = cells[4],
                    TotalSpent = cells[5],
                    LastOrderID = cells[6]
                };
                customerDataList.customers.Add(customer);
            }
        }

        // Convert to JSON for use in Unity
        string json = JsonUtility.ToJson(customerDataList, true);
        Debug.Log(json);

        // Save JSON to file or process as needed
        System.IO.File.WriteAllText(Application.dataPath + "/customerData.json", json);
    }
}
