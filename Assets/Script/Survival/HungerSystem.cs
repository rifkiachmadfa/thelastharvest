using System;
using UnityEngine;

/// <summary>
/// Rasa kenyang pemain (hunger). Nilai 0 sampai MaxSatiety: 100 = kenyang, 0 = kelaparan.
/// Kenyang berkurang pelan selama waktu gameplay berjalan (ikut jeda TimeManager.IsPaused, jadi berhenti saat
/// transisi tidur). Pemain makan kentang dari tas (InventorySystem: tombol 1, lalu E); tiap kentang menambah kenyang.
/// Stok Food di storage tidak berkurang sendiri.
/// Bila kenyang 0, hitung mundur kelaparan (starvationGraceSeconds) berjalan; makan sebelum habis membatalkannya.
/// Game Over belum dibuat; yang ada baru event OnStarvationFailure (hanya log), seperti SuitSystem.OnSuitOxygenEmpty.
/// Hanya logika; tampilan ada di HudUI, aksi makan di InventorySystem, pengingat di HungerReminder.
/// </summary>
public class HungerSystem : MonoBehaviour
{
    public static HungerSystem Instance { get; private set; }

    [Header("Kenyang (USULAN, ubah lewat Inspector setelah playtest)")]
    [SerializeField, Min(1f)] private float maxSatiety = 100f;
    [SerializeField, Min(0f)] private float startSatiety = 100f;
    [Tooltip("Kenyang yang berkurang per detik gameplay. 0,08 = penuh habis sekitar 21 menit (sekitar 2 hari game).")]
    [SerializeField, Min(0f)] private float drainPerSecond = 0.08f;

    [Header("Makan (USULAN)")]
    [Tooltip("Kenyang yang didapat dari memakan satu kentang. 12,5 = 4 kentang (satu panen) mengisi 50 kenyang.")]
    [SerializeField, Min(0.1f)] private float satietyPerPotato = 12.5f;

    [Header("Peringatan dan kelaparan (USULAN)")]
    [SerializeField, Range(0f, 1f)] private float warnFraction = 0.3f;
    [SerializeField, Range(0f, 1f)] private float dangerFraction = 0.1f;
    [Tooltip("Detik yang tersedia untuk makan sejak kenyang 0 sebelum kondisi gagal.")]
    [SerializeField, Min(1f)] private float starvationGraceSeconds = 60f;

    private float starvationTimer;
    private bool failureReported;

    public float Satiety { get; private set; }
    public float MaxSatiety => maxSatiety;
    public float Fraction => maxSatiety > 0f ? Satiety / maxSatiety : 0f;
    public float WarnFraction => warnFraction;
    public float DangerFraction => dangerFraction;
    public float StarvationGraceSeconds => starvationGraceSeconds;
    public float SatietyPerPotato => satietyPerPotato;
    public bool IsStarving => Satiety <= 0f;
    public float StarvationSecondsLeft => Mathf.Max(0f, starvationGraceSeconds - starvationTimer);

    /// <summary>True bila pemain belum kenyang penuh.</summary>
    public bool CanEat => Satiety < maxSatiety - 0.5f;

    /// <summary>Dipanggil setiap kenyang berubah: (nilai baru, maksimum).</summary>
    public event Action<float, float> OnSatietyChanged;

    /// <summary>Dipanggil sekali ketika hitung mundur kelaparan habis (Game Over belum dibuat).</summary>
    public event Action OnStarvationFailure;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("HungerSystem: sudah ada instance lain, objek ini dihapus.", this);
            Destroy(this);
            return;
        }

        Instance = this;
        Satiety = Mathf.Clamp(startSatiety, 0f, maxSatiety);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        TimeManager time = TimeManager.Instance;
        if (time != null && time.IsPaused) return;

        if (Satiety > 0f)
        {
            float next = Mathf.Max(0f, Satiety - drainPerSecond * Time.deltaTime);
            if (!Mathf.Approximately(next, Satiety))
            {
                Satiety = next;
                OnSatietyChanged?.Invoke(Satiety, maxSatiety);
            }
        }

        if (Satiety > 0f) return;

        starvationTimer += Time.deltaTime;
        if (starvationTimer >= starvationGraceSeconds && !failureReported)
        {
            failureReported = true;
            Debug.LogWarning("HungerSystem: kelaparan melewati batas. (Game Over belum dibuat.)");
            OnStarvationFailure?.Invoke();
        }
    }

    /// <summary>
    /// Menambah kenyang (dipanggil InventorySystem saat pemain memakan kentang). Dibatasi maksimum.
    /// Mengembalikan jumlah kenyang yang benar-benar bertambah.
    /// </summary>
    public float Feed(float amount)
    {
        if (amount <= 0f) return 0f;

        float gained = Mathf.Min(amount, maxSatiety - Satiety);
        if (gained <= 0f) return 0f;

        Satiety += gained;
        starvationTimer = 0f;
        failureReported = false;
        OnSatietyChanged?.Invoke(Satiety, maxSatiety);
        return gained;
    }

    /// <summary>
    /// Menurunkan kenyang ke proporsi tertentu (0 sampai 1 dari maksimum) bila saat ini lebih tinggi; tidak pernah menaikkan.
    /// Dipakai TutorialManager untuk adegan "lapar" di Day 1 agar pemain merasakan lapar sebelum diminta menanam kentang.
    /// Mengembalikan true bila kenyang benar-benar turun.
    /// </summary>
    public bool LowerSatietyTo(float fraction)
    {
        float target = Mathf.Clamp01(fraction) * maxSatiety;
        if (target >= Satiety) return false;

        Satiety = target;
        OnSatietyChanged?.Invoke(Satiety, maxSatiety);
        return true;
    }

    [ContextMenu("Debug: Kenyang jadi 0")]
    private void DebugSetEmpty()
    {
        Satiety = 0f;
        OnSatietyChanged?.Invoke(Satiety, maxSatiety);
    }
}
