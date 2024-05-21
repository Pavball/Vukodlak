using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RoleDistribution : MonoBehaviour
{

    public TMP_InputField nameInputField;
    public GameObject scrollViewContent;
    public GameObject nameRolePrefab;
    public TMP_Text errorMessageTXT;


    public void GenerateRoles()
    {
        //Destroys already assigned roles
        List<GameObject> assignedRolesToDestroy = new List<GameObject>(GameObject.FindGameObjectsWithTag("AssignedRoles"));
        if(assignedRolesToDestroy.Count > 0)
        {
            for (int i = 0; i < assignedRolesToDestroy.Count; i++)
            {
                Destroy(assignedRolesToDestroy[i]);
            }
        }
        

        string inputText = nameInputField.text;

        string[] namesArray = inputText.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        List<string> namesList = namesArray.ToList();

        //Get randomized roles
        List<string> fetchedRolesPreset = Role.ShufflePresetByPlayerCount(namesList.Count);

        //For each that assignes roles to each name
        int roleIndex = 0;
       

        foreach (string name in namesList)
        {
            string role = fetchedRolesPreset[roleIndex];

            Role selectedRole = Role.GetRole(role);
            Color32 roleColor = selectedRole.RoleColor;
            string hexColor = UnityEngine.ColorUtility.ToHtmlStringRGB(roleColor);

            roleIndex++;
            GameObject newEntry = Instantiate(nameRolePrefab, scrollViewContent.transform);
            TMP_Text entryText = newEntry.GetComponent<TMP_Text>();
            if (entryText != null)
            {
                entryText.text = $"{name}: <color=#{hexColor}>{role}</color>";
                Debug.Log(entryText.text);
            }
        }

        errorMessageTXT.gameObject.SetActive(false);

    }



}
