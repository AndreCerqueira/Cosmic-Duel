using Project.Runtime.Scripts.General;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class Planet : MonoBehaviour
{
    /* ───────── Enums & dados externos ───────── */
    public enum Difficulty { Easy, Medium, Hard, Boss }

    [Header("Referências externas")]
    [SerializeField] private ShipMover ship;
    [SerializeField] private LineRenderer line;
    [SerializeField] private FuelSystem fuel;

    private PlanetPopup popupInst;

    [Header("Shader Outline")]
    [SerializeField] private Color hoverColor = Color.white;
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color idleColor = new(1, 1, 1, 0);
    private Material auraMat;
    private static readonly int AURA_ID = Shader.PropertyToID("_OutlineColor");
    private static readonly int THICKNESS = Shader.PropertyToID("_OutlineThickness");

    [Header("Custos")]
    [SerializeField] private float costPerUnit = 10f;   // créditos por unidade

    [Header("Aparência depois de concluído")]
    [Range(0f, 1f)]
    private float completedBrightness = 0.50f;   // 0 = preto, 1 = normal


    /* ───────── estado ───────── */
    public int PlanetIndex { get; set; }
    public Difficulty difficulty;
    public bool hidden;

    public string PlanetName { get; private set; } = "Planet-X";   // valor default
    public void SetName(string n) => PlanetName = n;

    private Vector3 popupLocalOffset;

    private bool hovering, selected, completed, initialised;

    private void Awake()
    {
        auraMat = GetComponent<SpriteRenderer>().material;
        SetAura(idleColor);

        /* calcula o deslocamento: canto sup-dir + pequeno “margem” */
        var sr = GetComponent<SpriteRenderer>();
        float margin = 0.15f;                       // afasta um pouco (unidades)
        popupLocalOffset = new Vector3(sr.bounds.extents.x + margin,
                                       sr.bounds.extents.y + margin,
                                       0f);
    }

    /* ───────── Hover ───────── */
    private void OnMouseEnter()
    {
        if (completed) { ShowBanner(); return; }

        CursorManager.Instance.SetInteractCursor();

        hovering = true;
        if (!selected) SetAura(hoverColor);

        ShowBanner();

    }

    private void OnMouseExit()
    {
        hovering = false;
        if (!selected) SetAura(idleColor);
        
        CursorManager.Instance.SetDefaultCursor();

        PlanetBanner.Instance.Hide();
    }

    /* ───────── Clique / movimento ───────── */
    private void OnMouseDown()
    {
        if (completed) return;

        float dist = Vector3.Distance(transform.position, ship.transform.position);
        if (!fuel.TryConsumeForDistance(dist)) return;

        GameManager.Instance.shipPosition = ship.transform.position;
        ship.MoveTo(transform.position, OnShipArrived);

        selected = true;
        SetAura(selectedColor);
    }

    private void OnShipArrived()
    {
        StatusManager.Instance.SetFuel(fuel.CurrentFuel);
        GameManager.Instance.shipPosition = transform.position;
        GameManager.Instance.EnterPlanet(PlanetIndex);
    }

    /* ───────── Loop (actualiza popup) ───────── */
    private void Update()
    {
        if (!hovering || !initialised)
            return;

        UpdateBanner();
    }



    /* ---------- primeiro preenchimento ---------- */
    private void ShowBanner()
    {
        FillBanner();
        PlanetBanner.Instance.Show();
    }

    /* ---------- chamado no Update() ---------- */
    private void UpdateBanner()
    {
        FillBanner();                 // apenas actualiza se não concluído
    }

    /* ---------- lógica comum ---------- */
    private void FillBanner()
    {
        if (completed)
        {
            PlanetBanner.Instance.SetText(
                PlanetName,
                "0 <sprite name=\"iconsf6\"> ",               // sem distância
                "0",               // sem combustível
                "Concluído");
            return;
        }

        Vector3 shipPos = ship.transform.position;
        float dist = Vector3.Distance(shipPos, transform.position);
        float fuelUnits = dist * fuel.FuelPerUnit;
        float fuelPercent = fuelUnits / fuel.MaxFuel * 100f;

        PlanetBanner.Instance.SetText(
            PlanetName,
            FormatDistance(dist),
            $"-{fuelPercent:0} <sprite name=\"iconsf6\">",
            hidden ? "???" : difficulty.ToString());
    }



    /* ───────── Concluir planeta ───────── */
    public void MarkCompleted()
    {
        completed = true;
        hidden = false;                           // revela dificuldade

        Color tint = Color.white * completedBrightness;   // cria cinzento uniforme
        GetComponent<SpriteRenderer>().color = tint;


        if (popupInst != null) popupInst.gameObject.SetActive(false);

        var st = GameManager.Instance.planets[PlanetIndex];
        st.completed = true;
        st.hidden = false;
    }

    /* ───────── Helpers ───────── */
    private void SetAura(Color c)
    {
        auraMat.SetColor(AURA_ID, c);
        auraMat.SetFloat(THICKNESS, c.a < 0.01f ? 0f : 1f);
    }

    // devolve "123 ×10¹² km"   (usa sobrescrito)
    private static string FormatDistance(float units)
    {
        return $"{units:0} ×10<sup>12</sup> km";
    }


    /* ───────── Setup pelo spawner ───────── */
    public void Setup(ShipMover s, LineRenderer l, FuelSystem f,
                      Difficulty diff, bool hidden)
    {
        ship = s;
        line = l;
        fuel = f;
        difficulty = diff;
        this.hidden = hidden;
        initialised = true;
    }
}
