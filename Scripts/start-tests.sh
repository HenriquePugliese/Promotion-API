#!/bin/sh

source ./Scripts/apply-migrations.sh

dotnet test tests/Acropolis.Tests/ --configuration Release --logger trx --settings "runsettings.xml"