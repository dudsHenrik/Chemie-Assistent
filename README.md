# Chemie Assistent

Calculadora estequiométrica acessada via web, combinando um banco de dados SQL Server (dados atômicos e configuração eletrônica) com uma aplicação backend/frontend.

## Status

✅ **v1 concluída.** 
Obs.: A v1 é a **única versão planejada**; todo o restante está no backlog.

## Visão do projeto

Permitir que alunos de química e profissionais do ramo informem uma equação química e obtenham o balanceamento, a proporção estequiométrica e os cálculos derivados, sem fazer isso manualmente.

## Funcionalidades da v1

| ID | Funcionalidade | Situação |
| --- | --- | --- |
| RF01 | Ler a equação digitada em um único campo de texto (parênteses, colchetes e hidratos com `*`) | ✅ Concluído |
| RF02 | Balancear a equação (massa) | ✅ Concluído |
| RF03 | Calcular a proporção estequiométrica | ✅ Concluído |
| RF04 | Aplicar a proporção à massa informada de um reagente ou produto | ✅ Concluído |
| RF05 | Calcular a massa molar de cada composto | ✅ Concluído |
| RF06 | Identificar o reagente limitante e o excedente dos demais | ✅ Concluído |

Além dos RFs, o sistema compara os coeficientes digitados pelo usuário com os calculados e exibe no relatório as divergências, útil para estudantes.

### Como digitar a equação

- `+` separa os compostos; `->` ou `<->` (ou os botões → e ⇌) separam reagentes de produtos.
- Aceitos: compostos simples (`NaCl`), parênteses (`Ca(OH)2`), parênteses aninhados (`Al2(SO4)3`), colchetes (`K3[PW12O40]`) e hidratos com `*` (`CuSO4*5H2O`).
- O índice `1` é implícito: escreva `Fe`, não `Fe1`.
- Maiúsculas e minúsculas são diferenciadas (`CO` ≠ `Co`).
- Limite de 500 caracteres por equação.
- Íons e polímeros **não** são aceitos na v1.

Exemplo: `C3H8 + O2 -> CO2 + H2O` → `C₃H₈ + 5O₂ → 3CO₂ + 4H₂O`, proporção `1 : 5 : 3 : 4`.

## Backlog

Nada abaixo será entregue na v1.

- Nomenclatura de compostos inorgânicos
- Composição de compostos a partir de Cátion + Ânion
- Operações com íons (fórmulas com carga e balanceamento de carga)
- Cálculo de pH/pOH
- Equilíbrio de reação
- Termodinâmica

Detalhes completos dos requisitos em [`docs/DRS.md`](docs/DRS.md).

## Arquitetura

```
Front (TypeScript + Vite)  →  API (ASP.NET Core)  →  Domain (regras)  →  Infrastructure (SQL Server)
```

- **Domain:** biblioteca C# sem dependências externas, com toda a regra de negócio (leitura, balanceamento, massa molar, estequiometria).
- **Infrastructure:** acesso ao SQL Server (`Atomo`).
- **Api:** endpoint `POST /api/calcular`, que recebe a equação (e, opcionalmente, massas) e devolve o relatório em JSON. Erros do usuário voltam como HTTP 400 com um código de erro.
- **Front:** campo único de digitação e relatório na parte inferior da tela.

O sistema é público e não grava registros: não há autenticação nem persistência de dados do usuário.

## Estrutura do repositório

```
/database   → scripts SQL, numerados por ordem de execução
/docs       → documento de requisitos (DRS) e demais documentação
/src        → código da aplicação
   ChemieAssistent.Domain          → biblioteca de domínio (C#)
   ChemieAssistent.Infrastructure  → acesso ao banco (SQL Server)
   ChemieAssistent.Api             → API web
   <pasta-do-front>                → interface web (TypeScript, HTML, CSS)
/tests      → testes automatizados (xUnit)
```

## Como rodar

### Pré-requisitos

- SDK do .NET 10
- Node.js 20.19+ ou 22.12+
- SQL Server acessível (a configuração padrão usa o LocalDB)

### 1. Banco de dados

Execute os scripts de `/database` **na ordem numérica**, já que há dependências de chave estrangeira entre eles:

1. `01_Create_Atomo.sql`
2. `02_Create_SubnivelEletronico.sql`
3. `03_Create_ConfiguracaoEletronica.sql`
4. `04_Create_vw_CamadaValencia.sql`
5. `05_Create_vw_DistribuicaoEletronica.sql`

Crie antes um banco chamado `ChemieAssistent`. Os cálculos da v1 usam apenas a tabela `Atomo`; as demais guardam a configuração eletrônica.

### 2. API

A connection string fica em `appsettings.json` (chave `ConnectionStrings:ChemieAssistent`). Ajuste se não usar o LocalDB. Depois:

```
cd src/ChemieAssistent.Api
dotnet run
```

A API sobe em `http://localhost:5018`. O arquivo `ChemieAssistent.Api.http` traz exemplos de requisição.

### 3. Front-end

```
cd src/ChemieAssistent.Web
npm install
npm run dev
```

Acesse `http://localhost:5173` (origem liberada no CORS da API, em `Cors:Origens`). O endereço da API está definido em `src/api.ts`.

## Como rodar os testes

Na raiz do repositório:

```
dotnet test
```

Os testes do domínio rodam sem dependências externas. Os testes de infraestrutura (`Categoria = Integracao`) **precisam do banco criado e populado**; para rodar só os do domínio:

```
dotnet test --filter "Categoria!=Integracao"
```

## Tecnologias

- **Banco de dados:** SQL Server
- **Backend:** C# (.NET 10), ASP.NET Core
- **Frontend:** TypeScript, HTML, CSS (Vite)
- **Testes:** xUnit

## Licença

Este projeto está sob a licença MIT — veja [`LICENSE`](LICENSE) para detalhes.