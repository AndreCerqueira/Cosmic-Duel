using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanetPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text distText;
    [SerializeField] private TMP_Text fuelText;
    [SerializeField] private TMP_Text diffText;

    public void SetData(string planet, string dist, string fuel, string diffSprite, string diffName)
    {
        nameText.text = planet;
        distText.text = dist;
        fuelText.text = fuel;

        // ex.: diffSprite = "easy"  diffName = "Easy"
        diffText.text = $"<sprite name={diffSprite}> {diffName}";
    }
}
