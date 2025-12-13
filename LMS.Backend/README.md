

#  Bu proje, bir Kütüphane Yönetim Sistemi (library management system) için geliştirilmiş modern, katmanlı bir
#  RESTful Web API uygulamasıdır. Proje, kullanıcı kaydı/girişi, kitap/kütüphane yönetimi ve 
#  öğrenci-kitap ödünç alma işlemlerini yetkilendirme ile yönetmeyi amaçlamaktadır.

Yazılım Dili = C#
Framework = ASP.NET Core, NET 8 versiyonu
Veritabanı = MS SQL Server LocalDB
Veri Erişim Teknolojisi = Entity Framework Core
Güvenlik Mekanizması = JWT (JSON Web Token)
API Belgeleme = Swagger UI (OpenAPI)

# API Kullanım Akışı ve Test Adımları

Adım 1: Temel Veri Girişi 
   > Kütüphane Kaydı: POST /api/Libraries endpoint'ine geçerli bir LibraryCreateDto gönderin.

Adım 2: Güvenlik (Authentication) Adımları
   > Öğrenci Kaydı: POST /api/Students endpoint'ine yeni bir öğrenci bilgisi gönderin.
   > Giriş ve Token Alma: POST /api/Students/Login endpoint'ine kullanıcı adı ve şifreyi göndererek bir JWT Token alın.

Adım 3: Yetkilendirilmiş (Authorized) İşlemler
   > Token'ı Kullanma: Önceki adımda aldığınız JWT Token'ı kopyalayın.
   > Authorization: Bearer [Kopyaladığınız Token]
   > Token'ı başarıyla kullandıktan sonra, artık API'ye kim olduğunuzu kanıtladınız 
   ve ödünç alma işlemini başlatabilirsiniz.
   > Endpoint: POST /api/StudentBooks ile bir öğrenci-kitap ödünç alma kaydı oluşturun.
        
GET	/api/Books, /api/Students/{id}	Kaynakları listelemenizi (tüm kitaplar) veya 
belirli bir kaynağın detaylarını okumanızı sağlar.

