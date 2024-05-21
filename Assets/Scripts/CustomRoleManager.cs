using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import TextMeshPro namespace
using System.Linq;

public class CustomRoleManager : MonoBehaviour
{
    public TMP_InputField inputField; // Change to TMP_InputField if using TextMeshPro
    public Button generateRolesButton;
    public GameObject roleCustomizingScrollViewContent;
    public GameObject finalCustomPlayerScrollViewContent;
    public GameObject roleTemplatePrefab;
    public GameObject nameRolePrefab;
    public TMP_Text rolesLeftToAssignCountTXT;

    private Dictionary<string, int> roleCountsDictionary = new Dictionary<string, int>();
    private int totalRolesCount;
    private List<GameObject> buttonsList = new List<GameObject>();

    //Lista u koja glumi preset za custom game (uzima role od broja role koje je odredio player)
    public List<string> rolesToGivePlayers = new List<string>();
    public List<string> randomizedRolesToGivePlayers = new List<string>();
    public List<string> namesList;



    void Start()
    {
        generateRolesButton.onClick.AddListener(GenerateRoles);
    }

    void GenerateRoles()
    {
        // Clear existing content
        foreach (Transform child in roleCustomizingScrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }

        // Get names from the input field
        string inputText = inputField.text;
        namesList = inputText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        totalRolesCount = namesList.Count;
        Debug.Log("Total role count: " + totalRolesCount);

        

        // Define roles (for example purposes)
        List<string> roles = new List<string>();

        foreach (string role in Role.roleDictionary.Keys)
        {
            roles.Add(role);
        }

        for (int i = 0; i < roles.Count; i++)
        {
            string role = roles[i]; // Capture the role name in a local variable

            // Instantiate role template
            GameObject newRoleEntry = Instantiate(roleTemplatePrefab, roleCustomizingScrollViewContent.transform);

            // Get role name text component and set text
            TMP_Text roleNameText = newRoleEntry.transform.Find("RoleNameText").GetComponent<TMP_Text>();
            roleNameText.text = role;

            // Initialize role count to 1
            roleCountsDictionary[role] = 0;

            // Get count text component and set text
            TMP_Text roleCountText = newRoleEntry.transform.Find("RoleCountText").GetComponent<TMP_Text>();
            roleCountText.text = roleCountsDictionary[role].ToString();

            // Get buttons and assign click listeners
            Button incrementButton = newRoleEntry.transform.Find("IncrementButton").GetComponent<Button>();
            Button decrementButton = newRoleEntry.transform.Find("DecrementButton").GetComponent<Button>();

            // Assign role name to buttons using local variables to avoid closure issues
            incrementButton.onClick.AddListener(() => IncrementRoleCount(role, roleCountText));
            decrementButton.onClick.AddListener(() => DecrementRoleCount(role, roleCountText));
        }


        buttonsList = GameObject.FindGameObjectsWithTag("IncBtn").ToList();

        UpdateRemainingRolesText();
    }

    void IncrementRoleCount(string role, TMP_Text roleCountText)
    {

        roleCountsDictionary[role]++;
        roleCountText.text = roleCountsDictionary[role].ToString();
        UpdateRemainingRolesText();
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

        
        if (remainingRoles == 0) { 

        foreach (var button in buttonsList)
        {
            
            button.gameObject.GetComponent<Button>().interactable = false;
            GameObject.FindGameObjectWithTag("RandomizeBtn").GetComponent<Button>().interactable = true;

        }

        }
        else 
        {
            foreach (var button in buttonsList)
            {

                button.gameObject.GetComponent<Button>().interactable = true;
                GameObject.FindGameObjectWithTag("RandomizeBtn").GetComponent<Button>().interactable = false;
            }
        }

        rolesLeftToAssignCountTXT.text = "Remaining: " + remainingRoles.ToString();
    }



    public void ClearTemplate() {

        buttonsList.Clear();
        // Clear existing content
        foreach (Transform child in roleCustomizingScrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }

    }


    public void RandomizeRoles()
    {
        rolesToGivePlayers.Clear();

        foreach(var key in roleCountsDictionary.Keys)
        {
            for(int i = 0; i < roleCountsDictionary[key]; i++)
            {
                rolesToGivePlayers.Add(key);
            }


        }

        foreach (var item in rolesToGivePlayers)
        {
            Debug.Log("Roles given to player: " + item);
        }

        randomizedRolesToGivePlayers = ShuffleCustomPlayerRoles(rolesToGivePlayers);

        foreach (var item in randomizedRolesToGivePlayers)
        {
            Debug.Log("Randomized roles given to player: " + item);
        }

        //For each that assignes roles to each name
        int roleIndex = 0;

        // Clear existing content
        foreach (Transform child in finalCustomPlayerScrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }


        foreach (string name in namesList)
        {
            string role = randomizedRolesToGivePlayers[roleIndex];

            Role selectedRole = Role.GetRole(role);
            Color32 roleColor = selectedRole.RoleColor;
            string hexColor = UnityEngine.ColorUtility.ToHtmlStringRGB(roleColor);

            roleIndex++;
            GameObject newEntry = Instantiate(nameRolePrefab, finalCustomPlayerScrollViewContent.transform);
            TMP_Text entryText = newEntry.GetComponent<TMP_Text>();
            if (entryText != null)
            {
                entryText.text = $"{name}: <color=#{hexColor}>{role}</color>";
                Debug.Log(entryText.text);
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
