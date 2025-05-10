using UnityEngine;
using TMPro;

public class PlanetSceneController : MonoBehaviour
{
    [SerializeField] private TextMeshPro difficultyLabel;
    //[SerializeField] private FuelSystem fuel;   // se também gastas fuel aqui

    private void Start()
    {
        var state = GameManager.Instance.CurrentPlanetState;
        if (state != null)
        {
            difficultyLabel.text = state.mystery
                ? "???"
                : $"Dificuldade {state.difficulty}";
        }
    }

    public void OnBackButton()
    {
        //GameManager.Instance.currentFuel = fuel.CurrentFuel;  // se usou combustível aqui
        GameManager.Instance.ExitPlanet();
    }
}
