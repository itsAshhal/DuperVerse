using System.Collections.Generic;
using UnityEngine;
using CBS;
using CBS.Models;
using Unity.VisualScripting;

public class ObtainedCardsManager : MonoBehaviour
{
    public static ObtainedCardsManager Instance { get; private set; }

    private IProfile ProfileModule { get; set; }

    private void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this);
    }

    public void Upload(string cardName, List<float> statsList, string cardCategory, UI_ZoomedCard zoomedCard)
    {
        foreach (var stat in statsList)
        {
            Debug.Log($"CardStat is {stat}");
        }

        ProfileModule = CBSModule.Get<CBSProfileModule>();
        var profileDataKey = "OwnedCards";
        GameManager.instance.ownedCards.Clear();

        // Fetch existing data if available
        ProfileModule.GetProfileData(profileDataKey, (result) =>
        {
            if (result.IsSuccess && result.Data != null && result.Data.Count > 0)
            {
                // Deserialize existing data
                var dataDictionary = result.Data;
                List<CardSO> cardList = new List<CardSO>();
                List<CardSO> existingCards = new List<CardSO>();

                foreach (var kvp in dataDictionary)
                {
                    string jsonValue = kvp.Value.Value;
                    existingCards = CardsJSON_Manager.FromJSON(jsonValue);
                    cardList.AddRange(existingCards);
                }

                // FOR REMEMBERING

                /*
                      public float strength = 0f;
                  public float speed = 0f;
                  public float intelligence = 0f;
                  public float fight = 0f;
                  public float stamina = 0f;
                  public float strange = 0f;
                      */


                foreach (var card in cardList)
                {
                    Debug.Log($"ExistingCard strength: {card.strength}");
                    Debug.Log($"ExistingCard speed: {card.speed}");
                    Debug.Log($"ExistingCard fight: {card.fight}");
                    Debug.Log($"ExistingCard intelligence: {card.intelligence}");
                    Debug.Log($"ExistingCard stamina: {card.stamina}");
                    Debug.Log($"ExistingCard strange: {card.strange}");

                }



                // check for duplicate cards
                CardSO cardToRemove = null;
                foreach (var card in existingCards)
                {
                    if (card.cardName == cardName) cardToRemove = card;
                }

                // checking the card, we have just removed
                // Debug.Log($"RemovedCard strength: {cardToRemove.strength}");
                // Debug.Log($"RemovedCard speed: {cardToRemove.speed}");
                // Debug.Log($"RemovedCard intelligence: {cardToRemove.intelligence}");
                // Debug.Log($"RemovedCard fight: {cardToRemove.fight}");
                // Debug.Log($"RemovedCard stamina: {cardToRemove.stamina}");
                // Debug.Log($"RemovedCard strange: {cardToRemove.strange}");

                cardList.Remove(cardToRemove);

                // since we've removed this card, it continas the previous 
                // data which is to be incremented to the new one
                // Add new card to the list


                if (cardToRemove != null)
                {
                    // making 2 arrays for 2 groups
                    // for randomly distributing 1%

                    float[] group1 = { 0f, 0f, 0f };
                    float[] group2 = { 0f, 0f, 0f };

                    // now randomly assign 1 value to any index of the each group
                    group1[Random.Range(0, group1.Length)] = 1f;
                    group2[Random.Range(0, group2.Length)] = 1f;

                    // now just add random sequential indices to the newCard properties


                    CardSO newCard = new CardSO
                    {
                        cardName = cardName,
                        strength = cardToRemove.strength + group1[0],
                        speed = cardToRemove.speed + group1[1],
                        intelligence = cardToRemove.intelligence + group1[2],
                        fight = cardToRemove.fight + group2[0],
                        stamina = cardToRemove.stamina + group2[1],
                        strange = cardToRemove.strange + group2[2],
                        cardCategoryName = cardCategory,
                    };
                    cardList.Add(newCard);
                }
                else
                {
                    CardSO newCard = new CardSO
                    {
                        cardName = cardName,
                        strength = 0f,
                        speed = statsList[1],
                        intelligence = statsList[2],
                        fight = statsList[3],
                        stamina = statsList[4],
                        strange = statsList[5],
                        cardCategoryName = cardCategory,
                    };
                    cardList.Add(newCard);
                }



                // Convert list to JSON
                string newDataJson = CardsJSON_Manager.ToJSON(cardList);

                // Save updated data
                ProfileModule.SaveProfileData(profileDataKey, newDataJson, (saveResult) =>
                {
                    if (saveResult.IsSuccess)
                    {
                        Debug.Log("Data saved successfully!");

                        zoomedCard.OnCardAdded();
                        GameManager.instance.UpdateOwnedCards(cardList);
                        GameManager.instance.userProfile.PlayerTotalOwnedCards = cardList.Count;
                        UI_Manager.Instance.ui_mainMenu.SetupUserProfile();
                        Fetch("OwnedCards");
                    }
                    else
                    {
                        Debug.Log(saveResult.Error.Message);
                    }
                });
            }
            else if (result.IsSuccess && (result.Data == null || result.Data.Count == 0))
            {
                // No existing data, create new list with the new card
                List<CardSO> cardList = new List<CardSO>()
                {
                new CardSO
                {
                    cardName = cardName,
                    strength = 0f,
                    speed = statsList[1],
                    intelligence = statsList[2],
                    fight = statsList[3],
                    stamina = statsList[4],
                    strange = statsList[5],
                    cardCategoryName = cardCategory,
                }
                };

                // Convert list to JSON
                string newDataJson = CardsJSON_Manager.ToJSON(cardList);

                // Save new data
                ProfileModule.SaveProfileData(profileDataKey, newDataJson, (saveResult) =>
                {
                    if (saveResult.IsSuccess)
                    {
                        Debug.Log("Data saved successfully!");

                        zoomedCard.OnCardAdded();
                        GameManager.instance.UpdateOwnedCards(cardList);
                        GameManager.instance.userProfile.PlayerTotalOwnedCards = cardList.Count;
                        UI_Manager.Instance.ui_mainMenu.SetupUserProfile();
                        Fetch("OwnedCards");
                    }
                    else
                    {
                        Debug.Log(saveResult.Error.Message);
                    }
                });
            }
            else
            {
                Debug.Log(result.Error.Message);
            }
        });
    }

    [ContextMenu("FetchData")]
    public void Fetch(string key)
    {
        ProfileModule = CBSModule.Get<CBSProfileModule>();

        var profileDataKey = key;
        ProfileModule.GetProfileData(profileDataKey, OnGetProfileInfo);
    }

    private void OnGetProfileInfo(CBSGetProfileDataResult result)
    {
        if (result.IsSuccess)
        {
            // first destroy the old owned cards as we're updating them here
            //GameManager.instance.ownedCards.Clear();

            var dataDictionary = result.Data;

            // checking for nothing
            if (dataDictionary.ContainsKey("OwnedCards") == false) GameManager.instance.userProfile.PlayerTotalOwnedCards = 0;  // since there're no own cards
            if (dataDictionary.ContainsKey("PlayerLevel") == false) GameManager.instance.userProfile.PlayerLevel = "0";  // since there's no current playerLevel



            foreach (var kvp in dataDictionary)
            {
                string key = kvp.Key;

                // checking for cards key
                if (key.Equals("OwnedCards"))
                {
                    Debug.Log($"KeyName is {key}");
                    string jsonValue = kvp.Value.Value;

                    // Deserialize JSON string back into List<CardSO>
                    List<CardSO> cardList = CardsJSON_Manager.FromJSON(jsonValue);

                    // update the total number of cards for the player from the GameManager
                    GameManager.instance.userProfile.PlayerTotalOwnedCards = cardList.Count;
                    Debug.Log($"Total Cards we've found are {cardList.Count}");

                    // Now you have your list of CardSO objects, do whatever you need with it
                    foreach (var card in cardList)
                    {
                        Debug.Log($"Card obtained Name: {card.cardName}");
                        Debug.Log($"Card obtained Strength: {card.strength}");
                        Debug.Log($"Card obtained speed: {card.speed}");
                        Debug.Log($"Card obtained stamina: {card.stamina}");
                        Debug.Log($"Card obtained intelligence: {card.intelligence}");
                        Debug.Log($"Card obtained fight: {card.fight}");
                        Debug.Log($"Card obtained strange: {card.strange}");

                        // now update the UI_OwnedCard which are instantiated


                        GameManager.instance.ownedCards.Add(card);

                    }

                    GameManager.instance.UpdateOwnedCards(cardList);

                    // update the SO as well
                    //GameManager.instance.userProfile.PlayerTotalOwnedCards = cardList.Count;
                }
                //else GameManager.instance.userProfile.PlayerTotalOwnedCards = 0;  // since there're no own cards

                // checking for playerLevel key
                if (key.Equals("PlayerLevel"))
                {
                    // Get player level value
                    string levelValue = kvp.Value.Value;
                    int playerLevel = 0;
                    if (int.TryParse(levelValue, out int level))
                    {
                        playerLevel = level;

                        // save it in the UserProfile SO
                        GameManager.instance.userProfile.PlayerLevel = playerLevel.ToString();
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to parse PlayerLevel value: {levelValue}");
                    }
                }
                //else GameManager.instance.userProfile.PlayerLevel = "0";  // since there's no current playerLevel
            }
        }
        else
        {
            Debug.Log(result.Error.Message);
        }
    }
}
