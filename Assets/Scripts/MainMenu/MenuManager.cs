using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the game's main menu, _user selection, game selection, and _user creation menus.
/// Handles scene transitions for different games and manages _user data.
/// </summary>
public class MenuManager : MonoBehaviour
{
    // References to different menu GameObjects in the UI
    [SerializeField] private GameObject _mainMenu;

    [SerializeField] private GameObject _usersMenu;

    [SerializeField] private GameObject _gamesMenu;

    [SerializeField] private GameObject _userCreationMenu;

    // Game preview images
    [SerializeField] private GameObject _kitchenGameImg;

    [SerializeField] private GameObject _sortingGameImg;

    // User input fields for new _user creation
    [SerializeField] private TMP_InputField _userNameField;

    [SerializeField] private TMP_Dropdown _userHandField;

    // List of buttons that represent each _user in the selection menu
    [SerializeField] private List<GameObject> _usersPlayButtons;

    // Track current _user and game selection
    private User _currentUser;

    private GameNames _curGame;

    // List to hold all users
    private List<User> _users = new();

    // Constants for max number of users and add _user button text
    private const int _maxUsers = 6;

    private const string _addUserStr = "+";

    // Called when the object is initialized, sets up the menu based on saved data
    public void Awake()
    {
        SetUpMenu();
    }

    /// <summary>
    /// Initializes the menu setup by loading saved users and setting up the current _user or game context if available.
    /// </summary>
    private void SetUpMenu()
    {
        // Load all saved users
        _users = SaveManager.LoadAllUsers();
        _curGame = GameNames.Kitchen;

        // Check if a specific _user and game were saved in previous sessions
        if (SGSetUp.userName != string.Empty)
        {
            _currentUser = FindUserByName(SGSetUp.userName);
            EnterGamesMenu();
            ExitMainMenu();
            SGSetUp.userName = string.Empty; // Clear saved Name after loading
        }
        else if (KGSetUp.user != null)
        {
            _currentUser = KGSetUp.user;
            SetCurrentGame(GameNames.Kitchen);
            EnterGamesMenu();
            ExitMainMenu();
            KGSetUp.user = null;
        }
        else if (SGSetUp.user != null)
        {
            _currentUser = SGSetUp.user;
            SetCurrentGame(GameNames.Sort);
            EnterGamesMenu();
            ExitMainMenu();
            SGSetUp.user = null;
        }
    }

    // UI _mainMenu Navigation Methods
    public void EnterMainMenu() => _mainMenu.SetActive(true);

    public void ExitMainMenu() => _mainMenu.SetActive(false);

    public void EnterUserSelectionMenu()
    {
        SetButtons(_usersPlayButtons, true);
        _usersMenu.SetActive(true);
    }

    public void ExitUserSelectionMenu()
    {
        DeactivateButtons(_usersPlayButtons);
        _usersMenu.SetActive(false);
    }

    /// <summary>
    /// Enters the games menu based on a button's _user selection, or prompts for _user creation.
    /// </summary>
    public void EnterGamesMenu(TMP_Text buttonText)
    {
        if (buttonText.text.Equals(_addUserStr))
        {
            EnterUserCreationMenu();
        }
        else
        {
            _currentUser = FindUserByName(buttonText.text);
            _gamesMenu.SetActive(true);
            SetCurrentGame(_curGame);
        }
    }

    public void EnterGamesMenu()
    {
        _gamesMenu.SetActive(true);
        SetCurrentGame(_curGame);
    }

    public void ExitGamesMenu() => _gamesMenu.SetActive(false);

    public void EnterUserCreationMenu() => _userCreationMenu.SetActive(true);

    public void ExitUserCreationMenu()
    {
        ResetFields();
        _userCreationMenu.SetActive(false);
    }

    /// <summary>
    /// Loads the Sorting Game scene with the current _user.
    /// </summary>
    public void EnterSortingGame()
    {
        SGSetUp.user = _currentUser;
        SceneManager.LoadScene("SortingGame");
    }

    /// <summary>
    /// Loads the Kitchen Game scene with the current _user.
    /// </summary>
    public void EnterKitchenGame()
    {
        KGSetUp.user = _currentUser;
        SceneManager.LoadScene("KitchenGame");
    }

