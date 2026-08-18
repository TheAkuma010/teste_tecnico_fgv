#!/bin/bash

set -e

SQLCMD="/opt/mssql-tools18/bin/sqlcmd"

echo "Aguardando SQL Server..."

until $SQLCMD \
    -S sqlserver \
    -U sa \
    -P "$MSSQL_SA_PASSWORD" \
    -No \
    -b \
    -Q "SELECT 1" > /dev/null 2>&1
do
    sleep 2
done

echo "SQL Server disponível."

$SQLCMD \
    -S sqlserver \
    -U sa \
    -P "$MSSQL_SA_PASSWORD" \
    -No \
    -b \
    -Q "IF DB_ID('SalesDb') IS NULL CREATE DATABASE SalesDb"

echo "SalesDb criada/verificada."

$SQLCMD \
    -S sqlserver \
    -U sa \
    -P "$MSSQL_SA_PASSWORD" \
    -No \
    -b \
    -d SalesDb \
    -i /scripts/001-create-tables.sql

echo "Tabelas criadas."

$SQLCMD \
    -S sqlserver \
    -U sa \
    -P "$MSSQL_SA_PASSWORD" \
    -No \
    -b \
    -d SalesDb \
    -i /scripts/002-seed.sql

echo "Seed executado."
echo "Inicialização concluída."