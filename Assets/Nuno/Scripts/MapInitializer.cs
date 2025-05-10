using UnityEngine;

public class MapInitializer : MonoBehaviour
{
    [SerializeField] public FuelSystem fuelSystem;   // arrasta o FuelSystem da cena

    private void Start()
    {
        GameManager gm = GameManager.Instance;
        var states = gm.planets;                  // lista persistente

        if (states.Count == 0) return;                // nada a restaurar

        // obtém todos os planetas já existentes na cena
        Planet[] planetsInScene = FindObjectsOfType<Planet>();

        foreach (Planet p in planetsInScene)
        {
            int idx = p.PlanetIndex;
            if (idx < 0 || idx >= states.Count) continue;

            var st = states[idx];

            // reposiciona
            p.transform.position = st.position;

            // esconde ícone se já concluído
            if (st.completed)
                p.HideDifficultyIcon();
        }

        // repõe combustível no slider
        fuelSystem.SetFuel(gm.currentFuel);
    }
}
