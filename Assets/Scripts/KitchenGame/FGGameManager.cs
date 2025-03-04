using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the Frying Game, including spawning eggs, controlling game flow, and handling win/loss conditions.
/// </summary>
public class FGGameManager : MonoBehaviour
{
    public List<GameObject> eggs = new(); // List to hold all spawned eggs

    // UI
    [SerializeField] private GameObject _fryingGameMenu;
    [SerializeField] private GameObject _fryingGame;
    [SerializeField] private GameObject _winMenu;
    [SerializeField] private GameObject _loseMenu;

    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private Holder _holderLeft;
    [SerializeField] private Holder _holderRight;
    [SerializeField] private ShakeController _shakeController;
    [SerializeField] private ProgressBar _progressBar;
    [SerializeField] private List<GameObject> _eggPrefabes;

    [SerializeField] private SpriteRenderer _eggsSpawnArea;

    private float _gameTime = 0f;

    private float _savedEggsCount;

    private bool _cooking;
    private bool _playing;

    private const float _goal = 15f;
    private const float _maxEggs = 8f;

    /// <summary>
    /// Starts the frying game, initializes the game state, and spawns eggs.
    /// </summary>
    public void StartPlaying()
    {
        _fryingGame.SetActive(true);
        DeleteEggs();
        _shakeController.Play();
        _shakeController.curScore = 0;
        _shakeController.StartShaking();

        _progressBar.SetValue(0);
        _progressBar.SetMaxValue(_goal);

        _savedEggsCount = 0;
        SpawEggs();
        _cooking = false;
        _gameTime = 0;
        _playing = true;
    }

    public void Update()
    {
        if (_playing)
        {
            _gameTime += Time.deltaTime;
            ControlEggs();

            if (_shakeController.curScore >= _goal)
                Win();
            else if (eggs.Count <= 0)
                Lose();
        }
    }

    /// <summary>
    /// Spawns eggs within the designated area, up to the maximum allowed.
    /// </summary>
    private void SpawEggs()
    {
        for (int i = 0; i < _maxEggs; i++)
           _spawnManager.Spawn(RandomEggPrefabe(), _eggsSpawnArea.bounds.center, _eggsSpawnArea.bounds.size, eggs);
    }

    /// <summary>
    /// Deletes all eggs in the current game, cleaning up the scene.
    /// </summary>
    private void DeleteEggs()
    {
        eggs.Clear();
        Egg[] eggsOnScene = FindObjectsOfType<Egg>();
        if (eggsOnScene != null)
        {
            for (int i = 0; i < eggsOnScene.Length; i++) {
                eggsOnScene[i].SetMove(false);
                Destroy(eggsOnScene[i].gameObject);
            }
        }
    }

    /// <summary>
    /// Selects a random egg prefab to spawn.
    /// </summary>
    private GameObject RandomEggPrefabe() => _eggPrefabes[Random.Range(0, _eggPrefabes.Count)];

    /// <summary>
    /// Controls the egg cooking process based on user input and progress.
    /// </summary>
    private void ControlEggs()
    {
        if (_holderLeft.GetIsTouched() && _holderRight.GetIsTouched())
        {
            if (!_cooking)
            {
                SetEggsMove(true);
                _cooking = true;
                _shakeController.shake = true;
            }
            _progressBar.SetValue(_shakeController.curScore);
        }
        else
        {
            if (_cooking)
            {
                SetEggsMove(false);
                _cooking = false;
                _shakeController.shake = false;
            }
        }
    }

    private void SetEggsMove(bool move)
    {
        foreach (var e in eggs)
            e.GetComponent<Egg>().SetMove(move);
    }

    public float GetGameTime() => _gameTime;

    public float GetEggsScore() => _savedEggsCount / _maxEggs;

    /// <summary>
    /// Ends the frying game and resets the game state.
    /// </summary>
    private void EndPlaying()
    {
        _savedEggsCount = eggs.Count;
        DeleteEggs();

        _playing = false;
        _shakeController.shake = false;
        _holderLeft.SetIsTouched(false);
        _holderRight.SetIsTouched(false);
        _holderLeft.RestoreColor();
        _holderRight.RestoreColor();
        _shakeController.StopShaking();
        SetEggsMove(false);
        _cooking = false;

        _fryingGameMenu.SetActive(false);
        _fryingGame.SetActive(false);
    }

    /// <summary>
    /// Handles the win condition, showing the win menu and updating stats.
    /// </summary>
    private void Win()
    {
        EndPlaying();
        _winMenu.SetActive(true);
    }

    /// <summary>
    /// Handles the lose condition, showing the lose menu.
    /// </summary>
    private void Lose()
    {
        EndPlaying();
        _loseMenu.SetActive(true);
    }
}
