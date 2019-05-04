using AnketUygulamaSistemi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AnketUygulamaSistemi.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(string id, string password)
        {
            using (AnketEntities db = new AnketEntities())
            {
                Kullanici kullanici = db.Kullanici.Where(x => x.kAdi == id).Where(y => y.sifre == password).FirstOrDefault();
                if (kullanici == null)
                {
                    TempData["Error"] = "Kullanıcı Adı ya da Şifre Hatalı!";
                    return RedirectToAction("Index");
                }
                else
                {
                    Session["kullanici"] = "" + kullanici.id;
                    return RedirectToAction("AnketGoruntule");
                }
            }
        }
        public ActionResult Register(string fullName, string eMail, string userName, string kPassword, string kkPassword)
        {
            using (AnketEntities db = new AnketEntities())
            {
                Kullanici kullanici = db.Kullanici.Where(x => x.kAdi == userName).FirstOrDefault();
                if (kullanici == null)
                {
                    kullanici = new Kullanici();
                    kullanici.kAdi = userName;
                    kullanici.sifre = kPassword;
                    kullanici.eMail = eMail;
                    kullanici.adSoyad = fullName;
                    db.Kullanici.Add(kullanici);
                    db.SaveChanges();
                    TempData["RegSuccess"] = "Kullanıcı Kaydı Başarılı! Lütfen Giriş Yapınız";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["RegError"] = "Bu Kullanıcı adı zaten Mevcut Lütfen Tekrar Deneyiniz";
                    return RedirectToAction("Index");
                }
            }
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("AnketGoruntule");
        }
        public ActionResult Giris()
        {
            List<AnketTipleri> atipleri = new List<AnketTipleri>();
            using (AnketEntities db=new AnketEntities())
            {
                atipleri = db.AnketTipleri.ToList();
            }
            return View(atipleri);
        }
        [HttpPost]
        [ValidateInput(false)]
        public void Kaydet(List<String> gSoru)
        {
            Anket anket = new Anket();
            Sorular soru = new Sorular();
            using (AnketEntities db = new AnketEntities())
            {
                int counter = 0;
                anket.anketAdi = gSoru[0].ToString();
                anket.anketTur = gSoru[1].ToString();
                anket.anketKullaniciId = Convert.ToInt32(Session["kullanici"].ToString());
                foreach(var text in gSoru)
                {
                    if (text == "1$#$#" || text == "2$#$#" || text == "3$#$#")
                        counter++;
                }
                anket.anketSoruSayisi = counter;
                db.Anket.Add(anket);
                db.SaveChanges();
            }

            List<Soru> sorular = new List<Soru>();
            int yeni = 0;
            for (int i = 2; i < gSoru.Count; i++)
            {
                if (gSoru[i] == "1$#$#" || gSoru[i] == "2$#$#" || gSoru[i] == "3$#$#")
                {
                    using (AnketEntities db = new AnketEntities())
                    {
                        var soruC = db.Sorular.Where(x => x.soruId == soru.soruId).FirstOrDefault();
                        soruC.secenekSayisi = db.Secenekler.Where(x => x.soruId == soru.soruId).ToList().Count;
                        if (gSoru[i] == "1$#$#")
                            soruC.soruTipId = 1;
                        else if (gSoru[i] == "2$#$#")
                            soruC.soruTipId = 2;
                        else
                            soruC.soruTipId = 3;
                        db.SaveChanges();
                        soru = new Sorular();
                    }
                    yeni = 0;
                }
                else
                {
                    if (yeni == 0)
                    {
                        using (AnketEntities db = new AnketEntities())
                        {
                            soru.anketId = anket.anketId;
                            soru.soruMetni = gSoru[i].ToString();
                            
                            soru.soruTipId = 3;//çek
                            db.Sorular.Add(soru);
                            db.SaveChanges();
                        }
                        yeni++;
                    }

                    else
                    {
                        using (AnketEntities db = new AnketEntities())
                        {
                            int sId = soru.soruId; //int double ile değişecek vt'da da
                            Secenekler secenek;
                            secenek = new Secenekler();
                            secenek.secenekMetni = gSoru[i];
                            secenek.soruId = sId;
                            db.Secenekler.Add(secenek);
                            db.SaveChanges();
                        }
                    }

                }
            }
            /*using (AnketEntities db = new AnketEntities())
            {
                anket.anketSoruSayisi = db.Sorular.Where(x => x.anketId == anket.anketId).ToList().Count;
                db.Anket.Add(anket);
                db.SaveChanges();
            }*/
        }
        public ActionResult AnketDetay(int id)
        {
            Dictionary<Sorular, List<Secenekler>> model = new Dictionary<Sorular, List<Secenekler>>();   
            using(AnketEntities db=new AnketEntities())
            {
                var sorular = db.Sorular.Where(x => x.anketId == id).ToList();
                foreach (Sorular soru in sorular)
                {
                    var secenekler = db.Secenekler.Where(x => x.soruId == soru.soruId).ToList();
                    model.Add(soru, secenekler);
                }

            }
            return View(model);
        }

        public ActionResult SifreDegistir(string eskiSifre, string yeniSifre, string tYeniSifre)
        {
            //burada kaldık
            return View();
        }

        public ActionResult KullaniciSayfasi()
        {
            int id = Convert.ToInt32(Session["Kullanici"]);
            ViewBag.id = id;
            ViewBag.kul = null;
            using (AnketEntities db = new AnketEntities())
            {
                Kullanici kul = new Kullanici();
                kul = db.Kullanici.Where(x => x.id == id).FirstOrDefault();
                ViewBag.kul = kul;
            }
                return View();
        }

        public ActionResult AnketLog(FormCollection anket)
        {
            using (AnketEntities db = new AnketEntities())
            {
                int anketId = Convert.ToInt32(anket["anketBaslikId"]);
                String[] soruIds = new String[anket.Count];
                soruIds = anket.AllKeys;
                Cevaplar cevap;

                for(int i = 1;i < anket.Count; i++)
                {
                    cevap = new Cevaplar();
                    cevap.soruId = Convert.ToInt32(soruIds[i]);
                    cevap.cevap = anket[i];
                    db.Cevaplar.Add(cevap);
                }
                db.SaveChanges();
            }

            return RedirectToAction("AnketGoruntule");
        }
        public ActionResult AnketGoruntule()
        {
            List<Soru> soru = new List<Soru>();
            List<Anket> anketler;
            using (AnketEntities db = new AnketEntities())
            {
                 anketler = db.Anket.ToList();

                //var anket = db.Anket.Where(x => x.anketId == anketId).FirstOrDefault();
               // List<Sorular> sorular = db.Sorular.Where(x => x.anketId == anketId).ToList();


               
                //for(int i=0; i<sorular.Count; i++)
                //{
                //    Soru tmp = new Soru();
                //    tmp.soruMetni = sorular.ElementAt(i).soruMetni;
                //    List<Secenekler> secenekler = db.Secenekler.Where(x => x.soruId == sorular[i].soruId).ToList();
                //    for (int j = 0; j < sorular[i].secenekSayisi; j++)
                //    {
                //        soru[i].cevaplar[j] = secenekler[j].secenekMetni;
                //    }
                //    //tmp.cevaplar = ;

                //    soru[i].soruMetni = sorular[i].soruMetni;
                //    //soru[i].soruTipi=
                //    soru[i].cevaplar = new List<String>();
                    
                //}
            }

            //soru


                return View(anketler);
        } 
    }
}