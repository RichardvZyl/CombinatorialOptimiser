param(
	[Parameter(Mandatory=$true)][string]$Name
)

$docsDir = "docs/solvers"
if (-not (Test-Path $docsDir)) { New-Item -ItemType Directory -Path $docsDir | Out-Null }

$template = Get-Content "$docsDir/TEMPLATE.md" -Raw
$outFile = "$docsDir/$Name.md"
if (Test-Path $outFile) { Write-Host "File $outFile already exists."; exit 1 }

$content = $template -replace "<SolverName>", $Name
Set-Content -Path $outFile -Value $content -Encoding UTF8
Write-Host "Created $outFile"; exit 0
