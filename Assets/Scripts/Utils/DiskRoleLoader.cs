using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DiskRoleLoader
{
    public static List<Role> LoadRolesFromFile()
    {
        var filePath = Path.Combine(Application.dataPath, "roles.txt");


        if (!File.Exists(filePath))
        {
            FileStream file = File.Open(filePath, FileMode.Create);
            file.Close();
        }

        var textFromFile = File.ReadAllText(filePath);

        List<Role> roles = CreateRolesFromText(textFromFile);

        return roles;
    }

    private static List<Role> CreateRolesFromText(string textFromFile)
    {
        var roles = new List<Role>();
        var roleList = textFromFile.Split("##");
        // ignore first item because it is a description of the file contents
        for (int i = 1; i < roleList.Length; i++)
        {
            if (roleList[i].Length >= 7)
            {
                var item = roleList[i].Split(Environment.NewLine);
                var role = new Role(item[1], item[2], item[3], item[4], getRoleType(item[5]), getColor(item[6]), getIsDead(item[7]));
                roles.Add(role);
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