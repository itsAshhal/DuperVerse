using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System.IO;
using System;
using System.Collections;
using UnityEngine.Networking;

public class Signup : UI_Panel
{
    [Header("UI Components")]
    public TMP_InputField emailInputField;
    public TMP_InputField passInputField;
    public TextMeshProUGUI welcomeText, descriptionText, passwordText, errorText;
    public Button nextButton, nextButton2, nextButton3;
    public GameObject step1, step2, step3;

    private CustomerDataList customerDataList;

    [System.Serializable]
    public class CustomerData
    {
        public string CustomerID;
        public string FirstName;
        public string LastName;
        public string Email;
        public string TotalSpent;
        public string TotalOrders;
    }

    [System.Serializable]
    public class CustomerDataList
    {
        public List<CustomerData> customers = new List<CustomerData>();
    }

    private void Start()
    {
        InitializeUI();
        LoadCustomerData();
    }

    private void InitializeUI()
    {
        nextButton.onClick.AddListener(OnNextButtonClicked);
        nextButton2.onClick.AddListener(OnNextButton2Clicked);
        nextButton3.onClick.AddListener(OnNextButton3Clicked);
        ClearError();
    }

    private void LoadCustomerData()
    {
        StartCoroutine(FetchDataFromServer());
    }

    private IEnumerator FetchDataFromServer()
    {
        string dataUrl = "https://devgene.live/FetchUsers.php"; // URL to your PHP script
        UnityWebRequest www = UnityWebRequest.Get(dataUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error while fetching data: " + www.error);
            ShowError("Failed to load customer data.");
        }
        else
        {
            Debug.Log("Data fetched successfully.");
            ProcessData(www.downloadHandler.text);
        }
    }

    private void ProcessData(string jsonData)
    {
        try
        {
            customerDataList = JsonUtility.FromJson<CustomerDataList>(jsonData);
            if (customerDataList != null && customerDataList.customers.Count > 0)
            {
                Debug.Log("Customer data loaded successfully.");
            }
            else
            {
                Debug.LogError("No data found or data parsing error.");
                ShowError("No customer data available.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error parsing JSON: " + ex.Message);
            ShowError("Error loading customer data.");
        }
    }


    private void OnNextButtonClicked()
    {
        ClearError();
        string email = emailInputField.text.Trim();
        if (string.IsNullOrEmpty(email))
        {
            ShowError("Please enter an email address.");
            return;
        }

        CustomerData customer = customerDataList?.customers.Find(c => c.Email == email);
        if (customer != null)
        {
            SaveCustomerDataInPlayerPrefs(customer);
            ShowStep2();
        }
        else
        {
            ShowError("No customer with this email found. Please create an account at Duperverse.com.");
        }
    }

    private void OnNextButton2Clicked()
    {
        ClearError();
        string password = passInputField.text.Trim();
        if (string.IsNullOrEmpty(password))
        {
            ShowError("Password cannot be empty.");
            return;
        }
        else if (password.Length < 8)
        {
            ShowError("Password must be at least 8 characters long.");
            return;
        }

        PlayerPrefs.SetString("Password", password);
        string email = PlayerPrefs.GetString("UserEmail");
        HandleLoginOrRegister(email, password);
    }

    private void OnNextButton3Clicked()
    {
        Debug.Log("Step 3: Next button clicked.");
        UI_Manager.Instance.OpenPanel(typeof(UI_MainMenu), true);
        // Additional functionality for step 3 can be implemented here
    }

    private void SaveCustomerDataInPlayerPrefs(CustomerData customer)
    {
        PlayerPrefs.SetString("CustomerID", customer.CustomerID);
        PlayerPrefs.SetString("FirstName", customer.FirstName);
        PlayerPrefs.SetString("LastName", customer.LastName);
        PlayerPrefs.SetString("UserEmail", customer.Email);
        PlayerPrefs.SetString("TotalSpent", customer.TotalSpent);
        PlayerPrefs.SetString("TotalOrders", customer.TotalOrders);
        Debug.Log("Customer data saved in PlayerPrefs");
    }

    private void ShowStep2()
    {
        step1.SetActive(false);
        step2.SetActive(true);
        welcomeText.text = $"Welcome: {PlayerPrefs.GetString("FirstName")}";
        descriptionText.text = "You are already our valued customer at Duperverse.com";
        passwordText.text = "Please enter your account password";
    }

    private void ShowStep3()
    {
        step2.SetActive(false);
        step3.SetActive(true);
        welcomeText.text = $"You are all set: {PlayerPrefs.GetString("FirstName")}";
    }

    private void ShowError(string message)
    {
        errorText.text = message;
        Debug.LogError(message);
    }

    private void ClearError()
    {
        errorText.text = string.Empty;
    }

    private void HandleLoginOrRegister(string email, string password)
    {
        LoginWithEmail(email, password);
    }

    private void LoginWithEmail(string email, string password)
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login successful: " + result.PlayFabId);
        PlayerPrefs.SetInt("NewUser", 0);
        ShowStep3();
    }

    private void OnLoginError(PlayFabError error)
    {
        if (error.Error == PlayFabErrorCode.AccountNotFound)
        {
            RegisterWithEmail(PlayerPrefs.GetString("UserEmail"), PlayerPrefs.GetString("Password"), PlayerPrefs.GetString("FirstName"));
        }
        else
        {
            ShowError("Login failed: " + error.GenerateErrorReport());
        }
    }

    private void RegisterWithEmail(string email, string password, string username)
    {
        var request = new RegisterPlayFabUserRequest
        {
            Email = email,
            Password = password,
            Username = username
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterError);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Registration successful: " + result.PlayFabId);
        PlayerPrefs.SetInt("NewUser", 1);
        ShowStep3();
    }

    private void OnRegisterError(PlayFabError error)
    {
        ShowError("Registration failed: " + error.GenerateErrorReport());
    }
}
