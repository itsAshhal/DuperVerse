using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PlayFab.ClientModels;
using PlayFab;
using System;
using Random = UnityEngine.Random;

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
    [Header("Specific boolean for Avatar API")]
    public bool IsAvatarApiCalled = false;
    public List<GameObject> OriginalCards = new List<GameObject>();

    public Image[] GiveAwayCardsImages = new Image[0];
    public GameObject GiveAwayScreen;

    public GameObject DontShowThisAgainScreen;


    void Start()
    {
        AssetManager.Instance.SetTexts();

        playBtn.onClick.AddListener(OnClick_Play);
        BackBtn.onClick.AddListener(OnClick_Back);
        settingBtn.onClick.AddListener(OnClick_Settings);
        searchBtn.onClick.AddListener(OnClick_Search);
        //allCardsBtn.onClick.AddListener(OnClick_AllCards);
        //ownedCardsBtn.onClick.AddListener(OnClick_OwnedCards);
        categoryDropDownBtn.onClick.AddListener(OnClick_CategoryDropDown);

        if (PlayerPrefs.HasKey("DontShowThisAgain") || PlayerPrefs.GetInt("DontShowThisAgain") == 1)
        {
            DontShowThisAgainScreen.SetActive(false);
        }
        else
        {
            DontShowThisAgainScreen.SetActive(true);
        }


        InitCards();
        SetupUserProfile();

        // setting avatarSprite to 1st index as there is no customization option right now
        AvatarPlaceholder.sprite = Avatars[0];

        GiveAwayCards();


        if (PlayerPrefs.HasKey("UserEmail")) Debug.Log("User is logging for the second time");
        else Debug.Log($"User is logging for the second time");


        InvokeRepeating(nameof(KeepUpdatingTotalCards), 5f, .5f);
    }

    public void OnClick_DontShowThisAgain(Image image)
    {
        image.enabled = !image.enabled;
        if (image.enabled)
        {
            PlayerPrefs.SetInt("DontShowThisAgain", 1);
        }
        else
        {
            PlayerPrefs.SetInt("DontShowThisAgain", 0);
        }
    }


    void KeepUpdatingTotalCards()
    {
        playerTotalCardsText.text = GameManager.instance.ownedCards.Count.ToString();
    }


    public void GiveAwayCards()
    {
        // assign any 5 cards on first time creating an account
        if (GameManager.instance.userProfile.CurrentUserStatus == UserProfileSO.UserStatus.Old)
        {
            GiveAwayScreen.SetActive(false);
            return;
        }

        List<CardSO> giveAwayCards = new List<CardSO>();
        List<string> cardsDone = new List<string>();

        int counterIndex = 0;
        while (counterIndex < 5)
        {
            int randomIndex = Random.Range(0, GiveAwayCardsImages.Length);
            var card = GameManager.instance.allCards[randomIndex];

            // first check if it already exists
            if (cardsDone.Contains(card.cardName)) continue;

            // If not already added, add the card to the list and update the image
            giveAwayCards.Add(card);
            cardsDone.Add(card.cardName);
            GiveAwayCardsImages[counterIndex].sprite = card.frontSprite;
            counterIndex++;
        }

        // After collecting all cards, upload them once
        StartCoroutine(UploadGiveAwayCards(giveAwayCards));
    }

    IEnumerator UploadGiveAwayCards(List<CardSO> cards)
    {
        Debug.Log($"Uploading give away cards");

        // Create a list to hold stats for all cards
        List<string> cardNames = new List<string>();
        List<List<float>> cardStats = new List<List<float>>();

        foreach (var card in cards)
        {
            Debug.Log($"Inside cardUploading, card is {card.cardName}");

            // Prepare the stats for the card
            List<float> statsList = new List<float> { .1f, .1f, .1f, .1f, .1f };
            cardNames.Add(card.cardName);
            cardStats.Add(statsList);

            // Optionally wait between cards if needed
            yield return new WaitForSeconds(.5f);
        }

        // Call the Upload2 method with the collected data
        ObtainedCardsManager.Instance.UploadMultipleCards(cardNames, cardStats, cards[0].cardCategoryName);
    }

    public void OnInputField_Search(string search)
    {
        string trimmedSearch = search.Trim();

        // If the search is empty, show all cards
        if (string.IsNullOrEmpty(trimmedSearch))
        {
            foreach (var card in OriginalCards)
            {
                card.SetActive(true);
            }
            return;
        }

        // Show matching cards
        foreach (var card in OriginalCards)
        {
            if (card.name.IndexOf(trimmedSearch, StringComparison.OrdinalIgnoreCase) >= 0)
                card.SetActive(true);
            else
                card.SetActive(false);
        }
    }



    public void SetupUserProfile()
    {
        playerNameText.text = userProfile.PlayerName;
        playerTotalCardsText.text = userProfile.PlayerTotalOwnedCards.ToString();
        playerLevelText.text = userProfile.PlayerLevel;

        // use avatar sesction as well
        //GetAvatarIndexFromPlayFab();

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

    public void InstantiateAllCards()
    {
        // Clear previous content objects
        Debug.Log($"AllCards are instantiated");
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

            OriginalCards.Add(prefab);
            prefab.gameObject.name = card.cardName;

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
        return;
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
        Debug.Log($"Avatar index function called");

        // we need to make sure this method doesn't get called so many times because this is an api
        if (IsAvatarApiCalled) return;

        IsAvatarApiCalled = true;

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
