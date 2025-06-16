# 🎬 Movies Management System

A comprehensive web application for managing movies with OMDb API integration, built with ASP.NET Core MVC.

## ✨ Features

### 🔍 OMDb API Integration
- **Automatic Movie Data Fetching**: Search movies by title or IMDb ID
- **Auto-fill Forms**: Automatically populate movie information from OMDb database
- **Rich Movie Data**: Director, cast, plot, ratings, awards, runtime, and more
- **Smart Dropdown Selection**: Auto-select country, genre, and category based on OMDb data
- **Real-time Search**: AJAX-powered search with instant results
- **Data Validation**: Ensures data integrity when importing from OMDb

### 📱 Complete CRUD Operations
- **Create Movies**: Add new movies with comprehensive information
- **View Details**: Beautiful detailed view with all movie information
- **Edit Movies**: Full editing capabilities with validation
- **Delete Movies**: Safe deletion with confirmation dialog

### 🎨 Modern UI/UX
- **Responsive Design**: Works perfectly on desktop, tablet, and mobile
- **Bootstrap 5**: Modern and clean interface
- **Movie Cards**: Beautiful grid layout with posters and information
- **Interactive Forms**: Real-time validation and user feedback
- **Modal Dialogs**: Smooth user interactions

### 🗄️ Database Features
- **Entity Framework Core**: Robust data management
- **SQL Server**: Reliable database backend
- **Foreign Key Relationships**: Proper data relationships
- **Migration Support**: Easy database updates

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core 8.0 MVC
- **Frontend**: HTML5, CSS3, JavaScript, Bootstrap 5
- **Database**: SQL Server with Entity Framework Core
- **API Integration**: OMDb API, YouTube Data API v3 (optional)
- **Authentication**: ASP.NET Core Identity
- **Validation**: Server-side and client-side validation

## 📋 Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB or full version)
- Visual Studio 2022 or VS Code
- OMDb API Key (free from [OMDb API](http://www.omdbapi.com/))

## 🚀 Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/datjpro/Movies.git
cd Movies/MoviesApp
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

### 3. Configure OMDb API
Get your free API key from [OMDb API](http://www.omdbapi.com/) and update `appsettings.json`:
```json
{
  "OMDbSettings": {
    "ApiKey": "your-omdb-api-key-here",
    "BaseUrl": "http://www.omdbapi.com/"
  }
}
```

### 4. Run Database Migrations
```bash
dotnet ef database update
```

### 5. Run the Application
```bash
dotnet run
```

Navigate to `https://localhost:5001` or `http://localhost:5000`

## 📖 Usage

### Adding a New Movie
1. Click "Thêm phim mới" (Add New Movie)
2. Use OMDb search to find movie information
3. Click "Điền vào form" to auto-fill movie details
4. Add movie code and adjust any information
5. Save the movie

### Managing Movies
- **View All Movies**: Browse the movie gallery on the homepage
- **Movie Details**: Click "Chi tiết" to see comprehensive movie information
- **Edit Movie**: Click "Sửa" to modify movie information
- **Delete Movie**: Click "Xóa" and confirm deletion

### OMDb Integration
- Search by movie title: `Inception`, `The Dark Knight`
- Search by IMDb ID: `tt1375666`, `tt0468569`
- Automatic mapping of countries and genres
- Smart category detection (Movie vs Series)

## 🔧 Configuration

### Database Settings
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your SQL Server connection string"
  }
}
```

### OMDb API Settings
```json
{
  "OMDbSettings": {
    "ApiKey": "Your OMDb API key",
    "BaseUrl": "http://www.omdbapi.com/"
  }
}
```

### JWT Settings (for API)
```json
{
  "JwtSettings": {
    "Secret": "Your JWT secret key",
    "Issuer": "MoviesApp",
    "Audience": "MoviesApp",
    "TokenLifetimeInMinutes": 60
  }
}
```

## 📁 Project Structure

```
MoviesApp/
├── Areas/Admin/              # Admin area for advanced management
├── Controllers/              # MVC Controllers
│   ├── Api/                 # API Controllers
│   ├── PhimController.cs    # Main movie controller
│   └── ...
├── Data/                    # Database context and migrations
├── Models/                  # Data models and DTOs
├── Services/               # Business logic services
│   ├── OMDbService.cs      # OMDb API integration
│   └── ...
├── Views/                  # Razor views
│   ├── Phim/              # Movie views
│   │   ├── Index.cshtml   # Movie list
│   │   ├── Details.cshtml # Movie details
│   │   ├── Create.cshtml  # Add movie
│   │   ├── Edit.cshtml    # Edit movie
│   │   └── Delete.cshtml  # Delete confirmation
│   └── ...
└── wwwroot/               # Static files (CSS, JS, images)
```

## 🌟 Key Features Showcase

### OMDb Integration
- Real-time movie search and data fetching
- Automatic form population with IMDb data
- Smart mapping of genres and countries
- Support for both movie titles and IMDb IDs

### Movie Management
- Complete CRUD operations with validation
- Rich movie details with posters and metadata
- Responsive design for all screen sizes
- User-friendly confirmation dialogs

### Data Model
- Comprehensive movie information storage
- Support for series and individual movies
- IMDb ratings and metadata integration
- Flexible categorization system

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- [OMDb API](http://www.omdbapi.com/) for providing movie data
- [Bootstrap](https://getbootstrap.com/) for the UI framework
- [Font Awesome](https://fontawesome.com/) for icons
- ASP.NET Core team for the excellent framework

## 📞 Support

If you have any questions or need help, please:
- Open an issue on GitHub
- Check the documentation in the `docs/` folder
- Review the OMDb integration guide

## 🚀 Future Enhancements

- [ ] Movie trailer integration
- [ ] User reviews and ratings
- [ ] Advanced search and filtering
- [ ] Movie recommendations
- [ ] Export/Import functionality
- [ ] Multi-language support

## 📋 Recent Updates (v1.0)

### ✅ Latest Features
- **Complete OMDb Integration**: Full integration with OMDb API for automatic movie data fetching
- **CRUD Operations**: Complete Create, Read, Update, Delete functionality for movies
- **Smart Form Auto-fill**: Intelligent form population with automatic dropdown selection
- **Responsive Movie Gallery**: Beautiful card-based layout with movie posters
- **Data Validation**: Comprehensive server-side and client-side validation
- **Database Migrations**: Fully configured Entity Framework Core with SQL Server
- **Error Handling**: Robust error handling and logging for better debugging

### 🔧 Technical Improvements
- Enhanced `Phim` model with comprehensive movie metadata
- Optimized OMDbService with proper error handling and logging
- Improved JavaScript for real-time form interactions
- Updated views with modern Bootstrap 5 components
- Implemented proper navigation property handling in Entity Framework

---

**Made with ❤️ using ASP.NET Core MVC**