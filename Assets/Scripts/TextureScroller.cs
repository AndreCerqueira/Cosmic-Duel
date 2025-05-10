using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TextureScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.5f;

    private Material material;
    private float scrollValue = 0f;
    private static readonly int ScrollSpeedID = Shader.PropertyToID("_ScrollSpeed");

    void Start()
    {
        var renderer = GetComponent<SpriteRenderer>();
        material = renderer.material;

        // Criar instância única se necessário
        if (!Application.isPlaying || !material.name.EndsWith("(Instance)"))
        {
            material = new Material(material);
            renderer.material = material;
        }
    }

    void Update()
    {
        scrollValue += scrollSpeed * Time.deltaTime;
        material.SetFloat(ScrollSpeedID, scrollValue);
    }
}