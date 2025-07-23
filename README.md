# 📊 System Monitor Dashboard

A comprehensive **real-time system monitoring solution** built with **.NET 8**, featuring a **REST API backend** and a **cross-platform MAUI mobile application**. Monitor your services, track system logs, and maintain visibility across your infrastructure with an elegant, professional interface.

## 🚀 Features

### 🔧 **Backend API**
- **RESTful API** with comprehensive service and log endpoints
- **JWT-style authentication** with encrypted API keys
- **Real-time data generation** with realistic system scenarios
- **File-based persistence** with automatic backup every 5 minutes
- **Enhanced logging** with multiple severity levels
- **Swagger documentation** for easy API exploration
- **CORS support** for cross-platform integration

### 📱 **Mobile Application**
- **Cross-platform MAUI app** (Android, iOS, Windows, macOS)
- **Real-time dashboard** with live service status monitoring
- **Interactive log viewer** with color-coded severity levels
- **Manual API key management** with validation
- **Professional UI design** with modern Material Design principles
- **Responsive layouts** optimized for mobile and tablet devices

### 🎯 **Key Capabilities**
- **Service Health Monitoring**: Track 8+ system services with real-time status updates
- **Comprehensive Logging**: Monitor Info, Warning, Error, Success, and Debug events
- **Automatic Data Generation**: Realistic logs generated every minute with smart algorithms
- **Persistent Storage**: JSON-based file storage with automatic cleanup
- **Advanced Filtering**: Search and filter logs by date, level, and source
- **Security**: Encrypted API keys with expiration management

---

## 🏗️ Architecture

```
┌─────────────────────┐    HTTP/REST    ┌─────────────────────┐
│                     │ ◄──────────────► │                     │
│   MAUI Mobile App   │                  │    .NET 8 Web API   │
│                     │                  │                     │
│ • Dashboard UI      │                  │ • Services Controller│
│ • Log Viewer        │                  │ • Logs Controller   │
│ • API Key Manager   │                  │ • Auth Controller   │
│ • Real-time Updates │                  │ • Enhanced Data Svc │
└─────────────────────┘                  └─────────────────────┘
                                                    │
                                         ┌─────────▼─────────┐
                                         │                   │
                                         │   File Storage    │
                                         │                   │
                                         │ • services.json   │
                                         │ • logs.json       │
                                         │ • stats.json      │
                                         └───────────────────┘
```

---

## 📦 Installation & Setup

### Prerequisites
- **.NET 8.0 SDK** or later
- **Visual Studio 2022** or **VS Code** with C# extension
- **Android SDK** (for Android deployment)
- **iOS development tools** (for iOS deployment on macOS)

### 🔧 Backend Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/system-monitor-dashboard.git
   cd system-monitor-dashboard
   ```

2. **Navigate to API project**
   ```bash
   cd AppIntgration.API
   ```

3. **Restore dependencies**
   ```bash
   dotnet restore
   ```

4. **Run the API**
   ```bash
   dotnet run
   ```

5. **Access Swagger documentation**
   ```
   https://localhost:7039/swagger
   ```

### 📱 Mobile App Setup

1. **Navigate to MAUI project**
   ```bash
   cd AppIntgration.Maui
   ```

2. **Update API URL** in `Services/ApiService.cs`
   ```csharp
   private const string API_BASE_URL = "https://your-api-url:7039";
   ```

3. **Restore dependencies**
   ```bash
   dotnet restore
   ```

4. **Run the mobile app**
   ```bash
   dotnet build
   dotnet run --framework net8.0-android
   ```

---

## 🎮 Usage Guide

### 🔑 **Getting Started**

1. **Start the API server** - Launch the backend API
2. **Open the mobile app** - Start the MAUI application
3. **Generate API key** - Tap "Generate" to create a new API key
4. **Load data** - Tap "Services" or "Logs" to view real-time data

### 📊 **Dashboard Features**

#### **Service Monitoring**
- View 8 system services with real-time status updates
- Color-coded status indicators (Green=Active, Orange=Maintenance, Red=Error)
- Detailed service descriptions and creation timestamps
- Services include: Web Server, API Gateway, Authentication, File Upload, Email, Background Jobs, Analytics, and Cache services

#### **Log Management**
- Real-time log streaming with automatic updates every minute
- Color-coded severity levels with intuitive icons
- Detailed timestamps and source attribution
- Exception handling with expandable error details
- Smart filtering and search capabilities

#### **API Key Management**
- **Auto-generation**: Automatic secure key creation
- **Manual input**: Enter custom API keys
- **Validation**: Real-time key verification
- **Copy function**: Easy key sharing with clipboard integration

---

## 📈 Sample Data

The system generates realistic monitoring data including:

### **Services**
- 🌐 **Web Server (Nginx)** - Frontend web server handling HTTP requests
- 🚪 **API Gateway** - Main API gateway routing requests  
- 🔐 **Authentication Service** - User authentication and authorization
- 📁 **File Upload Service** - File upload and storage handling
- 📧 **Email Service** - Email notifications and messaging
- ⚙️ **Background Jobs** - Scheduled background tasks
- 📊 **Analytics Service** - Data analytics and reporting
- 💾 **Cache Service (Redis)** - In-memory caching system

### **Log Examples**
```
ℹ️ Info     | Web Server: Request processed successfully in 245ms
⚠️ Warning  | Analytics: High memory usage detected - 87% used  
❌ Error    | Email: Database connection failed - timeout after 15s
✅ Success  | Cache: Backup completed successfully - 542MB processed
🔧 Debug    | API: Cache hit ratio: 94%
```

---

## 🔧 API Endpoints

### **Authentication**
```http
GET  /api/auth/apikey     # Generate new API key
POST /api/auth/validate   # Validate existing key
```

### **Services**
```http
GET /api/services         # Get all services
GET /api/services/{id}    # Get specific service
```

### **Logs**
```http
GET /api/logs             # Get paginated logs
GET /api/logs/search      # Search logs with filters
```

### **Health Check**
```http
GET /health               # API health status
```

---

## 🛠️ Technical Stack

### **Backend (.NET 8 Web API)**
- **Framework**: ASP.NET Core 8.0
- **Authentication**: Custom JWT-style encrypted tokens
- **Data Storage**: JSON file-based persistence
- **Logging**: Built-in .NET logging with Elastic APM support
- **Documentation**: Swagger/OpenAPI 3.0
- **Security**: AES-256 encryption for API keys

### **Frontend (.NET MAUI)**
- **Framework**: .NET Multi-platform App UI (MAUI)
- **Architecture**: MVVM with CommunityToolkit.Mvvm
- **UI Framework**: Native platform controls with custom styling
- **HTTP Client**: System.Net.Http with JSON serialization
- **Platforms**: Android, iOS, Windows, macOS

### **Shared Components**
- **DTOs**: Shared data transfer objects
- **Constants**: Centralized API endpoints and status codes
- **Requests/Responses**: Strongly-typed API contracts

---

## 📁 Project Structure

```
AppIntgration.API.sln
├── 📂 AppIntgration.API/              # Web API Backend
│   ├── Controllers/                   # REST API Controllers
│   ├── Services/                      # Business Logic Services
│   ├── Middleware/                    # Authentication & CORS
│   └── Program.cs                     # API Configuration
├── 📂 AppIntgration.Maui/             # Cross-platform Mobile App
│   ├── Views/                         # XAML User Interface
│   ├── ViewModels/                    # MVVM View Models
│   ├── Services/                      # API Integration Services
│   └── Platforms/                     # Platform-specific Code
└── 📂 AppIntgration.Shard/            # Shared Components
    ├── DTOs/                          # Data Transfer Objects
    ├── Constants/                     # API Constants
    ├── Requests/                      # Request Models
    └── Responses/                     # Response Models
