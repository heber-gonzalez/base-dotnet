# Define base API directory
$baseApiDir = "C:\Users\heber.gonzalez\Documents\Proyectos\BaseBackend"

# Prompt user for project name
$projectName = Read-Host "Enter new project name"

# Define directory for new project
$newProjectDir = "C:\Users\heber.gonzalez\Documents\Proyectos\$projectName"

# Check if project directory already exists
if (Test-Path $newProjectDir -PathType Container) {
    Write-Error "Error: Project directory already exists!"
    Exit 1
}

# Copy base API template to new project directory
Write-Host "Creating new project directory..."
Copy-Item -Path $baseApiDir -Destination $newProjectDir -Recurse

# Remove the .git directory if it exists
if (Test-Path "$newProjectDir\.git" -PathType Container) {
    Remove-Item "$newProjectDir\.git" -Recurse -Force
}

# Initialize a new Git repository
Set-Location -Path $newProjectDir
git init

Write-Host "New project $projectName created successfully!"