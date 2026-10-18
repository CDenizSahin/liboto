namespace KutuphaneOtomasyon.Core.Models;

/// <summary>
/// Bir öğrencinin belirli bir kitabı ödünç aldığı işlemi temsil eder.
/// Composition: Kutuphane silinirse işlemler de anlamsız kalır.
/// </summary>
public class OduncIslemi
{
    public const decimal GunlukCeza = 2.0m; // TL/gün

    public string  IslemId      { get; }
    public Ogrenci Ogrenci      { get; }   // Association → Ogrenci
    public Kitap   Kitap        { get; }   // Association → Kitap
    public DateTime OduncTarihi { get; }
    public DateTime IadeTarihi  { get; }
    public DateTime? GeriDonus  { get; private set; }
    public Durum   Durum        { get; private set; }
    public decimal CezaMiktari  { get; private set; }

    public OduncIslemi(string islemId, Ogrenci ogrenci, Kitap kitap,
                       DateTime oduncTarihi, DateTime iadeTarihi)
    {
        IslemId     = islemId;
        Ogrenci     = ogrenci;
        Kitap       = kitap;
        OduncTarihi = oduncTarihi;
        IadeTarihi  = iadeTarihi;
        GeriDonus   = null;
        Durum       = Durum.Aktif;
        CezaMiktari = 0m;
    }

    /// <summary>Bugün itibarıyla gecikme gün sayısını döndürür.</summary>
    public int GecikmeHesapla()
    {
        var bugun = DateTime.Today;
        if (GeriDonus is null && bugun > IadeTarihi)
            return (bugun - IadeTarihi).Days;
        if (GeriDonus.HasValue && GeriDonus.Value.Date > IadeTarihi.Date)
            return (GeriDonus.Value.Date - IadeTarihi.Date).Days;
        return 0;
    }

    /// <summary>Gecikme süresine göre ceza tutarını hesaplar.</summary>
    public decimal CezaHesapla() => GecikmeHesapla() * GunlukCeza;

    /// <summary>İade işlemini tamamlar; gecikme varsa durumu Gecikti yapar.</summary>
    public void IadeyiTamamla()
    {
        if (Durum == Durum.IadeEdildi)
            throw new InvalidOperationException("Bu işlem zaten iade edildi.");

        GeriDonus   = DateTime.Today;
        CezaMiktari = CezaHesapla();
        Durum       = CezaMiktari > 0 ? Durum.Gecikti : Durum.IadeEdildi;
    }

    /// <summary>Şu an gecikmede mi?</summary>
    public bool GecikiyorMu() => GecikmeHesapla() > 0 && GeriDonus is null;

    public string RaporOlustur()
        => $"İşlem#{IslemId} | Öğrenci: {Ogrenci.TamAd} " +
           $"| Kitap: {Kitap.Baslik} " +
           $"| Gecikme: {GecikmeHesapla()} gün " +
           $"| Ceza: {CezaHesapla():C2} " +
           $"| Durum: {Durum}";

    public override string ToString() => $"<OduncIslemi: {IslemId} — {Durum}>";
}
