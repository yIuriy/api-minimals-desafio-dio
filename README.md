# Criando uma API em .NET
  Repositório que agrupa uma API feita .NET para o Desafio de Projeto "Trabalhando com ASP.NET Minimals APIs" da DIO.

## Tecnologias Utilizadas
- .NET9
- MySql
- Swagger API
- Jwt Web Token
- Entity Framework
- MSTest

## Gerenciador de Dependências
- Nuget
  
## minimal-api API destinada a ser um sistema que permite ver, cadastrar, editar e deletar veículos, possuindo dois tipos de usuário, "Adm" e "Editor";
### Estrutura do minimal-api
```
minimal-api/
├── Domain/
│   ├── DTOs/
│   │   ├── LoginDTO.cs
│   │   └── VehicleDTO.cs
│   ├── Entitys/
│   │   ├── Administrator.cs
│   │   └── Vehicle.cs
│   ├── Enuns/
│   │   └── Profile.cs
│   ├── Interfaces/
│   │   ├── IAdministratorService.cs
│   │   └── IVehicleService.cs
│   ├── ModelViews/
│   │   ├── AdministratorModelView.cs
│   │   ├── Home.cs
│   │   ├── LoggedAdministrator.cs
│   │   └── ValidationErros.cs
│   └── Services/
│       ├── AdministratorService.cs
│       └── VehicleService.cs
├── Infrastructure/
│   ├── Db/
│   │   └── DbContextMySql.cs
│   └── Migrations/
│       ├── 20250911134239_AdministratorMigration.cs
│       ├── 20250911134239_AdministratorMigration.Designer.cs
│       ├── 20250911135025_SeedAdministrator.cs
│       ├── 20250911135025_SeedAdministrator.Designer.cs
│       ├── 20250911143736_VehicleMigration.cs
│       ├── 20250911143736_VehicleMigration.Designer.cs
│       └── DbContextMySqlModelSnapshot.cs
├── obj/
├── Properties/
├── .gitignore
├── appsettings.Development.json
├── appsettings.json
├── minimal-api.csproj
├── Program.cs
└── Startup.cs
```
### Endpoints do minimal-api
#### POST /admin/login - realiza o login no sistema, caso credenciais corretas sejam inseridas, será gerado um token Jwt, que será utilizada para acessar endpoints protegidos respeitando as regras de permissão do projeto
- Exemplo de Request
  ```
  {
  "email": "string",
  "password": "string"
  }
  ```
- Exemplo de Response
  ```
  {
  "email": "test@test.com",
  "profile": "Adm",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9k"
  }
  ```
  
#### GET /admin - necessita de autorização para acesso, além de usuário do tipo Adm e retorna todos os usuários cadastrados
- Exemplo de Response
  ```
  [
        {
          "id": 1,
          "email": "test@test.com",
          "profile": "Adm"
        }
  ]
  ```

#### POST /admin - necessita de autorização para acesso, além de usuário do tipo Adm e cadastra um novo usuário no sistema
- Exemplo de Request
  ```
  {
    "email": "string",
    "password": "string",
    "profile": 0
  }
  ```
- Exemplo de Response
  ```
  {
    "id": 1,
    "email": "string",
    "profile": "Adm"
  }
  ```

#### GET /admin/{id} - necessita de autorização para acesso, além de usuário do tipo Adm e realiza a busca de um usuário por id
- Exemplo de Response
  ```
  {
    "id": 1,
    "email": "string",
    "password": "string",
    "profile": "Adm"
  }
  ```

#### POST /vehicles - necessita de autorização para acesso e cadastra um novo veículo no sistema
- Exemplo de Request
  ```
  {
  "name": "string",
  "model": "string",
  "year": 1960
  }
  ```
- Exemplo de Response
  ```
  {
  "id": 1,
  "name": "string",
  "model": "string",
  "year": 1960
  }
  ```

#### GET /vehicles - necessita de autorização para acesso e retorna todos os veículos cadastrados no sistema
- Exemplo de Response
  ```
  [
    {
      "id": 1,
      "name": "string",
      "model": "string",
      "year": 1960
    }
  ]
  ```

#### GET /vehicles/{ud} - necessita de autorização para acesso e retorna o veículo por Id 
- Exemplo de Response
  ```
  {
    "id": 1,
    "name": "string",
    "model": "string",
    "year": 1960
  }
  ```
#### PUT /vehicles/{id} - necessita de autorização para acesso, além de usuário do tipo Adm e altera as informações de um veículo previamente cadastrado no sistema
- Exemplo de Request
  ```
  {
    "name": "string",
    "model": "string",
    "year": 1970
  }
  ```
- Exemplo de Response
  ```
  {
    "name": "string",
    "model": "string",
    "year": 1970
  }
  ```
#### DELETE /vehicles/{id} - necessita de autorização para acesso, além de usuário do tipo Adm e excluí um veículo do sistema
  
## test - projeto destinado a testar a API, utilizando Testes Unitários e Mocks
### Estrutura do test
```
test/
├── bin/
├── Domain/
│   ├── Entitys/
│   │   ├── AdministratorTest.cs
│   │   └── VehicleTest.cs
│   ├── Service/
│   │   └── AdministratorServiceTest.cs
│   ├── Helpers/
│   │   └── Setup.cs
│   └── Mocks/
│       └── AdministratorServiceMock.cs
├── obj/
├── Requests/
│   └── AdministratorRequestTest.cs
├── TestResults/
│   ├── appsettings.json
│   └── MSTestSettings.cs
└────────────────────────────────────────────
```
