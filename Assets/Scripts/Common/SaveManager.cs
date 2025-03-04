using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

/// <summary>
/// Manages saving and loading user data in XML format.
/// </summary>
public static class SaveManager
{
    /// <summary>
    /// Saves a user's data to an XML file.
    /// </summary>
    /// <param Name="user">The user object to save.</param>
    public static void SaveUser(User user)
    {
        // Construct the file path for the user data XML file
        string path = Path.Combine(Application.persistentDataPath, $"{user.Name}.xml");

        // Create an XmlSerializer for the User.UserData Type
        XmlSerializer serializer = new(typeof(User.UserData));

        // Serialize the user data to XML format
        using (FileStream stream = new(path, FileMode.Create))
        {
            serializer.Serialize(stream, user.UserToData());
        }

        Debug.Log($"User data for {user.Name} saved to {path}.");
    }

    /// <summary>
    /// Loads all users from XML files in the persistent data path.
    /// </summary>
    /// <returns>A list of loaded User objects.</returns>
    public static List<User> LoadAllUsers()
    {
        List<User> users = new();
        string[] files = Directory.GetFiles(Application.persistentDataPath, "*.xml");

        // Iterate through each XML file found and load the user
        foreach (string file in files)
        {
            User loadedUser = LoadUser(file);
            if (loadedUser != null)
            {
                users.Add(loadedUser);
            }
        }

        return users;
    }

    /// <summary>
    /// Loads a user from the specified XML file path.
    /// </summary>
    /// <param Name="path">The path of the XML file to load.</param>
    /// <returns>The loaded User object, or null if loading failed.</returns>
    public static User LoadUser(string path)
    {
        // Check if the file exists before attempting to load
        if (File.Exists(path))
        {
            // Create an XmlSerializer for the User.UserData Type
            XmlSerializer serializer = new(typeof(User.UserData));

            // Open the file and deserialize the XML data
            using (FileStream stream = new(path, FileMode.Open))
            {
                User.UserData data = serializer.Deserialize(stream) as User.UserData;
                return User.DataToUser(data);
            }
        }
        else
        {
            Debug.LogError($"Save file not found at {path}");
            return null; // Return null if the file does not exist
        }
    }
}
