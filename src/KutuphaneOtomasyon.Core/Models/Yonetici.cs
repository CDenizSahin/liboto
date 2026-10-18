namespace KutuphaneOtomasyon.Core.Models;

/// <summary>
/// Kütüphane yöneticisi. Kitap ekler/siler/günceller, raporları görüntüler.
/// Kullanici'dan kalıtım alır (Inheritance).
/// </summary>
public class Yonetici : Kullanici
{
    public string PersonelId { get; }
    public string Yetki      { get; set; }

    public Yonetici(string kullaniciId, string ad, string soyad,
                    string email, string sifre,
                    string personelId, string yetki = "Tam Yetki")
        : base(kullaniciId, ad, soyad, email, sifre)
    {
        PersonelId = personelId;
        Yetki      = yetki;
    }

    /// <summary>Yönetici kimlik doğrulaması (ör: LDAP / SSO entegrasyonu).</summary>
    public override bool GirisYap(string email, string sifre)
        => Email == email && SifreDogrula(sifre);

    /// <summary>Kütüphaneye yeni kitap ekler.</summary>
    public void KitapEkle(Kitap kitap, Kutuphane kutuphane)
        => kutuphane.KitapEkle(kitap);

    /// <summary>Kitabı kütüphaneden kaldırır.</summary>
    public bool KitapSil(string kitapId, Kutuphane kutuphane)
        => kutuphane.KitapSil(kitapId);

    /// <summary>Kitap bilgilerini günceller.</summary>
    public void KitapGuncelle(Kitap kitap, string? baslik = null,
                               string? yazar = null, string? kategori = null)
    {
        if (baslik   is not null) kitap.Baslik   = baslik;
        if (yazar    is not null) kitap.Yazar    = yazar;
        if (kategori is not null) kitap.Kategori = kategori;
        Console.WriteLine($"[GÜNCELLE] '{kitap.Baslik}' güncellendi.");
    }

    /// <summary>Gecikmeli ödünç işlemlerini konsola yazdırır.</summary>
    public IReadOnlyList<OduncIslemi> GecikmeRaporuGor(Kutuphane kutuphane)
    {
        var gecikmeliler = kutuphane.TumGecikmeleriRaporla();
        Console.WriteLine($"\n📊 Gecikme Raporu ({gecikmeliler.Count} kayıt):");
        Console.WriteLine(new string('─', 70));
        foreach (var islem in gecikmeliler)
            Console.WriteLine($"  • {islem.RaporOlustur()}");
        Console.WriteLine(new string('─', 70));
        return gecikmeliler;
    }

    /// <summary>Stok durumunu konsola yazdırır.</summary>
    public void StokRaporuGor(Kutuphane kutuphane)
        => kutuphane.StokDurumGoster();

    public override string ToString()
        => $"<Yonetici: {TamAd} | {PersonelId} | {Yetki}>";
}
