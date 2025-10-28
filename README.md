# 🚗 RideFix - Car Maintenance & Emergency Service Platform

RideFix is a comprehensive car maintenance and emergency service platform built with .NET 8, designed to connect car owners with skilled technicians for both emergency roadside assistance and regular maintenance services.

## 🌟 Key Features

### 🚨 Emergency Request Services
- **Real-time Emergency Requests**: Car owners can create emergency service requests with location tracking
- **Technician Matching**: Smart algorithm matches car owners with nearby available technicians
- **PIN Verification**: Secure PIN-based authentication for emergency requests
- **Auto-cancellation**: Automatic request cancellation after 4 hours if not accepted
- **Request Management**: Complete request lifecycle management (create, cancel, complete)

### 👥 Car Owner Services
- **Profile Management**: Complete car owner profiles with car information
- **Car Registration**: Register and manage multiple vehicles
- **Request History**: View past emergency requests and maintenance records
- **Location Services**: GPS-based location tracking for accurate service delivery

### 🔧 Technical Services
- **Technician Profiles**: Detailed technician profiles with specializations and ratings
- **Service Categories**: Categorized services (tire change, battery jump, fuel delivery, etc.)
- **Availability Management**: Real-time technician availability status
- **Skill-based Matching**: Match technicians based on required service type and location

### 📊 Statistics & Reporting
- **Activity Reports**: Comprehensive activity tracking across all platform entities
- **Maintenance Analytics**: Detailed car maintenance history and analytics
- **Performance Metrics**: Service completion rates and technician performance
- **Financial Reports**: Revenue tracking and payment analytics

### 💬 Real-time Chat using SignalR
- **Live Messaging**: Real-time chat between car owners and technicians
- **Chat Sessions**: Persistent chat sessions for ongoing service requests
- **Message History**: Complete message history and conversation tracking
- **Connection Management**: Automatic connection/disconnection handling

### 🛒 E-Commerce Platform
- **Product Catalog**: Browse and search car maintenance products and accessories
- **Shopping Cart**: Add products to cart and manage quantities
- **Order Management**: Complete order processing and tracking
- **Product Reviews**: Rate and review products with comments
- **Inventory Management**: Real-time stock tracking and management

### 💳 Payment Services
- **RideCoins System**: Virtual currency system for platform transactions
- **Stripe Integration**: Secure payment processing using Stripe
- **Multiple Payment Methods**: Support for various payment options
- **Transaction History**: Complete payment and transaction records

### 🔧 Car Maintenance Services
- **Maintenance Records**: Track all car maintenance activities
- **Maintenance Types**: Categorized maintenance services (oil change, brake service, etc.)
- **Scheduled Reminders**: Email reminders for upcoming maintenance
- **Cost Tracking**: Track maintenance costs and expenses
- **Maintenance History**: Complete maintenance timeline and records

## 🏗️ Architecture

### Clean Architecture Implementation
- **Domain Layer**: Core business entities and contracts
- **Application Layer**: Service abstractions and business logic
- **Infrastructure Layer**: Data persistence and external service integrations
- **Presentation Layer**: API controllers and SignalR hubs

### Technology Stack
- **Backend**: .NET 8, ASP.NET Core Web API
- **Database**: SQL Server with Entity Framework Core
- **Real-time Communication**: SignalR for live chat and notifications
- **Authentication**: JWT Bearer tokens with ASP.NET Core Identity
- **Background Jobs**: Hangfire for scheduled tasks
- **Caching**: Redis for session management
- **Payment Processing**: Stripe integration
- **Email Services**: SMTP email delivery
- **Face Recognition**: Face++ API integration

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server
- Redis Server
- Visual Studio 2022 or VS Code

### Installation
1. Clone the repository
2. Update connection strings in `appsettings.json`
3. Run database migrations
4. Start the application

### API Endpoints

#### Emergency Requests
- `POST /api/Request` - Create emergency request
- `GET /api/Request/RequestBreifDTOs/{carOwnerID}` - Get request history
- `DELETE /api/Request/CancelAll/{CarOwnerID}` - Cancel all requests

#### Chat Services
- `GET /api/Chat/GetAllChats` - Get all chat sessions
- `GET /api/Chat/GetChatById` - Get specific chat details
- `GET /api/Chat/LoadCurrentChat` - Load current active chat

#### Car Maintenance
- `POST /api/CarMaintanance` - Add maintenance record
- `GET /api/CarMaintanance/GetMaintenanceSummary` - Get maintenance summary

#### E-Commerce
- `GET /api/Product` - Get product catalog
- `POST /api/Order` - Create order
- `GET /api/ShoppingCart` - Get shopping cart items

## 🔌 SignalR Hubs

### ChatHub (`/chathub`)
- Real-time messaging between car owners and technicians
- Connection management and user authentication
- Message broadcasting and delivery

### NotificationHub (`/notificationhub`)
- Real-time notifications for service requests
- Technician availability updates
- System-wide announcements

### RequestWatchDogHub (`/requestWatchDogHub`)
- Monitor emergency request status
- Real-time request updates
- Technician response tracking

## 📱 Features Overview

### For Car Owners
- Create emergency service requests
- Track request status in real-time
- Chat with assigned technicians
- Manage car maintenance records
- Shop for car products and accessories
- View service history and ratings

### For Technicians
- Receive emergency service requests
- Accept or decline service requests
- Chat with car owners
- Update service status
- Manage availability
- Track earnings and performance

### For Administrators
- Monitor platform activity
- Manage user accounts
- View system statistics
- Handle reports and disputes
- Configure system settings

## 🔒 Security Features

- JWT-based authentication
- Role-based authorization
- PIN verification for emergency requests
- Secure payment processing
- Data encryption and protection
- Input validation and sanitization

## 📈 Monitoring & Analytics

- Real-time activity tracking
- Performance metrics
- User engagement analytics
- Service completion rates
- Financial reporting
- System health monitoring

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📞 Support

For support and questions, please contact the development team or create an issue in the repository.

---

**RideFix** - Connecting car owners with skilled technicians for reliable automotive services.
