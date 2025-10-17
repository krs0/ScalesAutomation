# ScalesAutomation
Measurements automation and log parser and metrics for an industrial grade scale

Main branch is for Office 2016, for Office 2013 there is a separate branch. Releases are made on that branch.

Notes: Excel path is stored in Common/settings.

To make a release run on Office2013 branch makeDistribution.bat. This will create the deploy files. Make sure to not overwrite any .config settings!

Note: In Measurements Central app, CentralizatorMasuratori.xls is taken from 1 folder up of the selected measurements folder. The excel file will work with measurements from either Cantariri_Automate if found or CSVOutput if not. This way same xls fiel can be used on Server and local.
