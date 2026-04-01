# Softpark API

API desenvolvida para o desafio técnico Softpark, seguindo os requisitos do PDF:

- .NET 10
- C#
- Arquitetura Limpa
- Dapper
- SQL Server
- JWT
- Serilog
- Swagger
- Transações atômicas

## Objetivo

Implementar uma API para CRUD de usuários com autenticação, respeitando:

- separação em camadas `Api -> Application -> Domain -> Infrastructure`
- acesso a dados com Dapper
- persistência nas tabelas existentes `usuario` e `usuario_perfil`
- domínio rico com validações internas
- transações no insert e update
- autenticação com credenciais fixas `admin / 123`

---

## Estrutura da solução

```text
SoftparkSolution
├── Softpark.Api
├── Softpark.Application
├── Softpark.Domain
└── Softpark.Infrastructure
```

### Responsabilidade de cada camada

#### Softpark.Api
Responsável somente por HTTP:

- Controllers
- Swagger
- configuração de autenticação
- pipeline da aplicação
- middleware global de exceções

#### Softpark.Application
Responsável por:

- UseCases
- DTOs
- contratos de repositório
- serviços de aplicação

#### Softpark.Domain
Responsável por:

- entidades
- regras de negócio
- validações
- exceções de domínio

#### Softpark.Infrastructure
Responsável por:

- acesso a dados com Dapper
- implementação dos repositórios
- geração de token JWT
- fábrica de conexão
- injeção de dependência da infraestrutura

---

## Requisitos atendidos

- API em .NET 8+ (implementada em .NET 10)
- C#
- Arquitetura Limpa
- Dapper
- SQL Server
- Serilog
- Autenticação
- Endpoint de login
- Listagem paginada
- Obter usuário por ID
- Criar usuário
- Atualizar usuário
- Transações atômicas em criação e atualização
- Documentação via README

---

## Banco de dados

### Connection String

A aplicação está configurada para acessar o banco informado no desafio:

- Servidor: `infra.softpark.com.br,14338`
- Banco: `Entrevista`
- Usuário: `teste`

A senha foi mantida em configuração local para execução do projeto.

### Tabelas utilizadas

- `usuario`
- `usuario_perfil`

### Observação importante

O desafio não apresenta o schema detalhado das colunas.  
Esta implementação considera a estrutura mais provável:

#### tabela `usuario`
- `id`
- `usuario`
- `status`

#### tabela `usuario_perfil`
- `id`
- `usuario_id`
- `perfil`

Se os nomes reais das colunas diferirem, o ajuste deve ser feito apenas nas queries SQL do repositório `UsuarioRepository`.

---

## Autenticação

### Endpoint de login

**POST** `/api/auth/login`

### Credenciais fixas

```json
{
  "usuario": "admin",
  "senha": "123"
}
```

### Response de sucesso

```json
{
  "token": "jwt_token_aqui"
}
```

### Regras

- Somente o endpoint de login é público.
- Todos os demais endpoints exigem autenticação.
- Requisições sem token ou com token inválido retornam `401 Unauthorized`.

---

## Endpoints

## 1. Login

**POST** `/api/auth/login`

### Request

```json
{
  "usuario": "admin",
  "senha": "123"
}
```

### Response

```json
{
  "token": "jwt_token_aqui"
}
```

---

## 2. Listar usuários

**GET** `/api/usuarios?page=1&pageSize=10`

### Headers

```text
Authorization: Bearer {token}
```

### Query Params

- `page`: número da página
- `pageSize`: quantidade de registros por página

### Response

```json
{
  "page": 1,
  "pageSize": 10,
  "totalRecords": 2,
  "totalPages": 1,
  "data": [
    {
      "id": 1,
      "usuario": "joao",
      "status": true,
      "perfis": ["Administrador"]
    },
    {
      "id": 2,
      "usuario": "maria",
      "status": false,
      "perfis": ["Consulta"]
    }
  ]
}
```

---

## 3. Obter usuário por ID

**GET** `/api/usuarios/{id}`

### Headers

```text
Authorization: Bearer {token}
```

### Parâmetros de rota

- `id`: identificador do usuário

### Response

```json
{
  "id": 1,
  "usuario": "joao",
  "status": true,
  "perfis": ["Administrador", "Operador"]
}
```

---

## 4. Criar usuário

**POST** `/api/usuarios`

### Headers

```text
Authorization: Bearer {token}
```

### Request

```json
{
  "usuario": "rodrigo",
  "status": true,
  "perfis": ["Administrador", "Operador"]
}
```

### Response

```json
{
  "id": 10
}
```

### Regras de negócio aplicadas

- `usuario` é obrigatório
- `status` é obrigatório
- deve existir pelo menos um perfil
- perfis vazios não são aceitos
- perfis duplicados são desconsiderados pela entidade

---

## 5. Atualizar usuário

**PUT** `/api/usuarios/{id}`

### Headers

```text
Authorization: Bearer {token}
```

### Parâmetros de rota

- `id`: identificador do usuário

### Request

```json
{
  "usuario": "rodrigo.atualizado",
  "status": false,
  "perfis": ["Consulta"]
}
```

### Response

```text
204 No Content
```

### Regras de negócio aplicadas

- usuário precisa existir
- `usuario` é obrigatório
- deve existir pelo menos um perfil
- atualização da entidade é feita por método da própria entidade

