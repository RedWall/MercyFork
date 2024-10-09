# Set the number of repos to pull
$repoCount = 200

# Set the sorting parameter
$sort = "stars"

# Set the query terms
$queryTerms = "archived:false mirror:false template:false forks:<10 followers:>=100 stars:>=100"

# Create the Repos subfolder if it doesn't exist
$reposFolder = "Repos"
if (-not (Test-Path $reposFolder)) {
    New-Item -ItemType Directory -Path $reposFolder | Out-Null
}

# Set the headers for the API request
$headers = @{
 #   "Authorization" = "Bearer $token"
    "Accept" = "application/vnd.github.v3+json"
    "User-Agent" = "mercy-fork"
}

# Loop through the repos and write each one to a JSON file
for ($page = 1; $page -le ($repoCount / 100); $page++) {
    $url = "https://api.github.com/search/repositories?q=$queryTerms&sort=$sort&order=desc&page=$page&per_page=100"
    $repos = Invoke-RestMethod -Uri $url -Headers $headers

    # Create an object for Write-Output
    $outputObj = [PSCustomObject]@{
        "TotalCount" = $repos.total_count
        "IncompleteResults" = $repos.incomplete_results
        "ItemsCount" = $repos.items.count
    }
    
    Write-Output $outputObj
    
    foreach ($repo in $repos.items) {
        $repoName = $repo.name
        $repoJson = $repo | ConvertTo-Json -Depth 10

        $repoFilePath = Join-Path $reposFolder "$repoName.json"
        $repoJson | Out-File -FilePath $repoFilePath
    }
}