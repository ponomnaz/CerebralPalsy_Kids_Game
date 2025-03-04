using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents a user profile, storing information about their settings, achievements, 
/// and progress in different games within the app.
/// </summary>
[System.Serializable]
public class User
{
    public string Name;
    public Hand ParalyzedHand;

    // SortingGame specific attributes
    public List<int> SGBoughtSkins;
    public int SGChosenSkin;
    public int SGCurrentDifficulty;
    public int SGMoney;

    // SortingGame specific progress trackers
    public List<float> SGValues1;
    public List<float> SGValues2;
    public List<float> SGValues3;

    // KitchenGame specific progress trackers
    public List<float> FGTime;
    public List<float> FGEggs;

    public List<float> PGTime;
    public List<float> PGGlass;

    /// <summary>
    /// Constructor to initialize a User instance with provided values.
    /// </summary>
    public User(string name, Hand hand, List<int> SGBoughtSkins, int SGChosenSkin, int SGCurrentDifficulty, int SGMoney, 
        List<float> SGValues1, List<float> SGValues2, List<float> SGValues3,
        List<float> PGTime, List<float> PGGlass,
        List<float> FGTime, List<float> FGEggs)
    {
        Name = name;
        ParalyzedHand = hand;

        this.SGBoughtSkins = SGBoughtSkins;
        this.SGChosenSkin = SGChosenSkin;
        this.SGCurrentDifficulty = SGCurrentDifficulty;
        this.SGMoney = SGMoney;

        this.SGValues1 = SGValues1;
        this.SGValues2 = SGValues2;
        this.SGValues3 = SGValues3;

        this.PGTime = PGTime;
        this.PGGlass = PGGlass;

        this.FGTime = FGTime;
        this.FGEggs = FGEggs;
    }

    /// <summary>
    /// Default constructor to initialize a User instance with default values.
    /// </summary>
    public User()
    {
        Name = "David";
        ParalyzedHand = 0;

        SGBoughtSkins = new();
        SGChosenSkin = 0;
        SGCurrentDifficulty = 1;
        SGMoney = 100;

        SGValues1 = new();
        SGValues2 = new();
        SGValues3 = new();

        PGTime = new();
        PGGlass = new();

        FGTime = new();
        FGEggs = new();
    }

    /// <summary>
    /// Converts the User instance into a serializable UserData instance for data storage.
    /// </summary>
    public UserData UserToData()
    {
        return new UserData(Name, ParalyzedHand, SGBoughtSkins, SGChosenSkin, SGCurrentDifficulty, SGMoney, 
            SGValues1, SGValues2, SGValues3,
            PGTime, PGGlass,
            FGTime, FGEggs);
    }

    // <summary>
    /// Creates a User instance from a given UserData object.
    /// </summary>
    /// <param name="uD">The UserData object containing user data.</param>
    /// <returns>A User instance initialized with data from UserData.</returns>
    public static User DataToUser(UserData uD)
    {
        return new User(uD.name, (Hand)uD.hand, uD.SGBSkins.ToList(), uD.SGCSkin, uD.SGCurrentDifficulty, uD.SGMoney, 
            uD.SGValues1.ToList(), uD.SGValues2.ToList(), uD.SGValues3.ToList(),
            uD.PGTime.ToList(), uD.PGGlass.ToList(),
            uD.FGTime.ToList(), uD.FGEggs.ToList());
    }

    /// <summary>
    /// Nested serializable class to store user data for persistence.
    /// </summary>
    [System.Serializable]
    public class UserData
    {
        public string name;
        public int hand;

        public int[] SGBSkins;
        public int SGCSkin;
        public int SGCurrentDifficulty;
        public int SGMoney;

        public float[] SGValues1;
        public float[] SGValues2;
        public float[] SGValues3;

        public float[] FGTime;
        public float[] FGEggs;

        public float[] PGTime;
        public float[] PGGlass;

        /// <summary>
        /// Initializes a UserData object with given values.
        /// Converts lists to arrays for storage.
        /// </summary>
        public UserData(string name, Hand hand, List<int> SGBSkins, int SGCSkin, int SGCurrentDifficulty, int SGMoney,
            List<float> SGValues1, List<float> SGValues2, List<float> SGValues3,
            List<float> PGTime, List<float> PGGlass,
            List<float> FGTime, List<float> FGEggs)
        {
            this.name = name;
            this.hand = (int)hand;


            // Convert lists to arrays for serialization
            SGBSkins ??= new List<int>();
            SGValues1 ??= new List<float>();
            SGValues2 ??= new List<float>();
            SGValues3 ??= new List<float>();
            PGTime ??= new List<float>();
            PGGlass ??= new List<float>();
            FGTime ??= new List<float>();
            FGEggs ??= new List<float>();

            this.SGBSkins = SGBSkins.ToArray();
            this.SGCSkin = SGCSkin;
            this.SGCurrentDifficulty = SGCurrentDifficulty;
            this.SGMoney = SGMoney;
            this.SGValues1 = SGValues1.ToArray();
            this.SGValues2 = SGValues2.ToArray();
            this.SGValues3 = SGValues3.ToArray();
            this.PGTime = PGTime.ToArray();
            this.PGGlass = PGGlass.ToArray();
            this.FGTime = FGTime.ToArray();
            this.FGEggs = FGEggs.ToArray();
        }

        /// <summary>
        /// Initializes a UserData object with default values.
        /// </summary>
        public UserData()
        {
            name = string.Empty;
            hand = 0;
            SGBSkins = new int[0];

            SGCSkin = 0;
            SGCurrentDifficulty = 1;
            SGMoney = 0;

            SGValues1 = new float[0];
            SGValues2 = new float[0];
            SGValues3 = new float[0];

            PGTime = new float[0];
            PGGlass = new float[0];
            FGTime = new float[0];
            FGEggs = new float[0];
        }
    }

    /// <summary>
    /// Enum to represent the paralyzed hand (Left or Right).
    /// </summary>
    public enum Hand
    {
        Left,
        Right
    }
}
