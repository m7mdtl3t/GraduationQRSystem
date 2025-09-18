# GraduationQRSystem

نظام إدارة QR codes لحفل التخرج باستخدام ASP.NET Core MVC و PostgreSQL.

## المتطلبات

- .NET 8.0 SDK
- PostgreSQL Database

## التشغيل المحلي

```bash
dotnet restore

# إنشاء وتطبيق migrations للـ PostgreSQL
dotnet ef migrations add InitPostgres
dotnet ef database update

dotnet run
```

سيعمل التطبيق على `http://localhost:5000`

## Database Migration

### للـ PostgreSQL Setup:
```bash
# إنشاء migration جديدة
dotnet ef migrations add AddPhoneNumberToSeniorAndGuest

# تطبيق الـ migrations
dotnet ef database update
```

**ملاحظة**: في Production على Render، الـ migrations تتطبق تلقائياً عند التشغيل.

## النشر على Render

### خطوات النشر:

1. **إنشاء حساب على Render**
   - اذهب إلى [render.com](https://render.com) وأنشئ حساب

2. **ربط Repository**
   - ارفع المشروع على GitHub
   - في Render، اختر "New Web Service"
   - اربط GitHub repository الخاص بك

3. **إعدادات Build**
   ```
   Build Command:
   dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o ./publish
   ```

4. **إعدادات Start**
   ```
   Start Command:
   ./publish/GraduationQRSystem
   ```

5. **متغيرات البيئة (Environment Variables)**
   - `PORT`: سيتم تعيينه تلقائياً بواسطة Render
   - `DATABASE_URL`: PostgreSQL connection string من Render
   
6. **إعداد PostgreSQL Database**
   - في Render، أضف PostgreSQL Database
   - انسخ الـ DATABASE_URL من Render Dashboard
   - أضف Environment Variable في Web Service:
     - Name: `DATABASE_URL`
     - Value: `postgresql://user:password@host:port/dbname` (من Render)
   
   **ملاحظة**: التطبيق يدعم كلا الصيغتين:
   - URI format: `postgresql://user:password@host:port/dbname`
   - Key/Value format: `Host=host;Database=dbname;Username=user;Password=password;SSL Mode=Require;Trust Server Certificate=true`

### ملاحظات مهمة:

- ✅ **PostgreSQL Database**: قاعدة بيانات PostgreSQL من Render
- ✅ **Auto Migrations**: التطبيق سيطبق Migrations تلقائياً عند التشغيل
- ✅ **Sample Data**: إذا كانت قاعدة البيانات فارغة، سيتم إضافة بيانات تجريبية
- ✅ **Port Configuration**: التطبيق مُعد للعمل مع PORT الذي يحدده Render
- ✅ **Phone Numbers**: كل Senior وGuest لديه رقم موبايل مصري

### البيانات التجريبية:

عند التشغيل لأول مرة، سيتم إنشاء:
- **Senior Test** مع **Guest Test** واحد

### استكشاف الأخطاء:

1. **خطأ في Build**: تأكد من أن جميع dependencies موجودة في `.csproj`
2. **خطأ في Database**: تحقق من وجود ملف `graduation.db` في المشروع
3. **خطأ في Port**: التطبيق مُعد للعمل مع متغير البيئة `PORT`

### الميزات:

- 📱 **QR Code Generation**: إنتاج QR codes للطلاب وضيوفهم
- 👥 **Guest Management**: إدارة قائمة الضيوف لكل طالب
- 💾 **SQLite Database**: قاعدة بيانات محلية بسيطة وموثوقة
- 🌐 **Responsive Design**: تصميم متجاوب يعمل على جميع الأجهزة

### الهيكل:

```
GraduationQRSystem/
├── Controllers/        # Controllers للتطبيق
├── Data/              # Database Context
├── Models/            # Data Models (Senior, Guest)
├── Views/             # Razor Views
├── wwwroot/           # Static files (CSS, images, QR codes)
├── Migrations/        # Entity Framework Migrations
├── graduation.db      # SQLite Database
└── Program.cs         # نقطة البداية
```

## الدعم

في حالة وجود مشاكل، تحقق من:
- إعدادات Render Build & Start commands
- متغيرات البيئة
- ملفات logs في Render dashboard