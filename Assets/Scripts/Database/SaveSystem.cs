using System.IO;
using UnityEngine;

/// <summary>
/// A static database system for saving and loading game data to external JSON files.
/// Can be called from anywhere in the scripts. Supports any [Serializable] data type.
/// 
/// Usage:
///   SaveSystem.Save(myData, SavePath.PlayerData);
///   var data = SaveSystem.Load&lt;PlayerData&gt;(SavePath.PlayerData);
///   SaveSystem.Save(inventoryData, "Inventory/slot1");
/// </summary>
public static class SaveSystem
{
    /// <summary>
    /// The root directory for all save files.
    /// Uses Application.persistentDataPath which maps to a platform-appropriate location.
    /// </summary>
    public static string RootPath => Application.persistentDataPath;

    /// <summary>
    /// Save any [Serializable] data to a JSON file.
    /// </summary>
    /// <param name="data">The data object to save (must be [Serializable])</param>
    /// <param name="fileName">File name or relative path (e.g. "PlayerData" or "Saves/slot1")</param>
    public static void Save(object data, string fileName)
    {
        string path = GetFullPath(fileName);
        string directory = Path.GetDirectoryName(path);

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        Debug.Log($"[SaveSystem] Data saved to: {path}");
    }

    /// <summary>
    /// Load data from a JSON file into a new object of type T.
    /// </summary>
    /// <typeparam name="T">The [Serializable] type to deserialize into</typeparam>
    /// <param name="fileName">File name or relative path</param>
    /// <returns>The loaded data, or default(T) if the file does not exist</returns>
    public static T Load<T>(string fileName)
    {
        string path = GetFullPath(fileName);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[SaveSystem] File not found: {path}");
            return default;
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<T>(json);
    }

    /// <summary>
    /// Load data from a JSON file and overwrite the fields of an existing object.
    /// Useful for loading into MonoBehaviour or ScriptableObject instances.
    /// </summary>
    /// <param name="fileName">File name or relative path</param>
    /// <param name="target">The existing object to overwrite with loaded data</param>
    public static void LoadOverwrite(string fileName, object target)
    {
        string path = GetFullPath(fileName);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[SaveSystem] File not found: {path}");
            return;
        }

        string json = File.ReadAllText(path);
        JsonUtility.FromJsonOverwrite(json, target);

        Debug.Log($"[SaveSystem] Data loaded into existing object from: {path}");
    }

    /// <summary>
    /// Delete a specific save file.
    /// </summary>
    /// <param name="fileName">File name or relative path</param>
    /// <returns>True if the file was found and deleted</returns>
    public static bool Delete(string fileName)
    {
        string path = GetFullPath(fileName);

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"[SaveSystem] File deleted: {path}");
            return true;
        }

        Debug.LogWarning($"[SaveSystem] File not found for deletion: {path}");
        return false;
    }

    /// <summary>
    /// Check whether a save file exists.
    /// </summary>
    /// <param name="fileName">File name or relative path</param>
    /// <returns>True if the file exists on disk</returns>
    public static bool Exists(string fileName)
    {
        return File.Exists(GetFullPath(fileName));
    }

    /// <summary>
    /// Delete all .json save files in the save directory.
    /// Searches recursively so files in subdirectories are also deleted.
    /// </summary>
    public static void DeleteAll()
    {
        if (!Directory.Exists(RootPath))
            return;

        string[] files = Directory.GetFiles(RootPath, "*.json", SearchOption.AllDirectories);
        foreach (string file in files)
            File.Delete(file);

        Debug.Log($"[SaveSystem] All save files deleted from: {RootPath}");
    }

    /// <summary>
    /// Build the full file path from a file name.
    /// Automatically appends ".json" if not already present.
    /// Supports subdirectories (e.g. "Inventory/slot1" becomes "{RootPath}/Inventory/slot1.json").
    /// </summary>
    /// <param name="fileName">File name or relative path</param>
    /// <returns>The full absolute path to the file</returns>
    public static string GetFullPath(string fileName)
    {
        if (!fileName.EndsWith(".json"))
            fileName += ".json";

        return Path.Combine(RootPath, fileName);
    }
}
