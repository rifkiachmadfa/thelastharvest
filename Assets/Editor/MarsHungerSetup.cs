#if UNITY_EDITOR
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Menu Editor: membuat HungerSystem (+ HungerReminder), FoodStorage (storage), dan teks Kenyang di HUD,
/// lalu menyambungkan referensinya.
/// Aman dijalankan berulang kali; objek yang sudah ada dan referensi yang sudah Anda isi tidak ditimpa.
/// Mars > Setup Hunger and Food Storage
/// FoodStorage baru diletakkan di sekitar Player sebagai placeholder: Anda yang memindahkannya ke Control Room.
/// </summary>
public static class MarsHungerSetup
{
    [MenuItem("Mars/Setup Hunger and Food Storage")]
    public static void Setup()
    {
        StringBuilder report = new StringBuilder();

        PlayerMovement player = Object.FindFirstObjectByType<PlayerMovement>();
        if (player == null)
        {
            EditorUtility.DisplayDialog("Mars Setup", "PlayerMovement tidak ditemukan di scene.", "OK");
            return;
        }

        DialogueUI dialogueUI = Object.FindFirstObjectByType<DialogueUI>();
        if (dialogueUI == null) report.AppendLine("- PERINGATAN: DialogueUI tidak ditemukan. Jalankan Mars > Setup Dialogue and Prompt UI.");
        if (Object.FindFirstObjectByType<ResourceManager>() == null) report.AppendLine("- PERINGATAN: ResourceManager tidak ada di scene.");

        // 1. HungerSystem
        HungerSystem hunger = Object.FindFirstObjectByType<HungerSystem>();
        if (hunger == null)
        {
            GameObject go = new GameObject("HungerSystem");
            Undo.RegisterCreatedObjectUndo(go, "Create HungerSystem");
            hunger = Undo.AddComponent<HungerSystem>(go);
            report.AppendLine("- HungerSystem dibuat.");
        }
        else
        {
            report.AppendLine("- HungerSystem sudah ada.");
        }

        // 1b. HungerReminder pada objek yang sama
        HungerReminder reminder = Object.FindFirstObjectByType<HungerReminder>();
        if (reminder == null)
        {
            reminder = Undo.AddComponent<HungerReminder>(hunger.gameObject);
            report.AppendLine("- HungerReminder ditambahkan pada objek HungerSystem.");
        }
        else
        {
            report.AppendLine("- HungerReminder sudah ada.");
        }
        SetRefIfEmpty(reminder, "hunger", hunger);
        SetRefIfEmpty(reminder, "dialogueUI", dialogueUI);

        // 2. FoodStorage
        FoodStorage storage = Object.FindFirstObjectByType<FoodStorage>();
        if (storage == null)
        {
            int layer = LayerMask.NameToLayer("Interactable");
            GameObject go = new GameObject("FoodStorage", typeof(SpriteRenderer), typeof(BoxCollider2D));
            Undo.RegisterCreatedObjectUndo(go, "Create FoodStorage");

            if (layer >= 0) go.layer = layer;
            else report.AppendLine("- PERINGATAN: layer 'Interactable' tidak ditemukan. Set layer FoodStorage secara manual.");

            go.transform.position = player.transform.position + new Vector3(2f, 0f, 0f);

            // Ukuran diisi eksplisit: collider yang dibuat sebelum sprite ada akan berukuran hampir nol.
            BoxCollider2D box = go.GetComponent<BoxCollider2D>();
            box.isTrigger = true;
            box.size = new Vector2(1.5f, 1.5f);

            storage = Undo.AddComponent<FoodStorage>(go);
            report.AppendLine("- FoodStorage dibuat di sebelah kanan Player. Pindahkan ke Control Room dan pasang sprite pada SpriteRenderer.");
        }
        else
        {
            report.AppendLine("- FoodStorage sudah ada: " + storage.gameObject.name);
        }
        SetRefIfEmpty(storage, "dialogueUI", dialogueUI);

        // 3. Teks Kenyang di HUD
        HudUI hud = Object.FindFirstObjectByType<HudUI>();
        if (hud == null)
        {
            report.AppendLine("- PERINGATAN: HudUI tidak ditemukan. Jalankan Mars > Setup HUD and Time, lalu jalankan menu ini lagi.");
        }
        else if (MarsUISetup.IsConfigured(hud, "hungerText"))
        {
            report.AppendLine("- Teks Kenyang di HUD sudah ada.");
        }
        else
        {
            bool created;
            GameObject panel = MarsUISetup.GetOrCreatePanel(hud.transform, "HungerPanel", out created);
            if (created)
            {
                // Kiri atas (sejak 2026-11-27; sebelumnya kanan atas).
                RectTransform r = panel.GetComponent<RectTransform>();
                r.anchorMin = new Vector2(0f, 1f);
                r.anchorMax = new Vector2(0f, 1f);
                r.pivot = new Vector2(0f, 1f);
                r.anchoredPosition = new Vector2(20f, -20f);
                r.sizeDelta = new Vector2(300f, 56f);
            }

            Text hungerText = MarsUISetup.GetOrCreateText(panel.transform, "HungerText", "Kenyang: 0 / 0", 30,
                Color.white, TextAnchor.MiddleLeft, out created);
            if (created)
            {
                RectTransform r = hungerText.rectTransform;
                r.anchorMin = Vector2.zero;
                r.anchorMax = Vector2.one;
                r.offsetMin = new Vector2(15f, 5f);
                r.offsetMax = new Vector2(-15f, -5f);
            }

            MarsUISetup.SetRef(hud, "hungerText", hungerText);
            report.AppendLine("- Teks Kenyang dibuat di kiri atas HUD.");
        }

        EditorSceneManager.MarkSceneDirty(player.gameObject.scene);
        report.AppendLine();
        report.Append("Tekan Ctrl+S untuk menyimpan scene.");
        Debug.Log("MarsHungerSetup:\n" + report);
        EditorUtility.DisplayDialog("Mars Setup", report.ToString(), "OK");
    }

    private static void SetRefIfEmpty(Object target, string fieldName, Object value)
    {
        SerializedObject so = new SerializedObject(target);
        SerializedProperty prop = so.FindProperty(fieldName);
        if (prop != null && prop.objectReferenceValue == null) MarsUISetup.SetRef(target, fieldName, value);
    }
}
#endif
