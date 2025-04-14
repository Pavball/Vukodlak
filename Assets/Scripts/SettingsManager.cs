using System.Collections;
using TMPro;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public TMP_Dropdown dayDurationDropdownMenu;
    public TMP_Dropdown votingDurationDropdownMenu;
    public TMP_Dropdown explanationDurationDropdownMenu;

    public int dayDuration;
    public int votingDuration;
    public int explanationDuration;

    // Keys for PlayerPrefs
    private const string DayDropdownIndexKey = "DayDurationDropdownIndex";
    private const string NightDropdownIndexKey = "NightDurationDropdownIndex";
    private const string ExplanationDropdownIndexKey = "ExplanationDurationDropdownIndex";

    void Start()
    {
        // Load saved indices for all dropdowns
        LoadDropdownIndex(dayDurationDropdownMenu, DayDropdownIndexKey, ref dayDuration);
        LoadDropdownIndex(votingDurationDropdownMenu, NightDropdownIndexKey, ref votingDuration);
        LoadDropdownIndex(explanationDurationDropdownMenu, ExplanationDropdownIndexKey, ref explanationDuration);

        // Add listeners for each dropdown
        dayDurationDropdownMenu.onValueChanged.AddListener(delegate { OnDropdownValueChanged(dayDurationDropdownMenu, DayDropdownIndexKey, ref dayDuration); });
        votingDurationDropdownMenu.onValueChanged.AddListener(delegate { OnDropdownValueChanged(votingDurationDropdownMenu, NightDropdownIndexKey, ref votingDuration); });
        explanationDurationDropdownMenu.onValueChanged.AddListener(delegate { OnDropdownValueChanged(explanationDurationDropdownMenu, ExplanationDropdownIndexKey, ref explanationDuration); });
    }

    void OnEnable()
    {
        StartCoroutine(LoadDropdownIndexCoroutine(dayDurationDropdownMenu, DayDropdownIndexKey));
        StartCoroutine(LoadDropdownIndexCoroutine(votingDurationDropdownMenu, NightDropdownIndexKey));
        StartCoroutine(LoadDropdownIndexCoroutine(explanationDurationDropdownMenu, ExplanationDropdownIndexKey));
    }

    // Method called when a dropdown value changes
    public void OnDropdownValueChanged(TMP_Dropdown dropdown, string prefsKey, ref int duration)
    {
        // Save the selected index to PlayerPrefs
        PlayerPrefs.SetInt(prefsKey, dropdown.value);
        PlayerPrefs.Save();

        string selectedText = dropdown.options[dropdown.value].text;
        string numberPart = selectedText.Split(' ')[0];

        if (int.TryParse(numberPart, out int parsedDuration))
        {
            duration = parsedDuration;
            Debug.Log($"Duration set to: {duration} seconds for {prefsKey}");
        }
        else
        {
            Debug.LogWarning($"Failed to parse duration from: {selectedText}");
        }
    }

    private void LoadDropdownIndex(TMP_Dropdown dropdown, string prefsKey, ref int duration)
    {
        if (PlayerPrefs.HasKey(prefsKey))
        {
            int savedIndex = PlayerPrefs.GetInt(prefsKey);
            if (savedIndex >= 0 && savedIndex < dropdown.options.Count)
            {         
                string selectedText = dropdown.options[savedIndex].text;
                string numberPart = selectedText.Split(' ')[0];

                if (int.TryParse(numberPart, out int parsedDuration))
                {
                    duration = parsedDuration; // Set the duration variable
                    Debug.Log($"Duration set to: {duration} seconds for {prefsKey}");
                }
                else
                {
                    Debug.LogWarning($"Failed to parse duration from: {selectedText}");
                }

                dropdown.RefreshShownValue(); // Refresh to display the selected value
                Debug.Log($"Loaded saved dropdown index for {prefsKey}: {savedIndex}");
            }
            else
            {
                Debug.LogWarning($"Saved dropdown index for {prefsKey} is out of range. Resetting to default.");
            }
        }
        else
        {
            Debug.Log($"No saved dropdown index found for {prefsKey}. Using default value.");
            string selectedText = dropdown.options[0].text;
            string numberPart = selectedText.Split(' ')[0];

            if (int.TryParse(numberPart, out int parsedDuration))
            {
                duration = parsedDuration; // Set the duration variable
                Debug.Log($"Duration set to: {duration} seconds for {prefsKey}");
            }
            else
            {
                Debug.LogWarning($"Failed to parse duration from: {selectedText}");
            }
        }
    }

    private IEnumerator LoadDropdownIndexCoroutine(TMP_Dropdown dropdown, string prefsKey)
    {
        // Wait for the end of the frame to ensure the dropdown is fully initialized
        yield return new WaitForEndOfFrame();

        if (PlayerPrefs.HasKey(prefsKey))
        {
            int savedIndex = PlayerPrefs.GetInt(prefsKey);
            if (savedIndex >= 0 && savedIndex < dropdown.options.Count)
            {
                dropdown.value = savedIndex;
                dropdown.RefreshShownValue(); // Refresh to display the selected value
            }
            else
            {
                Debug.LogWarning($"Saved dropdown index for {prefsKey} is out of range. Resetting to default.");
            }
        }
        else
        {
            Debug.Log($"No saved dropdown index found for {prefsKey}. Using default value.");
        }
    }

}
