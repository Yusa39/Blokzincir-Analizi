# Blokzincir İşlem Ağları ve Fon Akış Analizi

Bu proje, bir blokzincir ağındaki cüzdanlar arası transfer işlemlerini graf veri yapısı kullanarak modellemek ve analiz etmek amacıyla geliştirilmiştir. Sistem üzerinden fonların izlediği rotalar takip edilebilmekte ve işlem bütünlüğü Merkle Tree yapısıyla doğrulanabilmektedir.

## Proje Özellikleri

- **Graf Veri Yapısı:** Cüzdanlar düğüm (node), işlemler ise kenar (edge) olarak modellenmiştir.
- **Fon Takibi (BFS & DFS):** Belirli bir cüzdandan çıkan fonların hangi adreslere ulaştığı Derinlik Öncelikli (DFS) ve Genişlik Öncelikli (BFS) arama algoritmalarıyla analiz edilmektedir.
- **Merkle Tree Entegrasyonu:** Blok içindeki işlemlerin hash bütünlüğü ikili ağaç yapısıyla korunmaktadır.
- **Bakiye Hesaplama:** Cüzdanların gelen/giden işlem geçmişine göre güncel bakiyeleri otomatik olarak hesaplanmaktadır.

##  Kullanılan Teknolojiler

- **Dil:** C# (.NET Core / Framework)
- **Veri Yapıları:** Directed Graph, Merkle Tree, Queues/Stacks (Algoritmalar için)
- **Kütüphaneler:** System.Security.Cryptography (Hash işlemleri için)

##  Ekip ve Görev Dağılımı

- **[Mehmet Akif Dereci]**: Graf veri yapısının kurulması, BFS ve DFS algoritmalarının C# entegrasyonu ve bakiye hesaplama mantığının geliştirilmesi.



