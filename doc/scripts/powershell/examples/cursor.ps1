function myWaitJob($job) {
$saveY = [console]::CursorTop
$saveX = [console]::CursorLeft   
$str = '|','/','-','\'     
do {
$str | ForEach-Object { Write-Host -Object $_ -NoNewline
        Start-Sleep -Milliseconds 100
        $Host.UI.RawUI.CursorPosition = @{ X = $saveX; Y = $saveY }
        # [console]::setcursorposition($saveX,$saveY)
        } # end foreach-object
    if ((Get-Job -Name $job).state -eq 'Running') 
    {$running = $true}
    else {$running = $false}
    } # end do
while ($running)
} # end function

Start-Job -ScriptBlock {Start-Sleep 10} -Name j1
myWaitJob j1
rjb j1