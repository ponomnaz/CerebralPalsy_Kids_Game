using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A database that stores references to various objects in the game, such as boxes and fillings.
/// This allows centralized management of objects that can be used across different game scenes or scripts.
/// </summary>
public class SGObjectDatabase : MonoBehaviour
{
    public List<SBoxes> BoxesObjects;

    public List<SFillings> FillingsObjects;

    public List<GameObject> Icons;


    [System.Serializable]
    public class SFillings
    {
        public List<GameObject> First;
        public List<GameObject> Second;
        public List<GameObject> Third;
    }

    [System.Serializable]
    public class SBoxes
    {
        public GameObject First;
        public GameObject Second;
        public GameObject Third;
    }
}
