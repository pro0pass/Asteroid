# Asteroid Setup Guide

## Prerequisites

- Meta Quest 3 Headset
- Unity 2022.3 LTS or newer
- Android SDK (API 29+)
- Meta Quest Development Kit
- ADB (Android Debug Bridge)

## Installation Steps

### 1. Clone Repository
```bash
git clone https://github.com/pro0pass/Asteroid.git
cd Asteroid
```

### 2. Open in Unity
1. Open Unity Hub
2. Select "Add project from disk"
3. Navigate to Asteroid folder
4. Open with Unity 2022.3 LTS

### 3. Configure Android Build Settings
1. Go to File > Build Settings
2. Select Android platform
3. Click "Switch Platform"
4. Configure player settings:
   - Player > Android
   - Other Settings:
     - Scripting Backend: IL2CPP
     - Target Architectures: ARM64
     - Minimum API Level: Android 10.0 (API 29)
     - Target API Level: Android 13 (API 33)
   - XR Plug-in Management:
     - Select Meta Quest as provider

### 4. Import Meta Quest SDK
1. Download Meta Quest SDK for Unity
2. Import OVR package into project
3. Accept all plugin imports

### 5. Setup Wireless Debugging

#### On Meta Quest 3:
1. Settings > System > Developer
2. Enable "USB Debugging"
3. Enable "Wireless Debugging"
4. Note the IP address shown

#### On Development Machine:
```bash
# Connect to device (replace IP with your device IP)
adb connect <DEVICE_IP>:5555

# Verify connection
adb devices
```

### 6. Build and Deploy

#### Using Unity
1. File > Build and Run
2. Select connected device
3. Build will compile and deploy to headset

#### Using Command Line
```bash
./gradlew assembleDebug
adb install -r build/outputs/apk/debug/Asteroid-debug.apk
```

### 7. Run Application
1. Put on Meta Quest 3 headset
2. Look for Asteroid app in library
3. Select and launch
4. Calibrate settings as needed

## Configuration

### Settings File Location
Settings are saved at: `/data/data/com.asteroid/shared_prefs/AsteroidSettings.xml`

### Default Settings
- World Scale: 10.0
- Height Offset: 0.0
- FOV: 80.0
- Movement Speed: 1.0x
- Battery Saver: Off
- Arm Offset: 0.0

### Adjusting Settings In-App
1. Launch Asteroid
2. Press Start button on right controller
3. Navigate to Settings
4. Adjust parameters with controller joysticks
5. Changes apply in real-time

## Troubleshooting

### Wireless Debugging Not Working
- Ensure device and PC are on same network
- Verify firewall isn't blocking port 5555
- Restart ADB: `adb kill-server && adb start-server`

### App Won't Deploy
- Check that Android build tools are installed
- Verify API levels match configuration
- Ensure device is in developer mode

### Performance Issues
- Enable Battery Saver in settings
- Reduce World Scale
- Disable cosmetic effects in settings

## Development Workflow

### Making Changes
1. Edit code in Unity
2. Save changes
3. File > Build and Run
4. Test in headset

### Using Terminal
1. Launch Asteroid
2. Open Terminal from menu
3. Type commands (adb, logcat, help)
4. Output displays in terminal window

### Debugging with Logcat
```bash
adb logcat | grep Asteroid
```

## Performance Tips

- Use 60 FPS target with Battery Saver enabled
- Enable 72 FPS for optimal experience (battery drain)
- Reduce particle count for cosmic shower if needed
- Monitor memory usage in terminal

## Support

For issues or questions:
1. Check existing GitHub issues
2. Enable debug logging in code
3. Collect logs from terminal
4. Submit issue with reproduction steps
