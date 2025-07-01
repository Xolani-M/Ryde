# Ryde: Console-Based Ride-Sharing System

Ryde is a modern, object-oriented C# console application that simulates a ride-sharing platform, inspired by real-world services like Uber and Bolt. It demonstrates best practices in OOP, persistent data storage, and user interaction in a console environment.

## Features

- **User Registration & Login**: Passengers and drivers can register and securely log in.
- **Ride Request & Acceptance**: Passengers request rides, which are stored as pending until a driver accepts.
- **Driver Assignment**: Drivers can view and accept pending ride requests, updating ride status in real time.
- **Payment System**: Passengers pay for rides using a wallet system; drivers earn from completed rides.
- **Rating & Feedback**: Passengers can rate drivers after each ride.
- **Reporting**: System generates reports on rides, earnings, and user activity.

## How It Works

1. **Register** as a passenger or driver.
2. **Login** to access your personalized menu.
3. **Passengers**:
   - Request a ride by entering pickup and drop-off locations.
   - View wallet balance, add funds, and see ride history.
   - Rate drivers after rides.
4. **Drivers**:
   - View all pending ride requests.
   - Accept a ride to become the assigned driver.
   - Complete rides and earn money.
   - View total earnings and ride history.

## Technical Highlights

- **OOP Principles**: Clean separation of concerns using interfaces, inheritance, and polymorphism.
- **Services & Helpers**: Modular services for rides, payments, ratings, and location logic.
- **JSON Persistence**: All ride data is saved and loaded from `Data/rides.json` using `System.Text.Json`.
- **Extensible Design**: Easily add new features, such as promotions, advanced reporting, or real-time notifications.

## Project Structure

```
Ryde/
├── Models/           # User, Driver, Passenger, Ride, Rating, Location, Enums
├── Services/         # RideService, PaymentService, RatingService, LocationService
├── Utils/            # ConsoleHelper, ValidationHelper, RideStorageHelper, etc.
├── Data/             # DatabaseInitializer, repositories, rides.json
├── Program.cs        # Main entry point and menu logic
├── Ryde.csproj       # Project file
└── README.md         # This file
```

## Getting Started

1. **Clone the repository**
2. **Open in Visual Studio or VS Code**
3. **Build and run the project**
4. **Follow the on-screen instructions**

> **Tip:** The system comes with sample data for quick testing. All ride requests and completions are persisted in `Data/rides.json`.

## Example Console Flow

```
Welcome to Ryde!
1. Register as Passenger
2. Register as Driver
3. Login
...

Passenger Menu:
1. Request a Ride
2. View Wallet Balance
...

Driver Menu:
1. View Available Ride Requests
2. Accept a Ride
...
```

## Contributing

Pull requests and suggestions are welcome! This project is ideal for learning, experimenting, and extending with new features.

## License

MIT License. See `LICENSE` file for details.

---

*Created with ❤️ for learning, innovation, and fun!*
