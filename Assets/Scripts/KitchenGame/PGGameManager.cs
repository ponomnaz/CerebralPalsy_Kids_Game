using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the Pouring Game, including pouring water into glasses, controlling game flow, and handling win/lose conditions.
/// </summary>
public class PGGameManager : MonoBehaviour
{
    public float CurGlasses;

    // UI
    [SerializeField] private GameObject _pouringGameMenu;
    [SerializeField] private GameObject _pouringGame;
    [SerializeField] private GameObject _winMenu;

    [SerializeField] private Holder _holderLeft;
    [SerializeField] private Holder _holderRight;
    [SerializeField] private Jug _jug;
    [SerializeField] private Glass _leftGlass;
    [SerializeField] private Glass _rightGlass;
    [SerializeField] private SpriteRenderer _waterSpawnArea;
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private GameObject _waterPrefab;

    private float _gameTime;
    private bool _play = false;

    private const float _maxGlasses = 5f;
    private const int _waterAmount = 450;
    private const float _waitTime = 2f;

    /// <summary>
    /// Starts the pouring game, initializes game state, and spawns water.
    /// </summary>
    public void StartPlaying()
    {
        _pouringGame.SetActive(true);

        SpawnWater();

        _leftGlass.Play(false);
        _rightGlass.Play(true);

        _jug.StartPouring();
        CurGlasses = 0;
        _gameTime = 0;
        _play = true;
    }

    public void Update()
    {
        if (_play)
        {
            _gameTime += Time.deltaTime;
            ControlJug();
        }
    }

    /// <summary>
    /// Controls the jug pouring process based on the holders' touch status.
    /// </summary>
    private void ControlJug()
    {
        if (_holderLeft.GetIsTouched() && _holderRight.GetIsTouched())
            _jug.ContinuePouring();
        else
            _jug.StopPouring();

        if (_jug.water >= _waterAmount)
            StartCoroutine(Win());
    }

    /// <summary>
    /// Handles the win condition. Waits for a short period and then shows the win panel.
    /// </summary>
    private IEnumerator Win()
    {
        _play = false;
        yield return new WaitForSeconds(_waitTime);
        EndPlaying();
        _pouringGameMenu.SetActive(false);
        _pouringGame.SetActive(false);
        _winMenu.SetActive(true);
    }

    /// <summary>
    /// Ends the pouring game, resets the state, and restores objects to their original positions.
    /// </summary>
    private void EndPlaying()
    {
        _jug.StopPouring();
        DeleteWater();
        _leftGlass.gameObject.transform.position = _leftGlass.originalPosition;
        _rightGlass.gameObject.transform.position = _rightGlass.originalPosition;
        _holderLeft.SetIsTouched(false);
        _holderRight.SetIsTouched(false);
        _holderLeft.RestoreColor();
        _holderRight.RestoreColor();
    }

    /// <summary>
    /// Deletes any remaining water objects from the scene.
    /// </summary>
    private void DeleteWater()
    {
        Liquid[] liquidOnScene = FindObjectsOfType<Liquid>();
        if (liquidOnScene != null)
        {
            for (int i = 0; i < liquidOnScene.Length; i++)
                Destroy(liquidOnScene[i].gameObject);
        }
    }

    /// <summary>
    /// Spawns water objects to fill the required amount.
    /// </summary>
    private void SpawnWater()
    {
        for (int i = 0; i < _waterAmount; i++)
            _spawnManager.Spawn(_waterPrefab, _waterSpawnArea.bounds.center, _waterSpawnArea.bounds.size, null);
    }

    public float GetGameTime() => _gameTime;

    public float GetGlassScore() => CurGlasses / _maxGlasses;
}
