$dir = "c:\Users\7lmiq\Desktop\app#\RetailApp\ViewModels"
Get-ChildItem -Path $dir -Filter *.cs -File | ForEach-Object {
    if ($_.Name -ne "BaseViewModel.cs") {
        $content = [System.IO.File]::ReadAllText($_.FullName)
        if ($content -match " : ObservableObject") {
            $newContent = $content -replace " : ObservableObject", " : BaseViewModel"
            [System.IO.File]::WriteAllText($_.FullName, $newContent, [System.Text.Encoding]::UTF8)
            Write-Host "Updated" $_.Name
        }
    }
}
