import os
import sys
import platform
import subprocess

# Set the fixed installation path for IpAddr executables
APP_DIR = "IpAddr/bin/Debug/net8.0"

# Define the executable names for different OS
APP_FILES = {
    "Windows": "IpAddr.exe",
    "Linux": "IpAddr",  # Linux binary is just "IpAddr"
    "Darwin": "IpAddr"  # macOS binary is also "IpAddr"
}

def detect_os():
    """Detects the OS and returns the correct IpAddr executable filename."""
    system = platform.system()
    
    if system in APP_FILES:
        return APP_FILES[system]
    
    print("Unsupported OS")
    sys.exit(1)

def make_executable(file_path):
    """Ensures the file is executable on Unix-based systems."""
    if platform.system() != "Windows":
        os.chmod(file_path, 0o755)  # Give execution permissions

def launch_application():
    """Finds and launches the IpAddr application from the fixed directory."""
    app_name = detect_os()
    app_path = os.path.join(APP_DIR, app_name)

    # Check if the application exists
    if not os.path.exists(app_path):
        print(f"Error: IpAddr executable not found: {app_path}")
        sys.exit(1)

    make_executable(app_path)

    try:
        # Run the application and forward command-line arguments
        process = subprocess.Popen([app_path] + sys.argv[1:], stdout=sys.stdout, stderr=sys.stderr)
        process.wait()
    except Exception as e:
        print(f"Error launching IpAddr: {e}")

if __name__ == "__main__":
    launch_application()
