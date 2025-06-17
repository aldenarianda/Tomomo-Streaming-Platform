# Tomomo - Streaming Platform

Ini adalah proyek platform streaming film yang dibuat dengan .NET 8 MVC dan SQL Server.

## Cara Menjalankan Proyek

1.  **Clone Repositori:**
    ```
    git clone [https://github.com/NAMA_ANDA/NAMA_REPO_ANDA.git](https://github.com/NAMA_ANDA/NAMA_REPO_ANDA.git)
    ```
2.  **Buat Database:**
    Jalankan skrip SQL yang ada di dalam deskripsi atau folder `DatabaseScripts` (jika Anda membuatnya) untuk membuat tabel yang diperlukan di SSMS.
3.  **Konfigurasi Koneksi:**
    - Buat file baru bernama `appsettings.json` di dalam folder utama proyek (`Tomomo7/`).
    - Isi file tersebut dengan format berikut, dan sesuaikan dengan konfigurasi server SQL Anda:
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=NAMA_SERVER_ANDA;Database=NAMA_DATABASE_ANDA;Trusted_Connection=True;Encrypt=False;"
      },
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft.AspNetCore": "Warning"
        }
      },
      "AllowedHosts": "*"
    }
    ```
4.  **Buka dan Jalankan:**
    Buka file `.sln` dengan Visual Studio dan jalankan proyeknya (tekan F5).
