using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanetProgressBar : MonoBehaviour
{
    [SerializeField] private Slider slider;    // arrasta o Fill Slider
    [SerializeField] private TMP_Text label;     // opcional x / total
    [SerializeField] private PlanetSpawner planetSpawner;

    private void OnEnable()
    {
        Refresh();   // primeiro refresh
        GameManager.Instance.PlanetCompleted += Refresh;
    }

    private void OnDisable()
    {
        GameManager.Instance.PlanetCompleted -= Refresh;
    }

    private void Refresh()
    {
        var list = GameManager.Instance.planets;
        int total = planetSpawner.PlanetCount;
        int completed = 0;

        foreach (var st in list)
            if (st.completed) completed++;

        slider.value = (float)completed / total;
        
        Debug.Log($"[PlanetProgressBar] {completed} / {total} ({slider.value})");

        if (label)
            label.text = $"{completed} / {total}";
    }
}
