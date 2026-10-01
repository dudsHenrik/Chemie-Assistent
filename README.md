# Chemie Assistent

Calculadora estequiométrica acessada via web, combinando um banco de dados SQL Server (dados atômicos, configuração eletrônica e lógica de domínio químico) com uma aplicação backend/frontend.

## Status

🚧 Em desenvolvimento — v1 em construção (leitura e balanceamento de equações químicas).

## Visão do projeto

Permitir que alunos de química e profissionais do ramo informem uma equação química e obtenham o balanceamento, a proporção estequiométrica e os cálculos derivados, sem fazer isso manualmente.

## Roadmap

- **v1** — Leitura e balanceamento de equações químicas (simples e complexas)
- **v2** — Motor de nomenclatura de compostos inorgânicos
- **Backlog** — pH/pOH, equilíbrio de reação, termodinâmica

Detalhes completos dos requisitos em [`docs/DRS.md`](docs/DRS.md).

## Estrutura do repositório

```
/database   → scripts SQL, numerados por ordem de execução
/docs       → documento de requisitos (DRS) e demais documentação
/src        → código da aplicação (em breve)
```

## Como rodar o banco localmente

1. Tenha um SQL Server acessível (local ou instância de desenvolvimento).
2. Execute os scripts de `/database` **na ordem numérica**, já que há dependências de chave estrangeira entre eles:
   1. `Create_Atomo.sql`
   2. `Create_SubnivelEletronico.sql`
   3. `Create_ConfiguracaoEletronica.sql`
   4. `Create_vw_CamadaValencia.sql`
   5. `Create_vw_DistribuicaoEletronica.sql`

## Tecnologias

- **Banco de dados:** SQL Server
- **Aplicação:** C#, TypeScript, HTML, CSS

## Licença

Este projeto está sob a licença MIT — veja [`LICENSE`](LICENSE) para detalhes.
