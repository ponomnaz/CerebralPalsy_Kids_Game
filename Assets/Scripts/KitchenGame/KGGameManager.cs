using UnityEngine;

/// <summary>
/// Manages the overall game state, handling both the Frying Game and Pouring Game.
/// </summary>
public class KGGameManager : MonoBehaviour
{
    public static KGGameManager instance;

    public FGGameManager FGGameManager;
    public PGGameManager PGGameManager;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
    }

    public void StartFryingGame() => FGGameManager.StartPlaying();

    public void StartPouringGame() => PGGameManager.StartPlaying();

    /// <summary>
    /// Handles the win condition by saving game statistics to the user profile.
    /// </summary>

    public void Win()
    {
        KGSetUp.user.FGTime.Add(FGGameManager.GetGameTime());
        KGSetUp.user.FGEggs.Add(FGGameManager.GetEggsScore());

        KGSetUp.user.PGTime.Add(PGGameManager.GetGameTime());
        KGSetUp.user.PGGlass.Add(PGGameManager.GetGlassScore());

        SaveManager.SaveUser(KGSetUp.user);
    }
}
