using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages the Main _mainMenu UI for the Sorting Game, allowing the user to adjust settings, view rules and statistics, 
/// and access game modes and skins.
/// </summary>
public class SGMenuManager : MonoBehaviour
{
    public bool StartPlaying;                         // Flag indicating if game is starting

    private User _user;                               // Holds the current user data

    [SerializeField] private GameObject _mainMenu;                       // Main menu UI
    [SerializeField] private GameObject _rules;                          // _rules UI
    [SerializeField] private GameObject _winMenu;                        // Win screen UI
    [SerializeField] private GameObject _statisticsMenu;                 // Statistics screen UI
    [SerializeField] private TMP_Text _difficultyIcon;                   // Displays current difficulty level
    [SerializeField] private GameObject _purchase;                       // _purchase UI for skins
    [SerializeField] private Image _playButtonImg;                       // Play button image

    [SerializeField] private List<Image> _filling0;                      // List of _filling0 images for different difficulty levels
    [SerializeField] private List<Image> _filling1;                      // List of _filling1 images for different difficulty levels
    [SerializeField] private List<Image> _filling2;                      // List of _filling2 images for different difficulty levels

    [SerializeField] private SGSkinDatabase _skins;                      // Database of available skins
    [SerializeField] private UIGridRenderer _grid;                       // Reference to grid renderer
    [SerializeField] private LineRendererHUD _line1, _line2, _line3;       // Line renderer HUD for statistics

    private readonly StatisticsManager _statisticsManager = new();  // Statistics manager instance
    private bool _canPlay;                            // Flag indicating if the user can _playing
    private const int _maxDifficulty = 3;             // Maximum difficulty level

    /// <summary>
    /// Initializes the game settings and enters the main menu.
    /// </summary>
    public void EnterGame(User u)
    {
        StartPlaying = false;
        _user = u;
        EnterMainMenu();
        SetCurrentSkin(_user.SGChosenSkin);
        AdjustDifficulty(_user.SGCurrentDifficulty);
    }

    /// <summary>
    /// Exits to the main game menu.
    /// </summary>
    public void ExitGame() => SceneManager.LoadScene("MainMenu");

    /// <summary>
    /// Starts the game if conditions are met.
    /// </summary>
    public void Play()
    {
        if (_canPlay)
        {
            LoadSortingGame();
            ExitMainMenu();
            StartPlaying = true;
        }
    }

    public void EnterMainMenu() => _mainMenu.SetActive(true);  // Shows the main menu

    public void ExitMainMenu() => _mainMenu.SetActive(false);  // Hides the main menu

    public void EnterRules() => _rules.SetActive(true);        // Shows the rules menu

    public void ExitRules() => _rules.SetActive(false);        // Hides the rules menu

    public void EnterWinMenu() => _winMenu.SetActive(true);    // Shows the win screen

    public void ExitWinMenu() => _winMenu.SetActive(false);    // Hides the win screen

    /// <summary>
    /// Opens the statistics menu and sets up the statistics display.
    /// </summary>
    public void EnterStatisticsMenu()
    {
        _statisticsMenu.SetActive(true);
        _statisticsManager.SetSGLines(_user, ref _line1, ref _line2, ref _line3, ref _grid);
    }

    public void ExitStatisticsMenu() => _statisticsMenu.SetActive(false);  // Hides the statistics menu

    /// <summary>
    /// Prepares the game setup by saving user data and setting game configurations.
    /// </summary>
    private void LoadSortingGame()
    {
        SaveManager.SaveUser(_user);
        SGSetUp.user = _user;
        SGSetUp.userName = _user.Name;
        SGSetUp.difficulty = _user.SGCurrentDifficulty;
        SGSetUp.boxesNum = _user.SGCurrentDifficulty;
        SGSetUp.chosenSkin = _user.SGChosenSkin;
        SGSetUp.hand = _user.ParalyzedHand;
    }

