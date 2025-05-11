using MoreMountains.Feedbacks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Nomes das cenas")]
    [SerializeField] private string mapScene = "MapScene";
    [SerializeField] private string planetScene = "PlanetScene";

    [SerializeField] private MMF_Player _changeToPlanetSceneFeedback;
    [SerializeField] private MMF_Player _changeToMapSceneFeedback;

    public Vector3 shipPosition;   // posição do Ship no mapa

    [System.Serializable]
    public class SpaceObjectState
    {
        public Vector3 position;
        public int spriteIndex;
    }

    public List<SpaceObjectState> spaceObjects = new();



    /* --------- dados persistentes --------- */
    [System.Serializable]
    public class PlanetState
    {
        public Vector3 position;
        public Planet.Difficulty difficulty;
        public bool hidden;
        public bool completed;
        public int spriteIndex;
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
        Debug.Log($"[GM] Entrar no planeta {index}");
        CurrentPlanetIndex = index;
        _changeToPlanetSceneFeedback?.PlayFeedbacks();
    }

    public void ExitPlanet()
    {
        Debug.Log($"[GM] Sair do planeta {CurrentPlanetIndex} — marcar concluído");
        if (CurrentPlanetState != null)
            CurrentPlanetState.completed = true;

        shipPosition = CurrentPlanetState.position;

        _changeToMapSceneFeedback?.PlayFeedbacks();
    }
}
