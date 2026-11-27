using System;
using UnityEngine;

/// <summary>
/// Satu Solar Panel (tampilan) di Outdoor. SEMUA Solar Panel di scene berbagi SATU status rusak/normal
/// (keputusan developer 2026-11-27): bila rusak, seluruh panel berhenti menambah Energy sampai diperbaiki lewat
/// RepairPoint; bila normal, tiap panel menambah Energy dan hasilnya saling menjumlah. Jumlah objek panel di scene
/// hanya urusan tampilan: semua panel ikut berubah sprite/warna saat status berubah.
/// Produksi ikut jeda TimeManager.IsPaused (transisi tidur). Kerusakan dipicu SolarPanelDamageScheduler (satu kejadian
/// per hari) atau menu konteks Debug di Inspector. Pemberitahuan ke pemain lewat TARSSystem.
/// Angka produksi adalah USULAN.
/// </summary>
public class SolarPanel : MonoBehaviour
{
    [Header("Produksi (USULAN, ubah lewat Inspector setelah playtest)")]
    [SerializeField, Min(0f)] private float normalEnergyPerSecond = 0.5f;
    [Tooltip("Energy ditambahkan ke stok setiap sekian detik (agar event HUD tidak terpicu tiap frame).")]
    [SerializeField, Min(0.1f)] private float tickSeconds = 1f;
    [Tooltip("Bila dicentang pada salah satu panel, seluruh Solar Panel mulai dalam keadaan rusak.")]
    [SerializeField] private bool startDamaged;

    [Header("Tampilan opsional")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite damagedSprite;
    [Tooltip("Dikalikan ke warna asli SpriteRenderer saat rusak, hanya bila Damaged Sprite kosong.")]
    [SerializeField] private Color damagedTint = new Color(1f, 0.45f, 0.35f);

    private Color baseColor = Color.white;
    private float tickTimer;

    /// <summary>True bila Solar Panel (seluruh susunan panel) sedang rusak.</summary>
    public static bool ArrayDamaged { get; private set; }

    /// <summary>Dipanggil sekali saat status rusak seluruh Solar Panel berubah (parameter: true bila rusak).</summary>
    public static event Action<bool> OnArrayDamagedChanged;

    /// <summary>Sama dengan ArrayDamaged: semua panel berbagi satu status.</summary>
    public bool IsDamaged => ArrayDamaged;

    /// <summary>Energy per detik panel ini saat normal (dipakai MachinePanelUI untuk membandingkan).</summary>
    public float NormalEnergyPerSecond => normalEnergyPerSecond;

    /// <summary>Energy per detik yang dihasilkan panel ini sekarang (0 bila rusak).</summary>
    public float CurrentEnergyPerSecond => ArrayDamaged ? 0f : normalEnergyPerSecond;

    // Mengatur ulang status statis saat Play Mode dimulai (aman bila Enter Play Mode tanpa domain reload).
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        ArrayDamaged = false;
        OnArrayDamagedChanged = null;
    }

    private void Awake()
    {
        if (spriteRenderer != null) baseColor = spriteRenderer.color;
        if (startDamaged) SetArrayDamaged(true);
    }

    private void OnEnable()
    {
        OnArrayDamagedChanged += HandleArrayDamagedChanged;
        ApplyVisual();
    }

    private void OnDisable()
    {
        OnArrayDamagedChanged -= HandleArrayDamagedChanged;
    }

    private void Update()
    {
        TimeManager time = TimeManager.Instance;
        if (time != null && time.IsPaused) return;

        tickTimer += Time.deltaTime;
        if (tickTimer < tickSeconds) return;

        float amount = CurrentEnergyPerSecond * tickTimer;
        tickTimer = 0f;

        ResourceManager resources = ResourceManager.Instance;
        if (resources != null && amount > 0f) resources.Add(ResourceType.Energy, amount);
    }

    /// <summary>Merusak seluruh Solar Panel. Tidak melakukan apa pun bila sudah rusak.</summary>
    public void Damage() => SetArrayDamaged(true);

    /// <summary>Memperbaiki seluruh Solar Panel. Tidak melakukan apa pun bila sudah normal.</summary>
    public void Repair() => SetArrayDamaged(false);

    /// <summary>Mengubah status rusak seluruh Solar Panel dan memberi tahu pelanggan event bila berubah.</summary>
    public static void SetArrayDamaged(bool damaged)
    {
        if (ArrayDamaged == damaged) return;

        ArrayDamaged = damaged;
        OnArrayDamagedChanged?.Invoke(damaged);
    }

    private void HandleArrayDamagedChanged(bool damaged) => ApplyVisual();

    private void ApplyVisual()
    {
        if (spriteRenderer == null) return;

        Sprite sprite = ArrayDamaged ? damagedSprite : normalSprite;
        if (sprite != null)
        {
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = baseColor;
            return;
        }

        // Tanpa sprite khusus: warna menjadi penanda.
        spriteRenderer.color = ArrayDamaged ? baseColor * damagedTint : baseColor;
    }

    [ContextMenu("Debug: Rusakkan panel")]
    private void DebugDamage() => Damage();

    [ContextMenu("Debug: Perbaiki panel")]
    private void DebugRepair() => Repair();
}
