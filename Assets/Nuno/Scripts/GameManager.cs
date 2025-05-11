using MoreMountains.Feedbacks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event System.Action PlanetCompleted;

    public void NotifyPlanetCompleted() => PlanetCompleted?.Invoke();

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

    public bool IsAnyPlanetReachable(Vector3 shipPos,
                                 float fuelLeftPercent,   // gasolina que tens em %
                                 float fuelPerUnit,       // unidades por unidade de dist.
                                 float maxFuel)           // depósito cheio em unidades
    {
        foreach (var st in planets)
        {
            if (st.completed) continue;

            float distUnits = Vector3.Distance(shipPos, st.position);
            float needUnits = distUnits * fuelPerUnit;
            float needPercent = needUnits;// / maxFuel * 100f;

            Debug.Log($"[GM] {st.difficulty} - {needPercent}% - {fuelLeftPercent}%");

            if (needPercent <= fuelLeftPercent)
                return true;           // existe pelo menos 1 destino
        }
        return false;                  // nenhum planeta é alcançável
    }

    internal void ResetGame()
    {
        Debug.Log("[GM] Reset Game");
        planets.Clear();
        spaceObjects.Clear();
        currentFuel = 1000f;
        CurrentPlanetIndex = -1;
        shipPosition = Vector3.zero;   // reposicionar nave no mapa

    }
}
