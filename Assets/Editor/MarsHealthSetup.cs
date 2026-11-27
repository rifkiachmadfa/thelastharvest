#if UNITY_EDITOR
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Menu Editor: membuat HealthSystem (HP pemain) dan panel HP di HUD (kiri atas, di bawah panel Kenyang),
/// lalu menyambungkan HudUI.hpText dan HudUI.hpBar.
/// Panel HP dibuat dengan menggandakan HungerPanel (tampilan sama), jadi jalankan dulu
/// Mars > Setup Hunger and Food Storage (dan Mars > Apply HUD Skin bila ingin bar/ikon).
/// Aman dijalankan berulang kali; objek yang sudah ada tidak dibuat ulang.
/// Mars > Setup Health and HP HUD
/// </summary>
public static class MarsHealthSetup
{
    private const float PanelGap = 10f;

    [MenuItem("Mars/Setup Health and HP HUD")]
    public static void Setup()
    {
        StringBuilder report = new StringBuilder();

        HudUI hud = Object.FindFirstObjectByType<HudUI>();
        if (hud == null)
        {
            EditorUtility.DisplayDialog("Mars Setup", "HudUI tidak ditemukan. Jalankan dulu Mars > Setup HUD and Time.", "OK");
            return;
        }

        if (Object.FindFirstObjectByType<HungerSystem>() == null)
            report.AppendLine("- PERINGATAN: HungerSystem tidak ada di scene. HP baru berkurang bila kenyang habis. Jalankan Mars > Setup Hunger and Food Storage.");

        // 1. HealthSystem
        HealthSystem health = Object.FindFirstObjectByType<HealthSystem>();
        if (health == null)
        {
            GameObject go = new GameObject("HealthSystem");
            Undo.RegisterCreatedObjectUndo(go, "Create HealthSystem");
            health = Undo.AddComponent<HealthSystem>(go);
            report.AppendLine("- HealthSystem dibuat.");
        }
        else
        {
            report.AppendLine("- HealthSystem sudah ada.");
        }

        // 1b. Panel Kenyang dipaksa ke kiri atas (scene yang tersimpan ulang bisa masih memakai posisi lama kanan atas)
        {
            SerializedProperty hp0 = new SerializedObject(hud).FindProperty("hungerText");
            Text ht = hp0 != null ? hp0.objectReferenceValue as Text : null;
            RectTransform hr = ht != null ? ht.transform.parent as RectTransform : null;
            if (hr != null)
            {
                Undo.RecordObject(hr, "Move hunger panel");
                hr.anchorMin = new Vector2(0f, 1f);
                hr.anchorMax = new Vector2(0f, 1f);
                hr.pivot = new Vector2(0f, 1f);
                hr.anchoredPosition = new Vector2(20f, -20f);
                report.AppendLine("- Panel Kenyang diposisikan di kiri atas.");
            }
        }

        // 2. Panel HP di HUD
        if (MarsUISetup.IsConfigured(hud, "hpText"))
        {
            SerializedProperty hpProp = new SerializedObject(hud).FindProperty("hpText");
            SerializedProperty hgProp = new SerializedObject(hud).FindProperty("hungerText");
            Text hpT = hpProp.objectReferenceValue as Text;
            Text hgT = hgProp != null ? hgProp.objectReferenceValue as Text : null;
            RectTransform hpR = hpT != null ? hpT.transform.parent as RectTransform : null;
            RectTransform hgR = hgT != null ? hgT.transform.parent as RectTransform : null;
            if (hpR != null && hgR != null)
            {
                Undo.RecordObject(hpR, "Move HP panel");
                hpR.anchorMin = hgR.anchorMin;
                hpR.anchorMax = hgR.anchorMax;
                hpR.pivot = hgR.pivot;
                hpR.anchoredPosition = hgR.anchoredPosition - new Vector2(0f, hgR.sizeDelta.y + PanelGap);
            }
            report.AppendLine("- Panel HP di HUD sudah ada (posisi dirapikan di bawah panel Kenyang).");
        }
        else
        {
            SerializedProperty hungerProp = new SerializedObject(hud).FindProperty("hungerText");
            Text hungerText = hungerProp != null ? hungerProp.objectReferenceValue as Text : null;
            Transform hungerPanel = hungerText != null ? hungerText.transform.parent : null;

            if (hungerPanel == null)
            {
                report.AppendLine("- PERINGATAN: panel Kenyang tidak ditemukan; panel HP tidak dibuat. Jalankan Mars > Setup Hunger and Food Storage lalu menu ini lagi.");
            }
            else
            {
                GameObject panel = Object.Instantiate(hungerPanel.gameObject, hungerPanel.parent, false);
                Undo.RegisterCreatedObjectUndo(panel, "Create HpPanel");
                panel.name = "HpPanel";

                RectTransform hungerRect = hungerPanel as RectTransform;
                RectTransform rect = panel.GetComponent<RectTransform>();
                rect.anchorMin = hungerRect.anchorMin;
                rect.anchorMax = hungerRect.anchorMax;
                rect.pivot = hungerRect.pivot;
                rect.sizeDelta = hungerRect.sizeDelta;
                rect.anchoredPosition = hungerRect.anchoredPosition - new Vector2(0f, hungerRect.sizeDelta.y + PanelGap);

                Text hpText = null;
                HudBar hpBar = null;
                foreach (Transform child in panel.transform)
                {
                    if (child.name == "HungerText") { child.name = "HpText"; hpText = child.GetComponent<Text>(); }
                    else if (child.name == "HungerIcon") child.name = "HpIcon";
                    else if (child.name == "HungerBar") { child.name = "HpBar"; hpBar = child.GetComponent<HudBar>(); }
                }

                if (hpText != null) hpText.text = "HP: 0 / 0";
                MarsUISetup.SetRef(hud, "hpText", hpText);
                if (hpBar != null) MarsUISetup.SetRef(hud, "hpBar", hpBar);
                report.AppendLine("- Panel HP dibuat di bawah panel Kenyang (kiri atas).");
            }
        }

        EditorSceneManager.MarkSceneDirty(hud.gameObject.scene);
        report.AppendLine();
        report.Append("Tekan Ctrl+S untuk menyimpan scene.");
        Debug.Log("MarsHealthSetup:\n" + report);
        EditorUtility.DisplayDialog("Mars Setup", report.ToString(), "OK");
    }
}
#endif
