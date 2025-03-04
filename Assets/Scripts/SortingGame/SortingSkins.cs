using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a collection of skins for various game objects.
/// </summary>
[System.Serializable]
public class SortingSkins
{
    /// <summary>
    /// The Price of the skin.
    /// </summary>
    public int Price;

    public List<Sprite> ClosedBoxes;

    public List<Sprite> OpenedBoxes;

    public List<Sprite> FullBoxes;

    public List<Sprite> Fillings0;

    public List<Sprite> Fillings1;

    public List<Sprite> Fillings2;
}
