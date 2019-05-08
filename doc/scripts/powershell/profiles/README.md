# Power-shell profiles per user

* All host: `profile.ps1`
* This host (TexoUI): `TexoUI_profile.ps1`

* Copy profile file(s) to `C:\Users\[UserName]\Documents\WindowsPowerShell`
* Make sure that the user has enough rights to process profiles (run script files).
  * `Get-ExecutionPolicy -Scope CurrentUser`
  * `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser`