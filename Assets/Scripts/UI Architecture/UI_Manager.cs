using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Analytics;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager _instance;
    public static UI_Manager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<UI_Manager>();

                if (!_instance)
                {
                    _instance = new GameObject("UI_Manager").AddComponent<UI_Manager>();
                }

            }

            return _instance;
        }
    }

    public Transform uiParent;
    Stack<GameObject> currentOpenedPanels = new Stack<GameObject>();
    public List<UI_Panel> UI_PanelPrefabs;
    public GameObject openedIndependantPanel;
    public bool stopOpenningPanels = false;
    public UI_Panel lastOpenedPanelForAll;
    public bool PanelIsAlreadyOpen = false;
    public Transform OwnedCardsParent;
    public UI_ZoomedCard ZoomedCardPrefab;
    public UI_OwnedCard OwnedCard;
    public UI_MainMenu ui_mainMenu;

    private void Awake()
    {
        Debug.Log("UIManager awake");

    }

    public UI_Panel OpenPanel(Type panelType, bool closeLastOpened = true)
    {
        if (stopOpenningPanels) return null;

        Debug.Log("NewPanel Opened");

        if (closeLastOpened && currentOpenedPanels.Count > 0)
        {
            CloseLastOpenedPanel();
        }

        GameObject newPanel = Instantiate(UI_PanelPrefabs.Find(x => x.GetComponent<UI_Panel>().GetType().Equals(panelType)).gameObject, uiParent);
        Debug.Log($"NewPanel instantiated is {newPanel.name}");

        currentOpenedPanels.Push(newPanel);

        //Debug.Log ("opening panel: " + panelType);
        if (panelType == typeof(UI_MainMenu))
        {
            Debug.Log("MainMenu Finalized");
            this.OwnedCardsParent = newPanel.GetComponent<UI_MainMenu>().ownedCardsParent;  // as this is the mainMenu prefab.
            this.ui_mainMenu = newPanel.GetComponent<UI_MainMenu>(); ;
            //PlayfabController.Instance.GetAllCardsAtOnce();
        }
        return newPanel.GetComponent<UI_Panel>();

    }


    public UI_Panel OpenIndepenedantPanel(Type panelType, bool bufferPanel = false)
    {
        if (stopOpenningPanels) return null;

        GameObject newPanel = Instantiate(UI_PanelPrefabs.Find(x => x.GetComponent<UI_Panel>().GetType().Equals(panelType)).gameObject, uiParent);


        if (bufferPanel)
            openedIndependantPanel = newPanel;
        //Debug.Log ("opening panel: " + panelType);
        return newPanel.GetComponent<UI_Panel>();

    }

    public void CloseLastOpenedIndependantPanel()
    {
        if (openedIndependantPanel != null)
        {
            Destroy(openedIndependantPanel);
        }
    }

    public void OpenPanelForTime(Type panelType, float time)
    {
        if (stopOpenningPanels) return;

        GameObject newPanel = Instantiate(UI_PanelPrefabs.Find(x => x.GetComponent<UI_Panel>().GetType().Equals(panelType)).gameObject, uiParent);

        Destroy(newPanel, time);
    }

    public void CloseLastOpenedPanel()
    {
        if (currentOpenedPanels.Count == 0) return;

        GameObject lastOpenedPanel = currentOpenedPanels.Pop();// Orginal Code
        Destroy(lastOpenedPanel);

        if (currentOpenedPanels != null && currentOpenedPanels.Count > 0)
        {
            if (currentOpenedPanels.Peek() != null)
            {
                if (currentOpenedPanels.Peek().GetComponent<UI_Panel>())
                {
                    UI_Panel currentPanel = currentOpenedPanels.Peek().GetComponent<UI_Panel>();
                    lastOpenedPanelForAll = currentPanel;
                    currentPanel.ResumePanel();
                }
            }
        }

    }

    internal void CloseLastOpenedPanel(Type type, bool v)
    {
        throw new NotImplementedException();
    }

    public void CloseAllPanels()
    {
        while (currentOpenedPanels != null && currentOpenedPanels.Count != 0)
        {
            CloseLastOpenedPanel();
        }
    }

    public UI_Panel GetLastOpenedPanel()
    {
        GameObject lastOpenedPanel = null;
        if (currentOpenedPanels != null && currentOpenedPanels.Count > 0)
        {
            lastOpenedPanel = currentOpenedPanels.Peek();
        }

        return lastOpenedPanel.GetComponent<UI_Panel>();
    }

}