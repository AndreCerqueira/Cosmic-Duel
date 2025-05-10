using UnityEngine;

public class MapInitializer : MonoBehaviour
{
    [SerializeField] public FuelSystem fuelSystem;   // arrasta o FuelSystem da cena

    private void Start()
    {
        GameManager gm = GameManager.Instance;
        var states = gm.planets;                  // lista persistente

        if (states.Count == 0) return;                // nada a restaurar

        // obt�m todos os planetas j� existentes na cena
        Planet[] planetsInScene = FindObjectsOfType<Planet>();

        foreach (Planet p in planetsInScene)
        {
            int idx = p.PlanetIndex;
            if (idx < 0 || idx >= states.Count) continue;

            var st = states[idx];

            // reposiciona
            p.transform.position = st.position;

            // esconde �cone se j� conclu�do
            if (st.completed)
                p.HideDifficultyIcon();
        }

        // rep�e combust�vel no slider
        fuelSystem.SetFuel(gm.currentFuel);
    }
}
