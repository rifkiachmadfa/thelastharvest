using UnityEngine;

/// <summary>
/// Titik perbaikan Solar Panel di Outdoor. SATU RepairPoint memperbaiki SELURUH Solar Panel: semua panel berbagi satu
/// status rusak/normal (lihat SolarPanel), jadi tidak ada lagi perbaikan panel satu per satu.
/// Tekan E: MachinePanelUI Solar Panel terbuka (status, output, Energy); tombol PERBAIKI (aktif bila rusak) membuka
/// minigame kabel (WiringMinigameUI), dan Solar Panel diperbaiki (SolarPanel.Repair()) hanya setelah semua kabel
/// tersambung. Esc membatalkan.
/// Tanpa MachinePanelUI di scene, E langsung membuka minigame bila rusak (perilaku lama, dialog bila normal).
/// Bila WiringMinigameUI tidak ada di scene, perbaikan langsung berhasil (cadangan, dengan peringatan di Console).
/// Pasang pada objek ber-layer Interactable dengan Collider2D.
/// </summary>
public class RepairPoint : MonoBehaviour, IInteractable
{
    [Tooltip("Panel perwakilan untuk tampilan MachinePanelUI dan posisi popup. Status rusak berlaku untuk semua panel. Kosong = panel pertama di scene.")]
    [SerializeField] private SolarPanel panel;

    [Header("Referensi (otomatis dicari jika kosong)")]
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private WiringMinigameUI minigame;
    [SerializeField] private MachinePanelUI panelUI;

    [SerializeField, Min(0f)] private float reinteractCooldown = 0.3f;

    private float nextInteractTime;
    private SolarPanel target;

    /// <summary>Panel perwakilan yang diisi di Inspector (bisa kosong).</summary>
    public SolarPanel Panel => panel;

    public string InteractionPrompt
    {
        get
        {
            bool damaged = SolarPanel.ArrayDamaged;
            if (panelUI != null) return damaged ? "Periksa Solar Panel (RUSAK)" : "Periksa Solar Panel";
            return damaged ? "Perbaiki Solar Panel" : "Periksa Solar Panel";
        }
    }

    private void Start()
    {
        if (dialogueUI == null) dialogueUI = FindFirstObjectByType<DialogueUI>();
        if (minigame == null) minigame = FindFirstObjectByType<WiringMinigameUI>();
        if (panelUI == null) panelUI = FindFirstObjectByType<MachinePanelUI>();
        if (panel == null) panel = FindFirstObjectByType<SolarPanel>();

        if (minigame == null)
            Debug.LogWarning("RepairPoint: WiringMinigameUI tidak ditemukan; perbaikan akan langsung berhasil. Jalankan menu Mars > Setup Panel Damage and Wiring Minigame.", this);

        if (panel == null)
            Debug.LogError("RepairPoint: tidak ada Solar Panel di scene (isi Panel di Inspector atau tambahkan SolarPanel).", this);
    }

    public bool CanInteract(GameObject interactor)
    {
        if (panel == null || Time.time < nextInteractTime || MachinePanelUI.IsOpen) return false;
        if (minigame != null && minigame.IsOpen) return false;
        return dialogueUI == null || !dialogueUI.IsOpen;
    }

    public void Interact(GameObject interactor)
    {
        target = panel;
        if (target == null) return;

        if (panelUI != null && panelUI.Open(target, BeginRepair, OnPanelClosed, null)) return;

        if (!SolarPanel.ArrayDamaged)
        {
            ShowMessage("Solar Panel berfungsi normal. Produksi total: " +
                TotalEnergyPerSecond().ToString("0.0") + " Energy per detik.");
            return;
        }

        BeginRepair();
    }

    // Jumlah produksi seluruh Solar Panel saat ini (dipakai hanya untuk pesan dialog tanpa MachinePanelUI).
    private static float TotalEnergyPerSecond()
    {
        float total = 0f;
        foreach (SolarPanel candidate in FindObjectsByType<SolarPanel>(FindObjectsSortMode.None))
            total += candidate.CurrentEnergyPerSecond;
        return total;
    }

    // Membuka minigame kabel (dipanggil langsung atau lewat tombol PERBAIKI di MachinePanelUI).
    private void BeginRepair()
    {
        if (target == null || !SolarPanel.ArrayDamaged) return;

        if (minigame == null)
        {
            OnRepairSucceeded(); // cadangan: tanpa minigame di scene, perbaikan langsung berhasil.
            return;
        }

        minigame.Open("Solar Panel", OnRepairSucceeded, OnRepairCancelled);
    }

    private void OnPanelClosed()
    {
        nextInteractTime = Time.time + reinteractCooldown;
    }

    private void OnRepairSucceeded()
    {
        SolarPanel repaired = target;
        if (repaired == null) return;

        repaired.Repair(); // memperbaiki seluruh Solar Panel sekaligus
        FloatingPopupManager.Show(FloatingPopupManager.AnchorAbove(repaired), PopupIcon.Repair, "Solar Panel diperbaiki");

        const string note = "Solar Panel diperbaiki. Produksi Energy kembali normal.";

        // Dengan panel mesin: kembali ke panel agar hasilnya (status, output) terlihat.
        if (panelUI != null && panelUI.Open(target, BeginRepair, OnPanelClosed, note)) return;

        ShowMessage(note);
    }

    private void OnRepairCancelled()
    {
        nextInteractTime = Time.time + reinteractCooldown;
    }

    private void ShowMessage(string message)
    {
        if (dialogueUI == null)
        {
            Debug.Log("RepairPoint: " + message);
            nextInteractTime = Time.time + reinteractCooldown;
            return;
        }

        dialogueUI.Show("Repair Point", new[] { message }, () => nextInteractTime = Time.time + reinteractCooldown);
    }
}
