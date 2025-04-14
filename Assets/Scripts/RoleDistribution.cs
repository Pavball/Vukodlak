using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoleDistribution : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public GameObject scrollViewContent;
    public GameObject nameRolePrefab;
    public string errorMessageTXT;

    private List<GameObject> roleEntries = new List<GameObject>();
    private List<GameObject> originalOrder = new List<GameObject>();
    private List<string> recurringNamesList = new List<string>();

    private PopupManager popupManager;
    private GameManager gameManager;
    private PhaseManager phaseManager;
    private bool checkForErrors = false;

    private void Start()
    {
        popupManager = GameObject.FindGameObjectWithTag("PopupManager").GetComponent<PopupManager>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        phaseManager = GameObject.FindGameObjectWithTag("PhaseManager").GetComponent<PhaseManager>();
    }

    public void GenerateRoles()
    {
        // Destroys already assigned roles
        foreach (Transform child in phaseManager.alivePlayerScrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in phaseManager.alivePlayerScrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }

        roleEntries.Clear();
        originalOrder.Clear();
        recurringNamesList.Clear();
        errorMessageTXT = string.Empty;

        string inputText = nameInputField.text;
        string[] namesArray = inputText.ToLower().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        List<string> namesList = namesArray.ToList();

        //Check if there are same names
        for (int i = 0; i < namesList.Count; i++)
        {
            for (int j = 0; j < namesList.Count; j++)
            {
                if (i != j)
                {
                    if (namesList[i].Equals(namesList[j]))
                    {
                        recurringNamesList.Add(namesList[i]);
                    }
                }
            }
        }

        //Distinct the same names
        recurringNamesList = recurringNamesList.Distinct().ToList();

        //Check For Errors (Recurring Names or Bad Player Count)
        if (recurringNamesList.Count == 0 && namesList.Count > 9 && namesList.Count < 21)
        {
            checkForErrors = false;
        }
        else if (recurringNamesList.Count > 0 && namesList.Count <= 9 || namesList.Count >= 21)
        {
            checkForErrors = true;
            errorMessageTXT = "Player count must be greater than 9 and lesser than 21. \r\n-------------------------------\r\nSame names:";
            foreach (string ocurringName in recurringNamesList)
            {
                errorMessageTXT += " " + ocurringName + ", ";
            }

            popupManager.ShowError(errorMessageTXT);
        }

        const string errorSeparator = "\r\n-------------------------------\r\n";
        errorMessageTXT = string.Empty;

        if (namesList.Count <= 9 || namesList.Count >= 21)
        {
            checkForErrors = true;
            errorMessageTXT += "Player count must be greater than 9 and lesser than 21.";
            popupManager.ShowError(errorMessageTXT);
        }

        if (recurringNamesList.Count != 0)
        {
            checkForErrors = true;
            errorMessageTXT += errorSeparator;
            errorMessageTXT += "Same names:";

            int counter = 0;
            foreach (string ocurringName in recurringNamesList)
            {
                errorMessageTXT += " " + ocurringName;
                counter++;
                if (counter != recurringNamesList.Count)
                {
                    errorMessageTXT += ", ";
                }
                
            }
            popupManager.ShowError(errorMessageTXT);
        }

        if (!checkForErrors)
        {
            // Get randomized roles
            List<string> fetchedRolesPreset = Role.ShufflePresetByPlayerCount(namesList.Count);

            // For each name, assign a role
            int fetchedRoleIndex = 0;
            Dictionary<string, Role> pairedPlayerNameWithRoleDictionary = new Dictionary<string, Role>();

            foreach (string name in namesList)
            {
                string role = fetchedRolesPreset[fetchedRoleIndex];
                Role selectedRole = Role.GetRole(role);
                pairedPlayerNameWithRoleDictionary.Add(name, selectedRole);
                fetchedRoleIndex++;
            }

            // Sort dictionary by faction and role name
            // Sort dictionary by faction and role name
            //var pairedPlayerNameWithRoleList = pairedPlayerNameWithRoleDictionary.ToList();
            //pairedPlayerNameWithRoleList.Sort((pair1, pair2) => pair1.Value.RoleFaction.CompareTo(pair2.Value.RoleFaction));
            //pairedPlayerNameWithRoleList.Sort((pair1, pair2) => pair1.Value.RoleName.CompareTo(pair2.Value.RoleName));

            var dictionarySortedByRoleFactionAndName = pairedPlayerNameWithRoleDictionary
            .OrderBy(x => x.Value.RoleFaction)
            .ThenBy(x => x.Value.RoleName)
            .ToDictionary(x => x.Key, x => x.Value);

            // Final assignment and adding to ScrollView
            foreach (var pair in dictionarySortedByRoleFactionAndName)
            {
                string role = pair.Value.RoleName;
                Role selectedRole = Role.GetRole(role);
                Color32 roleColor = selectedRole.RoleColor;
                string hexColor = ColorUtility.ToHtmlStringRGB(roleColor);

                // Instantiate role template
                GameObject newRoleEntry = Instantiate(nameRolePrefab, phaseManager.alivePlayerScrollViewContent.transform);

                // Get role name text component and set text
                TMP_Text roleNameText = newRoleEntry.transform.Find("PlayerRolePairTXT").GetComponent<TMP_Text>();
                roleNameText.text = $"{pair.Key}: <color=#{hexColor}>{role}</color>";

                Button sendToGraveyardOrBackBTN = newRoleEntry.transform.Find("ChangeStateOfDeathBTN").GetComponent<Button>();
                sendToGraveyardOrBackBTN.GetComponentInChildren<TMP_Text>().text = "X";
                sendToGraveyardOrBackBTN.onClick.AddListener(() => MoveRoleEntry(newRoleEntry));

                roleEntries.Add(newRoleEntry);
                originalOrder.Add(newRoleEntry);
            }

            gameManager.ChangeToGamemodeUI(8);
            phaseManager.InitializeGame();
        }
    }

    void MoveRoleEntry(GameObject roleEntry)
    {

        if (roleEntry.transform.parent == phaseManager.alivePlayerScrollViewContent.transform)
        {
            roleEntry.transform.SetParent(phaseManager.graveyardScrollViewContent.transform);
            roleEntry.transform.Find("ChangeStateOfDeathBTN").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "↑";
            if (phaseManager.currentPhase == PhaseManager.Phase.Voting)
            {
                phaseManager.CalculateVotesNeeded();
            }
            phaseManager.UpdateAliveDeadPlayerCount();
        }
        else
        {
            roleEntry.transform.SetParent(phaseManager.alivePlayerScrollViewContent.transform);
            roleEntry.transform.Find("ChangeStateOfDeathBTN").GetComponent<Button>().GetComponentInChildren<TMP_Text>().text = "X";
            if (phaseManager.currentPhase == PhaseManager.Phase.Voting)
            {
                phaseManager.CalculateVotesNeeded();
            }
            phaseManager.UpdateAliveDeadPlayerCount();
            SortToOriginalOrder();
        }
    }

    void SortToOriginalOrder()
    {
        foreach (var roleEntry in originalOrder)
        {
            if (roleEntry.transform.parent == phaseManager.alivePlayerScrollViewContent.transform)
            {
                roleEntry.transform.SetSiblingIndex(originalOrder.IndexOf(roleEntry));
            }
        }
    }

    public void ClearTemplateClassic()

    {
        foreach (Transform child in phaseManager.alivePlayerScrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in phaseManager.graveyardScrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }

        roleEntries.Clear();
        originalOrder.Clear();
    }
}
