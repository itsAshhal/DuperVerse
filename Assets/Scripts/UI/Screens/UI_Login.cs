using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CBS;
//using Unity.Profiling.Editor;
using PlayFab;
using PlayFab.ClientModels;
using System.Security.Cryptography;
using CBS.Models;
public class UI_Login : UI_Panel
{
    public TextMeshProUGUI dummyText;
    public Button loginBtn;
    public Button BackBtn;

    IAuth AuthModule;
    IProfile ProfileModule;
    void Start()
    {
        AuthModule = CBSModule.Get<CBSAuthModule>();

        loginBtn.onClick.AddListener(OnClick_Login);
        BackBtn.onClick.AddListener(OnClick_Back);

        Debug.Log($"UILogin started");

        AssetManager.Instance.SetTexts();


    }
    public void OnClick_Login()
    {
        if (PlayerPrefs.HasKey("UserEmail"))
        {
            AutoLogin();
            return;
        }



        Debug.Log($"LoginTest, loginClicked");
        //SoundManager.Instance.PlayButtonSound(0);

        loginBtn.interactable = false;

        AuthModule.LoginWithDevice((res) =>
        {
            if (res.IsSuccess)
            {
                // logic for getting displayName
                ProfileModule = CBSModule.Get<CBSProfileModule>();

                var playFabID = res.ProfileID;

                ProfileModule.GetProfileAccountInfo(playFabID, (res) =>
                {
                    // the result for getting the account information
                    if (res.IsSuccess)
                    {
                        // Checking
                        Debug.Log($"DisplayName {res.DisplayName}");

                        // Updating the display name for the user from the GameManager
                        GameManager.instance.userProfile.PlayerName = res.DisplayName;
                        GameManager.instance.userProfile.CurrentUserStatus = UserProfileSO.UserStatus.New;

                        // after getting displayName, get the total cards, as he must have some
                        /*// just use the Fetch method from the ObtainedCardsManager
                        ObtainedCardsManager.Instance.Fetch("OwnedCards");

                        // now obtain the PlayerLevel as well
                        ObtainedCardsManager.Instance.Fetch("PlayerLevel");*/

                        StartCoroutine(OnLoginSucc());
                    }
                    else Debug.Log($"Error getting display name");

                    loginBtn.interactable = true;
                });

                // previous loginCoroutine()
            }
            else
            {
                Debug.LogError($"{res.Error.Message}");
                //Debug.Log("Error");
            }
            loginBtn.interactable = true;
        });
        if (!PlayerPrefs.HasKey("NewUser"))
        {
            UI_Manager.Instance.CloseAllPanels();
            UI_Manager.Instance.OpenPanel(typeof(Signup));

            // Since the user is new and is signing up so we need a flag that this is a new user and give him some bunch of cards pack
            GameManager.instance.userProfile.CurrentUserStatus = UserProfileSO.UserStatus.New;
        }
        else
        {
            AutoLogin();

            // here the user is not new 
            GameManager.instance.userProfile.CurrentUserStatus = UserProfileSO.UserStatus.Old;
        }

    }
    public void OnClick_Back()
    {
        SoundManager.Instance.PlayButtonSound(0);
    }
    IEnumerator OnLoginSucc()
    {
        UI_Manager.Instance.OpenPanel(typeof(UI_LoadingForWait), false);

        yield return new WaitForSeconds(5);
        loginCallBack();
    }
    public void loginCallBack()
    {
        UI_Manager.Instance.CloseAllPanels();
        UI_Manager.Instance.OpenPanel(typeof(UI_MainMenu));

        // just use the Fetch method from the ObtainedCardsManager
        ObtainedCardsManager.Instance.Fetch("OwnedCards");

        // now obtain the PlayerLevel as well
        ObtainedCardsManager.Instance.Fetch("PlayerLevel");

        Debug.Log($"In the login callback");
    }

    void AutoLogin()
    {
        Debug.Log("Trying Auto Login");
        string email = PlayerPrefs.GetString("UserEmail");
        string password = PlayerPrefs.GetString("Password");

        Debug.Log($"AutoLogin, email is {email}");
        Debug.Log($"AutoLogin, password is {password}");

        // trying login with Email and Password using CBS
        var loginReq = new CBSMailLoginRequest
        {
            Mail = email,
            Password = password
        };
        AuthModule.LoginWithDevice(res =>
        {
            if (res.IsSuccess)
            {

                // logic for getting displayName
                ProfileModule = CBSModule.Get<CBSProfileModule>();

                var playFabID = res.ProfileID;

                ProfileModule.GetProfileAccountInfo(playFabID, (res) =>
                {
                    // the result for getting the account information
                    if (res.IsSuccess)
                    {
                        // Checking
                        Debug.Log($"Checking autoLogin, DisplayName is {res.DisplayName}");
                        Debug.Log($"DisplayName {res.DisplayName}");

                        // Updating the display name for the user from the GameManager
                        GameManager.instance.userProfile.PlayerName = res.DisplayName;

                        // after getting displayName, get the total cards, as he must have some
                        /*// just use the Fetch method from the ObtainedCardsManager
                        ObtainedCardsManager.Instance.Fetch("OwnedCards");

                        // now obtain the PlayerLevel as well
                        ObtainedCardsManager.Instance.Fetch("PlayerLevel");*/

                        //StartCoroutine(OnLoginSucc());
                        Debug.Log($"Finally in the main domain");
                        Debug.Log("Login successful: " + playFabID);
                        PlayerPrefs.SetInt("NewUser", 0);

                        /*UI_Manager.Instance.CloseAllPanels();
                        UI_Manager.Instance.OpenPanel(typeof(UI_MainMenu));*/

                        GameManager.instance.userProfile.CurrentUserStatus = UserProfileSO.UserStatus.Old;

                        loginCallBack();
                    }
                    else Debug.Log($"Error getting display name");

                    loginBtn.interactable = true;
                });

                // previous loginCoroutine()

                /*Debug.Log("Login successful: " + res.PlayFabId);
                PlayerPrefs.SetInt("NewUser", 0);
                UI_Manager.Instance.CloseAllPanels();
                UI_Manager.Instance.OpenPanel(typeof(UI_MainMenu));*/
            }

        });

        /*var request = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password
        };

       

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginError);*/
    }
    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login successful: " + result.PlayFabId);
        PlayerPrefs.SetInt("NewUser", 0);
        UI_Manager.Instance.CloseAllPanels();
        UI_Manager.Instance.OpenPanel(typeof(UI_MainMenu));
    }

    private void OnLoginError(PlayFabError error)
    {
        UI_Manager.Instance.CloseAllPanels();
        UI_Manager.Instance.OpenPanel(typeof(Signup));
    }
}
