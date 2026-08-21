# Orion

API em ASP.NET Core (.NET 10) estruturada como **modular monolith**, com módulos de negócio isolados em projetos próprios, autenticação via JWT Bearer Token e bloqueio de conta por tentativas de login falhas.

## Autor

**Raimundos Marques** — Analista de Sistemas

Projeto construído com propósito duplo: consolidar estudo de ASP.NET Core e arquitetura modular monolith, com a intenção de evoluir para um serviço em produção.

## Sumário

- [Arquitetura](#arquitetura)
- [Stack técnica](#stack-técnica)
- [Módulos](#módulos)
- [Segurança](#segurança)
- [Tratamento de erros](#tratamento-de-erros)
- [Endpoints](#endpoints)
- [Testes](#testes)
- [Como rodar localmente](#como-rodar-localmente)
- [Docker](#docker)
- [Estrutura de pastas](#estrutura-de-pastas)
- [Limitações conhecidas / roadmap](#limitações-conhecidas--roadmap)

## Arquitetura

Modular monolith: um único processo/deploy, mas cada módulo de negócio é um **projeto (`.csproj`) separado**, não só uma pasta. Isso faz o isolamento ser garantido pelo compilador — um módulo literalmente não compila se referenciar outro — em vez de depender só de disciplina de code review.

```
Api (host)  ──references──▶  Modules/Users
            ──references──▶  Modules/Authentication
            ──references──▶  Orion.SharedKernel

Modules/Users          ──references──▶  Orion.SharedKernel
Modules/Authentication ──references──▶  Orion.SharedKernel

Modules/Users  ✗  Modules/Authentication   (nunca se referenciam entre si)
```

Dentro de cada módulo, a separação em camadas é feita por **pasta**, não por projeto:

```
Modules/Users/
├── Domain/            entidade rica (User), sem dependência de framework
│   └── Abstractions/  interfaces que o Domain define e a Infrastructure implementa
├── Application/       casos de uso (um por ação: CreateUserUseCase, RenameUserUseCase...)
│   └── Abstractions/  interfaces que a Application usa e a Infrastructure implementa
└── Infrastructure/    EF Core, hashing de senha, controllers HTTP
    ├── Persistence/
    ├── Security/
    └── Web/
```

Um projeto por módulo + camadas por pasta foi uma decisão deliberada: separar camadas em projetos também (`Users.Domain.csproj`, `Users.Application.csproj`, ...) multiplicaria a quantidade de `.csproj` sem ganho proporcional de rigor — a mesma técnica de teste de arquitetura usada pra isolar módulos serviria igualmente pra isolar camada, sem precisar do overhead de projeto por camada.

### Comunicação entre módulos sem acoplamento direto

O módulo `Authentication` precisa validar credenciais que pertencem ao módulo `Users`, mas **não pode referenciar `Users` diretamente** — isso quebraria o isolamento. A solução é inversão de dependência via um contrato no `Orion.SharedKernel`:

- `Orion.SharedKernel.Contracts.IUserCredentialsChecker` — contrato, visível a todos os módulos.
- `Users.Infrastructure.Security.UserCredentialsChecker` — implementação real, dentro do módulo Users.
- `Authentication` depende só da interface; a implementação concreta é resolvida em runtime pelo container de DI, configurado no host (`Api`).

### `Orion.SharedKernel`

Projeto compartilhado com o mínimo indispensável usado por todos os módulos, sem dependência de ASP.NET Core:

- `Entity<TId>` — base de entidade de domínio (igualdade por identidade).
- `AppException`/`ErrorType` — hierarquia de exceções de aplicação (`AppNotFoundException`, `AppConflictException`, `AppUnauthorizedException`, `AppLockedException`, ...).
- `Contracts/IUserCredentialsChecker` — contrato cross-module descrito acima.

### Teste de isolamento de módulos

`src/Tests/Architecture/ModuleIsolationTests.cs` lê o XML de cada `.csproj` de módulo e falha se encontrar um `<ProjectReference>` de um módulo apontando pra outro. Roda como parte da suíte de testes normal — qualquer tentativa de acoplar `Users` e `Authentication` diretamente quebra o build de testes, não só uma revisão de código.

## Stack técnica

| Categoria | Tecnologia |
|---|---|
| Runtime / framework | .NET 10 / ASP.NET Core Web API |
| Persistência | Entity Framework Core 10 + SQL Server |
| Autenticação | JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`) |
| Hash de senha | `Microsoft.AspNetCore.Identity.PasswordHasher<TUser>` |
| Documentação de API | OpenAPI nativo (`Microsoft.AspNetCore.OpenApi`) + Swagger UI (`Swashbuckle.AspNetCore.SwaggerUI`) |
| Testes | xUnit + Moq |
| Health check | `Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore` |
| Containerização | Docker multi-stage build |

## Módulos

### Users

Dono do ciclo de vida da conta: criação, renomear, trocar senha, verificar email, ativar/desativar. Schema próprio no banco (`users`).

### Authentication

Dono da autenticação: login, emissão de JWT e bloqueio de conta por tentativas falhas. Schema próprio no banco (`auth`), completamente independente do schema `users` — a única coisa que os dois módulos compartilham é o contrato `IUserCredentialsChecker`.

## Segurança

- **Hash de senha**: `PasswordHasher<User>` do ASP.NET Core Identity (PBKDF2-HMACSHA256, 100k iterações, salt aleatório) — mantido oficialmente pela Microsoft, sem dependência de terceiro.
- **Autenticação**: JWT Bearer assinado com HMAC-SHA256. Claims: `sub` (id do usuário), `email`, `jti`.
- **Bloqueio de conta** (`Authentication.Domain.LoginLockoutPolicy`): 5 tentativas de login falhas em uma janela de 15 minutos bloqueiam novas tentativas — inclusive com a senha correta — até 15 minutos após a última falha. Cada tentativa (sucesso ou falha) é registrada em `auth.LoginAttempts`.

## Tratamento de erros

Toda exceção de domínio/aplicação herda de `AppException` e carrega um `ErrorType`. O `GlobalExceptionHandler` (`Api/Exceptions`) converte isso em `ProblemDetails` com o status HTTP correspondente:

| `ErrorType` | HTTP |
|---|---|
| `Validation` | 400 |
| `Unauthorized` | 401 |
| `Forbidden` | 403 |
| `NotFound` | 404 |
| `Conflict` | 409 |
| `Locked` | 423 |
| `Unexpected` (qualquer outra exceção) | 500 |

## Endpoints

### Users — `/api/users`

| Método | Rota | Ação |
|---|---|---|
| `POST` | `/api/users` | Cria usuário |
| `GET` | `/api/users/{id}` | Busca usuário por id |
| `PUT` | `/api/users/{id}/name` | Renomeia |
| `PUT` | `/api/users/{id}/password` | Troca senha |
| `POST` | `/api/users/{id}/verify-email` | Marca email como verificado |
| `POST` | `/api/users/{id}/activate` | Ativa |
| `POST` | `/api/users/{id}/deactivate` | Desativa |

### Authentication — `/api/auth`

| Método | Rota | Ação |
|---|---|---|
| `POST` | `/api/auth/login` | Login — retorna JWT |

### Infra

| Método | Rota | Ação |
|---|---|---|
| `GET` | `/health` | Verifica conectividade com os dois bancos (Users + Authentication) |

Documentação interativa em `/swagger` (ambiente Development).

## Testes

- `src/Tests/Modules/Users` — testes unitários do módulo Users (domínio + use cases com Moq).
- `src/Tests/Architecture` — teste de isolamento entre módulos.

```bash
dotnet test Orion.slnx
```

## Como rodar localmente

Pré-requisitos: .NET 10 SDK, SQL Server acessível, ferramenta `dotnet-ef` (`dotnet tool install --global dotnet-ef`).

```bash
# 1. Configurar segredos (nunca em appsettings.json)
dotnet user-secrets set "ConnectionStrings:DatabaseConnection" "<sua connection string>" --project src/Api/Api.csproj
dotnet user-secrets set "Jwt:SigningKey" "<chave aleatória de pelo menos 32 bytes>" --project src/Api/Api.csproj

# 2. Aplicar as migrations de cada módulo
dotnet ef database update --project src/Modules/Users/Users.csproj --startup-project src/Api/Api.csproj
dotnet ef database update --project src/Modules/Authentication/Authentication.csproj --startup-project src/Api/Api.csproj --context AuthenticationDbContext

# 3. Rodar
dotnet run --project src/Api/Api.csproj
```

## Docker

Dois caminhos, propositalmente separados:

- **Dev** — `docker compose up` aplica `docker-compose.yml` + `docker-compose.override.yml` automaticamente. Monta o user-secrets e o certificado de desenvolvimento da máquina host; força `ASPNETCORE_ENVIRONMENT=Development`. Nunca use isso em produção.
- **Produção** — precisa ser explicitamente pedido, não é aplicado por acidente:
  ```bash
  cp .env.production.example .env   # preencher com valores reais
  docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d
  ```
  Segredos entram via variável de ambiente (`ConnectionStrings__DatabaseConnection`, `Jwt__SigningKey`), não via arquivo. A imagem expõe `HEALTHCHECK` batendo em `/health`.

## Estrutura de pastas

```
src/
├── Api/                          host ASP.NET Core — Program.cs, Dockerfile, appsettings
├── Modules/
│   ├── Users/                    Domain / Application / Infrastructure
│   └── Authentication/           Domain / Application / Infrastructure
├── Orion.SharedKernel/           Entity<TId>, AppException, contratos cross-module
└── Tests/
    ├── Architecture/             isolamento entre módulos
    └── Modules/Users/            testes unitários do módulo Users
```

## Limitações conhecidas / roadmap

- Nenhum endpoint exige `[Authorize]` ainda — o JWT é emitido, mas nada valida ele hoje.
- Sem refresh token / logout / revogação de token.
- `auth.LoginAttempts` cresce indefinidamente — não há expurgo de registros antigos.
- Bloqueio de login é só por email, não por IP/dispositivo.
- Aplicação de migration em produção ainda é manual (`dotnet ef database update`), sem pipeline de deploy.
