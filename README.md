# GraduationQRSystem

نظام إدارة QR codes لحفل التخرج باستخدام ASP.NET Core MVC و SQLite.

## المتطلبات

- .NET 8.0 SDK
- SQLite (مدمج مع Entity Framework Core)

## التشغيل المحلي

```bash
dotnet restore
dotnet run
```

سيعمل التطبيق على `http://localhost:5000`

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
   - لا حاجة لإضافة متغيرات إضافية

### ملاحظات مهمة:

- ✅ **SQLite Database**: ملف `graduation.db` سيتم نسخه تلقائياً مع التطبيق
- ✅ **Auto Migrations**: التطبيق سيطبق Migrations تلقائياً عند التشغيل
- ✅ **Sample Data**: إذا كانت قاعدة البيانات فارغة، سيتم إضافة بيانات تجريبية
- ✅ **Port Configuration**: التطبيق مُعد للعمل مع PORT الذي يحدده Render

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