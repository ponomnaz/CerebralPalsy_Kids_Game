/// <summary>
/// Static class to manage game setup configurations based on selected difficulty.
/// </summary>
public static class SGSetUp
{
    public static User user;

    public static string userName = string.Empty;

    public static int difficulty = 2;

    public static int chosenSkin = 1;

    public static User.Hand hand = User.Hand.Right;

    // Game parameters that are set based on difficulty level.

    public static float spawnInterval;
    public static int boxesNum;
    public static int targetFillingNum;

    public static bool fadeCoin;
    public static float fadeCoinDuration;
    public static float coinDropProb;

    public static bool shrinkFilling;
    public static float shrinkFillingDuration;

    /// <summary>
    /// Sets game values based on the current difficulty level.
    /// </summary>
    public static void SetValues()
    {
        switch (difficulty) {
            case 1:
                DiffFirst();
                break;
                
            case 2:
                DiffSecond();
                break;

            case 3:
                DiffThird();
                break;
        }
    }

    /// <summary>
    /// Configurations for difficulty level 1 (Easy).
    /// </summary>
    private static void DiffFirst()
    {
        spawnInterval = 2.5f;
        boxesNum = 1;
        targetFillingNum = 10;

        SetCommonValues();
    }

    /// <summary>
    /// Configurations for difficulty level 2 (Medium).
    /// </summary>
    private static void DiffSecond()
    {
        spawnInterval = 3.5f;
        boxesNum = 2;
        targetFillingNum = 10;

        SetCommonValues();
    }

    /// <summary>
    /// Configurations for difficulty level 3 (Hard).
    /// </summary>
    private static void DiffThird()
    {
        spawnInterval = 5f;
        boxesNum = 3;
        targetFillingNum = 12;

        SetCommonValues();
    }

    /// <summary>
    /// Sets values that are common across difficulty levels, such as coin and filling behaviors.
    /// </summary>
    private static void SetCommonValues()
    {
        fadeCoin = true;
        fadeCoinDuration = 5f;
        coinDropProb = 0.15f;

        shrinkFilling = false;
        shrinkFillingDuration = 5f;
    }
}