    // Plays the selected game based on the current game selection
    public void Play()
    {
        switch (_curGame)
        {
            case GameNames.Sort:
                EnterSortingGame();
                break;
            case GameNames.Kitchen:
                EnterKitchenGame();
                break;
        }
    }

    // Exits the application or stops _playing mode in the editor
    public void ExitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(); // Quit the application on Android device
#endif
    }

    /// <summary>
    /// Searches for a _user by Name within the _user list.
    /// </summary>
    private User FindUserByName(string name)
    {
        foreach (var item in _users)
        {
            if (item.Name == name) return item;
        }
        return null;
    }

    /// <summary>
    /// Sets up the _user buttons to display existing users, with an option to add a new _user.
    /// </summary>
    private void SetButtons(List<GameObject> buttons, bool setAddButton)
    {
        for (int i = 0; i < _users.Count; i++)
        {
            buttons[i].GetComponentInChildren<TMP_Text>().text = _users[i].Name;
            buttons[i].SetActive(true);
        }

        if (setAddButton && _users.Count < _maxUsers)
            SetAddButton(buttons[_users.Count], _addUserStr);
    }

    private void DeactivateButtons(List<GameObject> buttons)
    {
        foreach (var b in buttons)
            b.SetActive(false);
    }

    /// <summary>
    /// Sets the current game and updates the game images accordingly.
    /// </summary>
    private void SetCurrentGame(GameNames name)
    {
        switch (name)
        {
            case GameNames.Sort:
                _sortingGameImg.SetActive(true);
                _kitchenGameImg.SetActive(false);
                break;
            case GameNames.Kitchen:
                _kitchenGameImg.SetActive(true);
                _sortingGameImg.SetActive(false);
                break;
            default:
                break;

        }
        _curGame = name;
    }

    // Toggles between game selections
    public void NextGame()
    {
        GameNames g = _curGame == GameNames.Kitchen ? GameNames.Sort : GameNames.Kitchen;
        SetCurrentGame(g);
    }

    /// <summary>
    /// Sets up the "Add User" button with the provided text.
    /// </summary>
    private void SetAddButton(GameObject button, string text)
    {
        button.GetComponentInChildren<TMP_Text>().text = text;
        button.SetActive(true);
    }

    /// <summary>
    /// Saves a new _user if _user input is valid.
    /// </summary>
    public void SaveUser()
    {
        if (CheckUserCorrectness())
        {
            User newUser = new(_userNameField.text, (User.Hand)_userHandField.value, new List<int>(), 0, 1, 0,
                new List<float>(), new List<float>(), new List<float>(),
                new List<float>(), new List<float>(),
                new List<float>(), new List<float>());
            _users.Add(newUser);
            SaveManager.SaveUser(newUser);
            ExitUserCreationMenu();
            EnterUserSelectionMenu();
        }
    }

    /// <summary>
    /// Validates if the _user input is correct and unique.
    /// </summary>
    private bool CheckUserCorrectness()
    {
        if (_userNameField.text.Equals(string.Empty))
        {
            _userNameField.placeholder.GetComponent<TMP_Text>().color = Color.red;
            return false;
        }
        else if (CheckUserExistence(_userNameField.text))
        {
            _userNameField.text = string.Empty;
            _userNameField.placeholder.GetComponent<TMP_Text>().color = Color.red;
            _userNameField.placeholder.GetComponent<TMP_Text>().text = "User already exists";
            return false;
        } else
        {
            return true;
        }
    }

    private bool CheckUserExistence(string name)
    {
        foreach (var user in _users)
            if (user.Name.Equals(name)) return true;
        return false;
    }

    /// <summary>
    /// Resets _user input fields to their default state.
    /// </summary>
    private void ResetFields()
    {
        _userNameField.text = string.Empty;
        _userNameField.placeholder.GetComponent<TMP_Text>().color = new Color(0.1960784f, 0.1960784f, 0.1960784f, 0.5f);
        _userNameField.placeholder.GetComponent<TMP_Text>().text = "Enter your Name";
        _userHandField.value = 0;
    }
}
