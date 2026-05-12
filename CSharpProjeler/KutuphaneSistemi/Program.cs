using System;
using System.Collections.Generic;
using System.IO;

namespace KutuphaneSistemi
{
    public abstract class Kisi
    {
        public string Ad { get; protected set; }

        public Kisi(string ad)
        {
            if (ad == "")
                throw new Exception("Isim bos olamaz.");

            Ad = ad;
        }

        public abstract string TurYaz();
    }

    public class Ogrenci : Kisi
    {
        public string Bolum { get; private set; }

        public Ogrenci(string ad, string bolum) : base(ad)
        {
            Bolum = bolum;
        }

        public override string TurYaz()
        {
            return "Ogrenci: " + Ad + " - " + Bolum;
        }
    }

    public class Kitap
    {
        public int Id { get; private set; }
        public string Ad { get; private set; }
        public string Yazar { get; private set; }
        public bool OduncMu { get; private set; }
        public string AlanKisi { get; private set; }

        public Kitap(int id, string ad, string yazar, bool oduncMu, string alanKisi)
        {
            if (ad == "")
                throw new Exception("Kitap adi bos olamaz.");

            if (yazar == "")
                throw new Exception("Yazar bos olamaz.");

            Id = id;
            Ad = ad;
            Yazar = yazar;
            OduncMu = oduncMu;
            AlanKisi = alanKisi;
        }

        public void OduncAl(Kisi kisi)
        {
            if (OduncMu)
                throw new Exception("Kitap zaten odunc alinmis.");

            OduncMu = true;
            AlanKisi = kisi.TurYaz();
        }

        public void IadeEt()
        {
            if (!OduncMu)
                throw new Exception("Kitap zaten kutuphanede.");

            OduncMu = false;
            AlanKisi = "-";
        }

        public override string ToString()
        {
            return Id + ";" + Ad + ";" + Yazar + ";" + OduncMu + ";" + AlanKisi;
        }
    }

    public interface IIslem<T>
    {
        void Ekle(T nesne);
        List<T> Listele();
        void Sil(int id);
        void Guncelle(int id, T nesne);
    }

    public class KitapDosya : IIslem<Kitap>
    {
        private string dosya = "kitaplar.txt";

        public KitapDosya()
        {
            if (!File.Exists(dosya))
                File.Create(dosya).Close();
        }

        public void Ekle(Kitap kitap)
        {
            File.AppendAllText(dosya, kitap.ToString() + Environment.NewLine);
        }

        public List<Kitap> Listele()
        {
            List<Kitap> liste = new List<Kitap>();
            string[] satirlar = File.ReadAllLines(dosya);

            foreach (string satir in satirlar)
            {
                if (satir == "")
                    continue;

                string[] p = satir.Split(';');
                liste.Add(new Kitap(int.Parse(p[0]), p[1], p[2], bool.Parse(p[3]), p[4]));
            }

            return liste;
        }

        public void Sil(int id)
        {
            List<Kitap> liste = Listele();
            bool bulundu = false;

            for (int i = 0; i < liste.Count; i++)
            {
                if (liste[i].Id == id)
                {
                    liste.RemoveAt(i);
                    bulundu = true;
                    break;
                }
            }

            if (!bulundu)
                throw new Exception("Kitap bulunamadi.");

            Kaydet(liste);
        }

        public void Guncelle(int id, Kitap yeni)
        {
            List<Kitap> liste = Listele();
            bool bulundu = false;

            for (int i = 0; i < liste.Count; i++)
            {
                if (liste[i].Id == id)
                {
                    liste[i] = yeni;
                    bulundu = true;
                    break;
                }
            }

            if (!bulundu)
                throw new Exception("Kitap bulunamadi.");

            Kaydet(liste);
        }

        private void Kaydet(List<Kitap> liste)
        {
            List<string> satirlar = new List<string>();

            foreach (Kitap kitap in liste)
                satirlar.Add(kitap.ToString());

            File.WriteAllLines(dosya, satirlar);
        }
    }

    public class Kutuphane
    {
        private KitapDosya dosya = new KitapDosya();

