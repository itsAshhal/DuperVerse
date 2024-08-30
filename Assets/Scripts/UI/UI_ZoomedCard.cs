using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;
using Unity.VisualScripting;
using System.Linq;

public class UI_ZoomedCard : MonoBehaviour
{
    public CardSO card;
    [SerializeField] Image cardImage;
    [SerializeField] Button flipBtn;
    [SerializeField] Button hideCardBtn;
    [SerializeField] Button ownCardBtn;
    [SerializeField] TMP_Text cardBtnText;
    bool front = true;
    bool thisCardIsAlreadyOwned = false;
    [SerializeField] GameObject burstParticle;



    public void InitUI(CardSO _card)
    {
        card = _card;
        front = true;

        cardImage.sprite = card.frontSprite;

        cardImage.transform.localScale = Vector3.zero;
        cardImage.transform.DOScale(Vector3.one, 0.5f);

        flipBtn.onClick.RemoveAllListeners();
        flipBtn.onClick.AddListener(Flip);

        hideCardBtn.onClick.RemoveAllListeners();
        hideCardBtn.onClick.AddListener(() => { Destroy(gameObject); });



    }

    private void Start()
    {
        CheckThisCardStatus();
    }

    private void Flip()
    {
        cardImage.transform.DOScaleX(0, 0.25f).OnComplete(() =>
        {
            // Change the sprite after the card is "hidden"
            if (front)
            {
                cardImage.sprite = card.backSprite;
                front = false;
            }
            else
            {
                cardImage.sprite = card.frontSprite;
                front = true;
            }

            // Flip the card back to its original scale
            cardImage.transform.DOScaleX(1, 0.25f);
        });
    }

    /// <summary>
    /// Attach this method to the Buy button on each card so when the user tries to buy it, it opens up the URL and complete the buying process
    /// </summary>
    public void OnClick_BuyThisCard()
    {
        // as this card is using the scriptable object containing its own product ID and the separate URL as well
        // Get the instance and get the job done
        if (OpenBuyButtonPage.Instance == null) return;

        OpenBuyButtonPage.Instance.OpenBuyButton(this.card.ProductID, this.card.BuyButtonURL);
    }


    public void OnClick_OwnThisCard()
    {
        return;
        bool owningForTheFirstTime = false;

        // condition to check if this card exists in the database or not
        owningForTheFirstTime = !GameManager.instance.ownedCards.Contains(this.card);

        Debug.Log($"AlreadyExists {owningForTheFirstTime}");
        List<float> statsList = new List<float> { 0f, 0f, 0f, 0f, 0f, 0f };

        /*
      public float strength = 0f;
  public float speed = 0f;
  public float intelligence = 0f;
  public float fight = 0f;
  public float stamina = 0f;
  public float strange = 0f;
      */

        // now as per requirements, there is either 1st time owning a card or further
        if (owningForTheFirstTime)
        {
            for (int i = 0; i < statsList.Count - 1; i++)
            {
                statsList[i] = 1f;
            }
            statsList[0] = 1f;
            statsList[statsList.Count - 1] = 10f;
        }
        else
        {
            // now here we're saving the stats for the second time
            // so make groups of 3 and take the data the lists of ownedCards
            var ownedCards = GameManager.instance.ownedCards;

            foreach (var card in ownedCards)
            {
                if (this.card.cardName == card.cardName)
                {
                    // now increase 1% of the 2 groups made distinct
                    // get the stats of the ownedCards


                    statsList[0] = card.strength;
                    statsList[1] += card.speed + 1;  // increasing 1% of the 100
                    statsList[2] = card.intelligence;
                    statsList[3] = card.fight;
                    statsList[4] += card.stamina + 1;  // increasing 1% of the 100
                    statsList[5] = card.strange;  // increasing 1% of the 100
                    break;
                    // we can later change this to be more generic and can randomly pick different stats
                }
            }

        }


        // Get the card details
        // card name and the card category
        string cardName = card.cardName;
        CardCategory cardCategory = this.card.category;
        Debug.Log($"Card category is {cardCategory}");

        // call the PlayfabController Instance to save this card category onto the playfab

        // as we are no longer using this traditional playfabController
        //PlayfabController.Instance.SavePlayerCard(cardName, cardCategory.ToString(), this);

        ObtainedCardsManager.Instance.Upload(cardName, statsList, cardCategory.ToString(), this);


    }

