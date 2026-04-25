# Roteiro de vídeo — Tech Challenge Fase 1 (até 15 minutos)

Use este roteiro para gravar a demonstração obrigatória. Ajuste tempos conforme a sua fluência; o total deve ficar **≤ 15 minutos**.

**Antes de gravar:** `dotnet build`, `dotnet test`, `dotnet run` em `src/FCG`; Swagger aberto; terminal à mostra se quiser mostrar testes.

---

## Bloco A — Contexto (1–2 min)

- Nome do projeto: **FIAP Cloud Games (FCG)** — API .NET 8, monólito.
- Objetivo da Fase 1: cadastro, JWT, perfis Usuario/Administrador, jogos, biblioteca, promoções, EF Core + migrations, middleware de erro, Swagger, testes xUnit.
- Mostrar rapidamente a estrutura de pastas no IDE (`Domain`, `Application`, `Infrastructure`, `Controllers`).

---

## Bloco B — Documentação DDD (1–2 min)

- Abrir `docs/DDD_EventStorming_e_Diagramas.md` (ou o board Miro, se vocês copiaram para lá).
- Mostrar **Event Storming** resumido: fluxo de **cadastro de usuário** e **cadastro de jogo** (post-its ou seções do markdown).
- Mostrar **um diagrama** (modelo de domínio / containers) no Mermaid ou no Miro.

---

## Bloco C — API no Swagger (6–8 min)

Sugestão de ordem (deixe a aba Swagger visível).

1. **Health:** `GET /health` — 200.
2. **Registro:** `POST /api/auth/register` — body com nome, e-mail, senha forte (ex.: `Teste@123x`). Copiar o **token** da resposta.
3. **Authorize** no Swagger: colar `Bearer {token}`.
4. **Perfil:** `GET /api/users/me` — id, nome, e-mail, role `Usuario`.
5. **Listar jogos:** `GET /api/games` — pode estar vazio.
6. **Login como admin (Development):** `POST /api/auth/login` com e-mail/senha do seed (`appsettings.Development.json` — ex.: `admin@fcg.local` / `Admin@123x`). Autorizar com o novo token.
7. **Criar jogo:** `POST /api/games` — título, gênero, preço.
8. **Listar jogos** de novo — mostrar o jogo criado.
9. **Voltar token de usuário comum** (login do usuário registrado no passo 2) ou registrar outro usuário — **adquirir jogo:** `POST /api/users/me/library/games/{gameId}` com o `gameId` retornado.
10. **Biblioteca:** `GET /api/users/me/library` — jogo aparece.
11. **Promoção (admin):** `POST /api/admin/promotions` — datas de vigência e desconto.
12. **Promoções ativas:** `GET /api/promotions`.
13. **Admin usuários:** `GET /api/admin/users`; opcional `PATCH /api/admin/users/{id}/role` (cuidado para não travar o único admin sem outro).

Se faltar tempo, pule o PATCH de role e foque em cadastro → login → jogo → biblioteca → promoção.

---

## Bloco D — Requisitos técnicos rápidos (2–3 min)

- **Middleware de erro:** provocar um 400 (ex.: registro com e-mail duplicado ou senha fraca) e mostrar JSON `{ error, traceId }`.
- **Migrations / banco:** mostrar `fcg.db` gerado ou mencionar que sobe com `MigrateAsync` na inicialização.
- **Testes:** no terminal, `dotnet test` e destacar `CredentialValidationTests` como regras de negócio cobertas (TDD/BDD no módulo de validação).

---

## Bloco E — Encerramento (30 s)

- Repositório público/privado + link no relatório.
- Onde está a documentação DDD (`docs/` ou Miro).

---

## Checklist antes de publicar o vídeo

- [ ] Áudio legível; tela em 1080p se possível.
- [ ] Nenhuma chave secreta real commitada (blur se necessário).
- [ ] Duração total ≤ 15 min.
