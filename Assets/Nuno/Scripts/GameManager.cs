using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Nomes das cenas")]
    [SerializeField] private string mapScene = "MapScene";
    [SerializeField] private string planetScene = "PlanetScene";

    /* --------- dados persistentes --------- */
    [System.Serializable]
    public class PlanetState
    {
        public Vector3 position;
        public int difficulty;
        public bool mystery;
        public bool completed;
    }

    public List<PlanetState> planets = new();  // preenchido pelo spawner
    public float currentFuel = 1000f;          // valor inicial

    public int CurrentPlanetIndex { get; private set; } = -1;
    public PlanetState CurrentPlanetState =>
        (CurrentPlanetIndex >= 0 && CurrentPlanetIndex < planets.Count)
        ? planets[CurrentPlanetIndex] : null;

    /* --------- singleton --------- */
    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    /* --------- mudanças de cena --------- */
    public void EnterPlanet(int index)
    {
        CurrentPlanetIndex = index;
        SceneManager.LoadScene(planetScene);
    }

    public void ExitPlanet()
    {
        if (CurrentPlanetState != null)
            CurrentPlanetState.completed = true;

        SceneManager.LoadScene(mapScene);
    }
}
