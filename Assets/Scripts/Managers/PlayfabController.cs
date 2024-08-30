using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;


public class PlayfabController : MonoBehaviour
{
    public static PlayfabController Instance { get; private set; }
    private UI_ZoomedCard m_zoomedcardInstance;
    public List<UI_OwnedCard> OwnedCards;
    [SerializeField] UI_OwnedCard OwnedCardPrefab;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        // lets instantiate all the cards when the scene is inititated
        // later when the call is updated, the same method will be called again
        GetAllCardsAtOnce();
    }

    #region PlayfabCalls

    // Keys for the data we want to store
    private const string keyCardName = "cardName";
    private const string keyCardCategory = "cardCategory";

    // Getting all cards data
    public void GetAllCardsAtOnce()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            Dictionary<string, string> allCards = new Dictionary<string, string>();


            foreach (var item in result.Data)
            {
                Debug.Log($"KEY -> {item.Key}, VALUE -> {item.Value.Value}");
                allCards[item.Key] = item.Value.Value;
            }

            // Process or return the data
            OnReceivedAllCardsAtonce(allCards);
        }, error =>
        {
            Debug.LogError("Error retrieving data: " + error.GenerateErrorReport());
            OnReceivedAllCardsAtonce(null);
        });
    }

    public void OnReceivedAllCardsAtonce(Dictionary<string, string> allCards)
    {
        if (allCards != null)
        {
            Debug.Log($"Setting up cards at start");
            foreach (var card in OwnedCards) Destroy(card.gameObject);
            OwnedCards.Clear();

            // Create a dictionary for fast lookup of ScriptableObject cards by name
            Dictionary<string, CardSO> cardLookup = new Dictionary<string, CardSO>();
            foreach (var mainCard in GameManager.instance.allCards)
            {
                cardLookup[mainCard.cardName] = mainCard;
            }

            foreach (var card in allCards)
            {
                Debug.Log($"CardName {card.Key}, CardCategory {card.Value}");
                var newOwnedCard = Instantiate(OwnedCardPrefab, UI_Manager.Instance.OwnedCardsParent);
                var cardName = card.Key;
                var cardCategory = card.Value;
                Sprite cardFront = null;

                // Check if the card name exists in the dictionary
                if (cardLookup.TryGetValue(cardName, out CardSO mainCard))
                {
                    Debug.Log($"COMPARISON -> {newOwnedCard.cardName}, {mainCard.cardName}");
                    // Change the sprite if the card is found
                    Debug.Log($"Card sprite found");
                    cardFront = mainCard.frontSprite;
                }

                // Set the card
                //newOwnedCard.SetCard(cardName, cardCategory, cardFront);

                // Add the card to the list
                OwnedCards.Add(newOwnedCard);
            }

            // Optionally, you can handle the data further
            // For example, update the UI or process the data in another way
        }
        else
        {
            Debug.Log("No card data available.");
        }

    }

    public void SavePlayerCard(string cardName, string cardCategory, UI_ZoomedCard zoomedCardInstance)
    {
        // save the instance for a while so we can send back the message that the card has been added to the collection
        this.m_zoomedcardInstance = zoomedCardInstance;

        var requestData = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                //{ cardName ,keyCardName },
                { cardName ,cardCategory}
            }
        };

        PlayFabClientAPI.UpdateUserData(requestData, OnDataSendSuccess, OnDataSendFailure);
    }

    public void RetrieveAllPlayerCards(UI_ZoomedCard zoomedCardInstance)
    {
        this.m_zoomedcardInstance = zoomedCardInstance;

        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            // Check if the specific card name is present in the retrieved data
            if (result.Data.ContainsKey(zoomedCardInstance.card.cardName))
            {
                Debug.Log($"CardData found");
                // Deserialize the specific card data (in this case, it is a single string)
                string storedCategory = result.Data[zoomedCardInstance.card.cardName].Value;

                // Prepare a dictionary with the single entry for compatibility with OnGetCardDataFromPlayfab
                Dictionary<string, string> singleCardData = new Dictionary<string, string>
            {
                { zoomedCardInstance.card.cardName, storedCategory }
            };

                // Invoke the method to check the card data
                OnAllCardsRetrieved(singleCardData);
            }
            else
            {
                Debug.Log("No card data found.");
                OnAllCardsRetrieved(null);
            }
        }, error =>
        {
            Debug.LogError("Error retrieving data: " + error.GenerateErrorReport());
            OnAllCardsRetrieved(null);
        });
    }
    private void OnAllCardsRetrieved(Dictionary<string, string> allCards)
    {
        // This method can be used to handle the data after retrieval
        // For example, you can update the UI or further process the data
        this.m_zoomedcardInstance.OnGetCardDataFromPlayfab(allCards);
    }

    private void OnDataSendSuccess(UpdateUserDataResult result)
    {
        Debug.Log("Card updated successful");



        // since the card is succesfully added, send back the message 
        this.m_zoomedcardInstance.CollectionStatus(true);
        return;
        /*
        // as the card is added to the collection in the playfab, add it to the list
        // of the OwnedCards so we can display it in the "OwnedCards" panel category
        OwnedCards.Add(m_zoomedcardInstance);

        // now update the OwnedCardsParent from the UIManager whenever the new card is added to the collection
        Transform OwnedCardTransform = UI_Manager.Instance.OwnedCardsParent;

        // deploy each card there
        foreach (var card in this.OwnedCards)
        {
            this.DeployedCards.Add(Instantiate(card.gameObject, OwnedCardTransform));
        }*/
    }

    private void OnDataSendFailure(PlayFabError error)
    {
        Debug.LogError("Card updated error: " + error.GenerateErrorReport());

        // since the card update is failed....
        this.m_zoomedcardInstance.CollectionStatus();
    }

    public void GetPlayerData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataReceiveSuccess, OnDataReceiveFailure);
    }

    private void OnDataReceiveSuccess(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey(keyCardName) && result.Data.ContainsKey(keyCardCategory))
        {
            string cardName = result.Data[keyCardName].Value;
            string cardCategory = result.Data[keyCardCategory].Value;

            Debug.Log($"Card Name: {cardName}, Card Category: {cardCategory}");
        }
        else
        {
            Debug.Log("No card data found.");
        }
    }

    private void OnDataReceiveFailure(PlayFabError error)
    {
        Debug.LogError("Data retrieval failed: " + error.GenerateErrorReport());
    }

    #endregion
}
