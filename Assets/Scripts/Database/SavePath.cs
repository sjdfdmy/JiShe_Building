/// <summary>
/// Centralized constants for save file paths.
/// Use these with SaveSystem to keep file names consistent and easy to manage.
/// 
/// Usage:
///   SaveSystem.Save(data, SavePath.PlayerData);
///   SaveSystem.Load&lt;MyData&gt;(SavePath.TalentTree);
/// 
/// Add new constants here as the game grows to keep all paths in one place.
/// Supports subdirectories by using "/" (e.g. "Inventory/slot1").
/// </summary>
public static class SavePath
{
    public const string PlayerData = "PlayerData";
    public const string Inventory = "Inventory";
    public const string TalentTree = "TalentTree";
    public const string Settings = "Settings";
}
