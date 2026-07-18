# Asteroid - Complete Setup Guide

## Quick Start (3 Steps)

### 1. **Download & Extract**
```bash
# Clone the repository
git clone https://github.com/pro0pass/Asteroid.git
cd Asteroid

# The project is ready! No need to change .zip to .apk
# Just rename the generated APK during build
```

### 2. **Open in Unity 2022.3+ LTS**
```bash
# Option A: Via Unity Hub
# - Open Unity Hub
# - Click "Add project from disk"
# - Select the Asteroid folder
# - Open with Unity 2022.3 LTS or newer

# Option B: Direct
# - Close any open Unity projects
# - Open the Asteroid folder directly in Unity
```

### 3. **Build & Deploy**
```bash
# The build is automated!
# File > Build Settings > Select "Android"
# Click "Build" - APK is automatically generated
# Deploy: adb install -r build/outputs/apk/quest3/debug/Asteroid-debug.apk
```

---

## Complete Setup (Detailed)

### Prerequisites
- Meta Quest 3 Headset
- Windows/Mac/Linux Computer
- Unity 2022.3 LTS or 2023 LTS
- Android SDK (API 29+)
- ADB installed

### Step 1: Clone Repository
```bash
git clone https://github.com/pro0pass/Asteroid.git
cd Asteroid
```

### Step 2: Install Dependencies

#### On Windows:
```batch
# Install Android SDK
# Download from: https://developer.android.com/studio
# Ensure API 34 is installed

# Set environment variables
set ANDROID_HOME=C:\Users\YourUsername\AppData\Local\Android\Sdk
set PATH=%PATH%;%ANDROID_HOME%\platform-tools
```

#### On Mac:
```bash
brew install android-sdk
export ANDROID_HOME=~/Library/Android/sdk
export PATH="$ANDROID_HOME/platform-tools:$PATH"
```

#### On Linux:
```bash
sudo apt-get install -y android-tools-adb android-tools-fastboot
export ANDROID_HOME=~/Android/Sdk
export PATH="$ANDROID_HOME/platform-tools:$PATH"
```

### Step 3: Configure Unity Project

1. **Open Unity**
   - Launch Unity Hub
   - Select "Add project from disk"
   - Navigate to Asteroid folder
   - Select Unity 2022.3 LTS or newer

2. **Configure Build Settings**
   - File → Build Settings
   - Select "Android" platform
   - Click "Switch Platform"
   - Wait for compilation

3. **Player Settings**
   - Edit → Project Settings → Player
   - **Android Tab:**
     - Company Name: Asteroid
     - Product Name: Asteroid
     - Default Icon: (optional)
   
   - **Other Settings:**
     - Scripting Backend: IL2CPP
     - Target Architectures: ARM64
     - Minimum API Level: Android 10.0 (API 29)
     - Target API Level: Android 14 (API 34)
     - Graphics APIs: OpenGL ES 3
   
   - **XR Plug-in Management:**
     - Install from Package Manager if needed
     - Meta Quest (Standalone):
       ✓ Enable
       ✓ Use Device-Level Tracking
       ✓ Enable Eye Tracking (for Quest 3)

### Step 4: Import Meta Quest SDK

1. **Window → TextMesh Pro → Import TMP Examples & Extras**

2. **Window → Oculus → Asset Reimporter**
   - This auto-imports OVR SDK

3. **Or Manual Import:**
   - Download Meta Quest SDK from: https://developer.oculus.com/downloads/
   - Unzip to Assets/Plugins
   - Wait for reimport

### Step 5: Setup Wireless Debugging

#### On Meta Quest 3:
1. Put on headset
2. Settings → System → Developer
3. Enable "USB Debugging"
4. Enable "Wireless Debugging"
5. Note the IP address shown
6. Also enable "USB Connection Dialog"

#### On Your Computer:
```bash
# Connect to device via WiFi
adb connect <YOUR_DEVICE_IP>:5555

# Verify connection
adb devices

# Should show:
# <IP>:5555 device
```

### Step 6: Build APK

#### Automatic Build:
```bash
# File → Build Settings → Build
# Or use the build script
./gradlew assembleDebug

# APK location: build/outputs/apk/quest3/debug/Asteroid-debug.apk
```

#### Install to Device:
```bash
# Wireless installation
adb install -r build/outputs/apk/quest3/debug/Asteroid-debug.apk

# Or USB installation
adb install -r build/outputs/apk/quest3/debug/Asteroid-debug.apk
```

#### Launch App:
```bash
adb shell am start -n com.asteroid/.MainActivity
```

### Step 7: Grant Permissions in App

1. Launch Asteroid on headset
2. Go to Settings
3. Enable "Wireless Debugging"
4. App will request permissions
5. Accept all prompts

---

## APK Output Locations

```
build/outputs/apk/
├── quest3/
│   ├── debug/
│   │   └── Asteroid-debug.apk  ← Use this
│   └── release/
│       └── Asteroid-release.apk
```

**Note:** No .zip to .apk conversion needed! The build system automatically generates APK files.

---

## Features Enabled by Wireless Debugging

✅ Seated Movement Simulation (Head-based movement for seated users)
✅ PSA Forward Drift (Hold A to sprint as sitting user)
✅ Instant Trigger Response (Ultra-responsive 1ms controls)
✅ Eye Tracking (Full gaze support on Quest 3)
✅ Hand Tracking (Gesture recognition)
✅ Performance Monitoring (Real-time FPS/memory)
✅ Input Debugging (Full input logging)
✅ Developer Dashboard (Real-time stats overlay)

---

## Troubleshooting

### "Build failed: Android SDK not found"
```bash
# Set Android SDK path in Unity:
# Edit → Preferences → External Tools → Android SDK Path
# Point to your SDK directory
```

### "Wireless debugging not connecting"
```bash
# Ensure on same WiFi network
adb kill-server
adb start-server
adb connect <IP>:5555 -v  # verbose output
```

### "APK won't install"
```bash
# Uninstall first
adb uninstall com.asteroid
adb install -r build/outputs/apk/quest3/debug/Asteroid-debug.apk
```

### "XR Plugin not initializing"
```
# In Unity:
# Edit → Project Settings → XR Plug-in Management
# ✓ Check Meta Quest (Standalone)
# ✓ Check Enable Eye Tracking
# Restart Unity
```

---

## Performance Tips

- Enable "Battery Saver" in Settings for 60 FPS
- Use Seated Movement for better battery life
- Disable Eye Tracking if not needed
- Monitor FPS with Developer Dashboard (Alt+D)

---

## Support

For issues:
1. Check Developer Dashboard (enabled in Settings)
2. View logcat: `adb logcat | grep Asteroid`
3. Check GitHub Issues
4. Enable Input Debugging for controller issues

---

**Ready to launch? Put on your headset and enjoy Asteroid!** 🚀
