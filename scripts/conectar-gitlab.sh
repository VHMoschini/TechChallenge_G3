#!/usr/bin/env bash
# Conecta ao GitLab: https://gitlab.com/leo_araujo/posfiap
#
# Antes: garanta Git OK (macOS: xcode-select --install ou instale Xcode).
#        git config --global user.name "Seu Nome"
#        git config --global user.email "seu@email.com"
#
# O push usa -u (e pode precisar de -f se o remoto já tiver histórico diferente).

set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT"

if ! git rev-parse --git-dir >/dev/null 2>&1; then
  git init
fi

if git remote get-url origin >/dev/null 2>&1; then
  git remote set-url origin https://gitlab.com/leo_araujo/posfiap.git
else
  git remote add origin https://gitlab.com/leo_araujo/posfiap.git
fi

git branch -M main

if ! git rev-parse -q --verify HEAD >/dev/null 2>&1; then
  git add -A
  git commit -m "Initial commit: FCG Tech Challenge Fase 1"
fi

# Equivalente a: git push -uf origin main
git push -uf origin main

echo "Remoto: https://gitlab.com/leo_araujo/posfiap"
