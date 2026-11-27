using System;
using UnityEngine;

/// <summary>
/// HP (nyawa) pemain. Nilai 0 sampai MaxHealth. Saat kenyang (HungerSystem) habis, HP berkurang tiap
/// damageInterval detik sebesar damageAmount sampai pemain makan lagi (kenyang naik di atas 0).
/// Sebaliknya, selama kenyang di atas regenAboveFraction (80 persen), HP pulih bertahap: regenAmount tiap
/// regenInterval detik sampai penuh. HP yang sudah 0 tidak pulih lagi.
/// Ikut jeda TimeManager.IsPaused, seperti HungerSystem.
/// Game Over belum dibuat; yang ada baru event OnDied (hanya log), seperti HungerSystem.OnStarvationFailure.
/// Hanya logika; tampilan ada di HudUI.
/// </summary>
public class HealthSystem : MonoBehaviour
{
    public static HealthSystem Instance { get; private set; }

    [Header("HP (USULAN, ubah lewat Inspector setelah playtest)")]
    [SerializeField, Min(1f)] private float maxHealth = 100f;
    [SerializeField, Min(0f)] private float startHealth = 100f;

    [Header("Kerusakan saat kelaparan (USULAN)")]
    [Tooltip("Jeda detik antar pengurangan HP selama kenyang 0.")]
    [SerializeField, Min(0.1f)] private float damageInterval = 2f;
    [Tooltip("HP yang berkurang tiap jeda. 3 dengan jeda 2 detik: HP penuh habis sekitar 67 detik (mendekati batas kelaparan 60 detik di HungerSystem).")]
    [SerializeField, Min(0.1f)] private float damageAmount = 3f;

    [Header("Pemulihan saat kenyang (USULAN)")]
    [Tooltip("HP hanya pulih bila kenyang di atas proporsi ini dari maksimum (0,8 = 80 persen).")]
    [SerializeField, Range(0f, 1f)] private float regenAboveFraction = 0.8f;
    [SerializeField, Min(0.1f)] private float regenInterval = 2f;
    [Tooltip("HP yang pulih tiap jeda. 2 dengan jeda 2 detik: dari 0 ke penuh sekitar 100 detik.")]
    [SerializeField, Min(0.1f)] private float regenAmount = 2f;

    private float damageTimer;
    private float regenTimer;
    private bool deathReported;

    public float Health { get; private set; }
    public float MaxHealth => maxHealth;
    public float Fraction => maxHealth > 0f ? Health / maxHealth : 0f;
    public bool IsDead => Health <= 0f;

    /// <summary>Dipanggil setiap HP berubah: (nilai baru, maksimum).</summary>
    public event Action<float, float> OnHealthChanged;

    /// <summary>Dipanggil sekali ketika HP mencapai 0 (Game Over belum dibuat).</summary>
    public event Action OnDied;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("HealthSystem: sudah ada instance lain, objek ini dihapus.", this);
            Destroy(this);
            return;
        }

        Instance = this;
        Health = Mathf.Clamp(startHealth, 0f, maxHealth);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        TimeManager time = TimeManager.Instance;
        if (time != null && time.IsPaused) return;

        HungerSystem hunger = HungerSystem.Instance;
        if (hunger == null || IsDead)
        {
            damageTimer = 0f;
            regenTimer = 0f;
            return;
        }

        if (!hunger.IsStarving)
        {
            damageTimer = 0f; // sudah makan: hitungan mulai dari awal saat lapar lagi

            if (hunger.Fraction > regenAboveFraction && Health < maxHealth)
            {
                regenTimer += Time.deltaTime;
                while (regenTimer >= regenInterval)
                {
                    regenTimer -= regenInterval;
                    Heal(regenAmount);
                }
            }
            else
            {
                regenTimer = 0f;
            }
            return;
        }

        regenTimer = 0f;
        damageTimer += Time.deltaTime;
        while (damageTimer >= damageInterval && !IsDead)
        {
            damageTimer -= damageInterval;
            Damage(damageAmount);
        }
    }

    /// <summary>Menambah HP (tidak lebih dari maksimum). Mengembalikan jumlah HP yang benar-benar bertambah.</summary>
    public float Heal(float amount)
    {
        if (amount <= 0f || IsDead) return 0f;

        float gained = Mathf.Min(amount, maxHealth - Health);
        if (gained <= 0f) return 0f;

        Health += gained;
        OnHealthChanged?.Invoke(Health, maxHealth);
        return gained;
    }

    /// <summary>Mengurangi HP (tidak kurang dari 0). Mengembalikan jumlah HP yang benar-benar berkurang.</summary>
    public float Damage(float amount)
    {
        if (amount <= 0f || IsDead) return 0f;

        float lost = Mathf.Min(amount, Health);
        Health -= lost;
        OnHealthChanged?.Invoke(Health, maxHealth);

        if (Health <= 0f && !deathReported)
        {
            deathReported = true;
            Debug.LogWarning("HealthSystem: HP habis. (Game Over belum dibuat.)");
            OnDied?.Invoke();
        }
        return lost;
    }

    [ContextMenu("Debug: HP jadi penuh")]
    private void DebugSetFull()
    {
        Health = maxHealth;
        deathReported = false;
        OnHealthChanged?.Invoke(Health, maxHealth);
    }
}
