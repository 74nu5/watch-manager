# Script PowerShell pour mettre à jour tous les packages NuGet obsolètes d'une solution .NET
# Usage : ./Update-NugetPackages.ps1

$ErrorActionPreference = 'Stop'

# Récupère la liste des packages obsolètes au format JSON
Write-Host "Analyse des packages NuGet obsolètes..."

$json = dotnet list package --outdated --format json | Out-String | ConvertFrom-Json

if (-not $json.projects) {
    Write-Host "Aucun projet trouvé."
    exit 0
}

foreach ($project in $json.projects) {
    $projectPath = $project.path
    Write-Host "\nProjet : $projectPath"
    foreach ($framework in $project.frameworks) {
        $fw = $framework.framework
        Write-Host "  Framework : $fw"
        foreach ($pkg in $framework.topLevelPackages) {
            $packageId = $pkg.id
            $resolved = $pkg.resolvedVersion
            $latest = $pkg.latestVersion
            if ($null -eq $latest -or $latest -eq $resolved -or $latest -like '*Introuvable*') { continue }
            Write-Host "    Mise à jour de $packageId ($resolved -> $latest)"
            dotnet add "$projectPath" package $packageId --version $latest
        }
    }
}

Write-Host "Mises à jour terminées. Pensez à valider vos modifications."
