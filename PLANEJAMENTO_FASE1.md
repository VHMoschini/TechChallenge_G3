# Planejamento do Projeto - Tech Challenge Fase 1 (FCG)

Este arquivo centraliza o plano de desenvolvimento, acompanhamento de andamento e checklist de entrega.

## 1) Objetivo da Fase 1

Construir uma API REST em .NET 8 (monolito) para:

- cadastro de usuarios;
- autenticacao via JWT;
- autorizacao com perfis `Usuario` e `Administrador`;
- gestao de jogos e biblioteca de jogos adquiridos.

---

## 2) Escopo Obrigatorio

- Cadastro de usuarios com `nome`, `e-mail` e `senha`.
- Validacao de e-mail e senha forte (minimo 8 caracteres, letras, numeros e caractere especial).
- Autenticacao com JWT.
- Dois niveis de acesso:
  - `Usuario`: acesso a plataforma e biblioteca;
  - `Administrador`: cadastrar jogos, administrar usuarios e criar promocoes.
- Persistencia com Entity Framework Core e migrations.
- API em .NET 8 (Minimal API ou Controllers MVC).
- Middleware de erros e logs estruturados.
- Swagger.
- Testes unitarios das principais regras de negocio (xUnit).
- Aplicacao de TDD ou BDD em pelo menos um modulo.
- Documentacao DDD (Event Storming e diagramas).

---

## 3) Status Atual do Projeto

### Implementado no codigo

- [x] Monolito `src/FCG` com `Controllers`, `Domain`, `Application`, `Infrastructure`, `Middleware`.
- [x] EF Core SQLite, `AppDbContext`, **migration inicial** (`Infrastructure/Persistence/Migrations`).
- [x] `POST /api/auth/register`, `POST /api/auth/login` com validacao de e-mail e senha forte; JWT.
- [x] Perfis `Usuario` / `Administrador` e `[Authorize(Roles = ...)]`.
- [x] `GET/POST` jogos (criacao admin); `GET /api/games` publico.
- [x] Biblioteca: `GET /api/users/me/library`, `POST /api/users/me/library/games/{gameId}`.
- [x] `GET /api/users/me` (perfil do token).
- [x] Admin: `GET/PATCH /api/admin/users`, `POST /api/admin/promotions`; promocoes ativas `GET /api/promotions`.
- [x] Middleware global de excecoes + `ILogger` com propriedades estruturadas.
- [x] Swagger com seguranca Bearer.
- [x] Seed opcional de admin em **Development** (`appsettings.Development.json`).
- [x] Testes xUnit para `CredentialValidation` (+ teste de `Usuario`).
- [x] `README.md` com comandos e tabela de endpoints.

### Pendencias de entrega (acao do grupo)

- [ ] Validar localmente: `dotnet build` e `dotnet test` na sua maquina.
- [x] Documentacao DDD em arquivo: `docs/DDD_EventStorming_e_Diagramas.md` (equivalente ao Miro; opcional copiar para Miro).
- [x] Roteiro de video: `docs/ROTEIRO_VIDEO_15MIN.md`.
- [x] Evidencia TDD/BDD: `docs/TDD_BDD_Evidencia.md`.
- [x] Template de relatorio: `ENTREGA_RELATORIO_TEMPLATE.txt`.
- [ ] Gravar e publicar video (ate 15 min) e colar links no relatorio.
- [ ] Preencher e enviar relatorio final (PDF/TXT) na data oficial.

### Modulo TDD/BDD (evidencia sugerida)

- Regras de credenciais em `Domain/Services/CredentialValidation.cs` com testes em `tests/FCG.Tests/Domain/CredentialValidationTests.cs` (comportamento documentado por testes).

---

## 4) Roadmap por Sprint

## Sprint 1 - Fundacao e Autenticacao

### Tarefas
- [x] Solucao + monolito.
- [x] EF Core SQLite + DI.
- [x] Migration inicial.
- [x] Cadastro e login JWT.
- [x] Roles e autorizacao.
- [x] Middleware de erros.

### Entregaveis
- [x] API com auth e banco migrado na subida.

---

## Sprint 2 - Jogos, Biblioteca e Regras de Acesso

### Tarefas
- [x] CRUD minimo de jogos (criar + listar).
- [x] Biblioteca (adquirir + listar).
- [x] Endpoints admin (usuarios e promocoes).

### Entregaveis
- [x] Fluxo principal do MVP na API.

---

## Sprint 3 - Qualidade, Documentacao e Entrega

### Tarefas
- [x] Testes unitarios das regras de validacao (xUnit).
- [x] README completo.
- [x] Event Storming e diagramas DDD (arquivo em `docs/`).
- [x] Roteiro de video (`docs/ROTEIRO_VIDEO_15MIN.md`).
- [ ] Gravacao e publicacao do video.
- [x] Template de relatorio (`ENTREGA_RELATORIO_TEMPLATE.txt`).
- [ ] Relatorio final preenchido e enviado.

---

## 5) Checklist de Entrega Final (FIAP)

- [x] API .NET 8 (projeto pronto para execucao).
- [x] Cadastro com validacoes obrigatorias.
- [x] JWT e dois niveis de acesso.
- [x] Jogos (admin) e biblioteca (usuario).
- [x] Promocoes (criacao admin + listagem ativa).
- [x] EF Core + migrations.
- [x] Middleware de erro + logging.
- [x] Swagger.
- [x] Testes unitarios (regras principais).
- [x] Evidencia TDD/BDD descrita em `docs/TDD_BDD_Evidencia.md`.
- [x] Event Storming e diagramas DDD em `docs/DDD_EventStorming_e_Diagramas.md`.
- [ ] Video de ate 15 min (gravar e publicar).
- [ ] Relatorio final com links (usar template e preencher).

---

Ultima atualizacao: 2026-04-09 (documentacao de entrega adicionada em docs/)
