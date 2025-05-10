using UnityEngine;

/// Faz o planeta rodar e flutuar, com parâmetros
/// aleatórios por instância (útil p/ vários prefabs iguais).
public class PlanetFloatAndSpin : MonoBehaviour
{
    /* --------- Intervalos que ajustas no Inspector --------- */

    [Header("Velocidade de rotação (graus/seg)")]
    [SerializeField] private float rotationSpeedMin = -30f;   // negativo = horário
    [SerializeField] private float rotationSpeedMax = 30f;   // positivo = anti-horário

    [Header("Amplitudes de flutuação (máximo deslocamento)")]
    [SerializeField] private Vector2 amplitudeXRange = new Vector2(0.1f, 0.4f);
    [SerializeField] private Vector2 amplitudeYRange = new Vector2(0.1f, 0.4f);
    [SerializeField] private Vector2 amplitudeZRange = new Vector2(0f, 0.1f);

    [Header("Frequência da flutuação (ciclos/seg)")]
    [SerializeField] private Vector2 frequencyXRange = new Vector2(0.3f, 0.7f);
    [SerializeField] private Vector2 frequencyYRange = new Vector2(0.3f, 0.7f);
    [SerializeField] private Vector2 frequencyZRange = new Vector2(0.3f, 0.7f);

    /* --------- variáveis geradas por instância --------- */
    private float _rotationSpeed;
    private Vector3 _floatAmplitude;
    private Vector3 _floatFrequency;
    private Vector3 _phase;
    private Vector3 _startPos;

    /* ------------------ SETUP ------------------ */
    private void Awake()
    {
        _startPos = transform.position;

        /* 1) rotação inicial aleatória */
        float startAngle = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0, 0, startAngle);

        /* 2) velocidade de rotação (dentro do intervalo) */
        _rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax);

        /* 3) amplitudes e frequências da flutuação */
        _floatAmplitude = new Vector3(
            Random.Range(amplitudeXRange.x, amplitudeXRange.y),
            Random.Range(amplitudeYRange.x, amplitudeYRange.y),
            Random.Range(amplitudeZRange.x, amplitudeZRange.y));

        _floatFrequency = new Vector3(
            Random.Range(frequencyXRange.x, frequencyXRange.y),
            Random.Range(frequencyYRange.x, frequencyYRange.y),
            Random.Range(frequencyZRange.x, frequencyZRange.y));

        /* 4) fase aleatória para não começarem sincronizados */
        _phase = Random.insideUnitSphere * Mathf.PI * 2f;
    }

    /* ------------------ LOOP ------------------ */
    private void Update()
    {
        /* rotação contínua */
        transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime);

        /* flutuação em 3D (Lissajous) */
        float t = Time.time;

        float x = Mathf.Sin((t + _phase.x) * _floatFrequency.x * Mathf.PI * 2f) * _floatAmplitude.x;
        float y = Mathf.Sin((t + _phase.y) * _floatFrequency.y * Mathf.PI * 2f) * _floatAmplitude.y;
        float z = Mathf.Sin((t + _phase.z) * _floatFrequency.z * Mathf.PI * 2f) * _floatAmplitude.z;

        transform.position = _startPos + new Vector3(x, y, z);
    }
}
