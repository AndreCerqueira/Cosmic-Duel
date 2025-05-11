using TMPro;
using UnityEngine;

public class PlanetBanner : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text distText;
    [SerializeField] private TMP_Text fuelText;
    [SerializeField] private TMP_Text diffText;

    /*  Singleton simples  */
    public static PlanetBanner Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);          // começa escondido
    }

    public void SetText(string name, string dist, string fuel, string diff)
    {
        nameText.text = name;
        distText.text = dist;
        fuelText.text = fuel;
        diffText.text = diff;
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

}
