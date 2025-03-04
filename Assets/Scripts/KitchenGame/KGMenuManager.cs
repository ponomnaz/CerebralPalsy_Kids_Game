using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the game's UI menus, including navigation and game state transitions.
/// </summary>
public class KGMenuManager : MonoBehaviour
{
    // UI elements for different menus
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _rulesMenu;
    [SerializeField] private GameObject _statisticsMenu;
    [SerializeField] private GameObject _FGGameMenu;
    [SerializeField] private GameObject _FGWinMenu;
    [SerializeField] private GameObject _FGLoseMenu;
    [SerializeField] private GameObject _PGGameMenu;
    [SerializeField] private GameObject _PGWinMenu;
    [SerializeField] private GameObject _winMenu;

    [SerializeField] private UIGridRenderer _grid;                       // Reference to grid renderer
    [SerializeField] private LineRendererHUD _line1, _line2;       // Line renderer HUD for statistics

    private readonly StatisticsManager _statisticsManager = new();  // Statistics manager instance

    public void EnterMainMenu() => _mainMenu.SetActive(true);

    public void ExitMainMenu() => _mainMenu.SetActive(false);

    public void EnterRulesMenu() => _rulesMenu.SetActive(true);

    public void ExitRulesMenu() => _rulesMenu.SetActive(false);

    public void EnterStatisticsMenu()
    {
        _statisticsMenu.SetActive(true);
        _statisticsManager.SetKGLines(KGSetUp.user, ref _line1, ref _line2, ref _grid);
    }

    public void ExitStatisticsMenu() => _statisticsMenu.SetActive(false);

    public void ExitGame() => SceneManager.LoadScene("MainMenu");

    public void EnterFryingGame()
    {
        _FGGameMenu.SetActive(true);
        KGGameManager.instance.StartFryingGame();
    }

    public void ExitFryingGame() => _FGGameMenu.SetActive(false);

    public void EnterFGWinMenu() => _FGWinMenu.SetActive(true);

    public void ExitFGWinMenu() => _FGWinMenu.SetActive(false);

    public void EnterFGLoseMenu() => _FGLoseMenu.SetActive(true);

    public void ExitFGLoseMenu() => _FGLoseMenu.SetActive(false);

    public void EnterPouringGame()
    {
        _PGGameMenu.SetActive(true);
        KGGameManager.instance.StartPouringGame();
    }

    public void ExitPouringGame() => _PGGameMenu.SetActive(false);

    public void EnterPGWinMenu() => _PGWinMenu.SetActive(true);

    public void ExitPGWinMenu() => _PGWinMenu.SetActive(false);

    public void EnterWinMenu()
    {
        _winMenu.SetActive(true);
        KGGameManager.instance.Win();
    }

    public void ExitWinMenu() => _winMenu.SetActive(false);
}
