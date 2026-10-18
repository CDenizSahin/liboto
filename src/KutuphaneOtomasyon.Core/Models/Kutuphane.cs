namespace KutuphaneOtomasyon.Core.Models;

/// <summary>
/// Kütüphane sisteminin merkez koordinatör sınıfı.
/// Kitap ve Kullanıcı → Aggregation (bağımsız yaşar)
/// OduncIslemi       → Composition  (kütüphanesiz anlamsız)
/// </summary>
public class Kutuphane
{
    public string KutuphanAdi { get; }
    public string Adres       { get; }

    private readonly List<Kitap>        _kitapListesi  = new();
    private readonly List<Kullanici>    _uyeListesi    = new();
    private readonly List<OduncIslemi>  _islemListesi  = new();

    public Kutuphane(string kutuphanAdi, string adres)
    {
        KutuphanAdi = kutuphanAdi;
        Adres       = adres;
    }

    // ── Kitap Yönetimi ────────────────────────────────────────────────────────
    public void KitapEkle(Kitap kitap)
    {
        ArgumentNullException.ThrowIfNull(kitap);
        _kitapListesi.Add(kitap);
        Console.WriteLine($"[EKLE] '{kitap.Baslik}' kütüphaneye eklendi.");
    }

    public bool KitapSil(string kitapId)
    {
        var kitap = _kitapListesi.FirstOrDefault(k => k.KitapId == kitapId);
        if (kitap is null) return false;
        _kitapListesi.Remove(kitap);
        Console.WriteLine($"[SİL] Kitap ({kitapId}) silindi.");
        return true;
    }

    public IReadOnlyList<Kitap> KitapAra(string kriter)
        => _kitapListesi
            .Where(k => k.Baslik.Contains(kriter, StringComparison.OrdinalIgnoreCase)
                     || k.Yazar.Contains(kriter, StringComparison.OrdinalIgnoreCase))
            .ToList()
            .AsReadOnly();

    public IReadOnlyList<Kitap> TumKitaplariGetir()
        => _kitapListesi.AsReadOnly();

    // ── Üye Yönetimi ─────────────────────────────────────────────────────────
    public void UyeEkle(Kullanici kullanici)
    {
        ArgumentNullException.ThrowIfNull(kullanici);
        _uyeListesi.Add(kullanici);
    }

    // ── İşlem Yönetimi ───────────────────────────────────────────────────────
    public void IslemKaydet(OduncIslemi islem)
    {
        ArgumentNullException.ThrowIfNull(islem);
        _islemListesi.Add(islem);
    }

    public IReadOnlyList<OduncIslemi> TumGecikmeleriRaporla()
        => _islemListesi.Where(i => i.GecikiyorMu()).ToList().AsReadOnly();

    public IReadOnlyList<OduncIslemi> TumIslemleriGetir()
        => _islemListesi.AsReadOnly();

    // ── Stok Raporu ───────────────────────────────────────────────────────────
    public void StokDurumGoster()
    {
        Console.WriteLine($"\n📚 {KutuphanAdi} — Stok Durumu:");
        Console.WriteLine(new string('─', 70));
        foreach (var kitap in _kitapListesi)
            Console.WriteLine($"  {kitap.BilgileriGoster()}");
        Console.WriteLine(new string('─', 70));
    }

    public override string ToString() => $"<Kutuphane: {KutuphanAdi}>";
}
