# FIAP Cloud Games (FCG) — Tech Challenge Fase 1

API REST em **.NET 8** (monólito) com cadastro, autenticação **JWT**, perfis **Usuario** / **Administrador**, jogos, biblioteca de jogos adquiridos e promoções.

## GitLab

Remoto sugerido: `https://gitlab.com/leo_araujo/posfiap.git`

No terminal (na pasta do projeto), após o Git e as credenciais GitLab estarem OK:

```bash
./scripts/conectar-gitlab.sh
```

Ou manualmente:

```bash
git init   # se ainda não existir repositório
git remote add origin https://gitlab.com/leo_araujo/posfiap.git
git branch -M main
git add -A && git commit -m "Initial commit"   # se ainda não houver commits
git push -uf origin main
```

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Configuração

1. Connection string e JWT em `src/FCG/appsettings.json` (e `appsettings.Development.json` para desenvolvimento).
2. **Jwt:Key** deve ter **pelo menos 32 caracteres**. Em produção use variável de ambiente ou cofre — **não** commite chaves reais.
3. **Seed (somente Development)**: em `appsettings.Development.json`, `Seed` cria um administrador se o e-mail ainda não existir (senha deve obedecer a política de senha forte).

## Executar

```bash
dotnet restore
dotnet build FCG.sln
```

```bash
cd src/FCG
dotnet run
```

- Swagger UI (Development): `https://localhost:<porta>/swagger`
- Health: `GET /health`

Na primeira execução, as **migrations** aplicam o SQLite (`fcg.db` no diretório de trabalho).

## Testes (xUnit)

```bash
dotnet test
```

Inclui testes das regras de **e-mail** e **senha forte** (`CredentialValidation`), alinhadas ao enunciado (módulo de domínio testável de forma isolada).

## Endpoints principais

| Método | Rota | Autenticação |
|--------|------|----------------|
| POST | `/api/auth/register` | Anônimo |
| POST | `/api/auth/login` | Anônimo |
| GET | `/api/users/me` | JWT (Usuario ou Admin) |
| GET | `/api/users/me/library` | JWT |
| POST | `/api/users/me/library/games/{gameId}` | JWT |
| GET | `/api/games` | Anônimo |
| POST | `/api/games` | JWT **Administrador** |
| GET | `/api/promotions` | Anônimo (promoções ativas) |
| POST | `/api/admin/promotions` | JWT **Administrador** |
| GET | `/api/admin/users` | JWT **Administrador** |
| PATCH | `/api/admin/users/{userId}/role` | JWT **Administrador** |

No Swagger, use **Authorize** e informe `Bearer {token}` após login ou registro.

## Política de senha

- Mínimo **8** caracteres  
- Pelo menos **uma letra**, **um número** e **um caractere especial**

## Migrations

```bash
dotnet ef database update --project src/FCG
```

Para nova migration após alterar o modelo:

```bash
dotnet ef migrations add NomeDaMigration --project src/FCG
```

## Estrutura interna (monólito)

- `Controllers/` — HTTP  
- `Domain/` — entidades (`Usuario`, `Jogo`, `UsuarioJogo`, `Promocao`) e regras (ex.: validação de credenciais)  
- `Application/` — contratos e DTOs  
- `Infrastructure/` — EF Core, JWT, serviços, seed  
- `Middleware/` — tratamento global de erros  

## Documentação DDD e entrega (Fase 1)

| Artefato | Caminho |
|----------|---------|
| Event Storming + diagramas (equivalente Miro) | [`docs/DDD_EventStorming_e_Diagramas.md`](docs/DDD_EventStorming_e_Diagramas.md) |
| Roteiro do vídeo (≤ 15 min) | [`docs/ROTEIRO_VIDEO_15MIN.md`](docs/ROTEIRO_VIDEO_15MIN.md) |
| Evidência TDD/BDD (módulo de validação) | [`docs/TDD_BDD_Evidencia.md`](docs/TDD_BDD_Evidencia.md) |
| Modelo de relatório para upload | [`ENTREGA_RELATORIO_TEMPLATE.txt`](ENTREGA_RELATORIO_TEMPLATE.txt) |

**O que ainda é manual:** gravar o vídeo, publicar o link, preencher nomes/Discord no relatório e, se a banca exigir Miro, copiar o conteúdo de `docs/DDD_EventStorming_e_Diagramas.md` para um board ou anexar o link do repositório como “equivalente”.
