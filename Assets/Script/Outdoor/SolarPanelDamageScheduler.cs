using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pemicu kerusakan Solar Panel: SATU kejadian acak per hari (keputusan developer 2026-11-16).
/// Di awal setiap hari (TimeManager.OnDayStarted, dan saat game mulai) scheduler menentukan satu waktu acak
/// di dalam hari itu. Saat waktu tersebut tercapai, satu panel yang masih normal dipilih acak lalu dirusak
/// lewat SolarPanel.Damage(). Panel yang sudah rusak tidak dipilih lagi; bila semua panel rusak, kejadian hari itu dilewati.
/// Waktu hanya berjalan saat TimeManager tidak dijeda (tidur dan minigame kabel ikut menjeda).
/// Pemain diberi tahu TARS: bila TARSSystem ada di scene, TARSSystem sendiri yang mengumumkan (ia memantau
/// SolarPanel.OnDamagedChanged); bila tidak ada, scheduler memakai dialog TARS langsung seperti sebelumnya.
/// Semua angka di bawah adalah USULAN, ubah lewat Inspector setelah playtest.
/// </summary>
public class SolarPanelDamageScheduler : MonoBehaviour
{
    [Header("Referensi (otomatis dicari jika kosong)")]
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private WiringMinigameUI minigame;

    [Header("Aturan kejadian (USULAN)")]
    [Tooltip("Kejadian baru aktif mulai hari ini. 2 = Day 1 (tutorial) bebas kerusakan. Isi 1 agar Day 1 juga terkena.")]
    [SerializeField, Min(1)] private int firstDamageDay = 2;
    [Tooltip("Waktu kejadian paling awal, sebagai bagian dari durasi hari (0 = awal hari, 1 = akhir hari).")]
    [SerializeField, Range(0f, 1f)] private float earliestFraction = 0.15f;
    [Tooltip("Waktu kejadian paling akhir. Dibuat jauh dari akhir hari agar pemain sempat memperbaiki panel.")]
    [SerializeField, Range(0f, 1f)] private float latestFraction = 0.7f;
    [Tooltip("Tampilkan dialog TARS saat panel rusak.")]
    [SerializeField] private bool announceWithDialogue = true;

    private float triggerElapsedSeconds;
    private bool damagePending;
    private string pendingMessage;

    /// <summary>True bila kejadian kerusakan hari ini sudah dijadwalkan tetapi belum terjadi.</summary>
    public bool DamagePending => damagePending;

    private void Start()
    {
        if (timeManager == null) timeManager = FindFirstObjectByType<TimeManager>();
        if (dialogueUI == null) dialogueUI = FindFirstObjectByType<DialogueUI>();
        if (minigame == null) minigame = FindFirstObjectByType<WiringMinigameUI>();

        if (timeManager == null)
        {
            Debug.LogError("SolarPanelDamageScheduler: TimeManager tidak ditemukan. Jalankan menu Mars > Setup HUD and Time.", this);
            enabled = false;
            return;
        }

        // Berlangganan di Start (bukan OnEnable) karena urutan Awake/OnEnable antar objek tidak terjamin.
        timeManager.OnDayStarted += HandleDayStarted;
        ScheduleDay(timeManager.CurrentDay);
    }

    private void OnDestroy()
    {
        if (timeManager != null) timeManager.OnDayStarted -= HandleDayStarted;
    }

    private void Update()
    {
        if (timeManager.IsPaused) return;

        if (damagePending && !timeManager.IsDayOver)
        {
            float elapsed = timeManager.DayDurationSeconds - timeManager.RemainingSeconds;
            if (elapsed >= triggerElapsedSeconds)
            {
                damagePending = false;
                TriggerDamage();
            }
        }

        TryAnnounce();
    }

    private void HandleDayStarted(int day) => ScheduleDay(day);

    private void ScheduleDay(int day)
    {
        damagePending = false;
        if (day < firstDamageDay) return;

        float minFraction = Mathf.Min(earliestFraction, latestFraction);
        float maxFraction = Mathf.Max(earliestFraction, latestFraction);
        triggerElapsedSeconds = Random.Range(minFraction, maxFraction) * timeManager.DayDurationSeconds;
        damagePending = true;
    }

    private void TriggerDamage()
    {
        SolarPanel[] allPanels = FindObjectsByType<SolarPanel>(FindObjectsSortMode.None);
        RepairPoint[] repairPoints = FindObjectsByType<RepairPoint>(FindObjectsSortMode.None);
        List<SolarPanel> healthy = new List<SolarPanel>();
        int unrepairable = 0;
        foreach (SolarPanel candidate in allPanels)
        {
            if (candidate.IsDamaged) continue;

            // Hanya panel yang dilayani sebuah RepairPoint yang boleh dirusak; kalau tidak, panel rusak tanpa jalan perbaikan (stuck).
            if (!HasRepairPoint(candidate, repairPoints))
            {
                unrepairable++;
                continue;
            }
            healthy.Add(candidate);
        }

        if (unrepairable > 0)
            Debug.LogWarning("SolarPanelDamageScheduler: " + unrepairable + " Solar Panel tidak dilayani RepairPoint mana pun dan dilewati. Pastikan ada RepairPoint di scene.", this);

        if (healthy.Count == 0)
        {
            Debug.Log("SolarPanelDamageScheduler: tidak ada panel normal yang bisa diperbaiki, kejadian kerusakan hari ini dilewati.");
            return;
        }

        SolarPanel chosen = healthy[Random.Range(0, healthy.Count)];
        chosen.Damage();
        Debug.Log("SolarPanelDamageScheduler: " + chosen.name + " rusak (hari " + timeManager.CurrentDay + ").");

        if (announceWithDialogue && TARSSystem.Instance == null)
            pendingMessage = chosen.name + " mengalami gangguan. Produksi energi menurun.";
    }

    private static bool HasRepairPoint(SolarPanel panel, RepairPoint[] repairPoints)
    {
        foreach (RepairPoint point in repairPoints)
        {
            if (point != null && point.Serves(panel)) return true;
        }
        return false;
    }

    // Dialog tidak ditumpuk: pesan menunggu sampai dialog lain dan minigame tertutup.
    private void TryAnnounce()
    {
        if (pendingMessage == null) return;

        if (dialogueUI == null)
        {
            Debug.Log("TARS: " + pendingMessage);
            pendingMessage = null;
            return;
        }

        if (dialogueUI.IsOpen || MachinePanelUI.IsOpen || (minigame != null && minigame.IsOpen)) return;

        dialogueUI.Show("TARS", new[] { pendingMessage }, null);
        pendingMessage = null;
    }

    [ContextMenu("Debug: Picu kerusakan sekarang")]
    private void DebugTriggerDamage()
    {
        if (!Application.isPlaying || timeManager == null) return;
        TriggerDamage();
    }
}
