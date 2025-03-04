using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the game's state, progression, and interactions within the Sorting Game.
/// Handles setup, gameplay, and victory conditions.
/// </summary>
public class SGGameManager : MonoBehaviour
{
    public static SGGameManager instance;

    public List<GameObject> Fillings;  // List of filling objects in the game
    public List<GameObject> Boxes;  // List of box objects
    public List<GameObject> Coins;  // List of coin objects

    // Gameplay mechanics variables
    public bool shrinkFilling;  // Determines whether fillings should shrink
    public float shrinkFillingDuration;  // Duration of shrink effect
    public bool fadeCoin;  // Determines if coins should fade out
    public float fadeCoinDuration;  // Duration of fade effect
    public float coinDropProb;  // Probability of coin drop

    public int boxesNum;  // Number of boxes in the game
    public int currentTouchCount;  // Tracks the number of touches currently on screen
    public uint fillingLayers;  // Tracks the number of filling layers
    public int fillingSorted;  // Number of fillings sorted correctly
    public int targetFillingNum;  // Target number of fillings to sort
    public int currentCoins;  // The current number of coins the player has
    public int coinsCollected;  // Total coins collected during the game
    public int gamesPlayed;  // Tracks the number of games played

    public int errors;  // Tracks the number of errors made in the game

    [SerializeField] private SGMenuManager _menuManager;  // Manages the main menu and transitions
    [SerializeField] private SGObjectDatabase _dataBase;  // Holds all objects related to the game (skins, boxes, fillings)
    [SerializeField] private SpawnManager _spawnManager;  // Manages the spawning of boxes, fillings, and coins
    [SerializeField] private PositionManager _positionManager;  // Manages object placements within the game world

    [SerializeField] private GameObject _progressBar;  // Progress bar UI element

    // Prefab lists for filling types based on game difficulty
    [SerializeField] private List<GameObject> _fillingPrefabesFirst;
    [SerializeField] private List<GameObject> _fillingPrefabesSeconde;
    [SerializeField] private List<GameObject> _fillingPrefabesThird;

    // Box prefab variants based on game setup
    [SerializeField] private List<GameObject> _boxesPrefabes;
    [SerializeField] private GameObject _barIcon;  // Icon for the progress bar
    [SerializeField] private GameObject _coinPrefabe;  // Prefab for the coin object
    [SerializeField] private TMP_Text _coinText;  // UI text showing current collected coins

    private float _spawnTimer;  // Timer for spawning fillings and coins
    private float _spawnInterval;  // Interval between object spawns
    private float _gameDuration;  // Total game duration in seconds

    private GameState _state;  // Current game state (_mainMenu, Playing, Victory)

    private List<SGInfo> _gamesInfo = new();  // Stores information about completed games

