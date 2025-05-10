using UnityEngine;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class Planet : MonoBehaviour
{
    /* ---------- Inspector ---------- */
    [Header("Referências externas")]
    [SerializeField] private ShipMover ship;
    [SerializeField] private LineRenderer line;
    [SerializeField] private TextMeshPro costLabel;
    [SerializeField] private FuelSystem fuel;
    [SerializeField] private TextMeshPro fuelCostLabel;

    [Header("Shader property")]
    [SerializeField] private string auraColorProperty = "_OutlineColor";

    [Header("Cores")]
    [SerializeField] private Color hoverColor = Color.white;
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color idleColor = new Color(1, 1, 1, 0);

    [Header("Economia")]
    [SerializeField] private float costPerUnit = 10f;
    [SerializeField] private float dashLength = 0.25f;

    [Header("UI do custo de combustível")]
    [SerializeField] private Color fuelLabelColor = Color.red;

    /* ---------- Icons ---------- */
    [Header("Dificuldade")]
    [SerializeField] private SpriteRenderer difficultyIconRenderer; // filho DifficultyIcon
    [SerializeField] private Sprite mysterySprite;
    [SerializeField] private List<Sprite> difficultySprites;      // index 0→diff1 … 4→diff5
    [SerializeField] private Vector3 difficultyIconOffset = new Vector3(1f, 0f, 0f); // ajuste vertical
    [SerializeField] private int sortingOrderBoost = 2;

    private int difficulty;   // 1-5
    private bool isMystery;    // true → mostra ?

    /* ---------- estado ---------- */
    private bool hovering;
    private bool selected;
    private bool isInitialized;

    private Material auraMat;
    private static Planet currentTarget;
    private static readonly int AURA_ID = Shader.PropertyToID("_OutlineColor");

    /* ---------- cache de texto ---------- */
    private string cachedCostText = "";
    private string cachedFuelText = "";

    /* ---------- SETUP ---------- */
    private void Awake()
    {
        auraMat = GetComponent<SpriteRenderer>().material;
        SetAura(idleColor);

        if (difficultyIconRenderer != null)
        {
            // 1) volta a ser filho para seguir posição
            difficultyIconRenderer.transform.SetParent(transform, false);

            // 2) centra-se exactamente sobre o planeta
            difficultyIconRenderer.transform.localPosition = Vector3.zero;

            // 3) reduz tamanho
            difficultyIconRenderer.transform.localScale = Vector3.one * 0.6f;

            // 4) mantém na vertical mesmo que o planeta rode
            difficultyIconRenderer.transform.rotation = Quaternion.identity;

            // 5) garante que fica à frente do sprite do planeta
            int baseOrder = GetComponent<SpriteRenderer>().sortingOrder;
            difficultyIconRenderer.sortingOrder = baseOrder + sortingOrderBoost;
        }
    }

    /* ---------- EVENTOS DE RATO ---------- */
    private void OnMouseEnter()
    {
        hovering = true;
        if (!selected) SetAura(hoverColor);

        line.enabled = true;
        costLabel.gameObject.SetActive(true);
        fuelCostLabel.gameObject.SetActive(true);

        RefreshInfo();                 // <-- calcula só uma vez
    }

    private void OnMouseExit()
    {
        hovering = false;
        if (!selected) SetAura(idleColor);

        line.enabled = false;
        costLabel.gameObject.SetActive(false);
        fuelCostLabel.gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        float distance = Vector3.Distance(transform.position, ship.transform.position);

        if (!fuel.TryConsumeForDistance(distance)) return;

        if (currentTarget != null && currentTarget != this)
            currentTarget.Deselect();

        currentTarget = this;
        selected = true;
        SetAura(selectedColor);

        ship.MoveTo(transform.position);

        //RefreshInfo();                 // recalcula se quiseres valor exacto após clique
    }

    /* ---------- LOOP ---------- */
    private void Update()
    {
        if (!hovering || !isInitialized) return;

        Vector3 shipPos = ship.transform.position;
        Vector3 planetPos = transform.position;

        // Actualiza apenas posições da UI e linha
        fuelCostLabel.transform.position = shipPos + Vector3.up * 1f;
        costLabel.transform.position = planetPos + Vector3.down * 20f;


        /* --- reposiciona o ícone sem rotação --- *//*
        if (difficultyIconRenderer != null)
        {
            difficultyIconRenderer.transform.position = planetPos + difficultyIconOffset;
            difficultyIconRenderer.transform.rotation = Quaternion.identity; // mantém na vertical
        }
        */
        float distReal = Vector3.Distance(shipPos, planetPos);
        UpdateLine(shipPos, planetPos, distReal);
    }

    private void LateUpdate()
    {
        // garante que o ícone fica sempre na vertical
        if (difficultyIconRenderer != null)
            difficultyIconRenderer.transform.rotation = Quaternion.identity;
    }

    /* ---------- cálculo pontual ---------- */
    private void RefreshInfo()
    {
        Vector3 shipPos = ship.transform.position;
        Vector3 planetPos = transform.position;
        float dist = Vector3.Distance(shipPos, planetPos);

        // custo em créditos
        float cost = dist * costPerUnit;
        cachedCostText = $"{cost:0} ×10¹² km";
        costLabel.text = cachedCostText;

        // combustível
        float fuelNeeded = dist * fuel.FuelPerUnit;
        float percentSpend = fuelNeeded / fuel.MaxFuel * 100f;
        int percInt = Mathf.CeilToInt(percentSpend);
        cachedFuelText = $"-{percInt} <voffset=0.9em><sprite name=fuel></voffset>";
        fuelCostLabel.text = cachedFuelText;
    }

    /* ---------- HELPERS ---------- */
    private void Deselect()
    {
        selected = false;
        SetAura(hovering ? hoverColor : idleColor);
    }

    private void SetAura(Color c) => auraMat.SetColor(AURA_ID, c);

    private void UpdateLine(Vector3 a, Vector3 b, float dist)
    {
        line.positionCount = 2;
        line.SetPosition(0, a);
        line.SetPosition(1, b);

        line.textureMode = LineTextureMode.Tile;
        line.material.mainTextureScale = new Vector2(dist / dashLength, 1f);
    }

    private void UpdateDifficultyIcon()
    {
        if (difficultyIconRenderer == null) return;

        if (isMystery)
        {
            difficultyIconRenderer.sprite = mysterySprite;
        }
        else
        {
            int index = Mathf.Clamp(difficulty - 1, 0, difficultySprites.Count - 1);
            difficultyIconRenderer.sprite = difficultySprites[index];
        }
    }



    /* ---------- API usado pelo Spawner ---------- */
    public void Setup(ShipMover s, LineRenderer l, TextMeshPro label,
                      FuelSystem f, TextMeshPro fuelCostLabel, int diff, bool mystery)
    {
        ship = s;
        line = l;
        costLabel = label;
        fuel = f;
        this.fuelCostLabel = fuelCostLabel;
        this.fuelCostLabel.color = fuelLabelColor;

        difficulty = Mathf.Clamp(diff, 1, 5);
        isMystery = mystery;

        UpdateDifficultyIcon();

        isInitialized = true;
    }
}
