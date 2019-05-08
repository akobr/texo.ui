function Write-Status 
{
  param([int]$Current,
        [int]$Total,
        [string]$Statustext,
        [string]$CurStatusText,
        [int]$ProgressbarLength = 35)

  # Save current Cursorposition for later
  [int]$XOrg = $host.UI.RawUI.CursorPosition.X

  # Create Progressbar
  [string]$progressbar = ""
  for ($i = 0 ; $i -lt $([System.Math]::Round($(([System.Math]::Round(($($Current) / $Total) * 100, 2) * $ProgressbarLength) / 100), 0)); $i++) {
    $progressbar = $progressbar + $([char]9608)
  }
  for ($i = 0 ; $i -lt ($ProgressbarLength - $([System.Math]::Round($(([System.Math]::Round(($($Current) / $Total) * 100, 2) * $ProgressbarLength) / 100), 0))); $i++) {
    $progressbar = $progressbar + $([char]9617)
  }
  # Overwrite Current Line with the current Status
  Write-Host -NoNewline "`r$Statustext $progressbar [$($Current.ToString("#,###").PadLeft($Total.ToString("#,###").Length)) / $($Total.ToString("#,###"))] ($($( ($Current / $Total) * 100).ToString("##0.00").PadLeft(6)) %) $CurStatusText"

  # There might be old Text behing the current Currsor, so let's write some blanks to the Position of $XOrg
  [int]$XNow = $host.UI.RawUI.CursorPosition.X
  for ([int]$i = $XNow; $i -lt $XOrg; $i++) {
    Write-Host -NoNewline " "
  }
  # Just for optical reasons: Go back to the last Position of current Line
  for ([int]$i = $XNow; $i -lt $XOrg; $i++) {
    Write-Host -NoNewline "`b"
  }
}

For ([int]$i=0; $i -le 8192; $i++) {
    Write-Status -Current $i -Total 8192 -Statustext "Running a long Task" -CurStatusText "Working on Position $i"
}