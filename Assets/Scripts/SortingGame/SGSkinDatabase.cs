using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that stores a list of skins for the Sorting Game.
/// This allows easy access and management of skins within Unity's inspector.
/// </summary>
[CreateAssetMenu]
public class SGSkinDatabase : ScriptableObject
{
    /// <summary>
    /// List of skins available in the Sorting Game.
    /// Each skin is represented by a SortingSkins object.
    /// </summary>
    public List<SortingSkins> skins;
}
