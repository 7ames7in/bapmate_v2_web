#!/bin/bash
# VPS PostgreSQL 데이터베이스 스크립트 실행기
set -e

SQL_FILE="bapmate_v2_web/postgres_schema_and_data.sql"
VPS_HOST="srv1651644.hstgr.cloud"
VPS_USER="root"
REMOTE_PATH="/tmp/postgres_schema_and_data.sql"

echo "=========================================================="
echo "          BapMate PostgreSQL Schema VPS Executor          "
echo "=========================================================="
echo "1. SQL 스크립트를 VPS (/tmp) 경로로 전송합니다..."
echo "*(VPS 비밀번호를 물어보면 입력해 주세요)*"
scp "$SQL_FILE" "${VPS_USER}@${VPS_HOST}:${REMOTE_PATH}"

echo -e "\n2. VPS 내부에서 psql을 호출하여 스크립트를 직접 실행합니다..."
echo "*(VPS 비밀번호를 한 번 더 물어보면 입력해 주세요)*"
ssh "${VPS_USER}@${VPS_HOST}" '
  # 1. 시스템 psql 찾기 (실행 파일만)
  PSQL_BIN=""
  if command -v psql &>/dev/null; then
    PSQL_BIN=$(command -v psql)
  else
    PSQL_BIN=$(find /usr/bin /usr/sbin /usr/local/bin /usr/lib/postgresql -name psql -type f -executable 2>/dev/null | head -n 1)
  fi
  
  if [ -n "$PSQL_BIN" ] && [ -x "$PSQL_BIN" ]; then
    echo "시스템 psql 발견: $PSQL_BIN"
    # postgres 기본 DB에 접속하여 bapmatedb 생성 시도
    PGPASSWORD="N8n_Postgres_2026!jun" "$PSQL_BIN" -h localhost -U n8n_user -d postgres -c "CREATE DATABASE bapmatedb;" 2>/dev/null || true
    # bapmatedb에 스키마 적용
    PGPASSWORD="N8n_Postgres_2026!jun" "$PSQL_BIN" -h localhost -U n8n_user -d bapmatedb -f /tmp/postgres_schema_and_data.sql
    exit 0
  fi

  # 2. 도커 컨테이너 검사
  if command -v docker &>/dev/null; then
    CONTAINER_ID=$(docker ps --filter "ancestor=postgres" --format "{{.ID}}" | head -n 1)
    if [ -z "$CONTAINER_ID" ]; then
      CONTAINER_ID=$(docker ps --filter "name=postgres" --format "{{.ID}}" | head -n 1)
    fi
    if [ -z "$CONTAINER_ID" ]; then
      CONTAINER_ID=$(docker ps --filter "name=db" --format "{{.ID}}" | head -n 1)
    fi

    if [ -n "$CONTAINER_ID" ]; then
      echo "PostgreSQL 도커 컨테이너 발견 (ID: $CONTAINER_ID)"
      echo "데이터베이스 bapmatedb 생성을 시도합니다..."
      
      # 1. n8n_user로 postgres DB에 접속해 bapmatedb 생성 시도
      docker exec -i "$CONTAINER_ID" env PGPASSWORD="N8n_Postgres_2026!jun" psql -U n8n_user -d postgres -c "CREATE DATABASE bapmatedb;" 2>&1 || \
      # 2. postgres superuser로 bapmatedb 생성 시도
      docker exec -i "$CONTAINER_ID" psql -U postgres -d postgres -c "CREATE DATABASE bapmatedb;" 2>&1 || \
      # 3. postgres unix 계정으로 bapmatedb 생성 시도
      docker exec -u postgres -i "$CONTAINER_ID" psql -d postgres -c "CREATE DATABASE bapmatedb;" 2>&1 || \
      echo "데이터베이스 생성 시도 완료 (이미 존재하거나 권한 부족)"

      # 컨테이너 안으로 SQL 파일 복사
      docker cp /tmp/postgres_schema_and_data.sql "$CONTAINER_ID":/tmp/postgres_schema_and_data.sql
      # 컨테이너 안에서 psql 실행
      docker exec -i "$CONTAINER_ID" env PGPASSWORD="N8n_Postgres_2026!jun" psql -U n8n_user -d bapmatedb -f /tmp/postgres_schema_and_data.sql
      exit 0
    fi
  fi

  echo "오류: VPS 내부에 psql 명령어나 PostgreSQL 도커 컨테이너를 찾을 수 없습니다."
  exit 1
'

echo -e "\n=========================================================="
echo "🎉 VPS 데이터베이스에 SQL 스크립트가 성공적으로 적용되었습니다!"
echo "=========================================================="
