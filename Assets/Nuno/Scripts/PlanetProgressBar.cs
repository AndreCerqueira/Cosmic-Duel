using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanetProgressBar : MonoBehaviour
{
    [SerializeField] private Slider slider;    // arrasta o Fill Slider
    [SerializeField] private TMP_Text label;     // opcional x / total

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
        int total = list.Count;
        int completed = 0;

        foreach (var st in list)
            if (st.completed) completed++;

        slider.value = (float)completed / total;

        if (label)
            label.text = $"{completed} / {total}";
    }
}
