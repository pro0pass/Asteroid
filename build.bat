@echo off
REM APK Build and Conversion Script for Asteroid (Windows)
REM Converts Android APK to installable format

echo ========================================
echo Asteroid APK Build ^& Setup
echo ========================================
echo.

REM Check for required tools
echo [1/5] Checking dependencies...
where gradle >nul 2>nul
if errorlevel 1 (
    echo ERROR: gradle is not installed
    echo Please install Gradle and add it to PATH
    exit /b 1
)

where adb >nul 2>nul
if errorlevel 1 (
    echo ERROR: adb is not installed
    echo Please install Android SDK Platform Tools
    exit /b 1
)

echo [2/5] Building APK...
call gradlew assembleDebug

echo [3/5] Locating APK files...
set APK_SOURCE=build\outputs\apk\quest3\debug\Asteroid-debug.apk
set APK_DEST=build\outputs\Asteroid.apk

if not exist "%APK_SOURCE%" (
    echo ERROR: APK not found at %APK_SOURCE%
    echo Build may have failed. Check output above.
    exit /b 1
)

echo [4/5] Copying APK to deployment folder...
if not exist "build\outputs" mkdir build\outputs
copy "%APK_SOURCE%" "%APK_DEST%"
echo. APK ready at: %APK_DEST%

echo [5/5] Installation Ready!
echo.
echo ========================================
echo Next Steps:
echo ========================================
echo.
echo Option A - Wireless Installation (Recommended):
echo   adb connect ^<YOUR_QUEST_IP^>:5555
echo   adb install -r %APK_DEST%
echo.
echo Option B - USB Installation:
echo   adb install -r %APK_DEST%
echo.
echo Launch app:
echo   adb shell am start -n com.asteroid/.MainActivity
echo.
echo View logs:
echo   adb logcat ^| find "Asteroid"
echo.
pause
