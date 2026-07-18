#!/bin/bash
# APK Build and Conversion Script for Asteroid
# Converts Android APK to installable format

set -e

echo "========================================"
echo "Asteroid APK Build & Setup"
echo "========================================"
echo ""

# Check for required tools
check_tool() {
    if ! command -v $1 &> /dev/null; then
        echo "ERROR: $1 is not installed"
        echo "Please install it and try again"
        exit 1
    fi
}

echo "[1/5] Checking dependencies..."
check_tool "gradle"
check_tool "adb"

echo "[2/5] Building APK..."
# Run Gradle build
./gradlew assembleDebug

echo "[3/5] Locating APK files..."
APK_SOURCE="build/outputs/apk/quest3/debug/Asteroid-debug.apk"
APK_DEST="build/outputs/Asteroid.apk"

if [ ! -f "$APK_SOURCE" ]; then
    echo "ERROR: APK not found at $APK_SOURCE"
    echo "Build may have failed. Check output above."
    exit 1
fi

echo "[4/5] Copying APK to deployment folder..."
mkdir -p build/outputs
cp "$APK_SOURCE" "$APK_DEST"
echo "✓ APK ready at: $APK_DEST"

echo "[5/5] Installation Ready!"
echo ""
echo "========================================"
echo "Next Steps:"
echo "========================================"
echo ""
echo "Option A - Wireless Installation (Recommended):"
echo "  adb connect <YOUR_QUEST_IP>:5555"
echo "  adb install -r $APK_DEST"
echo ""
echo "Option B - USB Installation:"
echo "  adb install -r $APK_DEST"
echo ""
echo "Launch app:"
echo "  adb shell am start -n com.asteroid/.MainActivity"
echo ""
echo "View logs:"
echo "  adb logcat | grep Asteroid"
echo ""
