#!/bin/sh

PROJECT=src/Acropolis.Infrastructure
BUILD_PROJECT=src/Acropolis.Api
SQL_CONTEXT_CLASS=AcropolisContext

dotnet ef database update --project ${PROJECT} --startup-project ${BUILD_PROJECT} --context ${SQL_CONTEXT_CLASS}