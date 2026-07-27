$rsa = [System.Security.Cryptography.RSA]::Create(2048)
$pubXml = $rsa.ToXmlString($false)
$privXml = $rsa.ToXmlString($true)
[System.IO.File]::WriteAllText('c:\Users\7lmiq\Desktop\app#\LicenseGenerator\rsa_public_key.xml', $pubXml)
[System.IO.File]::WriteAllText('c:\Users\7lmiq\Desktop\app#\LicenseGenerator\rsa_private_key.xml', $privXml)
Write-Output "Done"
