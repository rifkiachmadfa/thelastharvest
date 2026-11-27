using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD gameplay: hari, timer, serta Food, Water, Oxygen (habitat), dan Energy.
/// Hanya tampilan: membaca ResourceManager (lewat event) dan TimeManager.
/// Kenyang (hunger pemain) tampil di hungerText (opsional) dan membaca HungerSystem lewat event.
/// HP pemain tampil di hpText (opsional) dan membaca HealthSystem lewat event.
/// Suit Oxygen tampil di panel opsional (suitPanel) hanya saat suit dikenakan. Isi tas tampil di InventoryHotbarUI.
/// Food = jumlah kentang di storage.
/// </summary>
public class HudUI : MonoBehaviour
{
    [SerializeField] private Text dayText;
    [SerializeField] private Text timerText;
    [SerializeField] private Text foodText;
    [SerializeField] private Text waterText;
    [SerializeField] private Text oxygenText;
    [SerializeField] private Text energyText;

    [Header("Bar isi (opsional; diisi menu Mars > Apply HUD Skin (Pixel UI Sci-Fi))")]
    [SerializeField] private HudBar foodBar;
    [SerializeField] private HudBar waterBar;
    [SerializeField] private HudBar oxygenBar;
    [SerializeField] private HudBar energyBar;
    [SerializeField] private HudBar hungerBar;
    [SerializeField] private HudBar hpBar;
    [SerializeField] private HudBar suitBar;

    [Header("Ice (TIDAK DIPAKAI LAGI sejak ada tas; ditampilkan InventoryHotbarUI. Biarkan kosong)")]
    [SerializeField] private Text iceText;

    [Header("Kenyang / hunger (opsional; diisi menu Mars > Setup Hunger and Food Storage)")]
    [SerializeField] private Text hungerText;

    [Header("HP (opsional; diisi menu Mars > Setup Health and HP HUD)")]
    [SerializeField] private Text hpText;
    [Tooltip("Teks berubah kuning/merah di bawah proporsi ini dari HP maksimum (usulan).")]
    [SerializeField, Range(0f, 1f)] private float hpWarnFraction = 0.5f;
    [SerializeField, Range(0f, 1f)] private float hpDangerFraction = 0.25f;

    [Header("Suit (opsional; diisi menu Mars > Setup Suit and Gate)")]
    [SerializeField] private GameObject suitPanel;
    [SerializeField] private Text suitOxygenText;
    [Tooltip("Teks berubah kuning/merah di bawah proporsi ini dari kapasitas tabung (usulan, sekitar 30% dan 10% sesuai GDD).")]
    [SerializeField, Range(0f, 1f)] private float suitWarnFraction = 0.3f;
    [SerializeField, Range(0f, 1f)] private float suitDangerFraction = 0.1f;

    private ResourceManager resources;
    private TimeManager time;
    private SuitSystem suit;
    private HungerSystem hunger;
    private HealthSystem health;
    private int shownHungerValue = -1;
    private int shownHpValue = -1;
    private int shownSeconds = -1;
    private int shownSuitValue = -1;

    private void Start()
    {
        if (dayText == null || timerText == null || foodText == null ||
            waterText == null || oxygenText == null || energyText == null)
        {
            Debug.LogError("HudUI: referensi Text belum lengkap. Jalankan menu Mars > Setup HUD and Time.", this);
            enabled = false;
            return;
        }

        resources = ResourceManager.Instance;
        if (resources != null)
        {
            resources.OnResourceChanged += HandleResourceChanged;
            RefreshResource(ResourceType.Food);
            RefreshResource(ResourceType.Water);
            RefreshResource(ResourceType.HabitatOxygen);
            RefreshResource(ResourceType.Energy);
        }
        else
        {
            Debug.LogWarning("HudUI: ResourceManager tidak ada di scene.", this);
        }

        time = TimeManager.Instance;
        if (time != null)
        {
            time.OnDayStarted += HandleDayStarted;
            RefreshDay();
        }
        else
        {
            Debug.LogWarning("HudUI: TimeManager tidak ada di scene.", this);
        }

        suit = SuitSystem.Instance;
        if (suit != null) suit.OnEquippedChanged += HandleSuitEquippedChanged;
        RefreshSuit();

        hunger = HungerSystem.Instance;
        if (hunger != null) hunger.OnSatietyChanged += HandleSatietyChanged;
        RefreshHunger();

        health = HealthSystem.Instance;
        if (health != null) health.OnHealthChanged += HandleHealthChanged;
        RefreshHealth();
    }

    private void OnDestroy()
    {
        if (resources != null) resources.OnResourceChanged -= HandleResourceChanged;
        if (time != null) time.OnDayStarted -= HandleDayStarted;
        if (suit != null) suit.OnEquippedChanged -= HandleSuitEquippedChanged;
        if (hunger != null) hunger.OnSatietyChanged -= HandleSatietyChanged;
        if (health != null) health.OnHealthChanged -= HandleHealthChanged;
    }