    void Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = list[n];
            list[n] = list[k];
            list[k] = temp;
        }
    }


    /// <summary>
    /// Before doing anything, we need to check this card as if its name and category are in the playfab or not
    /// </summary>
    private void CheckThisCardStatus()
    {
        // ok since we have all the cards ready in GameManager.instance.ownedCards
        // we need to search if this card is already owned, then switch the options naming

        var ownedCardsArray = GameManager.instance.ownedCards.ToArray();
        thisCardIsAlreadyOwned = ownedCardsArray.Any(x => x.cardName == this.card.cardName);

        if (thisCardIsAlreadyOwned)
        {
            // change the optiom
            this.cardBtnText.text = "Upgrade";
        }
        else
        {
            // leave the option as it is
        }
    }

    public void OnGetCardDataFromPlayfab(Dictionary<string, string> cardData)
    {
        bool isMatch = false;
        if (cardData != null && cardData.ContainsKey(card.cardName))
        {
            string storedCategory = cardData[card.cardName];
            if (storedCategory == card.category.ToString())
            {
                Debug.Log("Card name and category match found!");
                isMatch = true;
            }
            else
            {
                Debug.Log("Card name found but category does not match.");
                isMatch = false;
            }
        }
        else
        {
            Debug.Log("Card name not found.");
            isMatch = false;
        }

        Debug.Log($"CardStatus {isMatch}");


        // checking other conditions
        if (isMatch)
        {
            this.ownCardBtn.interactable = false;
            this.cardBtnText.text = "Card already owned";
        }
        else
        {
            this.ownCardBtn.interactable = true;
            this.cardBtnText.text = "Own Card";
        }
    }




    /// <summary>
    /// If the card is added to the playfab collection successfully, the method is initiated
    /// </summary>
    /// <param name="added">TRUE if card is saved</param>
    public void CollectionStatus(bool added = false)
    {
        if (!added)
        {
            Debug.LogError($"Couldn't add the card");
            cardBtnText.text = "couldn't own card";
        }
        else
        {
            cardBtnText.text = "Card Owned";

            // now make sure this button is not interactive anymore
            ownCardBtn.interactable = false;

            // so since the card is added, call the method where we get the realtime data
            // since the data is changed again so we need to call it again
            PlayfabController.Instance.GetAllCardsAtOnce();
        }
    }

    public void OnCardAdded()
    {
        // since the card is added
        //this.cardBtnText.text = "Card owned";
        this.ownCardBtn.interactable = false;

        if (thisCardIsAlreadyOwned)
        {
            // change the optiom
            this.cardBtnText.text = "Upgrade Done";
        }
        else
        {
            // leave the option as it is
            this.cardBtnText.text = "Card owned";
        }


        // since the card is owned/added here, use DOTWEEN to slightly animate the card
        // Create a new sequence
        DG.Tweening.Sequence cardSequence = DOTween.Sequence();

        // Rotate the card 2 times
        cardSequence.Append(cardImage.transform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360).SetLoops(2, LoopType.Restart));

        // Flip the card horizontally
        cardSequence.Append(cardImage.transform.DOScaleX(-1f, 0.5f).SetEase(Ease.OutQuad));

        // Optionally, return to original size horizontally
        cardSequence.Append(cardImage.transform.DOScaleX(1f, 0.2f).SetEase(Ease.InQuad));


        // now just instantiate the burstParticle
        var part = Instantiate(this.burstParticle, this.transform);

        Destroy(part, 2f);


    }

}
