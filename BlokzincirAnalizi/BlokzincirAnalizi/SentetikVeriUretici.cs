namespace BlokzincirAnalizi
{
    /// <summary>
    /// Test ve demonstrasyon amaçlı sentetik blokzincir verisi üretir.
    ///
    /// Üretilen veri:
    ///   - 15–20 benzersiz cüzdan adresi (Bitcoin benzeri)
    ///   - 40–50 rastgele yönlü transfer işlemi
    ///   - Tekrarlanabilirlik için seed parametresi desteklenir
    /// </summary>
    public static class SentetikVeriUretici
    {
        private const string Alfabe = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        /// <summary>Bitcoin benzeri kısa cüzdan adresi üretir (örn: W03_AB4X9K7Z).</summary>
        private static string RastgeleCuzdan(int indeks, Random rng)
        {
            string sonek = new(Enumerable.Range(0, 8)
                .Select(_ => Alfabe[rng.Next(Alfabe.Length)])
                .ToArray());
            return $"W{indeks:D2}_{sonek}";
        }

        /// <summary>
        /// Belirtilen parametrelerle dolu bir BlokzincirAgi döndürür.
        /// </summary>
        /// <param name="cuzdanSayisi">Üretilecek cüzdan sayısı (varsayılan: 18)</param>
        /// <param name="islemSayisi">Üretilecek işlem sayısı (varsayılan: 45)</param>
        /// <param name="tohum">Rastgele sayı üreteci tohumu — tekrarlanabilirlik için</param>
        public static BlokzincirAgi Uret(
            int cuzdanSayisi = 18,
            int islemSayisi  = 45,
            int tohum        = 42)
        {
            var rng = new Random(tohum);
            var ag  = new BlokzincirAgi();

            // ---- Cüzdanları oluştur ----
            var adresler = new List<string>();
            for (int i = 1; i <= cuzdanSayisi; i++)
            {
                string adres = RastgeleCuzdan(i, rng);
                // Başlangıç bakiyesi: 0.5 – 100 BTC (geniş dağılım)
                decimal baslangic = Math.Round((decimal)(rng.NextDouble() * 99.5 + 0.5), 4);
                ag.CuzdanEkle(adres, baslangic);
                adresler.Add(adres);
            }

            // ---- İşlemleri oluştur ----
            // Son 30 günü kapsayan zaman penceresi
            DateTime baslangicZaman = DateTime.Now.AddDays(-30);

            for (int i = 0; i < islemSayisi; i++)
            {
                string gonderen = adresler[rng.Next(adresler.Count)];

                // Gönderici ile alıcının farklı olmasını garanti et
                string alici;
                do { alici = adresler[rng.Next(adresler.Count)]; }
                while (alici == gonderen);

                decimal miktar = Math.Round((decimal)(rng.NextDouble() * 7.99 + 0.01), 4);
                DateTime zaman = baslangicZaman
                    .AddHours(rng.Next(720))
                    .AddMinutes(rng.Next(60));

                ag.IslemEkle(new Islem(gonderen, alici, miktar, zaman));
            }

            return ag;
        }
    }
}
