$Version = Read-Host -Prompt 'Version'
$Pack = Read-Host -Prompt 'Pack (umb, nuget, both)'

if($Pack -eq 'nuget' -or $Pack -eq 'both'){

    nuget pack package.nuspec -Version $Version

    Write-Host "Succesfully created Nuget package with version $($Version)"
}

if($Pack -eq 'umb' -or $Pack -eq 'both'){

    umbpack pack package.xml -v $Version

    Write-Host "Succesfully created Umbraco package with version $($Version)"
}


$Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")