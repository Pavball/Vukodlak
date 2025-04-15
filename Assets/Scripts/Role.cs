using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Role : MonoBehaviour
{
    public enum RoleTypeEnum
    {
        Good,
        Evil,
        Neutral
    }

    public string RoleName { get; private set; }
    public string RoleFaction { get; private set; }
    public string RoleDescription { get; private set; }
    public string RoleGoal { get; private set; }

    public RoleTypeEnum RoleType { get; private set; }

    public Color32 RoleColor { get; private set; }

    public bool RoleIsDead { get; private set; }

    public Role(string RoleName, string RoleFaction, string RoleDescription, string RoleGoal, RoleTypeEnum RoleType, Color32 RoleColor, bool RoleIsDead)
    {
      this.RoleName = RoleName;
      this.RoleFaction = RoleFaction;
      this.RoleDescription = RoleDescription;
      this.RoleGoal = RoleGoal;
      this.RoleType = RoleType;
      this.RoleColor = RoleColor;
      this.RoleIsDead = RoleIsDead;
    }

    // Static constructor to initialize roleDictionary
    static Role()
    {
        Color32 colorWhite = new Color32(255, 255, 255, 255);
        Color32 colorGreen = new Color32(31, 232, 30, 255);
        Color32 colorBrown = new Color32(171, 115, 36, 255);
        Color32 colorPinky = new Color32(255, 0, 203, 255);
        Color32 colorCyan = new Color32(15, 170, 165, 255);
        Color32 colorGold = new Color32(198, 157, 12, 255);
        Color32 colorSilver = new Color32(177, 177, 177, 255);
        Color32 colorDarkGreen = new Color32(28, 67, 12, 255);
        Color32 colorDarkBlue = new Color32(24, 29, 173, 255);
        Color32 colorPinkysh = new Color32(201, 119, 195, 255);

        roleDictionary = new Dictionary<string, Role>()
        {
            {"Villager", new Role("Villager", "Village", "Villager has to help the village win!", "Goal: You win with the village. Lynch every other faction.", RoleTypeEnum.Good, colorGreen, false)},
            {"Werewolf", new Role("Werewolf", "Werewolf", "Decide with your team (or alone if you are the only werewolf alive) who will you kill each night.", "Goal: Kill all other factions except the witch and be the only faction alive.", RoleTypeEnum.Evil, colorBrown, false)},
            {"Witch", new Role("Witch", "Evil Neutral", "Control one player each night (you find out his role next night). Decide who will your controlled victim visit.", "Goal: Live and win with the werewolf faction.", RoleTypeEnum.Neutral, Color.magenta, false)},
            {"Lover", new Role("Lover", "Village", "On the first night he picks a target to be his lover. The picked person will know that she got picked. If one of the lovers dies, the other one dies also from a broken heart. The lover can protect his target once.", "Goal: You win with the village. Lynch every other faction.", RoleTypeEnum.Good, colorPinky, false)},
            {"Doctor", new Role("Doctor", "Village", "Heal one person each night. If you save someone from an attack, you will know it.", "Goal: You win with the village. Lynch every other faction.", RoleTypeEnum.Good, colorCyan, false)},
            {"Mayor", new Role("Mayor", "Village", "Can reveal during the day. When revealed, his vote is worth 3 votes.", "Goal: You win with the village. Lynch every other faction.", RoleTypeEnum.Good, colorGold, false)},
            {"Sheriff", new Role("Sheriff", "Village", "You can investigate someone each night to see if they are a werewolf.", "Goal: You win with the village. Lynch every other faction.", RoleTypeEnum.Good, colorSilver, false)},
            {"Deputy", new Role("Deputy", "Village", "Has 1 bullet. He can't shoot on the first day. If he shoots a villager he pulls the trigger on himself due to the guilt.", "Goal: You win with the village. Lynch every other faction.", RoleTypeEnum.Good, colorDarkGreen, false)},
            {"Serial Killer", new Role("Serial Killer", "Evil Killing", "Can kill one person each night. Is immune to werewolf attack.", "Goal: You win on your own. Lynch every other faction.", RoleTypeEnum.Neutral, colorDarkBlue, false)},
            {"Jester", new Role("Jester", "Evil Neutral", "Get yourself lynched by any means necessary.", "Goal: You win on your own. Lynch yourself.", RoleTypeEnum.Neutral, colorPinkysh, false)}
        };
        var jsonRoles = JsonRoleReader.LoadRolesFromFile();
        roleDictionary.Add(jsonRoles);
    }

    // Dictionary to store role objects
    public static Dictionary<string, Role> roleDictionary;

    // Method to get a role from the dictionary by its key
    public static Role GetRole(string key)
    {
        if (roleDictionary.ContainsKey(key))
        {
            return roleDictionary[key];
        }
        else
        {
            Debug.LogError("Role key not found: " + key);
            return null;
        }
    }

    public static List<string> ShufflePresetByPlayerCount(int playerCount)
    {

        List<string> fetchedPresetList = FetchPresetByPlayerCount(playerCount);

        return Shuffle(fetchedPresetList);
    }



    private static System.Random rng = new System.Random();
    public static List<string> Shuffle(List<string> list)
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


    public static List<string> FetchPresetByPlayerCount(int playerCount)
    {

        switch (playerCount)
        {
            case 10:
                return new List<string> { "Werewolf", "Werewolf", "Doctor", "Mayor", "Lover", "Sheriff", "Jester", "Villager", "Villager", "Villager" };

            case 11:
                return new List<string> { "Werewolf", "Werewolf", "Doctor", "Mayor", "Lover",  "Sheriff", "Jester", "Villager", "Villager", "Villager", "Villager" };

            case 12:
                return new List<string> { "Werewolf", "Werewolf", "Werewolf", "Doctor", "Mayor", "Lover", "Sheriff", "Deputy", "Jester", "Villager", "Villager" , "Villager" };

            case 13:
                return new List<string> { "Werewolf", "Werewolf", "Werewolf", "Doctor", "Mayor", "Lover", "Sheriff", "Deputy", "Jester", "Villager", "Villager", "Villager", "Villager" };

            case 14:
                return new List<string> { "Werewolf", "Werewolf", "Werewolf", "Doctor", "Mayor", "Lover", "Sheriff", "Deputy", "Witch", "Jester", "Villager", "Villager", "Villager", "Villager"};

            case 15:
                return new List<string> { "Werewolf", "Werewolf", "Werewolf", "Doctor", "Mayor", "Lover", "Sheriff", "Deputy", "Witch", "Jester", "Villager", "Villager", "Villager", "Villager", "Villager" };

            case 16:
                return new List<string> { "Werewolf", "Werewolf", "Werewolf", "Doctor", "Mayor", "Lover", "Sheriff", "Deputy", "Deputy", "Witch", "Serial Killer", "Jester", "Villager", "Villager", "Villager", "Villager" };

            case 17:
                return new List<string> { "Werewolf", "Werewolf", "Werewolf", "Werewolf", "Doctor", "Mayor", "Lover", "Sheriff", "Deputy", "Deputy", "Witch", "Serial Killer", "Jester", "Villager", "Villager", "Villager", "Villager" };

            case 18:
                return new List<string> { "Werewolf", "Werewolf", "Werewolf", "Werewolf", "Doctor", "Mayor", "Lover", "Sheriff", "Deputy", "Deputy", "Witch", "Serial Killer", "Jester", "Villager", "Villager", "Villager", "Villager", "Villager" };

            case 19:
                return new List<string> { "Werewolf", "Werewolf", "Werewolf", "Werewolf", "Doctor", "Mayor", "Lover", "Sheriff", "Deputy", "Deputy", "Witch", "Serial Killer", "Serial Killer", "Jester", "Villager", "Villager", "Villager", "Villager", "Villager"};

            case 20:
                return new List<string> { "Werewolf", "Werewolf", "Werewolf", "Werewolf", "Doctor", "Mayor", "Lover", "Sheriff", "Deputy", "Deputy", "Witch", "Serial Killer", "Serial Killer", "Jester", "Villager", "Villager", "Villager", "Villager", "Villager", "Villager" };

            default:
                GameObject.FindGameObjectWithTag("RoleDistribution").GetComponent<RoleDistribution>().errorMessageTXT = "Player count must be greater than 9 and lesser than 21.";
                throw new ArgumentOutOfRangeException(nameof(playerCount), "Player count must be greater than 9 and lesser than 21.");

        }
    }



}
