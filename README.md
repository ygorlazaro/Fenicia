
# Fenicia SaaS Platform – Auth Service

Este é o serviço de autenticação e autorização da plataforma **Fenicia**, um sistema SaaS modular e multi-tenant. O `AuthService` centraliza o gerenciamento de usuários, empresas, planos de assinatura e geração de tokens JWT utilizados por todos os serviços da plataforma.

---

## 🧩 Arquitetura do Projeto

- **Multi-tenant por banco de dados**: Cada empresa possui seu próprio banco (isolamento físico).
- **Modularidade**: Cada funcionalidade é um microserviço desacoplado (ex: Auth, Basic, Social, RH...).
- **JWT centralizado**: O `AuthService` é o único responsável por autenticar usuários e emitir tokens válidos para os demais módulos.
- **Claims do JWT**:
  - `sub` (ID do usuário)
  - `companyId` (ID da empresa)
  - `modules` (array com módulos assinados)
  - `tenantId` (usado para montar a string de conexão dos serviços)

---

## 🚀 Como rodar a aplicação localmente

### Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)
- [RabbitMQ (opcional, mas recomendado)](https://www.rabbitmq.com/download.html)
- [Docker (opcional, para facilitar o setup)]

---

### 🔧 Configuração

1. **Crie o banco de dados base (admin/central)**:
   Esse banco é onde o `AuthService` opera e armazena informações como usuários, empresas e tokens.

2. **Configure a string de conexão** no arquivo `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "TenantTemplate": "Host=localhost;Port=5432;Database=tenant_{tenant};Username=postgres;Password=senha",
    "Default": "Host=localhost;Port=5432;Database=auth_db;Username=postgres;Password=senha"
  },
  "Jwt": {
    "Issuer": "fenicia-auth",
    "Audience": "fenicia-clients",
    "Secret": "segredo-super-seguro"
  }
}
```

3. **Rodar as migrations** para o banco do Auth:

```bash
dotnet ef database update --project Fenicia.Module.Auth
```

Para rodar migrations de tenants:

```bash
dotnet run --project Fenicia.Module.Auth -- --migrate-tenants
```

*(Isso executará uma lógica que percorre todos os tenants registrados e roda as migrations para cada um.)*

---

## 🏁 Executando a aplicação

```bash
dotnet run --project Fenicia.Module.Auth
```

---

## 🛠 Estrutura do Projeto

- `Fenicia.Common`: Contém utilitários, interfaces e providers reutilizados pelos módulos.
- `Fenicia.Module.Auth`: Responsável por:
  - Cadastro/login de usuários
  - Criação de empresas
  - Assinatura de módulos
  - Geração de JWT
- `Fenicia.Module.Basic` (e demais): Consomem o JWT gerado pelo Auth e acessam seus próprios bancos via `tenantId`.

---

## 🔐 Segurança

- Tokens JWT são obrigatórios para qualquer requisição aos módulos.
- Middleware de autorização valida se o token possui permissão para o módulo acessado (`Claim: modules`).
- Multi-tenancy configurado via string de conexão dinâmica, baseada no `tenantId` presente no token.

---

## 📬 Comunicação entre serviços

- Utiliza RabbitMQ para eventos internos e integração entre módulos.
- Exemplo: Quando uma nova empresa é criada no `AuthService`, um evento pode ser enviado para o `BasicService` inicializar dados padrões no banco da empresa.

---

## ✅ TODO Futuro

- [ ] Rate limit e lockout após muitas tentativas de login
- [ ] Painel de administração para gerenciar empresas e módulos
- [ ] Integração com gateways de pagamento para billing

---

## 📄 Licença

Este projeto é livre para uso e distribuição privada durante o desenvolvimento. Direitos reservados à equipe Fenicia.
