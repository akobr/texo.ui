$up = $true
$finish = $false
$timeout = 50
Write-Host "Updating" -NoNewline
do {
    for ($i=0;$i -le 100;$i++){
        if ($i -eq 100){$up = !$up}
        if ($up){
            $str = ""
            $x=99-$i
            for($z=0;$z -le $i;$z++){$str+="."}
            for($y=0;$y -le $x;$y++){$str+=" "}
            Write-host "`rUpdating$str" -NoNewline
            Start-Sleep -Milliseconds $timeout
        }else{
            $str = ""
            $x=99-$i
            for($y=0;$y -le $x;$y++){$str+="."}
            for($z=0;$z -le $i;$z++){$str+=" "}
            Write-host "`rUpdating$str" -NoNewline
            Start-Sleep -Milliseconds $timeout
        }
    }
    if ($timeout -le 0){$finish = $true}
    $timeout-=10
} until ($finish)
$str = ""
for ($i=0;$i -le 93;$i++){$str+=" "}
Write-Host "`rUpdate Complete!$str"
Read-Host "Press [ENTER] to close this Window"