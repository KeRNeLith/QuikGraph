$docFx = (Resolve-Path "tools\docfx.exe").ToString();
& $docfx docs/docfx.json --serve