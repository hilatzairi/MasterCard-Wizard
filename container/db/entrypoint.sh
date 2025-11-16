#!/bin/bash

echo "=========================================="
echo "  Starting SQL Server..."
echo "=========================================="

# Start SQL Server in the background
/opt/mssql/bin/sqlservr &

# Wait for SQL Server to start up
echo ""
echo "Waiting for SQL Server to be ready..."
sleep 10

echo ""
echo "=========================================="
echo "  Initializing Database..."
echo "=========================================="

# Run the initialization script
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Hila1234 -C -i /usr/src/app/init.sql

if [ $? -eq 0 ]; then
    echo ""
    echo "=========================================="
    echo "  ✓ DATABASE READY!"
    echo "  All tables and data initialized."
    echo "=========================================="
    # Create a marker file to indicate initialization is complete
    touch /tmp/db_initialized
else
    echo ""
    echo "=========================================="
    echo "  ✗ DATABASE INITIALIZATION FAILED"
    echo "=========================================="
    exit 1
fi

# Keep the container running by waiting for the SQL Server process
wait

