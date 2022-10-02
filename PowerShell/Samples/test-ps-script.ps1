$mdtoSession = New-Object -ComObject DTO.DtoSession
$connected = $mdtoSession.connect('localhost', '', '')
$mdtoDatabase = New-Object -Com DTO.DtoDatabase
$mdtoDatabase.DataPath = 'C:\Creative\Manager\Userdata\'
$mdtoDatabase.DdfPath = 'C:\Creative\Manager\Userdata\'
$mdtoDatabase.Name = 'Test901DB'
$mdtoDatabase.Flags = 0
$mdtoSession.Databases.Add($mdtoDatabase)