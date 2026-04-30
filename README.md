Aşama 1: Çekirdek Blokzincir Altyapısı (Backend)
Geliştirici: [Yusif Hasanguliyev]

Bu modül, projenin temel veritabanı ve güvenlik motorunu oluşturur. Geleneksel veritabanları yerine, verilerin değiştirilemez (immutable) ve kriptografik olarak birbirine bağlı olduğu bir blokzincir (blockchain) mimarisi sıfırdan inşa edilmiştir.

Geliştirilen Temel Sınıflar (Sınıf Yapısı):

Islem.cs: Ağ üzerindeki fon transferlerini temsil eder. Her işlem; gönderen cüzdan, alıcı cüzdan, miktar ve zaman damgası (timestamp) verilerini içerir.

MerkleAgaci.cs: Bir blok içindeki tüm işlemlerin özetini (hash) hiyerarşik bir ağaç yapısında birleştirerek tek bir "Kök Hash" (Merkle Root) üretir. Veri bütünlüğünü sağlar.

Dugum.cs (Blok): İşlemleri (transferleri) içinde barındıran zincir halkalarıdır. Kendi Hash değeri, bir önceki bloğun Hash değeri (zincir bağı) ve Nonce (madencilik kanıtı) değerlerini tutar.

Blokzincir.cs: Sistemin ana beynidir. İlk bloğu (Genesis) oluşturur, madencilik (Proof of Work) simülasyonu ile yeni blokları kazar, zincire ekler ve tüm zincirin geçerliliğini (kırılıp kırılmadığını) denetler.

Nasıl Çalışır?
Oluşturulan yeni işlemler bir havuza alınır. Madencilik işlemi tetiklendiğinde, bu işlemler bir Merkle Ağacında toplanır ve SHA-256 algoritması kullanılarak önceki bloğa kriptografik bir şifre ile kilitlenir. Bu sayede, geçmişteki hiçbir cüzdan işlemi (transfer) sonradan değiştirilemez. Bu sağlam altyapı, bir sonraki "Graf Mimarisi ve Algoritmalar" aşaması için güvenilir veri kaynağını oluşturur.
