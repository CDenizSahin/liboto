namespace KutuphaneOtomasyon.Core.Models;

/// <summary>
/// Kütüphanedeki fiziksel kitap kaynağını temsil eder.
/// Stok sayacı encapsulation ile korunur.
/// </summary>
public class Kitap
{
    // ── Private stok alanları ────────────────────────────────────────────────
    private int _stokSayisi;
    private int _mevcutSayi;

    public string KitapId    { get; }
    public string Isbn       { get; }
    public string Baslik     { get; set; }
    public string Yazar      { get; set; }
    public string Kategori   { get; set; }
    public int    YayinYili  { get; set; }

    public int StokSayisi  => _stokSayisi;
    public int MevcutSayi  => _mevcutSayi;

    public Kitap(string kitapId, string isbn, string baslik,
                 string yazar, string kategori,
                 int stokSayisi, int yayinYili)
    {
        if (stokSayisi < 0)
            throw new ArgumentOutOfRangeException(nameof(stokSayisi),
                "Stok sayısı negatif olamaz.");

        KitapId      = kitapId;
        Isbn         = isbn;
        Baslik       = baslik;
        Yazar        = yazar;
        Kategori     = kategori;
        _stokSayisi  = stokSayisi;
        _mevcutSayi  = stokSayisi;   // Başlangıçta tamamı müsait
        YayinYili    = yayinYili;
    }

    /// <summary>Şu an ödünç verilebilecek kopya var mı?</summary>
    public bool MusaitMi() => _mevcutSayi > 0;

    /// <summary>Ödünç alındığında mevcut sayıyı bir azaltır.</summary>
    public void StokAzalt()
    {
        if (_mevcutSayi <= 0)
            throw new InvalidOperationException(
                $"'{Baslik}' kitabında stok kalmadı.");
        _mevcutSayi--;
    }

    /// <summary>İade edildiğinde mevcut sayıyı bir artırır.</summary>
    public void StokArttir()
    {
        if (_mevcutSayi >= _stokSayisi)
            throw new InvalidOperationException(
                "Mevcut sayı toplam stok limitini aşamaz.");
        _mevcutSayi++;
    }

    public string BilgileriGoster()
        => $"[{KitapId}] {Baslik} — {Yazar} ({YayinYili}) | " +
           $"{(MusaitMi() ? "✅ Müsait" : "❌ Stokta Yok")} " +
           $"({_mevcutSayi}/{_stokSayisi})";

    public override string ToString() => $"<Kitap: {Baslik}>";
}
