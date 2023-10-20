@echo off
cls
@echo Reseting IIS
iisreset /restart
@echo Done!
timeout 10