using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] _groundSprites;
    public Material[] skyboxes;
    
    
    void Start()
    {
        var color = AlienColorPalette.Instance.GetRandomColor();
        foreach (var sprite in _groundSprites)
        {
            sprite.color = color;
        }
        
        if (skyboxes.Length > 0)
        {
            int randomIndex = Random.Range(0, skyboxes.Length);
            RenderSettings.skybox = skyboxes[randomIndex];
        }
    }

}
