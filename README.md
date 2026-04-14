# Desafio Técnico - Sistema de Emissão de Notas Fiscais (Korp-ERP)

Este repositório foi desenvolvido para o desafio técnico de Emissão de Notas Fiscais. O sistema gerencia o ciclo de vida completo de Notas Fiscais, integrando o faturamento com o controle de estoque em tempo real, garantindo resiliência, persistência e integridade dos dados através de uma arquitetura moderna e desacoplada.

## 🛠 Tecnologias e Ecossistema Técnico

### **Backend (C# / .NET)**
* **Framework Principal:** .NET 8 (Web API).
* **Persistência (ORM):** Entity Framework Core (EF Core).
* **Banco de Dados:** MySQL Server 8.0.
* **Documentação Interativa:** Swagger (OpenAPI) para mapeamento, teste e documentação de todos os endpoints das APIs.
* **Comunicação:** Integração via HttpClient para comunicação síncrona entre microsserviços.

### **Frontend (Angular)**
* **Framework:** Angular 21+.
* **Linguagem:** TypeScript.
* **Gerenciamento Assíncrono:** RxJS (Reactive Extensions para JavaScript).
* **UI/UX:** Angular Material Design.
* **Estilização:** CSS3 e ícones via Material Icons.

---

## 🧠 Detalhamento Técnico

### 1. Arquitetura de Microsserviços e Integração
O sistema foi projetado sob o padrão de microsserviços, garantindo o desacoplamento e a separação de responsabilidades:
* **Serviço de Estoque:** Gerencia o cadastro de produtos, códigos, descrições e o controle de saldo físico em tempo real.
* **Serviço de Faturamento:** Gerencia o cabeçalho das notas fiscais e seus itens vinculados, sendo o serviço principal de negócio.
* **Integração:** O serviço de faturamento consome o serviço de estoque via requisições HTTP REST. Implementei uma lógica de **Reserva de Estoque** no momento da criação da nota (status "Aberta"), onde o sistema já deduz o saldo para garantir que os itens selecionados não sejam utilizados por outras notas enquanto esta estiver em edição.

### 2. Persistência de Dados e MySQL
* **Conexão Real:** O projeto utiliza o **MySQL Server** para garantir que todos os cadastros de produtos e notas sejam persistidos fisicamente, atendendo aos requisitos obrigatórios.
* **Migrations (Code First):** Toda a estrutura de tabelas, chaves primárias, chaves estrangeiras (`Foreign Keys`) e relacionamentos foi modelada em C# e gerada no banco de dados via *Entity Framework Migrations*.
* **Uso de LINQ (Language Integrated Query):** O LINQ foi aplicado extensivamente para:
    * Consultas complexas com filtros e projeções de dados (`.Select()`).
    * Ordenação sequencial decrescente para garantir a numeração automática das notas.
    * Carregamento adiantado (`.Include()`) para buscar os itens das notas em uma única consulta, evitando gargalos de performance.

### 3. Padrão DTO e Segurança
Utilizei o padrão **DTO (Data Transfer Object)** em ambos os microsserviços. Isso garante que as entidades do banco de dados não sejam expostas diretamente para o frontend, permitindo validar e filtrar exatamente o que é enviado e recebido, além de facilitar a manutenção e a evolução dos contratos da API.

### 4. Frontend Angular: Ciclos de Vida e RxJS
Para atender às exigências técnicas do desafio, o frontend foi estruturado da seguinte forma:
* **Ciclos de Vida Utilizados:**
    * `ngOnInit`: Utilizado para inicializar o carregamento das tabelas de produtos e notas fiscais assim que o componente é renderizado.
    * `ngOnDestroy`: Implementado para gerenciar o encerramento de inscrições (`unsubscriptions`) de Observables, evitando vazamentos de memória (Memory Leaks).
* **Uso de RxJS:**
    * A biblioteca foi essencial para lidar com a natureza assíncrona das chamadas de API.
    * Utilizei `Observables` para o fluxo de dados e operadores como `map` para tratar as respostas e `catchError` para capturar falhas de rede de forma reativa.
* **Componentes Visuais:**
    * Utilizei o **Angular Material** para garantir uma interface profissional e intuitiva. Destaque para o uso de `MatTable` (listagens), `MatDialog` (modais), `MatSnackBar` (notificações de sucesso/erro) e indicadores de progresso visual durante a emissão da nota.

### 5. Resiliência, Erros e Falhas
* **Tratamento de Exceções no Backend:** Implementei um middleware de tratamento global de erros no .NET, garantindo que qualquer falha retorne um JSON padronizado com mensagens amigáveis e o status HTTP correto (400 para erros de negócio, 404 para não encontrado ou 500 para falhas críticas).
* **Tratamento de Falhas (Recuperação):** O sistema foi projetado para lidar com quedas de serviço. Caso o microsserviço de Estoque esteja offline, o serviço de Faturamento intercepta a falha e o frontend notifica o usuário via interface, permitindo que a operação seja retomada assim que a conexão for restabelecida.
* **Tratamento de Concorrência (Opcional):** Apliquei travas de validação no backend para garantir que, se dois usuários tentarem reservar o último item de um produto simultaneamente, apenas um consiga realizar a operação, mantendo a integridade do saldo e a consistência do banco de dados.

---

## 🚀 Como Executar o Projeto

### Pré-requisitos
* MySQL Server instalado e rodando.
* SDK .NET 8 instalado.
* Node.js e Angular CLI instalados.

### Configuração do Banco de Dados
1. Ajuste a *Connection String* nos arquivos `appsettings.json` de cada API (Estoque e Faturamento) com suas credenciais locais do MySQL.
2. Na pasta raiz de cada projeto API, execute o comando para criar as tabelas automaticamente:
   ```bash
   dotnet ef database update

### Inicialização
1. **API de Estoque:** Navegue até `backend/estoque.API` e execute `dotnet run --launch-profile http`. (Porta padrão: 5000)
2. **API de Faturamento:** Navegue até `backend/faturamento.API` e execute `dotnet run --launch-profile http`. (Porta padrão: 5001)
3. **Frontend:** Navegue até a pasta `frontend`, execute `npm install` e depois `ng serve`.
4. **Acesso:** Abra o navegador em `http://localhost:4200`.