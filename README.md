# 🎬 Movies Management System with CDN Video Streaming

A comprehensive web application for managing movies with OMDb API integration and CDN-powered video streaming, built with ASP.NET Core MVC.

## ✨ Features

### 🔍 OMDb API Integration
- **Automatic Movie Data Fetching**: Search movies by title or IMDb ID
- **Auto-fill Forms**: Automatically populate movie information from OMDb database
- **Rich Movie Data**: Director, cast, plot, ratings, awards, runtime, and more
- **Smart Dropdown Selection**: Auto-select country, genre, and category based on OMDb data
- **Real-time Search**: AJAX-powered search with instant results
- **Data Validation**: Ensures data integrity when importing from OMDb

### 🎥 CDN Video Streaming
- **Video Upload API**: Upload videos through HTTP API endpoint `http://localhost:5288/api/v1/videos/upload`
- **CDN Integration**: Optimized video delivery through Content Delivery Network
- **Adaptive Bitrate Streaming**: Automatic quality adjustment based on user's internet connection
- **Multiple Format Support**: Compatible with MP4, MOV, FLV, CMAF formats
- **Streaming Protocols**: Support for HLS, DASH, WebRTC, RTMP, and RTSP protocols
- **Video Content Management**: Secure video storage and delivery system
- **DRM Protection**: Built-in Digital Rights Management for content security

### 📱 Complete CRUD Operations
- **Create Movies**: Add new movies with comprehensive information and video upload
- **View Details**: Beautiful detailed view with embedded video player
- **Edit Movies**: Full editing capabilities with video replacement options
- **Delete Movies**: Safe deletion with confirmation dialog and CDN cleanup

### 🎨 Modern UI/UX
- **Responsive Design**: Works perfectly on desktop, tablet, and mobile
- **Bootstrap 5**: Modern and clean interface
- **Movie Cards**: Beautiful grid layout with posters and video previews
- **Interactive Video Player**: HTML5 video player with CDN streaming
- **Modal Dialogs**: Smooth user interactions
- **Upload Progress**: Real-time video upload progress indicators

### 🗄️ Database & Storage Features
- **Entity Framework Core**: Robust data management
- **SQL Server**: Reliable database backend
- **CDN Storage**: Distributed video storage for optimal performance
- **Foreign Key Relationships**: Proper data relationships including video metadata
- **Migration Support**: Easy database updates

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core 8.0 MVC
- **Frontend**: HTML5, CSS3, JavaScript, Bootstrap 5
- **Database**: SQL Server with Entity Framework Core
- **CDN**: Content Delivery Network for video streaming
- **API Integration**: OMDb API, Video Upload API
- **Video Processing**: HTTP-based video upload and streaming
- **Authentication**: ASP.NET Core Identity
- **Validation**: Server-side and client-side validation

