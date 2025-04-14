using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro; // Import TextMeshPro namespace
using UnityEngine;
using UnityEngine.UI;

public class CustomRoleManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button generateRolesButton;
    public GameObject roleCustomizingScrollViewContent;
    public GameObject roleTemplatePrefab;
    public GameObject nameRolePrefab;
    public TMP_Text rolesLeftToAssignCountTXT;
    private string errorMessageTXT;

    private Dictionary<string, int> roleCountsDictionary = new Dictionary<string, int>();
    private int totalRolesCount;

    // Lists for storing roles and names
    public List<string> rolesToGivePlayers = new List<string>();
    public List<string> randomizedRolesToGivePlayers = new List<string>();
    public List<string> namesList;

    public List<GameObject> buttonsList = new List<GameObject>();
    private List<GameObject> roleEntries = new List<GameObject>();
    private List<GameObject> originalOrder = new List<GameObject>();
    private List<string> recurringNamesList = new List<string>();
    private bool checkForErrors = false;

    private PopupManager popupManager;
    private GameManager gameManager;
    private PhaseManager phaseManager;

    void Start()
    {
        generateRolesButton.onClick.AddListener(GenerateRoles);
        popupManager = GameObject.FindGameObjectWithTag("PopupManager").GetComponent<PopupManager>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        phaseManager = GameObject.FindGameObjectWithTag("PhaseManager").GetComponent<PhaseManager>();
    }

    void GenerateRoles()
    {
        // Clear existing content
        foreach (Transform child in roleCustomizingScrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }

        recurringNamesList.Clear();

        // Get names from the input field
        string inputText = inputField.text;
        namesList = inputText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        totalRolesCount = namesList.Count;

        // Check for duplicate names
        for (int i = 0; i < namesList.Count; i++)
        {
            for (int j = 0; j < namesList.Count; j++)
            {
                if (i != j && namesList[i].Equals(namesList[j]))
                {
                    recurringNamesList.Add(namesList[i]);
                }
            }
        }

        recurringNamesList = recurringNamesList.Distinct().ToList();

        if (recurringNamesList.Count > 0)
        {
            checkForErrors = true;
            errorMessageTXT = "Duplicate names found: " + string.Join(", ", recurringNamesList);
            popupManager.ShowCustomError(errorMessageTXT);
        }
        else
        {
            checkForErrors = false;
        }

        if (!checkForErrors)
        {
            List<string> roles = new List<string>(Role.roleDictionary.Keys);

            foreach (string role in roles)
            {
                GameObject newRoleEntry = Instantiate(roleTemplatePrefab, roleCustomizingScrollViewContent.transform);
                TMP_Text roleNameText = newRoleEntry.transform.Find("RoleNameText").GetComponent<TMP_Text>();

                Role selectedRole = Role.GetRole(role);
                Color32 roleColor = selectedRole.RoleColor;
                string hexColor = ColorUtility.ToHtmlStringRGB(roleColor);

                roleNameText.text = $"<color=#{hexColor}>{role}</color>";

                roleCountsDictionary[role] = 0;
                TMP_Text roleCountText = newRoleEntry.transform.Find("RoleCountText").GetComponent<TMP_Text>();
                roleCountText.text = roleCountsDictionary[role].ToString();

                Button incrementButton = newRoleEntry.transform.Find("IncrementButton").GetComponent<Button>();
                Button decrementButton = newRoleEntry.transform.Find("DecrementButton").GetComponent<Button>();

                incrementButton.onClick.AddListener(() => IncrementRoleCount(role, roleCountText));
                decrementButton.onClick.AddListener(() => DecrementRoleCount(role, roleCountText));
            }

            // Start the coroutine to wait before fetching the buttons
            StartCoroutine(WaitAndFetchButtons());

            // Change to the next UI screen
            gameManager.ChangeToGamemodeUI(4);
        }
    }

    IEnumerator WaitAndFetchButtons()
    {
        Canvas.ForceUpdateCanvases();
        yield return new WaitForEndOfFrame();

        // Continuously check until buttons are found
        while (buttonsList.Count == 0)
        {
            buttonsList = roleCustomizingScrollViewContent.GetComponentsInChildren<Button>(true)
                .Where(button => button.CompareTag("IncBtn"))
                .Select(button => button.gameObject)
                .ToList();

            yield return null; // Wait for the next frame
        }

        UpdateRemainingRolesText();
    }


    void IncrementRoleCount(string role, TMP_Text roleCountText)
    {
        int assignedRolesCount = 0;
        foreach (var count in roleCountsDictionary.Values)
        {
            assignedRolesCount += count;
        }

        int remainingRoles = totalRolesCount - assignedRolesCount;

        if (remainingRoles > 0)
        {
            roleCountsDictionary[role]++;
            roleCountText.text = roleCountsDictionary[role].ToString();
            UpdateRemainingRolesText();
        }
        else
        {
            Debug.LogWarning("No remaining roles to assign.");
            UpdateRemainingRolesText();
        }
    }

    void DecrementRoleCount(string role, TMP_Text roleCountText)
    {
        if (roleCountsDictionary[role] > 0)
        {
            roleCountsDictionary[role]--;
            roleCountText.text = roleCountsDictionary[role].ToString();
            UpdateRemainingRolesText();
        }
    }

    void UpdateRemainingRolesText()
    {
        int assignedRolesCount = 0;
        foreach (var count in roleCountsDictionary.Values)
        {
            assignedRolesCount += count;
        }
        int remainingRoles = totalRolesCount - assignedRolesCount;

        rolesLeftToAssignCountTXT.text = "Remaining: " + remainingRoles.ToString();

        if (remainingRoles == 0)
        {
            foreach (var button in buttonsList)
            {
                button.GetComponent<Button>().interactable = false;
                GameObject.FindGameObjectWithTag("RandomizeBtn").GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            foreach (var button in buttonsList)
            {
                button.GetComponent<Button>().interactable = true;
                GameObject.FindGameObjectWithTag("RandomizeBtn").GetComponent<Button>().interactable = false;
            }
        }
    }


    //PART OF THE CODE WHERE THE ROLES ARE RANDOMIZED AND GIVEN TO PEOPLE AFTER THE USER HAS SET UP WHICH ROLES THEY WANNA USE
    public void RandomizeRoles()
    {
        rolesToGivePlayers.Clear();
        roleEntries.Clear();
        originalOrder.Clear();

        foreach (var key in roleCountsDictionary.Keys)
        {
            for (int i = 0; i < roleCountsDictionary[key]; i++)
            {
                rolesToGivePlayers.Add(key);
            }
        }

        randomizedRolesToGivePlayers = ShuffleCustomPlayerRoles(rolesToGivePlayers);

        // For each name, assign a role
        int fetchedRoleIndex = 0;
        Dictionary<string, Role> pairedPlayerNameWithRoleDictionary = new Dictionary<string, Role>();

        foreach (string name in namesList)
        {
            string role = randomizedRolesToGivePlayers[fetchedRoleIndex];
            Role selectedRole = Role.GetRole(role);
            pairedPlayerNameWithRoleDictionary.Add(name, selectedRole);
            fetchedRoleIndex++;
        }

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


    //Sorting and randomizing algorithms
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

    private static System.Random rng = new System.Random();
    public static List<string> ShuffleCustomPlayerRoles(List<string> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            string value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

        return list;
    }

    //Clear everything
    public void ClearTemplateButton()
    {

        buttonsList.Clear();
        // Clear existing content
        foreach (Transform child in roleCustomizingScrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }

    }

    public void ClearTemplateTable()
    {
        // Clear existing content
        foreach (Transform child in phaseManager.alivePlayerScrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in phaseManager.graveyardScrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }

    }

    //public void ReadDictionary()
    //{
    //    int i = 1;
    //    foreach(string role in roleCountsDictionary.Keys)
    //    {

    //        Debug.Log(i + "# Role key: " + role);
    //        i++;
    //    }

    //    int j = 1;
    //    foreach (int role in roleCountsDictionary.Values)
    //    {

    //        Debug.Log(j + "# Role value: " + role);
    //        j++;
    //    }

    //}
}