    private void Update()
    {
        if (time == null) return;

        int seconds = Mathf.CeilToInt(time.RemainingSeconds);
        if (seconds == shownSeconds) return;

        shownSeconds = seconds;
        timerText.text = (seconds / 60).ToString("00") + ":" + (seconds % 60).ToString("00");
    }

    private void HandleResourceChanged(ResourceType type, float amount, float max)
    {
        RefreshResource(type);
    }

    private void HandleDayStarted(int day)
    {
        RefreshDay();
    }

    private void HandleSuitEquippedChanged(bool equipped)
    {
        RefreshSuit();
    }

    private void HandleSatietyChanged(float satiety, float max)
    {
        RefreshHunger();
    }

    private void HandleHealthChanged(float hp, float max)
    {
        RefreshHealth();
    }

    private void RefreshHealth()
    {
        if (hpText == null || health == null) return;

        int rounded = Mathf.RoundToInt(health.Health);
        if (rounded == shownHpValue) return; // hindari menulis teks yang sama
        shownHpValue = rounded;

        hpText.text = "HP: " + rounded + " / " + Mathf.RoundToInt(health.MaxHealth);

        float fraction = health.Fraction;
        if (hpBar != null) hpBar.SetFraction(fraction);
        if (fraction <= hpDangerFraction) hpText.color = new Color(1f, 0.35f, 0.3f);
        else if (fraction <= hpWarnFraction) hpText.color = new Color(1f, 0.85f, 0.3f);
        else hpText.color = Color.white;
    }

    private void RefreshHunger()
    {
        if (hungerText == null || hunger == null) return;

        int rounded = Mathf.RoundToInt(hunger.Satiety);
        if (rounded == shownHungerValue) return; // kenyang berubah tiap frame; hindari menulis teks yang sama
        shownHungerValue = rounded;

        hungerText.text = "Kenyang: " + rounded + " / " + Mathf.RoundToInt(hunger.MaxSatiety);

        float fraction = hunger.Fraction;
        if (hungerBar != null) hungerBar.SetFraction(fraction);
        if (fraction <= hunger.DangerFraction) hungerText.color = new Color(1f, 0.35f, 0.3f);
        else if (fraction <= hunger.WarnFraction) hungerText.color = new Color(1f, 0.85f, 0.3f);
        else hungerText.color = Color.white;
    }

    private void RefreshSuit()
    {
        if (suitPanel == null || suitOxygenText == null) return;

        bool show = suit != null && suit.IsEquipped && resources != null;
        suitPanel.SetActive(show);
        if (!show)
        {
            shownSuitValue = -1; // paksa digambar ulang saat suit dikenakan lagi
            return;
        }

        float current = resources.Get(ResourceType.SuitOxygen);
        float max = resources.GetMax(ResourceType.SuitOxygen);

        int rounded = Mathf.RoundToInt(current);
        if (rounded == shownSuitValue) return; // oksigen suit berubah tiap frame saat di luar; hindari menulis teks yang sama
        shownSuitValue = rounded;

        suitOxygenText.text = "Suit O2: " + rounded + " / " + Mathf.RoundToInt(max);

        float fraction = max > 0f ? current / max : 0f;
        if (suitBar != null) suitBar.SetFraction(fraction);
        if (fraction <= suitDangerFraction) suitOxygenText.color = new Color(1f, 0.35f, 0.3f);
        else if (fraction <= suitWarnFraction) suitOxygenText.color = new Color(1f, 0.85f, 0.3f);
        else suitOxygenText.color = Color.white;
    }

    private void RefreshDay()
    {
        dayText.text = "Day " + time.CurrentDay + " / " + time.TotalDays;
        shownSeconds = -1; // paksa timer digambar ulang
    }

    private void RefreshResource(ResourceType type)
    {
        string label;
        Text target;
        HudBar bar;

        switch (type)
        {
            case ResourceType.Food: label = "Food"; target = foodText; bar = foodBar; break;
            case ResourceType.Water: label = "Water"; target = waterText; bar = waterBar; break;
            case ResourceType.HabitatOxygen: label = "Oxygen"; target = oxygenText; bar = oxygenBar; break;
            case ResourceType.Energy: label = "Energy"; target = energyText; bar = energyBar; break;
            case ResourceType.Ice: return; // Ice kini ada di tas (InventorySystem), tampil di InventoryHotbarUI
            case ResourceType.SuitOxygen: RefreshSuit(); return;
            default: return;
        }

        float value = resources.Get(type);
        float max = resources.GetMax(type);
        target.text = label + ": " + Mathf.RoundToInt(value) + " / " + Mathf.RoundToInt(max);
        if (bar != null) bar.SetFraction(max > 0f ? value / max : 0f);
    }
}
