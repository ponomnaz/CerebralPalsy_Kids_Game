/// <summary>
/// Stores configurable game information and settings for gameplay control.
/// </summary>
public class SGInfo
{
    public float SpawnInterval;

    public int BoxesNum;

    public int TargetFillingNum;

    public bool FadeCoin;
    public float FadeCoinDuration;
    public float CoinDropProb;

    public bool ShrinkFilling;
    public float ShrinkFillingDuration;

    public float Duration;

    public int Errors;

    /// <summary>
    /// Constructor to initialize all game settings and parameters.
    /// </summary>
    /// <param Name="SpawnInterval">Interval between spawns in seconds.</param>
    /// <param Name="BoxesNum">Number of boxes to spawn or interact with.</param>
    /// <param Name="TargetFillingNum">Target number of fillings to reach the _goal.</param>
    /// <param Name="FadeCoin">Whether coins should fade after being spawned.</param>
    /// <param Name="FadeCoinDuration">Duration of coin fade effect in seconds.</param>
    /// <param Name="CoinDropProb">Probability of a coin dropping during gameplay (0 to 1).</param>
    /// <param Name="ShrinkFilling">Whether fillings should shrink.</param>
    /// <param Name="ShrinkFillingDuration">Duration of filling shrink effect in seconds.</param>
    /// <param Name="Duration">Total game round Duration in seconds.</param>
    /// <param Name="Errors">Number of Errors.</param>
    public SGInfo(float spawnInterval, int boxesNum, int targetFillingNum, bool fadeCoin, float fadeCoinDuration, float coinDropProb, bool shrinkFilling, float shrinkFillingDuration, float duration, int errors)
    {
        this.SpawnInterval = spawnInterval;
        this.BoxesNum = boxesNum;
        this.TargetFillingNum = targetFillingNum;
        this.FadeCoin = fadeCoin;
        this.FadeCoinDuration = fadeCoinDuration;
        this.CoinDropProb = coinDropProb;
        this.ShrinkFilling = shrinkFilling;
        this.ShrinkFillingDuration = shrinkFillingDuration;
        this.Duration = duration;
        this.Errors = errors;
    }
}
