using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;
public enum CardCategory
{
    Ancient,
    Anomaly,
    Entity,
    Evil,
    Freak,
    Lab,
    Maniac,
    Mystic,
    Outsider,
    Wild
}

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/CardSO", order = 1)]
public class CardSO : ScriptableObject
{
    public string cardName = string.Empty;
    public CardCategory category;
    public string cardCategoryName;
    public Sprite frontSprite;
    public Sprite backSprite;
    // stats system
    public float strength = 0f;
    public float speed = 0f;
    public float intelligence = 0f;
    public float fight = 0f;
    public float stamina = 0f;
    public float strange = 0f;

    [Tooltip("So when we spawn different owned cards of the user, we can spawn matching lowerBackgrounds as well")]
    public Sprite LowerBackground;


    [Header("Buying Categories")]
    public string BuyButtonURL = string.Empty;
    public string ProductID = string.Empty;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(cardName))
        {
            cardName = this.name;
        }

    }
}

[Serializable]
class CardSerializable
{
    public string _name;
    public float strength = 0f;
    public float speed = 0f;
    public float intelligence = 0f;
    public float fight = 0f;
    public float stamina = 0f;
    public float strange = 0f;

    //public string _cardCategory;
}
[Serializable]
class CardListSerializable
{
    public List<CardSerializable> _cards = new List<CardSerializable>();
}
public class CardsJSON_Manager
{
    public static string ToJSON(List<CardSO> cardList)
    {
        CardListSerializable cardListSer = new();
        foreach (var item in cardList)
        {
            cardListSer._cards.Add(new CardSerializable
            {
                _name = item.cardName,
                strength = item.strange,
                speed = item.speed,
                intelligence = item.intelligence,
                fight = item.fight,
                stamina = item.stamina,
                strange = item.strange
                //_cardCategory = item.category.ToString(),

            });

        }

        return JsonUtility.ToJson(cardListSer);
    }
    public static List<CardSO> FromJSON(string json)
    {
        CardListSerializable cardListSer = new();
        cardListSer = JsonUtility.FromJson<CardListSerializable>(json);
        List<CardSO> cardSoList = new();

        foreach (var item in cardListSer._cards)
        {
            foreach (var card in GameManager.instance.allCards)
            {
                if (card.cardName.Equals(item._name))
                {
                    card.strength = item.strength;
                    card.speed = item.speed;
                    card.intelligence = item.intelligence;
                    card.fight = item.fight;
                    card.stamina = item.stamina;
                    card.strange = item.strange;
                    cardSoList.Add(card);
                    break;
                }
            }
        }

        return cardSoList;
    }
}
