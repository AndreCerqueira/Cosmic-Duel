using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// Instancia planetas em posições aleatórias dentro da câmara,
/// respeitando um “padding” às bordas.
public class PlanetSpawner : MonoBehaviour
{
    /* ---------- Inspector ---------- */

    [Header("Câmara-alvo (vazio → Camera.main)")]
    [SerializeField] private Camera targetCamera;

    [Header("Prefab do planeta")]
    [SerializeField] private GameObject planetPrefab;

    [Header("Sprites possíveis")]
    [SerializeField] private List<Sprite> planetSprites;

    [Header("Quantos planetas criar")]
    [SerializeField] private int planetCount = 5;

    [Header("Distância mínima entre planetas")]
    [SerializeField] private float minDistance = 1f;

    [Header("Padding (recuo a partir da borda visível)")]
    [SerializeField] private Vector2 padding = new Vector2(1f, 1f);

    [Header("Dependências")]
    [SerializeField] private ShipMover ship;
    [SerializeField] private LineRenderer line;
    [SerializeField] private TextMeshPro costLabel;
    [SerializeField] private FuelSystem fuelSystem;
    [SerializeField] private TextMeshPro fuelCostLabel;

    /* ---------- rectângulo calculado ---------- */
    private float minX, maxX, minY, maxY;

    /* ---------- ciclo ---------- */
    private void Awake()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        if (targetCamera == null)
        {
            Debug.LogError("PlanetSpawner: não encontrou Camera.");
            enabled = false; return;
        }

        if (!targetCamera.orthographic)
        {
            Debug.LogError("PlanetSpawner: script pensado para câmara ortográfica.");
            enabled = false; return;
        }

        /* 1) calcular as bordas internas com padding */
        float halfHeight = targetCamera.orthographicSize;
        float halfWidth = halfHeight * targetCamera.aspect;

        Vector3 camPos = targetCamera.transform.position;

        minX = camPos.x - halfWidth + padding.x;
        maxX = camPos.x + halfWidth - padding.x;
        minY = camPos.y - halfHeight + padding.y;
        maxY = camPos.y + halfHeight - padding.y;
    }

    private void Start()
    {
        if (!enabled) return;

        if (planetPrefab == null || planetSprites.Count == 0 || planetCount <= 0)
        {
            Debug.LogWarning("PlanetSpawner: faltam referências ou planetCount ≤ 0.");
            return;
        }

        /* baralhar sprites */
        List<Sprite> shuffled = new List<Sprite>(planetSprites);
        Shuffle(shuffled);

        /* instanciar */
        for (int i = 0; i < planetCount; i++)
        {
            Vector3 pos;
            bool overlapped; int attempts = 0;

            do
            {
                pos = RandomPointInCamera();
                overlapped = Physics2D.OverlapCircle(pos, minDistance) != null;
            }
            while (overlapped && ++attempts < 20);

            if (overlapped)
            {
                Debug.LogWarning($"PlanetSpawner: não encontrou espaço p/ planeta {i}.");
                continue;
            }
            var planet = Instantiate(planetPrefab, pos, Quaternion.identity);
            planet.GetComponent<Planet>()
                  .Setup(ship, line, costLabel, fuelSystem, fuelCostLabel);
            /*
            var planet = Instantiate(planetPrefab, pos, Quaternion.identity);
            planet.GetComponent<Planet>().Setup(ship, line, costLabel);
            */
            Sprite chosen = shuffled[i % shuffled.Count];
            planet.GetComponent<SpriteRenderer>().sprite = chosen;
        }
    }

    /* ---------- helper ---------- */
    private Vector3 RandomPointInCamera() =>
        new Vector3(Random.Range(minX, maxX),
                    Random.Range(minY, maxY),
                    0f);                    // Z fixo em 2D

    private void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
