using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonRoleReader
{
    public static List<Role> LoadRolesFromFile()
    {
        var filePath = Path.Combine(Application.persistentDataPath, "Roles.json");


        if (!File.Exists(filePath))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/Roles.json", FileMode.Create);
            file.Close();
        }

        var json = File.ReadAllText(filePath);

        List<Role> roles = JsonConvert.DeserializeObject<List<Role>>(json, settings);

        return roles;
    }
}