$Version = Read-Host -Prompt 'Version'
$Pack = Read-Host -Prompt 'Pack (umb, nuget, both)'
try
{
	if($Pack -eq 'nuget' -or $Pack -eq 'both'){

		dotnet pack  ..\src\TruePeople.SharePreview\TruePeople.SharePreview.csproj -o . -p:PackageVersion=$Version -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg

		Write-Host "Succesfully created Nuget package with version $($Version)"
	}

	if($Pack -eq 'umb' -or $Pack -eq 'both'){

		umbpack pack package.xml -v $Version

		Write-Host "Succesfully created Umbraco package with version $($Version)"
	}
}
catch {
	Write-Host "Error occured:";
	Write-Host $_;
	 $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyUp") > $null
}