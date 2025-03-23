<#
.Synopsis
    Invoke git, handling its quirky stderr that isn't error

.Outputs
    Git messages, and lastly the exit code

.Example
    Invoke-Git push

.Example
    Invoke-Git "add ."
#>
function Invoke-Git
{
param(
[Parameter(Mandatory)]
[string] $Command )

    try {

        $exit = 0
        $path = [System.IO.Path]::GetTempFileName()

        Invoke-Expression "git $Command 2> $path"
        $exit = $LASTEXITCODE
        if ( $exit -gt 0 )
        {
            Write-Error (Get-Content $path).ToString()
        }
        else
        {
            Get-Content $path | Select-Object -First 1
        }
        $exit
    }
    catch
    {
        Write-Host "Error: $_`n$($_.ScriptStackTrace)"
    }
    finally
    {
        if ( Test-Path $path )
        {
            Remove-Item $path
        }
    }
}

# Only master Release
if ($env:Configuration -ne "Release")
{
    "Documentation update ignored: Not Release build.";
    Return;
}

if ($env:APPVEYOR_REPO_BRANCH -ne "master")
{
    "Documentation update ignored: Not master branch.";
    Return;
}

# Chocolatey DocFX
#cinst docfx --version $env:DocFXVersion -y

git config --global core.autocrlf true
git config --global core.eol lf

git config --global user.email $env:GITHUB_EMAIL
git config --global user.name "KeRNeLith"

"Generating documentation site..."
docfx ./docs/docfx.json

$SOURCE_DIR=$pwd.Path
$TEMP_REPO_DIR="$pwd/../QuikGraph-gh-pages"

if (Test-Path $TEMP_REPO_DIR)
{
    "Removing temporary documentation directory $TEMP_REPO_DIR..."
    rm -recurse $TEMP_REPO_DIR
}

mkdir $TEMP_REPO_DIR

"Cloning the repository gh-pages branch."
# -q is to avoid git to output thing to stderr for no reason
git clone -q https://github.com/KeRNeLith/QuikGraph.git --branch gh-pages $TEMP_REPO_DIR

"Clear local repository gh-pages directory..."
cd $TEMP_REPO_DIR
git rm -r *

"Copying documentation into the local repository gh-pages directory..."
cp -recurse $SOURCE_DIR/docs/_site/* .

Invoke-Git "add -A ."

"Checking if there are changes in the documentation..."
if (-not [string]::IsNullOrEmpty($(git status --porcelain)))
{
    "Pushing the new documentation to the remote gh-pages branch..."

    git commit -m "Update generated documentation."

    git remote set-url origin https://$($env:GITHUB_ACCESS_TOKEN)@github.com/KeRNeLith/QuikGraph.git

    # -q is to avoid git to output thing to stderr for no reason
    git push -q origin gh-pages

    "Documentation updated in remote gh-pages branch."
}
else
{
    "Documentation update ignored: No relevant changes in the documentation."
}