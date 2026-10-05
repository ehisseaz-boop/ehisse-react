# E-Hisseler

Bu layihə e-ticarət / ehtiyat hissələri idarəetmə sistemidir. Backend ASP.NET Core, Frontend isə Vite + React ilə hazırlanıb.

## Proyektin strukturunu

- Backend/: ASP.NET Core tətbiqi
- Frontend/: Vite frontend
- client/: əlavə frontend/klient tərtibatı

## Lokal işə salmaq

1. Backend-i işə salın:
   dotnet restore
   dotnet run --project Backend/EHisseler.Web.csproj

2. Frontend-i işə salın:
   cd Frontend
   npm install
   npm run dev

## GitHub-a yükləmə

```bash
git remote add origin <GITHUB_REPO_URL>
git branch -M main
git push -u origin main
```

> `appsettings.json` və digər lokal konfiqurasiya faylları `.gitignore` ilə yoxlanmışdır.