    // Singleton pattern to ensure only one instance of SGGameManager exists
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
    }

    void Start()
    {
        _state = GameState.Menu;  // Start in the menu state
        SGSetUp.user ??= new User();  // Ensure the user object is initialized
        _menuManager.EnterGame(SGSetUp.user);  // Enter the game with the user data
    }

    void Update()
    {
        switch (_state)
        {
            case GameState.Menu:
                if (_menuManager.StartPlaying)  // If the player starts the game, transition to Playing state
                {
                    _state = GameState.Playing;
                    SetUpGame();  // Setup the game parameters
                    StartGame();  // Start the game _timer and gameplay
                }
                break;
            case GameState.Playing:
                Play();  // Continuously update game state while playing
                break;
        }
    }

    /// <summary>
    /// Setup the initial game parameters before starting.
    /// Initializes all the variables and prepares the gameplay environment.
    /// </summary>
    private void SetUpGame()
    {
        coinsCollected = 0;
        if (SGSetUp.user != null)
            currentCoins = SGSetUp.user.SGMoney;
        gamesPlayed = 0;
        _positionManager.AdjustPositions(SGSetUp.hand);  // Adjust positions based on user hand preference
        _progressBar.SetActive(true);  // Display the progress bar

        SGSetUp.SetValues();  // Load game values from the setup

        _spawnInterval = SGSetUp.spawnInterval;
        targetFillingNum = SGSetUp.targetFillingNum;
        boxesNum = SGSetUp.boxesNum;
        shrinkFilling = SGSetUp.shrinkFilling;
        shrinkFillingDuration = SGSetUp.shrinkFillingDuration;
        fadeCoin = SGSetUp.fadeCoin;
        fadeCoinDuration = SGSetUp.fadeCoinDuration;
        coinDropProb = SGSetUp.coinDropProb;

        // Set up difficulty-specific box and filling prefabs
        if (SGSetUp.difficulty >= 1)
            SetUpBoxAndFillings(_dataBase.BoxesObjects[SGSetUp.chosenSkin].First, _dataBase.FillingsObjects[SGSetUp.chosenSkin].First, _fillingPrefabesFirst);

        if (SGSetUp.difficulty >= 2)
            SetUpBoxAndFillings(_dataBase.BoxesObjects[SGSetUp.chosenSkin].Second, _dataBase.FillingsObjects[SGSetUp.chosenSkin].Second, _fillingPrefabesSeconde);

        if (SGSetUp.difficulty >= 3)
            SetUpBoxAndFillings(_dataBase.BoxesObjects[SGSetUp.chosenSkin].Third, _dataBase.FillingsObjects[SGSetUp.chosenSkin].Third, _fillingPrefabesThird);

        Instantiate(_dataBase.Icons[SGSetUp.chosenSkin], _barIcon.transform);  // Instantiate the chosen skin's bar icon
    }

    /// <summary>
    /// Starts the game by initializing gameplay variables and resetting the environment.
    /// </summary>
    private void StartGame()
    {
        _spawnTimer = 0f;  // Reset the spawn _timer
        currentTouchCount = 0;
        fillingLayers = 0;
        fillingSorted = 0;
        errors = 0;

        // Initialize progress bar UI
        _progressBar.GetComponentInChildren<ProgressBar>().SetValue(0);
        _progressBar.GetComponentInChildren<ProgressBar>().SetMaxValue(targetFillingNum);

        // Clear existing objects and spawn new ones
        ClearObjects(Boxes);
        ClearObjects(Fillings);
        ClearObjects(Coins);

        SpawnBoxes();  // Spawn the boxes at the start of the game

        _state = GameState.Playing;  // Transition to Playing state
        _gameDuration = 0f;  // Reset game duration _timer
    }

    /// <summary>
    /// Main gameplay loop for when the game is in the Playing state.
    /// Handles filling spawning, user inputs, and game progress.
    /// </summary>
    private void Play()
    {
        SpawnFillings();  // Spawn new fillings if necessary
        if (currentTouchCount != Input.touchCount)  // Detect screen touch changes
            ScreenTouched();
         
        TriggerDrags();  // Handle object dragging logic
        _gameDuration += Time.deltaTime;  // Update game duration
    }

    /// <summary>
    /// Ends the current game and evaluates the result (victory or loss).
    /// </summary>
    public void EndGame()
    {
        // Check if the player has met the target filling number to win
        if (fillingSorted >= targetFillingNum)
        {
            ClearObjects(Coins);
            _menuManager.StartPlaying = false;
            _state = GameState.Menu;
            _menuManager.EnterWinMenu();  // Show victory screen when game ends

            SGInfo gI = new(_spawnInterval, boxesNum, targetFillingNum, fadeCoin, fadeCoinDuration, coinDropProb, shrinkFilling, shrinkFillingDuration, _gameDuration, errors);
            _gamesInfo.Add(gI);  // Add game info to the list

            gamesPlayed++;  // Increment the number of games played

            // Update user's money after the game
            if (SGSetUp.user != null)
                SGSetUp.user.SGMoney = currentCoins + coinsCollected;

            // Update user statistics based on the number of boxes
            switch (boxesNum)
            {
                case 1:
                    if (SGSetUp.user != null)
                    {
                        if (gamesPlayed == 1)
                            SGSetUp.user.SGValues1.Add(CalculateGameValue());
                        else
                            SGSetUp.user.SGValues1[SGSetUp.user.SGValues1.Count - 1] = CalculateGameValue();
                    }
                    break;
                case 2:
                    if (SGSetUp.user != null)
                    {
                        if (gamesPlayed == 1)
                            SGSetUp.user.SGValues2.Add(CalculateGameValue());
                        else
                            SGSetUp.user.SGValues2[SGSetUp.user.SGValues2.Count - 1] = CalculateGameValue();
                    }


                    break;
                case 3:
                    if (SGSetUp.user != null) {
                        if (gamesPlayed == 1)
                            SGSetUp.user.SGValues3.Add(CalculateGameValue());
                        else
                            SGSetUp.user.SGValues3[SGSetUp.user.SGValues3.Count - 1] = CalculateGameValue();
                    }
                    break;
            }
            _coinText.text = coinsCollected.ToString();  // Update coin UI text
            if (SGSetUp.user != null)
                SaveManager.SaveUser(SGSetUp.user);  // Save the user's progress
        }
    }

    /// <summary>
    /// Continues the game from where the user left off.
    /// </summary>
    public void ContinueGame()
    {
        switch (boxesNum)
        {
            case 1:
                OneBox();  // Adjust settings for one box
                break;
            case 2:
                TwoBoxes();  // Adjust settings for two boxes
                break;
            case 3:
                ThreeBoxes();  // Adjust settings for three boxes
                break;
        }

        _menuManager.ExitWinMenu();  // Exit the win menu
        StartGame();  // Restart the game
    }

    /// <summary>
    /// Sets up the boxes and fillings based on difficulty level.
    /// </summary>
    private void SetUpBoxAndFillings(GameObject box, List<GameObject> fillings, List<GameObject> targetFillings)
    {
        _boxesPrefabes.Add(box);
        SetUpFillings(fillings, targetFillings);
    }

    /// <summary>
    /// Adds a list of fillings to the target list.
    /// </summary>
    private void SetUpFillings(List<GameObject> fi, List<GameObject> target)
    {
        foreach (var f in fi)
            target.Add(f);
    }

    /// <summary>
    /// Spawns the boxes in the game world based on the number of boxes.
    /// </summary>
    private void SpawnBoxes()
    {
        for (int i = 0; i < boxesNum; i++)
            _spawnManager.Spawn(_boxesPrefabes[i], _positionManager.BoxesSpawnArea.bounds.center, _positionManager.BoxesSpawnArea.bounds.size, Boxes);

        // Set additional properties for each box (e.g., target filling number, coin drop probability)
        foreach (var box in Boxes)
        {
            box.GetComponent<Box>().SetTargetFillingNum(targetFillingNum / boxesNum);
            box.GetComponent<Box>().SetCoinDropProb(coinDropProb);
        }
    }

    /// <summary>
    /// Spawns fillings periodically based on the spawn interval and difficulty level.
    /// </summary>
    private void SpawnFillings()
    {
        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= _spawnInterval)
        {
            if (SGSetUp.difficulty >= 1)
                _spawnManager.Spawn(_fillingPrefabesFirst[UnityEngine.Random.Range(0, _fillingPrefabesFirst.Count)], _positionManager.FillingsSpawnArea.bounds.center, _positionManager.FillingsSpawnArea.bounds.size, Fillings);

            if (SGSetUp.difficulty >= 2)
                _spawnManager.Spawn(_fillingPrefabesSeconde[UnityEngine.Random.Range(0, _fillingPrefabesSeconde.Count)], _positionManager.FillingsSpawnArea.bounds.center, _positionManager.FillingsSpawnArea.bounds.size, Fillings);

            if (SGSetUp.difficulty >= 3)
                _spawnManager.Spawn(_fillingPrefabesThird[UnityEngine.Random.Range(0, _fillingPrefabesThird.Count)], _positionManager.FillingsSpawnArea.bounds.center, _positionManager.FillingsSpawnArea.bounds.size, Fillings);

            _spawnTimer = 0f;  // Reset spawn _timer after each spawn
        }
    }

    /// <summary>
    /// Spawns a coin at the specified center position.
    /// </summary>
    public void SpawnCoin(Vector2 center)
    {
        _spawnManager.Spawn(_coinPrefabe, center, _positionManager.CoinSpawnRadius, Coins);

        // Apply fade settings to each spawned coin
        foreach (var coin in Coins)
        {
            coin.GetComponent<Coin>().SetFade(fadeCoin);
            coin.GetComponent<Coin>().SetFadeDuration(fadeCoinDuration);
        }
    }

    /// <summary>
    /// Detects when the screen is touched and triggers necessary actions.
    /// </summary>
    private void ScreenTouched()
    {
        TriggerTouches();  // Trigger touch actions for all game objects
    }

    /// <summary>
    /// Triggers the touch actions for all _active game objects (fillings, boxes, and coins).
    /// </summary>
    private void TriggerTouches()
    {
        for (int i = 0; i < Fillings.Count; i++)
            Fillings[i].GetComponent<Filling>().OnTriggerTouch();
        for (int i = 0; i < Boxes.Count; i++)
            Boxes[i].GetComponent<Box>().OnTriggerTouch();
        for (int i = 0; i < Coins.Count; i++)
            Coins[i].GetComponent<Coin>().OnTriggerTouch();
    }

    /// <summary>
    /// Triggers the drag actions for all _active game objects (fillings, boxes, and coins).
    /// </summary>
    private void TriggerDrags()
    {
        for (int i = 0; i < Fillings.Count; i++)
            Fillings[i].GetComponent<Filling>().OnTriggerDrag();
        for (int i = 0; i < Boxes.Count; i++)
            Boxes[i].GetComponent<Box>().OnTriggerDrag();
    }

    /// <summary>
    /// Deletes the specified filling object from the game.
    /// </summary>
    public void DeleteFilling(GameObject tr, bool inTrashCan)
    {
        if (inTrashCan)
        {
            fillingSorted++;
            _progressBar.GetComponentInChildren<ProgressBar>().SetValue(fillingSorted);
            EndGame();  // Check for victory conditions
        }
        Fillings.Remove(tr);  // Remove the filling from the list
        Destroy(tr);  // Destroy the filling object
    }

    /// <summary>
    /// Deletes the specified coin object from the game.
    /// </summary>
    public void DeleteCoin(GameObject coin, bool collected)
    {
        if (collected)
            coinsCollected++;  // Increment the coins collected count

        Coins.Remove(coin);  // Remove the coin from the list
        Destroy(coin);  // Destroy the coin object
    }

    /// <summary>
    /// Adjusts game parameters when there is one box.
    /// </summary>
    public void OneBox()
    {
        _spawnInterval *= 0.9f;  // Decrease the spawn interval to make the game faster
        targetFillingNum += 3;  // Increase the target filling number

        // If spawn interval is reduced below a threshold, increase difficulty
        if (_spawnInterval <= 1.2f)
        {
            targetFillingNum += 3;
            if (_spawnInterval <= 1f)
            {
                if (shrinkFilling && shrinkFillingDuration >= 2f)
                    shrinkFillingDuration *= 0.9f;

                shrinkFilling = true;  // Enable filling shrink effect
                _spawnInterval *= 1.05f;  // Further reduce spawn interval
            }
        }
    }

    /// <summary>
    /// Adjusts game parameters when there are two boxes.
    /// </summary>
    public void TwoBoxes()
    {
        _spawnInterval *= 0.9f;
        targetFillingNum += 4;
        if (_spawnInterval <= 1.4f)
        {
            targetFillingNum += 2;
            if (_spawnInterval <= 1.2f)
            {
                if (shrinkFilling && shrinkFillingDuration >= 2f)
                    shrinkFillingDuration *= 0.9f;

                shrinkFilling = true;
                _spawnInterval *= 1.05f;
            }
        }
    }

    /// <summary>
    /// Adjusts game parameters when there are three boxes.
    /// </summary>
    public void ThreeBoxes()
    {
        _spawnInterval *= 0.9f;
        targetFillingNum += 3;
        if (_spawnInterval <= 1.4f)
        {
            targetFillingNum += 3;
            if (_spawnInterval <= 1.2f)
            {
                if (shrinkFilling && shrinkFillingDuration >= 2f)
                    shrinkFillingDuration *= 0.9f;

                shrinkFilling = true;
                _spawnInterval *= 1.05f;
            }
        }
    }

    /// <summary>
    /// Calculates a value representing the performance of the user across games.
    /// </summary>
    private float CalculateGameValue()
    {
        float allRate = 0;

        // Iterate through all the game info and calculate rates based on performance
        foreach (var game in _gamesInfo)
        {
            float durationRate = game.Duration / (float)game.TargetFillingNum;
            float rightRate = (float)game.TargetFillingNum / ((float)game.Errors + (float)game.TargetFillingNum);
            allRate += (1 / durationRate) * rightRate;
        }

        return allRate;
    }

    /// <summary>
    /// Clears all objects from the given list.
    /// </summary>
    private void ClearObjects(List<GameObject> objects)
    {
        foreach (GameObject obj in objects)
            Destroy(obj);
        objects.Clear();
    }

    // Debugging method for visualizing objects in the scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(0, 0), new Vector2(3, 3));
        Gizmos.DrawLine(new Vector2(3, 3), new Vector2(12, 12));
    }
}
