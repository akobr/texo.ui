for ($i = 1; $i -le 10; $i++)
{
    $p = $i*10;
    Write-Progress -Activity "Search in Progress" -Status "$p% Complete:" -PercentComplete $p;
    Start-Sleep -Milliseconds 200;
}