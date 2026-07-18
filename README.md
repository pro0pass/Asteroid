# Asteroid - Meta Quest 3D App Launcher

**Asteroid** is a comprehensive Meta Quest 3 application that provides a beautiful 3D menu interface for managing apps, settings, APK installation, signing, and wireless debugging.

## Features

### 🎮 3D Menu System
- Smooth, black Meta Quest launcher-inspired design
- Falling cosmic shower background animation
- Scalable menu interface
- Preloaded apps with logos and names
- Bottom-right positioned UI

### ⚙️ Settings
- **World Scale**: 10-100 (base: 10)
- **Height Offset**: 0-1 (base: 0)
- **FOV**: 80-120 (base: 80)
- **WSAD Movement**: Joystick-based with controller support
- **Battery Saver**: Optimize for battery life
- **Arm Offset**: 0-1 (base: 0)

### 📦 App Management
- View all preloaded APKs
- Download and install apps from store
- APK signer for development
- Wireless debugging support

### 🖥️ Terminal
- Integrated terminal for debugging
- Wireless debugging enabled
- Command execution

### 📱 Preloaded Apps
- Asteroid Settings
- Additional Meta Quest apps

## Project Structure

```
Asteroid/
├── Assets/
│   ├── UI/
│   │   ├── Menu3D/
│   │   ├── Icons/
│   │   └── Background/
│   ├── Models/
│   ├── Shaders/
│   └── Sounds/
├── Scripts/
│   ├── Menu/
│   ├── Settings/
│   ├── AppManager/
│   ├── Terminal/
│   ├── Controllers/
│   └── Utils/
├── Scenes/
│   ├── MainMenu.unity
│   ├── Settings.unity
│   └── AppLoader.unity
├── Plugins/
│   └── Android/
├── gradle.properties
├── build.gradle
└── AndroidManifest.xml
```

## Requirements

- Meta Quest 3
- Unity 2022.3+
- Android API Level 29+
- Wireless Debugging Enabled

## Installation

1. Clone the repository
2. Open in Unity 2022.3+
3. Configure Android build settings
4. Build and deploy to Meta Quest 3

## Usage

### Settings Configuration
1. Launch Asteroid
2. Access Settings from main menu
3. Adjust World Scale, Height Offset, FOV, movement speed, etc.
4. Changes apply in real-time

### Installing Apps
1. Navigate to Store
2. Browse available apps
3. Install to your headset

### APK Management
1. Go to "All APKs" section
2. View installed applications
3. Sign APKs for distribution

### Terminal Access
1. Open Terminal from menu
2. Execute commands for debugging
3. Monitor wireless debugging output

## Development

### Wireless Debugging
- Enabled by default
- Connect via ADB over TCP/IP
- Port: 5555

### Building
```bash
./gradlew build
./gradlew assembleDebug
```

## License

MIT
