using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI Objects")]
    public GameObject GamemodeUI;
    public GameObject ClassicGamemodeUI;
    public GameObject CustomGamemodeUI;
    public GameObject CustomTemplateUI;
    public GameObject GameTableUI;
    public GameObject RoleDescriptionUI;
    public GameObject GameDescriptionUI;
    public GameObject SettingsUI;

    public TMP_Text descriptionText;
    public TMP_Text goalText;
    public RectTransform goalRectTransform;

    private CustomRoleManager customRoleManager;
    private SettingsManager settingsManager;
    void Start()
    {
        customRoleManager = GameObject.FindGameObjectWithTag("CustomRoleManager").GetComponent<CustomRoleManager>();
        settingsManager = GameObject.FindGameObjectWithTag("SettingsManager").GetComponent<SettingsManager>();
    }

    void Update()
    {
        if (RoleDescriptionUI.activeInHierarchy)
        {
            // Get the height of the description text
            float descriptionHeight = descriptionText.preferredHeight;

            // Update the position of the goal text
            Vector2 goalPosition = goalRectTransform.anchoredPosition;
            goalPosition.y = -descriptionHeight; // Adjust this value to add some padding between description and goal text
            goalRectTransform.anchoredPosition = goalPosition;
        }
    }

    public void ChangeToGamemodeUI(int btnNum)
    {

        switch (btnNum)
        {
            case 0:
                GamemodeUI.SetActive(true);
                ClassicGamemodeUI.SetActive(false);
                CustomGamemodeUI.SetActive(false);
                customRoleManager.ClearTemplateButton();
                customRoleManager.ClearTemplateTable();
                RoleDescriptionUI.SetActive(false);
                GameTableUI.SetActive(false);
                SettingsUI.SetActive(false);
                break;
            case 1:
                GamemodeUI.SetActive(false);
                ClassicGamemodeUI.SetActive(true);
                break;
            case 2:
                GamemodeUI.SetActive(false);
                CustomGamemodeUI.SetActive(true);
                break;
            case 3:
                GamemodeUI.SetActive(false);
                RoleDescriptionUI.SetActive(true);
                break;
            case 4:
                CustomGamemodeUI.SetActive(false);
                CustomTemplateUI.SetActive(true);
                break;
            case 5:
                customRoleManager.ClearTemplateButton();
                CustomGamemodeUI.SetActive(true);
                CustomTemplateUI.SetActive(false);
                break;
            case 8:
                ClassicGamemodeUI.SetActive(false);
                CustomTemplateUI.SetActive(false);
                GameTableUI.SetActive(true);
                break;
            case 10:
                GameDescriptionUI.SetActive(true);
                GamemodeUI.SetActive(false);
                break;
            case 11:
                GamemodeUI.SetActive(true);
                GameDescriptionUI.SetActive(false);
                break;
            case 12:
                GamemodeUI.SetActive(false);
                SettingsUI.SetActive(true);
                break;
            default:
                // code block
                break;
        }
    }

}
