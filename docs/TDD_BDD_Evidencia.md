# Evidência — TDD / BDD no módulo de validação de credenciais

O enunciado pede **TDD ou BDD em pelo menos um módulo**. Este projeto documenta o módulo escolhido e como os testes amarram o comportamento.

## Módulo escolhido

- **Domínio:** `src/FCG/Domain/Services/CredentialValidation.cs`  
- **Testes:** `tests/FCG.Tests/Domain/CredentialValidationTests.cs` (xUnit: `[Fact]` e `[Theory]`)

## Comportamento especificado (regras de negócio)

1. **E-mail:** formato válido (regex simples e objetiva para o MVP).  
2. **Senha forte:** mínimo 8 caracteres; pelo menos uma letra, um número e um caractere especial (conforme Tech Challenge).

## Como isso se relaciona com TDD / BDD

- **Abordagem TDD (testes como especificação):** os testes descrevem o comportamento esperado da função pura `CredentialValidation` **sem** depender de HTTP, banco ou JWT. Falhas de regra aparecem como falhas de teste antes de qualquer alteração na API.  
- **BDD (linguagem de comportamento):** os nomes dos métodos de teste e os dados em `[InlineData]` leem como cenários (“senha fraca sem número”, “e-mail inválido”).

## O que mostrar na apresentação ou relatório

1. Abrir `CredentialValidation.cs` e `CredentialValidationTests.cs` lado a lado.  
2. Rodar `dotnet test --filter FullyQualifiedName~CredentialValidationTests`.  
3. (Opcional) Mostrar um commit histórico onde os testes foram adicionados junto com a classe de domínio.

> Observação: a ordem “teste antes do código” ideal do TDD puro nem sempre fica visível no histórico final; o importante é que **um módulo** tenha regras claras e **cobertura de teste unitário** demonstrável — este módulo atende a esse critério.
