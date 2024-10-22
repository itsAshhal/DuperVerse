using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class CategoriesLoader : MonoBehaviour
{
    [SerializeField] UI_MainMenu mainMenu;

    [SerializeField] Button categoryBtn;
    [SerializeField] GameObject dropDownObject;
    [SerializeField] Transform content;
    [SerializeField] GameObject categoryContentPrefab;
    [SerializeField] TMP_Text _mainCategoryText;

    private void Start()
    {
        dropDownObject.SetActive(false);
        DestroyAllChildren();
        InitBtn();
        InitUI();
    }

    private void DestroyAllChildren()
    {
        foreach (Transform i in content)
        {
            Destroy(i.gameObject);
        }

        //GameObject.FindObjectOfType<UI_MainMenu>().InstantiateAllCards();
    }

    private void InitBtn()
    {
        categoryBtn.onClick.RemoveAllListeners();
        categoryBtn.onClick.AddListener(() =>
        {
            if (dropDownObject.activeInHierarchy)
                dropDownObject.SetActive(false);
            else
                dropDownObject.SetActive(true);
        });
    }

    private void InitUI()
    {
        foreach (CardCategory value in Enum.GetValues(typeof(CardCategory)))
        {
            GameObject btn = Instantiate(categoryContentPrefab, content);

            btn.GetComponentInChildren<TMP_Text>().text = value.ToString();
            btn.gameObject.name = value.ToString();
            btn.GetComponent<Button>().onClick.RemoveAllListeners();
            btn.GetComponent<Button>().onClick.AddListener(() => { mainMenu.ShowCategorySpecificCards(value); });
            var newComponent = btn.AddComponent<CategoryButton>();
            newComponent.MainCategoryTitle = this._mainCategoryText;
        }

        //  foreach (Transform i in content)
        // {
        //     Destroy(i.gameObject);
        // }

        //GameObject.FindObjectOfType<UI_MainMenu>().InstantiateAllCards();
    }




}