    /// <summary>
    /// Switches to the next available skin.
    /// </summary>
    public void NextSkin()
    {
        _user.SGChosenSkin++;
        if (_user.SGChosenSkin >= _skins.skins.Count)
            _user.SGChosenSkin = 0;
        SetCurrentSkin(_user.SGChosenSkin);
    }

    public void PreviousSkin()
    {
        _user.SGChosenSkin--;
        if (_user.SGChosenSkin < 0)
            _user.SGChosenSkin = _skins.skins.Count - 1;
        SetCurrentSkin(_user.SGChosenSkin);
    }

    /// <summary>
    /// Increases difficulty level up to a maximum limit.
    /// </summary>
    public void IncreaseDifficulty()
    {
        _user.SGCurrentDifficulty++;
        if (_user.SGCurrentDifficulty > _maxDifficulty)
            _user.SGCurrentDifficulty = _maxDifficulty;
        AdjustDifficulty(_user.SGCurrentDifficulty);
    }

    public void DecreaseDifficulty()
    {
        _user.SGCurrentDifficulty--;
        if (_user.SGCurrentDifficulty < 1)
            _user.SGCurrentDifficulty = 1;
        AdjustDifficulty(_user.SGCurrentDifficulty);
    }

    /// <summary>
    /// Purchases the selected skin if the user has enough money.
    /// </summary>
    public void BuySkin()
    {
        if (_user.SGMoney >= _skins.skins[_user.SGChosenSkin].Price)
        {
            _user.SGBoughtSkins.Add(_user.SGChosenSkin);
            _user.SGMoney -= _skins.skins[_user.SGChosenSkin].Price;
            SetCurrentSkin(_user.SGChosenSkin);
            SaveManager.SaveUser(_user);
        }
    }

    /// <summary>
    /// Updates UI based on the current skin and availability of the skin for _playing.
    /// </summary>
    private void SetCurrentSkin(int skinNum)
    {
        _canPlay = _user.SGBoughtSkins?.Contains(_user.SGChosenSkin) == true;
        _playButtonImg.color = _canPlay ? Color.white : Color.red;

        SetImages(_filling0, _skins.skins[skinNum].Fillings0, _canPlay);
        SetImages(_filling1, _skins.skins[skinNum].Fillings1, _canPlay);
        SetImages(_filling2, _skins.skins[skinNum].Fillings2, _canPlay);
    }

    /// <summary>
    /// Sets the images for the different filling levels and locks them if the skin is unavailable.
    /// </summary>
    private void SetImages(List<Image> container, List<Sprite> target, bool bought)
    {
        for (int i = 0; i < container.Count; i++)
            container[i].sprite = target[i];

        if (!bought)
            LockImages(container);
        else
            UnlockImages(container);
    }

    /// <summary>
    /// Adjusts UI elements based on the selected difficulty.
    /// </summary>
    private void AdjustDifficulty(int difficulty)
    {
        ActivateImages(_filling0);
        ActivateImages(_filling1);
        ActivateImages(_filling2);
        _difficultyIcon.text = difficulty.ToString();

        switch (difficulty)
        {
            case 1:
                HideImages(_filling1, _filling1.Count);
                HideImages(_filling2, _filling2.Count);
                break;
            case 2:
                HideImages(_filling2, _filling2.Count);
                break;
            case 3:
                break;
        }
    }

    private void HideImages(List<Image> images, int num)
    {
        for (int i = images.Count - 1; i >= images.Count - num; i--)
            images[i].gameObject.SetActive(false);
    }

    private void ActivateImages(List<Image> images)
    {
        foreach (Image image in images)
            image.gameObject.SetActive(true);
    }

    private void LockImages(List<Image> images)
    {
        foreach (var image in images)
            image.color = Color.black;
        _purchase.SetActive(true);
        _purchase.GetComponentInChildren<TMP_Text>().text = $"{_user.SGMoney}/{_skins.skins[_user.SGChosenSkin].Price}";
    }

    private void UnlockImages(List<Image> images)
    {
        foreach (var image in images)
            image.color = Color.white;
        _purchase.SetActive(false);
    }
}
