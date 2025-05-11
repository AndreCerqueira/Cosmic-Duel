using UnityEngine;

public class MapInitializer : MonoBehaviour
{
    [SerializeField] private FuelSystem fuelSystem;
    [SerializeField] private ShipMover ship;

    [Header("Perdeu o jogo")]
    [SerializeField] private GameObject gameOverPop;

    private void Start()
    {
        GameManager gm = GameManager.Instance;
        var states = gm.planets;          // lista persistente

        if (states.Count == 0)
        {
            Debug.Log("[MapInit] Nenhum estado salvo � nada para repor");
            return;
        }

        /* ---------- nave ---------- */
        ship.transform.position = gm.shipPosition;
        Debug.Log($"[MapInit] Nave movida para {gm.shipPosition}");

        /* ---------- planetas na cena ---------- */
        Planet[] planetsInScene = FindObjectsOfType<Planet>();

        foreach (Planet p in planetsInScene)
        {
            int idx = p.PlanetIndex;
            if (idx < 0 || idx >= states.Count) continue;

            GameManager.PlanetState st = states[idx];

            /* posi��o */
            p.transform.position = st.position;

            /* conclu�do? -> bloquear intera��o + mostrar dificuldade real */
            if (st.completed)
            {
                p.MarkCompleted();            // j� faz HideDifficultyIcon()
            }
            else
            {
                // garante que flag hidden coincide (para �cone ? ou n�o)
                p.hidden = st.hidden;
            }
        }
        FuelSystem fs = fuelSystem;  // ref local só p/ legibilidade

        bool reachable = GameManager.Instance.IsAnyPlanetReachable(
                        ship.transform.position,
                        fs.CurrentFuel,  // % que resta
                        fs.FuelPerUnit,
                        fs.MaxFuel);


        if (!reachable)
        {
            Debug.Log("[MapInit] nenhum planeta alcançável — GAME OVER");
            gameOverPop.SetActive(true);      // mostra painel
            ship.enabled = false;             // opcional: bloqueia nave
        }
        /* ---------- combust�vel ---------- */
        //fuelSystem.SetFuel(gm.currentFuel);
    }
}
