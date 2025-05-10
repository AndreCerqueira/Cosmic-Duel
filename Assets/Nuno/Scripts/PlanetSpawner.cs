using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// Gera planetas no primeiro carregamento e, quando há dados
/// persistentes no GameManager, apenas os recria visualmente.
public class PlanetSpawner : MonoBehaviour
{
    /* ---------- Inspector ---------- */

    [Header("Câmara-alvo (vazio → Camera.main)")]
    [SerializeField] private Camera targetCamera;

    [Header("Prefab do planeta")]
    [SerializeField] private GameObject planetPrefab;

    [Header("Sprites possíveis")]
    [SerializeField] private List<Sprite> planetSprites;

    [Header("Quantos planetas criar (primeira vez)")]
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

    [Header("Dificuldade (primeira vez)")]
    [SerializeField] private int minDifficulty = 1;
    [SerializeField] private int maxDifficulty = 5;
    [SerializeField] private float mysteryChance = 0.2f;

    /* ---------- limites calculados ---------- */
    private float minX, maxX, minY, maxY;

    /* ---------- Awake ---------- */
    private void Awake()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        if (targetCamera == null || !targetCamera.orthographic)
        {
            Debug.LogError("[PlanetSpawner] Precisa de Camera ortográfica.");
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

        GameManager gm = GameManager.Instance;
        var states = gm.planets;

        /* --- A) Já existem estados salvos: reconstruir apenas a parte visual --- */
        if (states.Count > 0)
        {
            RebuildFromSavedData(states);
            fuelSystem.SetFuel(gm.currentFuel);   // usa o método que criaste no FuelSystem
            return;
        }

        /* --- B) Primeira vez: gerar planetas e registar estado --- */
        if (planetPrefab == null || planetSprites.Count == 0 || planetCount <= 0)
        {
            Debug.LogWarning("[PlanetSpawner] Faltam referências ou planetCount <= 0.");
            return;
        }

        List<Sprite> shuffled = new List<Sprite>(planetSprites);
        Shuffle(shuffled);

        for (int i = 0; i < planetCount; i++)
        {
            // tenta achar posição livre
            Vector3 pos;
            bool overlapped; int attempts = 0;

            do
            {
                pos = RandomPointInCamera();
                overlapped = Physics2D.OverlapCircle(pos, minDistance) != null;
            }
            while (overlapped && ++attempts < 20);

            if (overlapped) continue;

            int diff = Random.Range(minDifficulty, maxDifficulty + 1);
            bool myst = Random.value < mysteryChance;

            CreateAndRegisterPlanet(pos, diff, myst, shuffled[i % shuffled.Count]);
        }
    }

    /* ---------- Reconstrução visual ---------- */
    private void RebuildFromSavedData(List<GameManager.PlanetState> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var st = list[i];

            Planet p = CreatePlanetVisual(
                st.position,
                st.difficulty,
                st.mystery,
                planetSprites[i % planetSprites.Count],   // sprite qualquer
                i);                                      // índice original

            if (st.completed)
                p.HideDifficultyIcon();
        }
    }

    /* ---------- Cria só o GameObject ---------- */
    private Planet CreatePlanetVisual(Vector3 pos, int diff, bool mystery,
                                      Sprite sprite, int planetIndex)
    {
        GameObject go = Instantiate(planetPrefab, pos, Quaternion.identity);

        Planet p = go.GetComponent<Planet>();
        p.PlanetIndex = planetIndex;
        p.Setup(ship, line, costLabel, fuelSystem, fuelCostLabel, diff, mystery);

        go.GetComponent<SpriteRenderer>().sprite = sprite;
        return p;
    }

    /* ---------- Cria e REGISTA no GameManager ---------- */
    private void CreateAndRegisterPlanet(Vector3 pos, int diff, bool mystery, Sprite sprite)
    {
        GameManager gm = GameManager.Instance;

        Planet p = CreatePlanetVisual(pos, diff, mystery, sprite, gm.planets.Count);

        gm.planets.Add(new GameManager.PlanetState
        {
            position = pos,
            difficulty = diff,
            mystery = mystery,
            completed = false
        });
    }

    /* ---------- Utilidades ---------- */
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
