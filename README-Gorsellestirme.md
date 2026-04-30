# Görselleştirme, Arayüz ve Test Verileri Yönetimi

Bu çalışma alanı, projenin backend tarafında geliştirilen veri yapılarının (Graf yapısı, Merkle Ağacı, BFS/DFS algoritmaları) kullanıcıya sunulması ve sistemin test edilmesi sürecini kapsar.

## 🚀 Görev Kapsamı

### 1. Sentetik Veri Üretimi (Data Seeding)
Sistemi test etmek amacıyla gerçekçi cüzdan adresleri ve transfer simülasyonları oluşturan modüldür.
* Rastgele cüzdan ID'leri oluşturma.
* Geçmişe dönük işlem (transaction) geçmişi üretimi.

### 2. Graf Görselleştirme
İşlem ağının interaktif bir şekilde ekrana çizdirilmesi:
* **Düğüm Boyutu:** Cüzdan bakiyesine göre dinamik ölçeklendirme.
* **Kenar Kalınlığı:** Transfer edilen miktara (weight) göre çizgi kalınlığı ayarı.

### 3. Arayüz Etkileşimi ve Analiz
* **Yol Takibi:** Kullanıcı bir ID girdiğinde BFS/DFS sonuçlarının ekranda vurgulanması.
* **Merkle Paneli:** Veri bütünlüğünü gösteren Merkle Ağacı yapısının görselleştirilmesi.

## 🛠️ Teknik Bileşenler (Scriptler)
* `DataGenerator.cs`: Test verilerini otomatik üretir.
* `GraphRenderer.cs`: Graf çizim mantığını yönetir.
* `UIController.cs`: Kullanıcı girişlerini ve çıktı panellerini kontrol eder.

> **Not:** Bu çalışma "arayuz-ve-test" branch'i altında, ekip içi görev paylaşımı doğrultusunda geliştirilmektedir.
