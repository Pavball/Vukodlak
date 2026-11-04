using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiskRoleLoader
{
    private const int LinesPerRole = 7;

    public static List<Role> LoadRolesFromFile()
    {
        // Assets/Resources/roles.txt is packaged into builds, including Android
        var rolesFile = Resources.Load<TextAsset>("roles");

        if (rolesFile == null)
        {
            Debug.LogWarning("Resources/roles.txt not found. Using built-in roles only.");
            return new List<Role>();
        }

        return CreateRolesFromText(rolesFile.text);
    }

    private static List<Role> CreateRolesFromText(string textFromFile)
    {
        var roles = new List<Role>();
        var roleList = textFromFile.Split("##");
        // ignore first item because it is a description of the file contents
        for (int i = 1; i < roleList.Length; i++)
        {
            var entry = roleList[i].Trim();
            if (entry.Length == 0)
            {
                continue;
            }

            // A bad entry only loses that role, not every role in the game
            try
            {
                // Accept any line ending: git and other editors can change them in roles.txt
                var item = entry.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                    .Select(line => line.Trim())
                    .ToArray();

                if (item.Length < LinesPerRole)
                {
                    throw new FormatException("expected " + LinesPerRole + " lines, found " + item.Length);
                }

                roles.Add(new Role(item[0], item[1], item[2], item[3], getRoleType(item[4]), getColor(item[5]), getIsDead(item[6])));
            }
            catch (Exception e)
            {
                Debug.LogWarning("Skipping roles.txt entry " + i + ": " + e.Message);
            }
        }
        return roles;
    }

    private static Role.RoleTypeEnum getRoleType(string roleType)
    {
        switch (roleType)
        {
            case "Neutral": return Role.RoleTypeEnum.Neutral;
            case "Good": return Role.RoleTypeEnum.Good;
            case "Evil": return Role.RoleTypeEnum.Evil;
            default: throw new Exception("Role type error - " + roleType);
        }
    }

    private static Color32 getColor(string colorRGBA)
    {
        var colorData = colorRGBA.Split(",");
        return new Color32(Convert.ToByte(colorData[0]), Convert.ToByte(colorData[1]), Convert.ToByte(colorData[2]), Convert.ToByte(colorData[3]));
    }

    private static bool getIsDead(string isDead)
    {
        if (isDead.Equals("true") || isDead.Equals("yes"))
        {
            return true;
        } 
        else
        {
            return false;
        }
    }

}