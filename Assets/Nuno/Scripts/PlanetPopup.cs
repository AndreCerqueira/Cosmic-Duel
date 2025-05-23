using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanetPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text distText;
    [SerializeField] private TMP_Text fuelText;
    [SerializeField] private TMP_Text diffText;


    public void SetData(string planet, string dist, string fuel, string diffName)
    {
        nameText.text = planet;
        distText.text = dist;
        fuelText.text = fuel;
        diffText.text = diffName;

    }
}
