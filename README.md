# .NET-kafka-efcore-webapi-template

Este repositório é um template para projetos baseados em microsserviços utilizando .NET, Apache Kafka e Entity Framework Core. O objetivo é fornecer uma estrutura inicial para desenvolvimento de aplicações distribuídas, com comunicação assíncrona via Kafka e persistência de dados com EF Core.

## Estrutura do Projeto

- **Producer/**: Microsserviço responsável por produzir mensagens para o Kafka. Inclui controllers para expor endpoints HTTP e enviar dados para o tópico Kafka.
- **Consumer/**: Microsserviço responsável por consumir mensagens do Kafka e processá-las conforme a lógica de negócio.
- **Shared/**: Projeto compartilhado contendo modelos de domínio, DTOs, contexto do Entity Framework Core, configurações comuns e migrações.
- **compose.yaml**: Arquivo de orquestração Docker Compose para facilitar o deploy local dos microsserviços, do Kafka e do banco de dados.

## Funcionalidades

- Comunicação entre microsserviços via Apache Kafka.
- API RESTful para envio de dados (Producer).
- Consumo e processamento de mensagens (Consumer).
- Persistência de dados utilizando Entity Framework Core.
- Estrutura modular e reutilizável para novos microsserviços.
- Suporte a migrações de banco de dados via EF Core.
- Pronto para integração com MySQL ou PostgreSQL.

## Tecnologias Utilizadas

- **.NET 8** (Worker Service e Web API)
- **Apache Kafka** (mensageria)
- **Entity Framework Core** (ORM)
- **Docker Compose** (orquestração de containers)
- **MySQL/PostgreSQL** (banco de dados relacional)

## Como usar

1. Clone este repositório.
2. Utilize o `compose.yaml` para subir os serviços necessários (Kafka, Producer, Consumer, banco de dados).
3. Crie sua tabela no MySQL ou PostgreSQL conforme o contexto do EF Core e execute as migrações.
4. Implemente suas regras de negócio a partir deste template.

## Observações

- O projeto já inclui exemplos de DTOs, models e migrações para facilitar o início do desenvolvimento.
- As configurações de conexão com o banco de dados e Kafka podem ser ajustadas nos arquivos `appsettings.json` de cada microsserviço.
- Recomenda-se utilizar o Docker Compose para garantir o funcionamento integrado dos serviços.

Este projeto serve como ponto de partida para arquiteturas baseadas em microsserviços com mensageria e persistência de dados.
