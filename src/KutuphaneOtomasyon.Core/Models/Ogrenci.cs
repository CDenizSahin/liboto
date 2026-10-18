namespace KutuphaneOtomasyon.Core.Models;

/// <summary>
/// Üniversite öğrencisi. Kitap arayabilir, ödünç alabilir, iade edebilir.
/// Kullanici'dan kalıtım alır (Inheritance).
/// </summary>
public class Ogrenci : Kullanici
{
    public const int MaxOduncLimit = 5;

    public string OgrenciNo          { get; }
    public string Bolum              { get; set; }
    public int    AktifOduncSayisi   { get; private set; }

    private readonly List<OduncIslemi> _oduncGecmisi = new();

    public Ogrenci(string kullaniciId, string ad, string soyad,
                   string email, string sifre,
                   string ogrenciNo, string bolum)
        : base(kullaniciId, ad, soyad, email, sifre)
    {
        OgrenciNo         = ogrenciNo;
        Bolum             = bolum;
        AktifOduncSayisi  = 0;
    }

    /// <summary>Öğrenci giriş doğrulaması. (Gerçekte veritabanıyla karşılaştırılır.)</summary>
    public override bool GirisYap(string email, string sifre)
        => Email == email && SifreDogrula(sifre);

    /// <summary>Başlık veya yazar adına göre kitap listesinde arar.</summary>
    public IReadOnlyList<Kitap> KitapAra(string anahtar,
                                          IEnumerable<Kitap> kitapListesi)
        => kitapListesi
            .Where(k => k.Baslik.Contains(anahtar, StringComparison.OrdinalIgnoreCase)
                     || k.Yazar.Contains(anahtar, StringComparison.OrdinalIgnoreCase))
            .ToList()
            .AsReadOnly();

    /// <summary>
    /// Kitabı ödünç alır. Limit doluysa veya kitap stokta yoksa null döner.
    /// </summary>
    public OduncIslemi? KitapOduncAl(Kitap kitap, Kutuphane kutuphane,
                                      int oduncSuresiGun = 14)
    {
        if (AktifOduncSayisi >= MaxOduncLimit)
        {
            Console.WriteLine($"⚠ [{TamAd}] Maksimum ödünç limitine ulaşıldı ({MaxOduncLimit}).");
            return null;
        }

        if (!kitap.MusaitMi())
        {
            Console.WriteLine($"⚠ '{kitap.Baslik}' şu an mevcut değil.");
            return null;
        }

        kitap.StokAzalt();
        AktifOduncSayisi++;

        var islem = new OduncIslemi(
            islemId:     $"ISL-{DateTime.Now:yyyyMMddHHmmss}-{OgrenciNo}",
            ogrenci:     this,
            kitap:       kitap,
            oduncTarihi: DateTime.Today,
            iadeTarihi:  DateTime.Today.AddDays(oduncSuresiGun)
        );

        _oduncGecmisi.Add(islem);
        kutuphane.IslemKaydet(islem);
        Console.WriteLine($"✅ '{kitap.Baslik}' başarıyla ödünç alındı. " +
                          $"İade tarihi: {islem.IadeTarihi:dd.MM.yyyy}");
        return islem;
    }

    /// <summary>İade işlemini gerçekleştirir; varsa cezayı raporlar.</summary>
    public void KitapIade(OduncIslemi islem)
    {
        if (islem.Ogrenci != this)
            throw new InvalidOperationException("Bu işlem bu öğrenciye ait değil.");

        islem.IadeyiTamamla();
        islem.Kitap.StokArttir();
        AktifOduncSayisi--;
        Console.WriteLine($"✅ İade tamamlandı. Ceza: {islem.CezaMiktari:C2}");
    }

    /// <summary>Tüm ödünç geçmişini döndürür (kopya liste).</summary>
    public IReadOnlyList<OduncIslemi> OduncGecmisiniGor()
        => _oduncGecmisi.AsReadOnly();

    public override string ToString()
        => $"<Ogrenci: {TamAd} | {OgrenciNo} | {Bolum}>";
}
