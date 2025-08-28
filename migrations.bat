@echo off
setlocal enabledelayedexpansion

REM ==== Configuration ====
set ProjectPath=Organization.Infrastructure
set ConfigFile=%ProjectPath%\appsettings.json
set OutputDir=Persistence\Migrations

REM ==== Menu ====
:menu
cls
echo.
echo ==== EF Core Migration Tool ====
echo [1] Build and add migration
echo [2] Empty the Migrations folder
echo [3] Rollback to previous migration
echo [4] Apply all migrations to database
echo [5] Change database configuration
echo [6] View current configuration
echo [q] Quit
echo =================================
set /p choice=Choose an option: 

if /I "%choice%"=="1" goto run_migration
if /I "%choice%"=="2" goto confirm_empty_folder
if /I "%choice%"=="3" goto rollback_migration
if /I "%choice%"=="4" goto apply_migrations
if /I "%choice%"=="5" goto change_config
if /I "%choice%"=="6" goto view_config
if /I "%choice%"=="q" goto quit
goto menu

REM ==== Option 1: Run migration ====
:run_migration
dotnet tool list -g | findstr dotnet-ef >nul
if %errorlevel% neq 0 (
    echo dotnet-ef is not installed. Installing...
    dotnet tool install --global dotnet-ef
) else (
    echo dotnet-ef is already installed.
)

echo.
echo === Building the project to verify everything is OK ===
dotnet build %ProjectPath%
if %errorlevel% neq 0 (
    echo.
    echo [ERROR] Build failed. Fix errors before continuing.
    pause
    exit /b %errorlevel%
)

:input_migration
set /p MigrationName=Enter migration name (or 'q' to quit): 
if /I "%MigrationName%"=="q" goto menu
if "%MigrationName%"=="" (
    echo Migration name cannot be empty.
    goto input_migration
)

dotnet ef migrations add %MigrationName% --project %ProjectPath% -o %OutputDir%
pause
goto menu

REM ==== Option 2: Empty Migrations folder ====
:confirm_empty_folder
echo.
echo WARNING: This will delete all files in "%ProjectPath%\%OutputDir%"
set /p confirm=Are you sure you want to continue? (y/n): 
if /I "%confirm%"=="y" goto empty_folder
if /I "%confirm%"=="n" goto menu
echo Invalid input.
pause
goto menu

:empty_folder
echo Deleting files in "%ProjectPath%\%OutputDir%"...
rmdir /S /Q "%ProjectPath%\%OutputDir%"
mkdir "%ProjectPath%\%OutputDir%"
echo Migrations folder emptied.
pause
goto menu

REM ==== Option 3: Rollback migration ====
:rollback_migration
echo.
echo === Current migrations ===
dotnet ef migrations list --project %ProjectPath%
echo.
set /p TargetMigration=Enter target migration to rollback to (or '0' for empty DB, or 'q' to cancel): 
if /I "%TargetMigration%"=="q" goto menu
if "%TargetMigration%"=="" (
    echo Migration name cannot be empty.
    pause
    goto rollback_migration
)
dotnet ef database update %TargetMigration% --project %ProjectPath%
pause
goto menu

REM ==== Option 4: Apply all migrations ====
:apply_migrations
echo.
echo === Applying all migrations to the database ===
dotnet ef database update --project %ProjectPath%
pause
goto menu

REM ==== Option 5: Change DB Config ====
:change_config
echo.
echo === Change database configuration ===
set /p DbHost=Host (e.g. 127.0.0.1, or 'q' to quit): 
if /I "%DbHost%"=="q" goto menu
set /p DbPort=Port (default 5432): 
if /I "%DbPort%"=="q" goto menu
set /p DbName=Database name: 
if /I "%DbName%"=="q" goto menu
set /p DbUser=Username: 
if /I "%DbUser%"=="q" goto menu
set /p DbPass=Password: 
if /I "%DbPass%"=="q" goto menu

(
    echo {
    echo     "ConnectionStrings": {
    echo         "DefaultConnection": "Host=%DbHost%;Port=%DbPort%;Database=%DbName%;Username=%DbUser%;Password=%DbPass%"
    echo     }
    echo }
) > %ConfigFile%

echo.
echo Configuration updated successfully.
pause
goto menu

REM ==== Option 6: View config ====
:view_config
echo.
echo === Current Configuration (%ConfigFile%) ===
for /f "usebackq tokens=*" %%i in (%ConfigFile%) do echo     %%i
pause
goto menu

REM ==== Quit ====
:quit
echo Goodbye!
pause
exit /b 0
