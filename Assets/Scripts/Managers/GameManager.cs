using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<CardSO> allCards;
    public List<CardSO> ownedCards;
    [Tooltip("The cards that will be gifted to the user who has just logged in for the for the first time")]
    public List<CardSO> GiveAwayCards;
    public UserProfileSO userProfile;

    // private 
    List<UI_OwnedCard> instantiatedOwnedCards = new List<UI_OwnedCard>();

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        InitGame();
    }

    public void InitGame()
    {
        UI_Manager.Instance.OpenPanel(typeof(UI_Loading), true);


    }

    public void UpdateOwnedCards(List<CardSO> cardList)
    {
        // make sure previous data is destroyed, before instantiating new one
        foreach (var card in instantiatedOwnedCards) Destroy(card.gameObject);
        instantiatedOwnedCards.Clear();



        Transform parent = UI_Manager.Instance.OwnedCardsParent;
        var cardPrefab = UI_Manager.Instance.OwnedCard;

        foreach (var card in ownedCards)
        {
            var newOwnedCard = Instantiate(cardPrefab, parent);
            newOwnedCard.SetCard(card.name, card.cardCategoryName, card.frontSprite, 0f, 0f, 0f, 0f, 0f, 0f, card.LowerBackground);

            if (instantiatedOwnedCards.Contains(newOwnedCard) == false) instantiatedOwnedCards.Add(newOwnedCard);
        }

        // setting up the values of different stats for UI_OwnedCards
        foreach (var card in cardList)
        {
            foreach (var myCard in instantiatedOwnedCards)
            {
                if (card.cardName == myCard.cardName)
                {
                    myCard.SetCard(
                        myCard.cardName,
                        myCard.cardCategory,
                        myCard.cardFront,
                        card.strength,
                        card.speed,
                        card.intelligence,
                        card.stamina,
                        card.strange,
                        card.fight,
                        myCard.cardLowerBackground.sprite
                        );
                }
            }
        }

        // making sure for ownedCards of CardSO
        //ownedCards.Clear();

        // as we've updated the cards, now update the SO as well
        //userProfile.PlayerTotalOwnedCards++;
        // update the name from the UI as well
        UI_Manager.Instance.ui_mainMenu.SetupUserProfile();
    }


    // <summary>
    /// The method checks whether the user is the new user, if he is then he's been given a pack of free cards otherwise he won't be given any
    /// </summary>
    public void GiveAwayAtFirstLogin()
    {
        return;
        // Ensure the player receives 5 unique cards
        HashSet<int> selectedIndices = new HashSet<int>();

        while (selectedIndices.Count < 5 && selectedIndices.Count < allCards.Count)
        {
            int randomIndex = Random.Range(0, allCards.Count);

            // Ensure no duplicates
            if (!selectedIndices.Contains(randomIndex))
            {
                selectedIndices.Add(randomIndex);
                CardSO selectedCard = allCards[randomIndex];
                GiveAwayCards.Add(selectedCard);

                // Add to owned cards as well
                ownedCards.Add(selectedCard);
            }
        }

        // Update the player's total owned cards count
        userProfile.PlayerTotalOwnedCards += selectedIndices.Count;
        // You might also want to save the updated profile here or update the UI
        UI_Manager.Instance.ui_mainMenu.SetupUserProfile();
    }


}
