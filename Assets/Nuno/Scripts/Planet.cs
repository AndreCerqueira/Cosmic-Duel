using UnityEngine;
using TMPro;

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
    //[SerializeField] private Vector3 fuelLabelOffset = new Vector3(3f, 0f, 0f);   // ↑ deslocamento
    [SerializeField] private Color fuelLabelColor = Color.red;                 // cor do texto



    /* ---------- estado ---------- */
    private bool hovering;
    private bool isInitialized;
    private bool selected;

    private Material auraMat;

    private static Planet currentTarget;          // <- planeta destino global
    private static readonly int AURA_ID = Shader.PropertyToID("_OutlineColor");

    /* ---------- SETUP ---------- */
    private void Awake()
    {
        auraMat = GetComponent<SpriteRenderer>().material;   // cópia p/ instância
        SetAura(idleColor);
    }

    /* ---------- EVENTOS DE RATO ---------- */
    private void OnMouseEnter()
    {


        hovering = true;
        if (!selected) SetAura(hoverColor);

        line.enabled = true;
        costLabel.gameObject.SetActive(true);
        fuelCostLabel.gameObject.SetActive(true);

    }

    private void OnMouseExit()
    {

        // onde já ligas costLabel & line

        hovering = false;
        if (!selected) SetAura(idleColor);

        line.enabled = false;
        costLabel.gameObject.SetActive(false);
        fuelCostLabel.gameObject.SetActive(false);

    }

    private void OnMouseDown()
    {
        float distance = Vector3.Distance(transform.position, ship.transform.position);

        // pergunta ao FuelSystem se há combustível
        if (!fuel.TryConsumeForDistance(distance)) return;   // sem fuel ⇒ não viaja

        /* 1) dizer ao destino antigo para desmarcar-se */
        if (currentTarget != null && currentTarget != this)
            currentTarget.Deselect();

        /* 2) este passa a ser o novo destino */
        currentTarget = this;
        selected = true;
        SetAura(selectedColor);

        ship.MoveTo(transform.position);
    }

    /* ---------- LOOP ---------- */
    private void Update()
    {
        if (!hovering) return;
        if (!isInitialized) return;

        Vector3 shipPos = ship.transform.position;
        Vector3 planetPos = transform.position;

        float dist = Vector3.Distance(shipPos, planetPos);
        float cost = dist * costPerUnit;

        // ----- combustível que vai gastar -----
        float fuelNeeded = dist * fuel.FuelPerUnit;          // torna FuelPerUnit público (getter) ou expõe
        float percentSpend = fuelNeeded / fuel.MaxFuel * 100f; // % do reservatório total

        fuelCostLabel.text = $"-{percentSpend:0}%";
        fuelCostLabel.transform.position = shipPos + Vector3.up * 10f; // 2 unidades acima da nave
        //fuelCostLabel.transform.position = shipPos + fuelLabelOffset;


        UpdateLine(shipPos, planetPos, dist);

        costLabel.text = $"{cost:0} ×10¹² km";
        costLabel.transform.position = planetPos + Vector3.up * 4f;
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

    /* ---------- API usado pelo Spawner ---------- */
    public void Setup(ShipMover s, LineRenderer l, TextMeshPro label, FuelSystem f, TextMeshPro fuelCostLabel)
    {
        ship = s;
        line = l;
        costLabel = label;
        fuel = f;
        this.fuelCostLabel = fuelCostLabel;
        this.fuelCostLabel.color = fuelLabelColor;

        isInitialized = true;
    }
}
