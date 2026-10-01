# Movie Management System Using ASP.NET

An ASP.NET MVC web application for managing movies, categories, bookings, and user authentication.

## 🚀 Features

- **User Management**:
  - User Registration & Login
  - User Profile Management
- **Movie Categories**:
  - Add, edit, list, and manage movie categories (Genre/Type)
- **Movie Catalog**:
  - Add new movies with release date, category, and ticket rate
  - Edit and delete movies
  - Search movies by title or category
- **Booking System**:
  - Book tickets for movies
  - View and manage bookings
  - Edit booking details

## 🛠️ Tech Stack

- **Framework**: ASP.NET MVC 5 (.NET Framework 4.7.2+)
- **Language**: C#
- **Database**: Microsoft SQL Server (ADO.NET / `SqlConnection`)
- **Frontend**: Razor View Engine, Bootstrap 5, HTML5, CSS3, jQuery

## 📋 Database Setup

Configure your SQL Server connection string in `assignment_1/Web.config`:
```xml
<connectionStrings>
  <add name="MovieDBConnection" 
       connectionString="Data Source=YOUR_SERVER;Initial Catalog=MovieDB;Integrated Security=True;" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

Key tables:
- `Tbl_User` (`User_ID`, `Username`, `Password`, `Email`, etc.)
- `Tbl_Movie_Category` (`Cat_ID`, `Cat_Type`)
- `Tbl_Movie` (`Movie_ID`, `Movie_name`, `Release_Date`, `Cat_ID`, `rate`)
- `Tbl_Booking` (`Booking_ID`, `User_ID`, `Movie_ID`, `Booking_Date`, `Tickets`, `Total_Amount`)

## 💻 Getting Started

1. Open `assignment_1.sln` in **Visual Studio 2019 / 2022**.
2. Right-click the Solution and select **Restore NuGet Packages**.
3. Update the `MovieDBConnection` connection string in `Web.config` to match your local SQL Server instance.
4. Press `F5` or `Ctrl + F5` to run the application on IIS Express.