        public void KitapEkle()
        {
            Console.Write("Id: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Kitap adi: ");
            string ad = Console.ReadLine();

            Console.Write("Yazar: ");
            string yazar = Console.ReadLine();

            Kitap kitap = new Kitap(id, ad, yazar, false, "-");
            dosya.Ekle(kitap);

            Console.WriteLine("Kitap eklendi.");
        }

        public void KitapListele()
        {
            List<Kitap> kitaplar = dosya.Listele();

            if (kitaplar.Count == 0)
            {
                Console.WriteLine("Kitap yok.");
                return;
            }

            foreach (Kitap k in kitaplar)
            {
                Console.WriteLine(k.Id + " - " + k.Ad + " - " + k.Yazar + " - " +
                    (k.OduncMu ? "Oduncte / " + k.AlanKisi : "Rafta"));
            }
        }

        public void KitapSil()
        {
            Console.Write("Silinecek id: ");
            int id = int.Parse(Console.ReadLine());

            dosya.Sil(id);
            Console.WriteLine("Kitap silindi.");
        }

        public void KitapGuncelle()
        {
            Console.Write("Guncellenecek id: ");
            int eskiId = int.Parse(Console.ReadLine());

            Console.Write("Yeni id: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Yeni kitap adi: ");
            string ad = Console.ReadLine();

            Console.Write("Yeni yazar: ");
            string yazar = Console.ReadLine();

            Kitap kitap = new Kitap(id, ad, yazar, false, "-");
            dosya.Guncelle(eskiId, kitap);

            Console.WriteLine("Kitap guncellendi.");
        }

        public void OduncAl()
        {
            Console.Write("Kitap id: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Ogrenci adi: ");
            string ad = Console.ReadLine();

            Console.Write("Bolum: ");
            string bolum = Console.ReadLine();

            Kisi ogrenci = new Ogrenci(ad, bolum);
            List<Kitap> kitaplar = dosya.Listele();

            foreach (Kitap k in kitaplar)
            {
                if (k.Id == id)
                {
                    k.OduncAl(ogrenci);
                    dosya.Guncelle(id, k);
                    Console.WriteLine("Kitap odunc verildi.");
                    return;
                }
            }

            throw new Exception("Kitap bulunamadi.");
        }

        public void IadeEt()
        {
            Console.Write("Kitap id: ");
            int id = int.Parse(Console.ReadLine());

            List<Kitap> kitaplar = dosya.Listele();

            foreach (Kitap k in kitaplar)
            {
                if (k.Id == id)
                {
                    k.IadeEt();
                    dosya.Guncelle(id, k);
                    Console.WriteLine("Kitap iade edildi.");
                    return;
                }
            }

            throw new Exception("Kitap bulunamadi.");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Kutuphane kutuphane = new Kutuphane();
            bool devam = true;

            while (devam)
            {
                Console.Clear();
                Console.WriteLine("--- KUTUPHANE SISTEMI ---");
                Console.WriteLine("1- Kitap ekle");
                Console.WriteLine("2- Kitap listele");
                Console.WriteLine("3- Kitap guncelle");
                Console.WriteLine("4- Kitap sil");
                Console.WriteLine("5- Odunc al");
                Console.WriteLine("6- Iade et");
                Console.WriteLine("0- Cikis");
                Console.Write("Secim: ");

                string secim = Console.ReadLine();

                try
                {
                    if (secim == "1")
                        kutuphane.KitapEkle();
                    else if (secim == "2")
                        kutuphane.KitapListele();
                    else if (secim == "3")
                        kutuphane.KitapGuncelle();
                    else if (secim == "4")
                        kutuphane.KitapSil();
                    else if (secim == "5")
                        kutuphane.OduncAl();
                    else if (secim == "6")
                        kutuphane.IadeEt();
                    else if (secim == "0")
                        devam = false;
                    else
                        Console.WriteLine("Yanlis secim.");
                }
                catch (Exception hata)
                {
                    Console.WriteLine("Hata: " + hata.Message);
                }

                if (devam)
                {
                    Console.WriteLine("Devam etmek icin tusa bas...");
                    Console.ReadKey();
                }
            }
        }
    }
}

