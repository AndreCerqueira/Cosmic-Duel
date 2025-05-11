using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// Guarda e gere o combustível da nave.
public class FuelSystem : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Combustível máximo")]
    [SerializeField] private float maxFuel = 1000f;

    [Tooltip("Litros (ou unidades) gastos por unidade de distância")]
    [SerializeField] private float fuelPerUnit = 0.5f;   // 10 p/ 100 ⇒ 0.1

    [Header("UI")]
    [SerializeField] private Slider fuelSlider;
    [SerializeField] private TextMeshProUGUI fuelLabel;

    public float FuelPerUnit => fuelPerUnit;   // getter público
    public float MaxFuel => maxFuel;      // idem


    public float CurrentFuel { get; private set; }

    /* ---------- lifecycle ---------- */
    private void Awake()
    {
        CurrentFuel = maxFuel;
        fuelSlider.minValue = 0f;
        fuelSlider.maxValue = maxFuel;

        RefreshUI();          // ⬅️ função que põe slider + texto em sincronia
    }

    /* ---------- API ---------- */

    /// <summary>Calcula o custo e, se houver saldo, retira-o.  
    /// devolve true se a viagem pode ser feita.</summary>
    public bool TryConsumeForDistance(float distance)
    {
        float needed = distance * fuelPerUnit;
        if (CurrentFuel < needed) return false;

        CurrentFuel -= needed;
        RefreshUI();
        return true;
    }

    public void SetFuel(float value)
    {
        CurrentFuel = Mathf.Clamp(value, 0f, maxFuel);
        RefreshUI();
    }


    /* ---------- UI helper ---------- */
    public void RefreshUI()
    {
        fuelSlider.value = CurrentFuel;

        float percent = CurrentFuel / maxFuel * 100f;
        fuelLabel.text = $"{percent:0}%";          // 75 %
    }
}
