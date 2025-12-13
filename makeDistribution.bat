robocopy *.* .\out\Release\ .\distribution\ScalesAutomation\
robocopy *.* .\src\resources\Images\ .\distribution\ScalesAutomation\Images\
robocopy *.* .\resources .\distribution\Server CentralizatorMasuratori.xlsm /XF "Manual - Automatizare Cantar.docx"
robocopy .\resources .\distribution\ScalesAutomation CentralizatorMasuratori.xlsm