---

## Arquitetura e arquivos principais

## Softpark.Domain

### `Entities/Usuario.cs`
Implementa a entidade principal com:

- `private set`
- validação de nome de usuário
- validação de mínimo de perfis
- remoção de perfis inválidos
- remoção de duplicidade
- método `Atualizar(...)`
- método `DefinirId(...)`

### `Entities/UsuarioPerfil.cs`
Representa o perfil do usuário com validação própria.

### `Exceptions/DomainException.cs`
Exceção específica para violações de regra de negócio.

---

## Softpark.Application

### DTOs
Contém os contratos de entrada e saída:

- `LoginRequestDto`
- `LoginResponseDto`
- `CriarUsuarioRequestDto`
- `AtualizarUsuarioRequestDto`
- `UsuarioResponseDto`
- `PagedResultDto<T>`

### Interfaces
Contratos:

- `IUsuarioRepository`
- `ITokenService`

### Services
`AuthService` valida o login fixo e delega a geração do token ao `ITokenService`.

### UseCases
Casos de uso isolados:

- `CriarUsuarioUseCase`
- `AtualizarUsuarioUseCase`
- `ObterUsuarioPorIdUseCase`
- `ListarUsuariosUseCase`

---

## Softpark.Infrastructure

### `Data/ConnectionFactory.cs`
Cria conexões com SQL Server.

### `Repositories/UsuarioRepository.cs`
Implementa acesso a dados via Dapper.

#### Responsabilidades
- criar usuário e perfis
- atualizar usuário e seus perfis
- obter por id
- listar paginado
- garantir transação no create
- garantir transação no update

### `Services/JwtTokenService.cs`
Gera token JWT.

### `DependencyInjection/InfrastructureDependencyInjection.cs`
Centraliza registro dos serviços da infraestrutura.

---

## Softpark.Api

### `Program.cs`
Responsável por:

- registrar infraestrutura
- registrar casos de uso
- configurar JWT
- configurar Swagger
- configurar Serilog
- montar pipeline HTTP

### `Controllers/AuthController.cs`
Responsável apenas por receber o request HTTP de login e chamar `AuthService`.

### `Controllers/UsuariosController.cs`
Responsável apenas por receber requests HTTP e chamar os UseCases.

### `Middlewares/ExceptionHandlingMiddleware.cs`
Captura exceções:

- `DomainException` -> `400 Bad Request`
- exceções não tratadas -> `500 Internal Server Error`

---

## Transações

O desafio exige consistência entre `usuario` e `usuario_perfil`.

Por isso:

- o `create` abre transação, insere o usuário, insere os perfis e faz `commit`
- o `update` abre transação, atualiza o usuário, remove perfis anteriores, insere novos perfis e faz `commit`
- qualquer falha executa `rollback`

---

## Boas práticas aplicadas

- separação clara de camadas
- controllers sem regra de negócio
- domínio rico
- Dapper sem Entity Framework
- repositório acessado por interface
- token gerado em serviço próprio
- tratamento global de exceções
- uso de `CancellationToken`
- DTOs separados por responsabilidade
- paginação padronizada
- `private set` nas entidades
- código orientado a casos de uso
- Serilog configurado

---

## Como executar

### Pré-requisitos

- .NET 10 SDK
- Visual Studio compatível com .NET 10
- acesso ao SQL Server do desafio

### Passos

1. Abrir a solução no Visual Studio
2. Restaurar os pacotes NuGet
3. Confirmar a connection string em `appsettings.json`
4. Definir `Softpark.Api` como Startup Project
5. Executar a aplicação
6. Abrir o Swagger
7. Autenticar via `/api/auth/login`
8. Informar o token no botão **Authorize**
9. Testar os endpoints protegidos

---

## Como testar no Swagger

### 1. Login
Faça `POST /api/auth/login` com:

```json
{
  "usuario": "admin",
  "senha": "123"
}
```

### 2. Copie o token retornado

### 3. Clique em `Authorize`

### 4. Informe:

```text
Bearer SEU_TOKEN_AQUI
```

### 5. Execute os endpoints:

- `GET /api/usuarios`
- `GET /api/usuarios/{id}`
- `POST /api/usuarios`
- `PUT /api/usuarios/{id}`

---

## Possíveis ajustes finais

### 1. Nomes reais das colunas do banco
Se divergirem do assumido, ajustar apenas os SQLs do `UsuarioRepository`.

### 2. Ambiente local
Se houver incompatibilidade de Visual Studio com `net10.0`, usar versão compatível com esse target.

### 3. Token no Swagger
Caso o Swagger não envie o header corretamente em alguma combinação de versões, validar também no Postman.

---

## Checklist de entrega

- [x] API .NET 10
- [x] C#
- [x] Clean Architecture
- [x] Dapper
- [x] SQL Server
- [x] Autenticação JWT
- [x] Login com `admin / 123`
- [x] Listagem paginada
- [x] Obter por ID
- [x] Criar usuário
- [x] Atualizar usuário
- [x] Transações
- [x] Serilog
- [x] Swagger
- [x] README
- [x] Sem Entity Framework

---

## Observação final

Este projeto foi estruturado para aderir integralmente ao desafio técnico Softpark.  
O único ponto que depende de validação contra o banco real é o nome exato das colunas das tabelas existentes.