PUT	/api/Books/{id}	Var olan bir kaynağın (belirli bir ID'ye sahip kitap) tüm bilgilerini güncellemenizi sağlar.

<Kurulan Temel NuGet Paketleri>

> Projenin temel mimarisini oluşturmak için öncelikle Entity Framework Core paketleri kurulmuştur.
Bunlar; C# Model sınıflarını veritabanı tablolarına dönüştüren ana paket:
Microsoft.EntityFrameworkCore, MS SQL Server ile iletişim için gereken sağlayıcı paket
Microsoft.EntityFrameworkCore.

> SqlServer ve Migration komutlarını çalıştıran araç paketi Microsoft.EntityFrameworkCore.Tools'dur.

> Güvenlik katmanı için, kullanıcı girişinde üretilen JWT Token'ını yetkili işlemlerde doğrulamak üzere
Microsoft.AspNetCore.Authentication.JwtBearer paketi eklenmiştir. 

> API endpoint'lerini, DTO şemalarını ve modelleri otomatik olarak algılayıp interaktif Swagger UI test arayüzünü
oluşturmak amacıyla Swashbuckle.AspNetCore ve Microsoft.AspNetCore.OpenApi paketleri kurulmuştur.


#  Temel NuGet paketlerini kurduktan sonra, projenin veri yapısı oluşturuldu.

>Varlık Modelleri (Models): Projenin ana varlıklarını (Book, Library, Student, StudentBook) temsil eden C# sınıfları tanımlandı.

> Veritabanı Bağlantısı (DbContext): Bu Modelleri veritabanına bağlamak için LMSDbContext.cs sınıfı,
Entity Framework Core'un DbContext sınıfından miras alınarak oluşturuldu ve Modeller DbSet olarak buraya eklendi.

> Bağlantı Dizesi: Veritabanı adresi ve bağlantı bilgileri (DefaultConnection) appsettings.json dosyasına eklendi.

> Veritabanı Oluşturma: Migration komutları (dotnet ef database update) çalıştırılarak veritabanı yapısı
fiziksel olarak oluşturuldu.


#  Model ve veritabanı yapısı hazırlandıktan sonra, uygulamanın çalışması için gerekli olan tüm hizmetler 
#  ve kurallar Program.cs dosyasında tanımlanmıştır.

> Veritabanı Hizmetinin Eklenmesi: appsettings.json dosyasındaki bağlantı dizesi kullanılarak, 
Entity Framework Core'un LMSDbContext sınıfı bir hizmet olarak uygulamaya tanımlanmıştır 
(builder.Services.AddDbContext<LMSDbContext>).

> Controller ve Swagger Hizmetleri: API Controller'ları ve interaktif dokümantasyon aracı olan 
Swagger/OpenAPI hizmetleri (AddControllers, AddSwaggerGen) uygulamaya eklenmiştir.

> JWT Yetkilendirme (Authentication) Tanımlaması: Uygulamanın güvenlik mimarisi yapılandırılmıştır.
appsettings.json içindeki Jwt:Key (Gizli Anahtar), Issuer ve Audience değerleri kullanılarak 
Token doğrulama kuralları belirlenmiş ve JwtBearer şeması tanımlanmıştır. 
Bu, yetki gerektiren API endpoint'lerinin korunmasını sağlar.

> Geliştirme Ortamı Ayarları: Uygulamanın Geliştirme ortamında çalışması için gerekli olan 
Middleware'ler (örneğin Swagger UI) etkinleştirilmiştir.

> Middleware Zinciri: Gelen HTTP isteklerinin sırasıyla Yönlendirme (UseRouting), 
Yetkilendirme (UseAuthentication, UseAuthorization) ve Controller'lara Eşleme (MapControllers) 
aşamalarından geçmesi sağlanmıştır.


# Veritabanı ve temel hizmetler tanımlandıktan sonra, API'nin iş mantığını 
# ve dış dünyaya açılan kapılarını oluşturma sürecine geçilmiştir.

> Veri Transfer Nesneleri (DTO'lar): DTOs klasörü altında, veritabanı modellerinin (Models) belirli işlemler
için basitleştirilmiş ve güvenli sürümleri (BookCreateDto, StudentLoginDto, StudentReadDto) oluşturulmuştur.
Bu yaklaşım, gereksiz verilerin API'ye gönderilmesini/alınmasını engellemiş ve güvenlik 
(örneğin şifre alanını gizlemek) sağlamıştır.

> İş Mantığı ve Endpoint'ler (Controller'lar): Controllers klasöründe, API isteklerini işleyecek sınıflar
(BooksController.cs, StudentsController.cs vs.) oluşturulmuştur. Bu Controller'lar, HTTP metotlarına
(GET, POST, PUT, DELETE) yanıt vererek veritabanı işlemleri (CRUD) ve JWT tabanlı login mantığını yürütür.

# Projenin güvenlik, performans ve temiz kod açısından yapılan ana iyileştirmeleri:

> NuGet Paket Uyumsuzluğu Çözümü:
Projenin kurulum aşamasında, özellikle Entity Framework Core ve JWT ile ilgili paketlerin eski veya 
uyumsuz versiyonlarını kullanmaya çalışmaktan kaynaklanan hatalar alındı. Bu sorun, projenin hedeflediği
.NET 8 sürümüyle tam olarak eşleşen, paketlerin en güncel ve kararlı versiyonları tercih edilerek çözüldü.
Ayrıca, başarısız yüklemelerden sonra projeyi temizleyip yeniden inşa etmek (Clean and Rebuild Solution) ve
Microsoft.EntityFrameworkCore.Tools gibi geliştirme araçlarının konfigürasyonunu doğru yapmak,
temel bağımlılıkların hatasız ve sağlam bir şekilde kurulmasını sağlamıştır.

> JWT Token Doğrulama Çözümü: Başlangıçta yaşanan, POST /api/Books gibi endpoint'lerde Token'ın algılanmaması 
ve sürekli 401 Unauthorized hatası alınması problemi çözülmüştür. Çözüm, appsettings.json dosyasındaki
Jwt:Key değerinin HMAC-SHA256 standardına uygun (minimum 32 karakterden uzun) ve karmaşık bir anahtar ile 
değiştirilmesi ve Program.cs dosyasında Encoding.UTF8 kullanımıyla tutarlı hale getirilmesidir.

> Nullable Referans Türlerinin Kullanımı: C# 8 ve üzeri sürümlerin getirdiği Nullable özelliği proje genelinde
etkinleştirilmiştir (<Nullable>enable</Nullable>). Bu, olası NullReferenceException hatalarını derleme aşamasında
yakalamayı sağlayarak uygulamanın dayanıklılığını artırmıştır.

#  Gelecek İyileştirme Notu: DTO ve Model Dönüşümleri

> Şu anda proje, DTO'lar (Veri Transfer Nesneleri) ile veritabanı modelleri arasındaki veri dönüşümlerini
manuel olarak gerçekleştirmektedir. Projenin ölçeklenebilirliğini artırmak, kod tekrarını önlemek 
(DRY Prensibi) ve Controller katmanını daha temiz tutmak amacıyla, gelecekte bu manuel eşleme işlemlerini
AutoMapper kütüphanesi ile otomatikleştirmek hedeflenmektedir. Bu, özellikle veri güvenliği
(okuma DTO'larında şifre hash'lerini hariç tutma) ve yeni alan eklemelerindeki bakım kolaylığı açısından 
kritik bir iyileştirme olacaktır.

> Gelecekte, API'yi kullananların daha kolay gezinmesi için Swagger UI arayüzündeki Controller gruplarının 
(Book, Library, Student) ve Schemas (DTO) bölümündeki veri modellerinin gösterim sırasını projenin mantıksal 
akışına uygun şekilde düzenlemek hedeflenmektedir.