```

---

## 🔒 Security Features

- **Encrypted API Keys**: AES-256 encryption with secure key management
- **Token Expiration**: 24-hour automatic key expiration
- **Request Validation**: Comprehensive input validation and sanitization
- **CORS Protection**: Configurable cross-origin resource sharing
- **Authentication Middleware**: Custom JWT-style authentication pipeline

---

## 🚀 Performance Optimizations

- **Efficient Data Generation**: Smart algorithms for realistic system simulation
- **Memory Management**: Automatic cleanup with 1000-log retention limit
- **File I/O Optimization**: Batched file operations every 5 minutes
- **Responsive UI**: Asynchronous operations with loading indicators
- **Caching Strategy**: In-memory caching with persistence backup

---

## 🧪 Testing

### **API Testing**
```bash
# Run API tests
dotnet test AppIntgration.API.Tests

# Test specific endpoint
curl -H "Authorization: Bearer YOUR_API_KEY" \
     https://localhost:7039/api/services
```

### **Mobile Testing**
- **Android Emulator**: Test on various Android API levels
- **iOS Simulator**: Test on different iOS versions (macOS only)
- **Windows App**: Test native Windows implementation

---

## 🤝 Contributing

1. **Fork the repository**
2. **Create a feature branch** (`git checkout -b feature/amazing-feature`)
3. **Commit your changes** (`git commit -m 'Add amazing feature'`)
4. **Push to the branch** (`git push origin feature/amazing-feature`)
5. **Open a Pull Request**

### **Development Guidelines**
- Follow **C# coding conventions**
- Maintain **comprehensive documentation**
- Include **unit tests** for new features
- Ensure **cross-platform compatibility**

---

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- **Microsoft .NET Team** for the excellent .NET 8 and MAUI frameworks
- **Elastic.Apm** for application performance monitoring capabilities
- **CommunityToolkit.Mvvm** for clean MVVM architecture implementation
- **Swashbuckle** for comprehensive API documentation

---

## 📞 Support

For questions, bug reports, or feature requests:


- **Email**: mohamed.dev321@gmail.com

---

## 🔮 Roadmap

- [ ] **Database Integration** (SQL Server, PostgreSQL)
- [ ] **Docker Containerization** with docker-compose
- [ ] **Real-time WebSocket** connections for live updates
- [ ] **Advanced Analytics** with charts and dashboards
- [ ] **Push Notifications** for critical system alerts
- [ ] **Multi-tenant Support** for enterprise deployments
- [ ] **Export Functionality** (PDF, Excel, CSV)
- [ ] **Role-based Access Control** (RBAC)

---

<div align="center">

**⭐ Star this repository if you find it helpful!**

[![GitHub stars](https://img.shields.io/github/stars/yourusername/system-monitor-dashboard.svg?style=social&label=Star)](https://github.com/yourusername/system-monitor-dashboard)
[![GitHub forks](https://img.shields.io/github/forks/yourusername/system-monitor-dashboard.svg?style=social&label=Fork)](https://github.com/yourusername/system-monitor-dashboard/fork)

</div>
