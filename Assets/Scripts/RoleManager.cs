using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoleManager : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Dropdown dropdownMenu;
    public TMP_Text roleNameTXT;
    public TMP_Text roleFactionTXT;
    public TMP_Text roleDescTXT;
    public TMP_Text roleGoalTXT;

    private Role roleScript;
    private GameManager gameManager;
    void Start()
    {
        // Find the GameObject with the Role script attached
        GameObject roleObject = GameObject.FindGameObjectWithTag("Role");
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();


        if (roleObject != null)
        {
            // Get the Role component from the GameObject
            roleScript = roleObject.GetComponent<Role>();

            // Add listener for dropdown value changed event
            dropdownMenu.onValueChanged.AddListener(delegate {
                DropdownValueChanged(dropdownMenu);
            });
        }
        else
        {
            Debug.LogError("Role GameObject not found in the scene.");
        }

    }

    void DropdownValueChanged(TMP_Dropdown dropdown)
    {
        gameManager.scrollbar.value = 1f;

        string selectedRoleName = dropdown.options[dropdown.value].text;

        Role selectedRole = Role.GetRole(selectedRoleName);

        // Update UI with role details

        switch (selectedRole.RoleType)
        {
            case Role.RoleTypeEnum.Good:
                Color32 colorGreen = new Color32(31, 232, 30, 255);
                roleFactionTXT.color = colorGreen;
                break;

            case Role.RoleTypeEnum.Evil:
                Color32 colorRed = new Color32(232, 31, 31, 255);
                roleFactionTXT.color = colorRed;
                break;

            case Role.RoleTypeEnum.Neutral:
                Color32 colorWhite = new Color32(255, 255, 255, 255);
                roleFactionTXT.color = colorWhite;
                break;
            default:
                // code block
                break;
        }

        //Sets Role Color
        roleNameTXT.color = selectedRole.RoleColor;
        roleNameTXT.text = "Role: " + selectedRole.RoleName;

        roleFactionTXT.text = "Faction: " + selectedRole.RoleFaction;
        roleDescTXT.text = selectedRole.RoleDescription;
        roleGoalTXT.text = selectedRole.RoleGoal;
    }
}
