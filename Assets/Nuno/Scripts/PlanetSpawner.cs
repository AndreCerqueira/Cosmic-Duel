using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// Instancia planetas em posições aleatórias dentro da câmara,
/// respeitando um padding às bordas. Só cria se ainda não existirem
/// estados guardados no GameManager.
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

    [Header("Padding (borda visível)")]
    [SerializeField] private Vector2 padding = new Vector2(1f, 1f);

    [Header("Dependências")]
    [SerializeField] private ShipMover ship;
    [SerializeField] private LineRenderer line;
    [SerializeField] private TextMeshPro costLabel;
    [SerializeField] private FuelSystem fuelSystem;
    [SerializeField] private TextMeshPro fuelCostLabel;

    [Header("Dificuldade")]
    [SerializeField] private int minDifficulty = 1;
    [SerializeField] private int maxDifficulty = 5;
    [SerializeField] private float mysteryChance = 0.2f;

    /* ---------- rectângulo calculado ---------- */
    private float minX, maxX, minY, maxY;

    /* ---------- Awake ---------- */
    private void Awake()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        if (targetCamera == null || !targetCamera.orthographic)
        {
            Debug.LogError("PlanetSpawner: precisa de Camera ortográfica.");
            enabled = false; return;
        }

        float halfH = targetCamera.orthographicSize;
        float halfW = halfH * targetCamera.aspect;
        Vector3 cam = targetCamera.transform.position;

        minX = cam.x - halfW + padding.x;
        maxX = cam.x + halfW - padding.x;
        minY = cam.y - halfH + padding.y;
        maxY = cam.y + halfH - padding.y;
    }

    /* ---------- Start ---------- */
    private void Start()
    {
        if (!enabled) return;

        // ─── 0) já existem planetas guardados? então não cria nada ───
        if (GameManager.Instance != null && GameManager.Instance.planets.Count > 0)
            return;

        // ─── 1) validações ───
        if (planetPrefab == null || planetSprites.Count == 0 || planetCount <= 0)
        {
            Debug.LogWarning("PlanetSpawner: faltam referências ou planetCount <= 0.");
            return;
        }

        // baralha sprites para não repetir padrão
        List<Sprite> shuffled = new List<Sprite>(planetSprites);
        Shuffle(shuffled);

        // ─── 2) instanciar planetas ───
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

            // criar planeta
            GameObject go = Instantiate(planetPrefab, pos, Quaternion.identity);

            // dificuldade & mistério
            int diff = Random.Range(minDifficulty, maxDifficulty + 1);
            bool mystery = Random.value < mysteryChance;

            // índice real dentro do GameManager (antes de adicionar)
            int planetIndex = GameManager.Instance.planets.Count;

            // configurar script Planet
            Planet p = go.GetComponent<Planet>();
            p.PlanetIndex = planetIndex;
            p.Setup(ship, line, costLabel, fuelSystem, fuelCostLabel, diff, mystery);

            // sprite visual
            go.GetComponent<SpriteRenderer>().sprite = shuffled[i % shuffled.Count];

            // guardar estado persistente
            GameManager.Instance.planets.Add(new GameManager.PlanetState
            {
                position = pos,
                difficulty = diff,
                mystery = mystery,
                completed = false
            });
        }
    }

    /* ---------- helpers ---------- */
    private Vector3 RandomPointInCamera() =>
        new Vector3(Random.Range(minX, maxX),
                    Random.Range(minY, maxY),
                    0f);

    private void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
