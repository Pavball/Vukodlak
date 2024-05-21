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
    public GameObject CustomTemplateTableUI;
    public GameObject RoleDescriptionUI;

    public Scrollbar scrollbar;
    public TMP_Text descriptionText;
    public TMP_Text goalText;
    public RectTransform goalRectTransform;

    private CustomRoleManager customRoleManager;

    void Start()
    {
        customRoleManager = GameObject.FindGameObjectWithTag("CustomRoleManager").GetComponent<CustomRoleManager>();
    }



    void Update()
    {

        if (RoleDescriptionUI.activeInHierarchy)
        {
            // Get the height of the description text
            float descriptionHeight = descriptionText.preferredHeight;

            // Update the position of the goal text
            Vector2 goalPosition = goalRectTransform.anchoredPosition;
            goalPosition.y = -descriptionHeight + 700f; // Adjust this value to add some padding between description and goal text
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
                RoleDescriptionUI.SetActive(false);
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
            //Custom Template active after Custom generating
            case 4:
                CustomGamemodeUI.SetActive(false);
                CustomTemplateUI.SetActive(true);
                break;
            case 5:
                customRoleManager.ClearTemplate();
                CustomGamemodeUI.SetActive(true);
                CustomTemplateUI.SetActive(false);
                break;
            case 6:
                CustomTemplateUI.SetActive(false);
                CustomTemplateTableUI.SetActive(true);
                break;
            case 7:
                CustomTemplateUI.SetActive(true);
                CustomTemplateTableUI.SetActive(false);
                break;
            default:
                // code block
                break;
        }


    }



}
