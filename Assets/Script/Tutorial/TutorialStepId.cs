/// <summary>
/// Pengenal langkah tutorial Day 1 (GDD 5.2, PHASE.md 12.1). Urutan langkah ditentukan oleh array steps di
/// TutorialManager, bukan oleh nilai enum. ID bersifat tetap (tersimpan di scene sebagai angka): jangan dipakai ulang
/// bila langkah dihapus dan jangan menyisipkan ID baru di tengah; tambahkan di akhir.
/// </summary>
public enum TutorialStepId
{
    Intro,          // 12.1.1 Mengenal Control Room, TARS, HUD, dan Storage
    Suit,           // 12.1.2 Mengambil dan mengisi Astronaut Suit
    CollectIce,     // 12.1.3 Keluar dan mengumpulkan Ice dalam jumlah kecil
    PurifyWater,    // 12.1.4 Membawa Ice ke Water Purifier dan menghasilkan Water
    Farming,        // 12.1.5 Menanam dan menyiram di Farming Plot
    OxygenGenerator,// 12.1.6 Mengenal Oxygen Generator dan hubungan Water/Energy
    Sleep,          // 12.1.7 Kembali ke Control Room dan tidur
    RemoveSuit,     // 12.1.8 Melepas suit di Suit Station (ditambahkan 2026-11-17; ditaruh di akhir agar ID lama di scene tidak bergeser)
    Eat,            // 12.1.9 Makan kentang dari tas (ditambahkan 2026-11-17; ID 8, letak dalam alur diatur array steps)
    Hungry,         // 12.1.10 Adegan lapar: kenyang diturunkan sekali, TARS menjelaskan kenyang, HP, dan kentang (ID 9; selesai saat dialog dibaca)
    Harvest,        // 12.1.11 Menunggu tanaman matang lalu panen (ID 10)
    Recap           // 12.1.12 Ringkasan rantai resource sebelum tidur (ID 11; selesai saat dialog dibaca)
}
