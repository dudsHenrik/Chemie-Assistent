# Chemie Assistent

Calculadora estequiométrica acessada via web, combinando um banco de dados SQL Server (dados atômicos e configuração eletrônica) com uma aplicação backend/frontend.

## Status

🚧 Em desenvolvimento — v1 em construção. A v1 é a **única versão planejada**; todo o restante está no backlog.

## Visão do projeto

Permitir que alunos de química e profissionais do ramo informem uma equação química e obtenham o balanceamento, a proporção estequiométrica e os cálculos derivados, sem fazer isso manualmente.

## Escopo da v1

| ID | Funcionalidade | Situação |
| --- | --- | --- |
| RF01 | Ler a equação digitada em um único campo de texto (parênteses, colchetes e hidratos com `*`) | ✅ Domínio |
| RF02 | Balancear a equação (massa) | ✅ Domínio |
| RF03 | Calcular a proporção estequiométrica | ✅ Domínio |
| RF04 | Aplicar a proporção à massa informada de um reagente ou produto | ✅ Domínio |
| RF05 | Calcular a massa molar de cada composto | ✅ Domínio |
| RF06 | Identificar o reagente limitante | ⏳ Pendente |

"Domínio" indica que a regra já existe na biblioteca `ChemieAssistent.Domain`; o endpoint e a interface web ainda estão por vir.

Íons e polímeros **não** são aceitos na v1.

## Backlog

Nada abaixo será entregue na v1.

- Nomenclatura de compostos inorgânicos
- Composição de compostos a partir de Cátion + Ânion
- Operações com íons (fórmulas com carga e balanceamento de carga)
- Cálculo de pH/pOH
- Equilíbrio de reação
- Termodinâmica

Detalhes completos dos requisitos em [`docs/DRS.md`](docs/DRS.md).

## Estrutura do repositório

```
/database   → scripts SQL, numerados por ordem de execução
/docs       → documento de requisitos (DRS) e demais documentação
/src        → código da aplicação (biblioteca de domínio em C#)
/tests      → testes automatizados do domínio (xUnit)
```

## Como rodar o banco localmente

1. Tenha um SQL Server acessível (local ou instância de desenvolvimento).
2. Execute os scripts de `/database` **na ordem numérica**, já que há dependências de chave estrangeira entre eles:
   1. `01_Create_Atomo.sql`
   2. `02_Create_SubnivelEletronico.sql`
   3. `03_Create_ConfiguracaoEletronica.sql`
   4. `04_Create_vw_CamadaValencia.sql`
   5. `05_Create_vw_DistribuicaoEletronica.sql`

## Como rodar os testes

Com o SDK do .NET 10 instalado, na raiz do repositório:

```
dotnet test
```

## Tecnologias

- **Banco de dados:** SQL Server
- **Aplicação:** C#, TypeScript, HTML, CSS

## Licença

Este projeto está sob a licença MIT — veja [`LICENSE`](LICENSE) para detalhes.
