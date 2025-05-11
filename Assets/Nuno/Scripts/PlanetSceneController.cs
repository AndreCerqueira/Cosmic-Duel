using UnityEngine;
using TMPro;

public class PlanetSceneController : MonoBehaviour
{
    [SerializeField] private TextMeshPro difficultyLabel;

    // Se gasta combustível dentro da PlanetScene, expõe isso aqui
    //[SerializeField] private FuelSystem fuelSystem;

    private GameManager.PlanetState state;

    /* ---------- Start ---------- */
    private void Start()
    {
        state = GameManager.Instance.CurrentPlanetState;
        if (state == null) return;

        difficultyLabel.text = state.hidden
            ? "???"
            : DifficultyToText(state.difficulty);
    }

    /* ---------- Botão Voltar ---------- */
    public void OnBackButton()
    {
        // Se consumiste fuel aqui, guarda antes de sair:
        // GameManager.Instance.currentFuel = fuelSystem.CurrentFuel;

        state.completed = true;
        state.hidden = false;              // revela para o regresso
        GameManager.Instance.ExitPlanet();
    }

    /* ---------- util ---------- */
    private static string DifficultyToText(Planet.Difficulty d) =>
        d switch
        {
            Planet.Difficulty.Easy => "Easy",
            Planet.Difficulty.Medium => "Medium",
            Planet.Difficulty.Hard => "Hard",
            Planet.Difficulty.Boss => "BOSS",
            _ => "?"
        };
}
