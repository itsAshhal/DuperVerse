using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PlayFab.ClientModels;
using PlayFab;

public class UI_MainMenu : UI_Panel
{
    public TextMeshProUGUI dummyText;
    public Button playBtn;
    public Button BackBtn;
    public Button settingBtn;
    public Button searchBtn;
    public Button allCardsBtn;
    public Button ownedCardsBtn;
    public Button categoryDropDownBtn;
    public Transform ownedCardsParent;

    public GameObject allCardsPanel;
    public GameObject zoomedCardPrefab;
    public UserProfileSO userProfile;
    public Text playerNameText;
    public Text playerLevelText;
    public Text playerTotalCardsText;
    public Sprite[] Avatars;
    public Image AvatarPlaceholder;
    void Start()
    {
        playBtn.onClick.AddListener(OnClick_Play);
        BackBtn.onClick.AddListener(OnClick_Back);
        settingBtn.onClick.AddListener(OnClick_Settings);
        searchBtn.onClick.AddListener(OnClick_Search);
        //allCardsBtn.onClick.AddListener(OnClick_AllCards);
        //ownedCardsBtn.onClick.AddListener(OnClick_OwnedCards);
        categoryDropDownBtn.onClick.AddListener(OnClick_CategoryDropDown);


        InitCards();
        SetupUserProfile();


        if (PlayerPrefs.HasKey("UserEmail")) Debug.Log("User is logging for the second time");
        else Debug.Log($"User is logging for the second time");
    }

    public void SetupUserProfile()
    {
        playerNameText.text = userProfile.PlayerName;
        playerTotalCardsText.text = userProfile.PlayerTotalOwnedCards.ToString();
        playerLevelText.text = userProfile.PlayerLevel;

        // use avatar sesction as well
        GetAvatarIndexFromPlayFab();

        if (GameManager.instance.userProfile.CurrentUserStatus == UserProfileSO.UserStatus.New) GameManager.instance.GiveAwayAtFirstLogin();

    }
    void InitCards()
    {
        OnClick_AllCards();
    }
    public void OnClick_Play()
    {

    }
    public void OnClick_Back()
    {

    }
    public void OnClick_Settings()
    {
        SoundManager.Instance.PlayButtonSound(0);
        UI_Manager.Instance.OpenPanel(typeof(UI_Settings), false);
    }
    public void OnClick_Search()
    {
        SoundManager.Instance.PlayButtonSound(0);
    }
    public void OnClick_AllCards()
    {
        SoundManager.Instance.PlayButtonSound(0);
        EventSystem.current.SetSelectedGameObject(allCardsBtn.gameObject);
        allCardsPanel.SetActive(true);
        InstantiateAllCards();
    }
    public void OnClick_OwnedCards()
    {
        SoundManager.Instance.PlayButtonSound(0);
        allCardsPanel.SetActive(true);
        InstantiateOwnedCards();
    }
    public void OnClick_CategoryDropDown()
    {

    }



    public GameObject cardPrefab;   // Prefab for the card group that holds four cards
    public Transform content;            // The content object of the ScrollView

    void InstantiateAllCards()
    {
        // Clear previous content objects
        if (content.childCount > 0)
        {
            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }
        }

        foreach (CardSO card in GameManager.instance.allCards)
        {
            GameObject prefab = Instantiate(cardPrefab, content);
            prefab.SetActive(true);

            Transform cardTransform = prefab.transform;
            Image cardImage = cardTransform.GetComponent<Image>();
            cardImage.sprite = card.frontSprite;
            Button cardButton = cardTransform.GetComponent<Button>();
            cardButton.onClick.AddListener(() =>
            {
                GameObject zoomedCard = Instantiate(zoomedCardPrefab, UI_Manager.Instance.transform);
                UI_ZoomedCard prefabScript = zoomedCard.GetComponent<UI_ZoomedCard>();

                prefabScript.InitUI(card);
            });
        }

    }




    public Transform contentOwnedCards;            // The content object of the ScrollView

    void InstantiateOwnedCards()
    {
        if (content.childCount > 0)
        {
            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }
        }

        foreach (CardSO card in GameManager.instance.ownedCards)
        {
            GameObject prefab = Instantiate(cardPrefab, content);
            prefab.SetActive(true);

            Transform cardTransform = prefab.transform;
            Image cardImage = cardTransform.GetComponent<Image>();
            cardImage.sprite = card.frontSprite;
            Button cardButton = cardTransform.GetComponent<Button>();
            cardButton.onClick.AddListener(() =>
            {
                GameObject zoomedCard = Instantiate(zoomedCardPrefab, UI_Manager.Instance.transform);
                UI_ZoomedCard prefabScript = zoomedCard.GetComponent<UI_ZoomedCard>();

                prefabScript.InitUI(card);
            });
        }
    }


    public void ShowCategorySpecificCards(CardCategory category)
    {

        // Clear previous content objects
        if (content.childCount > 0)
        {
            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }
        }


        int totalImages = GameManager.instance.allCards.Count;

        foreach (CardSO card in GameManager.instance.allCards)
        {
            if (card.category != category)
                continue;

            GameObject prefab = Instantiate(cardPrefab, content);
            prefab.SetActive(true);

            Transform cardTransform = prefab.transform;
            Image cardImage = cardTransform.GetComponent<Image>();
            cardImage.sprite = card.frontSprite;
            Button cardButton = cardTransform.GetComponent<Button>();
            cardButton.onClick.AddListener(() =>
            {
                GameObject zoomedCard = Instantiate(zoomedCardPrefab, UI_Manager.Instance.transform);
                UI_ZoomedCard prefabScript = zoomedCard.GetComponent<UI_ZoomedCard>();

                prefabScript.InitUI(card);
            });
        }
    }

    #region AvatarSelection

    public void GetAvatarIndex(int index)
    {
        PlayerPrefs.SetInt("AvatarIndex", index);
        SetAvatarIndexInPlayFab(index);
    }

    private void SetAvatarIndexInPlayFab(int index)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "AvatarIndex", index.ToString() }
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSendSuccess, OnDataSendError);
    }

    private void OnDataSendSuccess(UpdateUserDataResult result)
    {
        Debug.Log("Successfully set avatar index on PlayFab");
        SetAvatar(PlayerPrefs.GetInt("AvatarIndex"));
    }

    private void OnDataSendError(PlayFabError error)
    {
        Debug.LogError("Error setting avatar index: " + error.GenerateErrorReport());
    }

    private void GetAvatarIndexFromPlayFab()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataReceived, OnDataReceiveError);
    }

    private void OnDataReceived(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey("AvatarIndex"))
        {
            int index = int.Parse(result.Data["AvatarIndex"].Value);
            PlayerPrefs.SetInt("AvatarIndex", index);
            Debug.Log("Avatar index retrieved: " + index);
            // Update avatar selection UI if necessary
            SetAvatar(PlayerPrefs.GetInt("AvatarIndex"));
        }
    }

    private void OnDataReceiveError(PlayFabError error)
    {
        Debug.LogError("Error retrieving avatar index: " + error.GenerateErrorReport());
    }

    void SetAvatar(int Index)
    {
        AvatarPlaceholder.sprite = Avatars[Index];
    }

    #endregion
}
