using UnityEngine;

public class MapInitializer : MonoBehaviour
{
    [SerializeField] private FuelSystem fuelSystem;
    [SerializeField] private ShipMover ship;

    private void Start()
    {
        GameManager gm = GameManager.Instance;
        var states = gm.planets;          // lista persistente

        if (states.Count == 0)
        {
            Debug.Log("[MapInit] Nenhum estado salvo — nada para repor");
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

            /* posição */
            p.transform.position = st.position;

            /* concluído? -> bloquear interação + mostrar dificuldade real */
            if (st.completed)
            {
                p.MarkCompleted();            // já faz HideDifficultyIcon()
            }
            else
            {
                // garante que flag hidden coincide (para ícone ? ou não)
                p.hidden = st.hidden;
            }
        }

        /* ---------- combustível ---------- */
        fuelSystem.SetFuel(gm.currentFuel);
    }
}