## 📋 Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB or full version)
- Visual Studio 2022 or VS Code
- OMDb API Key (free from [OMDb API](https://www.omdbapi.com/))
- CDN Service Account (Cloudflare, AWS CloudFront, or similar)
- Video API endpoint running on `http://localhost:5288`

## 🚀 Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/datjpro/movie-web-mvc-dotnet.git
cd movie-web-mvc-dotnet/MoviesApp
```

### 2. Configure Database
Update `appsettings.json` with your SQL Server connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WebMovies;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### 3. Configure APIs and CDN
Update `appsettings.json` with your API keys and CDN settings:

```json
{
  "OMDbSettings": {
    "ApiKey": "your-omdb-api-key-here",
    "BaseUrl": "http://www.omdbapi.com/"
  },
  "VideoSettings": {
    "UploadApiUrl": "http://localhost:5288/api/v1/videos/upload",
    "CdnBaseUrl": "https://your-cdn-domain.com",
    "MaxFileSize": 104857600,
    "AllowedFormats": ["mp4", "mov", "avi", "mkv"]
  },
  "CdnSettings": {
    "Provider": "Cloudflare",
    "ApiKey": "your-cdn-api-key",
    "ZoneId": "your-zone-id"
  }
}
```

### 4. Run Database Migrations
```bash
dotnet ef database update
```

### 5. Start Video API Service
Ensure your video API service is running on `http://localhost:5288`

### 6. Run the Application
```bash
dotnet run
```
Navigate to `https://localhost:5000` or `http://localhost:5032`

## 📖 Usage

### Adding a Movie with Video
1. Click "Thêm phim mới" (Add New Movie)
2. Use OMDb search to find movie information
3. Click "Điền vào form" to auto-fill movie details
4. **Upload Video**: Select video file and upload through CDN API
5. Monitor upload progress and CDN processing
6. Save the movie with video URL

### Video Management
- **Video Upload**: Drag-and-drop or browse video files
- **CDN Processing**: Automatic video optimization and distribution
- **Streaming**: Adaptive bitrate streaming for optimal viewing
- **Video Preview**: Thumbnail generation and preview functionality
- **Quality Options**: Multiple quality levels for different devices

### Managing Movies
- **View All Movies**: Browse the movie gallery with video previews
- **Movie Details**: Click "Chi tiết" to see movie info with embedded video player
- **Edit Movie**: Modify information and replace videos
- **Delete Movie**: Safe deletion with CDN cleanup

## 🔧 Video API Integration

### Upload Video Endpoint
```javascript
// Upload video to CDN
const uploadVideo = async (file, movieId) => {
    const formData = new FormData();
    formData.append('video', file);
    formData.append('movieId', movieId);
    
    const response = await fetch('http://localhost:5288/api/v1/videos/upload', {
        method: 'POST',
        body: formData,
        headers: {
            'Authorization': 'Bearer ' + token
        }
    });
    
    return await response.json();
};
```

### CDN Configuration
```csharp
// Configure CDN in ASP.NET Core
services.Configure<CdnSettings>(Configuration.GetSection("CdnSettings"));
services.AddScoped<ICdnService, CdnService>();
services.AddScoped<IVideoService, VideoService>();
```

## 🎥 Video Features

### Upload Process
- **File Validation**: Check format, size, and codec compatibility
- **CDN Upload**: Transfer to CDN storage via HTTP API
- **Processing**: Automatic transcoding and optimization
- **Distribution**: Content distributed across CDN edge servers
- **Metadata Storage**: Video information saved to database

### Streaming Capabilities
- **Adaptive Streaming**: Automatic quality adjustment based on bandwidth
- **Global Delivery**: CDN edge servers for worldwide content delivery
- **Security**: DRM protection and access control
- **Analytics**: Video performance and viewing statistics
- **Caching**: Intelligent caching for faster content delivery

## 📁 Project Structure

```
MoviesApp/
├── Areas/Admin/              # Admin area with video management
├── Controllers/              # MVC Controllers
│   ├── Api/                 # API Controllers
│   ├── PhimController.cs    # Main movie controller
│   ├── VideoController.cs   # Video management controller
│   └── ...
├── Data/                    # Database context and migrations
├── Models/                  # Data models and DTOs
│   ├── VideoModels/        # Video-related models
│   └── ...
├── Services/               # Business logic services
│   ├── OMDbService.cs      # OMDb API integration
│   ├── CdnService.cs       # CDN integration service
│   ├── VideoService.cs     # Video processing service
│   └── ...
├── Views/                  # Razor views
│   ├── Phim/              # Movie views with video player
│   ├── Video/             # Video management views
│   └── ...
└── wwwroot/               # Static files including video player assets
```

## 🌟 Advanced Video Features

### CDN Integration Benefits
- **Performance**: Reduced loading times through global distribution
- **Scalability**: Handle high traffic loads with CDN infrastructure
- **Reliability**: Multiple edge servers ensure high availability
- **Cost Efficiency**: Optimized bandwidth usage and reduced server load

### Video Security
- **DRM Protection**: Content protection against unauthorized access
- **Access Control**: Role-based video access permissions
- **Encrypted Streaming**: Secure video transmission
- **Anti-Piracy**: Advanced protection mechanisms

## 🚀 Future Enhancements

- [ ] **Live Streaming**: Real-time video streaming capabilities
- [ ] **Video Analytics**: Detailed viewing statistics and insights
- [ ] **Mobile App**: Native mobile applications for video streaming
- [ ] **AI Integration**: Automatic video tagging and content analysis
- [ ] **Social Features**: Comments, ratings, and sharing functionality
- [ ] **Offline Viewing**: Download videos for offline consumption
- [ ] **Multi-language Subtitles**: Support for multiple subtitle tracks

## 📋 Recent Updates (v2.0)

### ✅ Latest Video Features
- **CDN Integration**: Full Content Delivery Network integration for video streaming
- **HTTP Video API**: RESTful API for video upload and management
- **Adaptive Streaming**: Dynamic quality adjustment based on connection speed
- **Multiple Format Support**: Support for various video formats and codecs
- **Global Distribution**: Worldwide content delivery through CDN edge servers
- **Video Security**: DRM protection and access control implementation
- **Performance Optimization**: Enhanced loading times and streaming quality

### 🔧 Technical Video Improvements
- Enhanced video upload with progress tracking
- CDN-optimized video delivery and caching
- Improved video player with adaptive bitrate streaming
- Robust error handling for video operations
- Optimized database schema for video metadata storage
- Implemented video thumbnail generation and preview

---

Made with ❤️ using ASP.NET Core MVC + CDN Video Streaming
