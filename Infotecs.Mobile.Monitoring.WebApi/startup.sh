#!/bin/bash
service nginx start
dotnet /app/Infotecs.Mobile.Monitoring.WebApi.dll
