using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Kotak dialog sederhana. Menampilkan baris teks satu per satu.
/// Lanjut ke baris berikutnya / menutup dialog di baris terakhir: tombol E, Spasi, atau klik mouse di mana saja.
/// Tombol Tutup (opsional, closeButton) menutup dialog langsung, melewati sisa baris.
/// Dialog TIDAK menghentikan waktu atau resource (lihat TimeManager.pauseDuringDialogue).
/// Hanya tampilan: isi dialog dikirim oleh pemanggil (misalnya TARS).
/// Pesan tutorial (Show dengan tutorialInfo) memakai kartu panduan TutorialDialogueSkin di tengah atas layar, bukan kotak
/// dialog biasa di bawah, agar pemain bisa membedakannya. Input, IsOpen, dan callback tetap satu jalur di kelas ini.
/// </summary>
public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogueRoot;
    [SerializeField] private Text speakerText;   // opsional
    [SerializeField] private Text lineText;
    [Tooltip("Opsional. Tombol Tutup: menutup dialog langsung. Dibuat oleh menu Mars > Setup Dialogue and Prompt UI.")]
    [SerializeField] private Button closeButton;

    [Header("Input lanjut / tutup dialog")]
    [SerializeField] private KeyCode advanceKey = KeyCode.E;
    [SerializeField] private KeyCode alternateAdvanceKey = KeyCode.Space;
    [Tooltip("Klik mouse kiri di mana saja = lanjut / tutup (di luar tombol Tutup).")]
    [SerializeField] private bool clickAnywhereToAdvance = true;

    private string[] lines;
    private int lineIndex;
    private Action onFinished;
    private int openedFrame = -1;
    private TutorialDialogueSkin tutorialSkin;
    private bool usingTutorialSkin;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        if (dialogueRoot == null || lineText == null)
        {
            Debug.LogError("DialogueUI: Dialogue Root / Line Text belum diisi di Inspector.", this);
            enabled = false;
            return;
        }

        dialogueRoot.SetActive(false);

        if (closeButton != null) closeButton.onClick.AddListener(CloseNow);
    }

    private void OnDestroy()
    {
        if (closeButton != null) closeButton.onClick.RemoveListener(CloseNow);
        if (tutorialSkin != null) tutorialSkin.OnCloseClicked -= CloseNow;
    }

    private void Update()
    {
        if (!IsOpen) return;

        // Abaikan tombol yang sama yang baru membuka dialog.
        if (Time.frameCount == openedFrame) return;

        bool advance =
            Input.GetKeyDown(advanceKey) ||
            Input.GetKeyDown(alternateAdvanceKey) ||
            (clickAnywhereToAdvance && Input.GetMouseButtonDown(0) && !IsPointerOverCloseButton());

        if (advance) ShowNextLine();
    }

    /// <summary>Menutup dialog sekarang juga (melewati sisa baris). Dipakai tombol Tutup.</summary>
    private void CloseNow()
    {
        if (IsOpen) Close();
    }

    // Klik pada tombol Tutup ditangani tombolnya sendiri; jangan dihitung juga sebagai "klik di mana saja".
    private bool IsPointerOverCloseButton()
    {
        if (usingTutorialSkin && tutorialSkin != null) return tutorialSkin.IsPointerOverClose(Input.mousePosition);

        if (closeButton == null || !closeButton.gameObject.activeInHierarchy) return false;

        Canvas canvas = closeButton.GetComponentInParent<Canvas>();
        Camera uiCamera = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay) ? canvas.worldCamera : null;
        return RectTransformUtility.RectangleContainsScreenPoint((RectTransform)closeButton.transform, Input.mousePosition, uiCamera);
    }

    public void Show(string speaker, string[] dialogueLines, Action finishedCallback)
    {
        Show(speaker, dialogueLines, finishedCallback, null);
    }

    /// <summary>
    /// Menampilkan dialog. Bila tutorialInfo diisi, pesan tampil di kartu panduan tutorial (TutorialDialogueSkin);
    /// bila null, di kotak dialog biasa.
    /// </summary>
    public void Show(string speaker, string[] dialogueLines, Action finishedCallback, TutorialDialogueInfo? tutorialInfo)
    {
        if (!enabled || dialogueLines == null || dialogueLines.Length == 0)
        {
            finishedCallback?.Invoke();
            return;
        }

        lines = dialogueLines;
        lineIndex = 0;
        onFinished = finishedCallback;
        openedFrame = Time.frameCount;
        usingTutorialSkin = tutorialInfo.HasValue;

        if (usingTutorialSkin)
        {
            EnsureTutorialSkin();
            dialogueRoot.SetActive(false);
            tutorialSkin.Open(tutorialInfo.Value);
        }
        else
        {
            if (tutorialSkin != null) tutorialSkin.Hide();
            if (speakerText != null) speakerText.text = speaker;
            dialogueRoot.SetActive(true);
        }

        IsOpen = true;
        RenderLine();
    }

    private void EnsureTutorialSkin()
    {
        if (tutorialSkin != null) return;

        // Saudara DialogueRoot di Canvas yang sama, tepat di atasnya dalam urutan gambar.
        Transform parent = dialogueRoot.transform.parent;
        TarsWanderer tars = FindFirstObjectByType<TarsWanderer>();
        Sprite portrait = tars != null ? tars.PortraitSprite : null;
        tutorialSkin = TutorialDialogueSkin.Create(parent, dialogueRoot.transform.GetSiblingIndex() + 1, lineText.font, portrait);
        tutorialSkin.OnCloseClicked += CloseNow;
    }

    private void RenderLine()
    {
        if (usingTutorialSkin && tutorialSkin != null) tutorialSkin.Render(lineIndex, lines.Length, lines[lineIndex]);
        else lineText.text = lines[lineIndex];
    }

    private void ShowNextLine()
    {
        lineIndex++;

        if (lineIndex >= lines.Length)
        {
            Close();
            return;
        }

        RenderLine();
    }

    private void Close()
    {
        IsOpen = false;
        dialogueRoot.SetActive(false);
        if (tutorialSkin != null) tutorialSkin.Hide();
        usingTutorialSkin = false;

        Action callback = onFinished;
        onFinished = null;
        callback?.Invoke();
    }
}
