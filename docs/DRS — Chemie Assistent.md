# Documento de Requisitos de Software (DRS)

## Projeto: Chemie Assistent

---

## 1. Visão Geral

O **Chemie Assistent** é uma calculadora estequiométrica acessada via web, combinando um banco de dados SQL Server (dados atômicos, configuração eletrônica e lógica de domínio químico) com uma aplicação backend/frontend.

**Objetivo duplo do projeto:**

- Produzir uma aplicação funcional e apresentável como peça de portfólio.
- Ganhar experiência prática com SQL Server e desenvolvimento web, aplicando conceitos de Engenharia de Software (levantamento de requisitos, modelagem, processo).

**Stack:**

- **Banco de dados:** SQL Server
- **Aplicação:** C#, TypeScript, HTML, CSS

---

## 2. Usuários-alvo

- Alunos de química
- Profissionais do ramo

---

## 3. Escopo do Produto (Roadmap)

| Versão | Escopo |
| --- | --- |
| **v1** | Leitura e balanceamento de equações químicas — simples e complexas |
| **v2** | Motor de nomenclatura de compostos inorgânicos |
| **Backlog** | Cálculo de pH/pOH; equilíbrio de reação; termodinâmica |

**Nota de processo:** o escopo da v1 foi deliberadamente reduzido durante o levantamento de requisitos — nomenclatura e composição de íons foram identificadas como não-bloqueadoras da funcionalidade central (balanceamento) e movidas para v2, para evitar scope creep.

---

## 4. Requisitos Funcionais

| ID | Requisito | Descrição |
| --- | --- | --- |
| RF01 | Ler uma equação química informada pelo usuário | O sistema deve aceitar como entrada uma equação química completa, em notação padrão, incluindo compostos hidratados (ex: `CuSO4·5H2O`) e íons complexos com notação de carga (ex: `[Fe(CN)6]^4-`). |
| RF02 | Balancear a equação | A partir da leitura (RF01), calcular os coeficientes estequiométricos que balanceiam massa (número de átomos de cada elemento) e, quando aplicável, carga, em ambos os lados da equação. |
| RF03 | Calcular a proporção estequiométrica | Com a equação balanceada, apresentar a proporção molar entre reagentes e produtos. |
| RF04 | Aplicar a proporção a uma quantidade informada | O usuário informa a quantidade de um participante da reação; o sistema calcula as quantidades correspondentes dos demais. |
| RF05 | Calcular massa molar | Massa molar de cada composto envolvido, a partir das massas atômicas de `Atomo`. |
| RF06 *(v2)* | Nomear compostos inorgânicos | Sais, ácidos, bases e óxidos nomeados automaticamente com base em regras internas de nomenclatura. |
| RF07 *(v2)* | Compor Composto a partir de Cátion + Ânion | Dada uma fórmula, decompor em cátion + ânion (ou montar a fórmula a partir da escolha de ambos), respeitando balanceamento de carga. |

**Dependência de dado (v1):** RF01–RF05 dependem primariamente da tabela `Atomo` já existente (símbolo, massa atômica). Não dependem da modelagem de `Ion`/`Cátion`/`Ânion`, que é pré-requisito de RF06/RF07 (v2).

---

## 5. Requisitos Não Funcionais

| ID | Categoria | Descrição |
| --- | --- | --- |
| RNF01 | Precisão numérica | Cálculos de massa molar e proporções devem manter casas decimais consistentes, sem acúmulo de erro de arredondamento. |
| RNF02 | Validação de entrada | Entradas inválidas devem ser rejeitadas com mensagem clara, nunca produzindo resultado silenciosamente incorreto. |
| RNF03 | Internacionalização | Interface disponível em português (pt-BR) e inglês (en-US). |
| RNF04 | Portabilidade | Acesso via navegador, sem instalação. |
| RNF05 | Manutenibilidade | Schema e regras documentados o suficiente para estender a v2 sem retrabalho estrutural. |
| RNF06 | Segurança | Sistema público, sem gravação de registros (sem autenticação de usuário nem persistência de dados pessoais). |

---

## 6. Backlog (sem versão definida)

- **Cálculo de pH/pOH** — depende de concentração e, para ácidos/bases fracos, de constantes de dissociação (Ka/Kb) ainda não modeladas.
- **Equilíbrio de reação** — ainda sem levantamento detalhado.
- **Reação termodinâmica** — depende de dados de entalpia/entropia/energia livre de formação (fonte sugerida: NIST Chemistry WebBook), ainda não modelados.

---

## 7. Decisões de Modelagem já Tomadas

- Chave natural (`Simbolo`) preferida a chave surrogate, quando estável e única.
- Configuração eletrônica normalizada em duas tabelas (`SubnivelEletronico` + `ConfiguracaoEletronica`), não em formato wide.
- Formatação de exibição (superscript/subscript) feita em *view* (via `TRANSLATE`), nunca armazenada junto ao dado bruto.
- Modelagem de íon prevista para v2: `Ion` (Formula, Carga, Nome, Tipo) como supertipo; `IonMonoatomico` como subtipo ligado a `Atomo`. Cátion/Ânion seriam views filtradas por sinal de `Carga`.
- Fonte de referência para íons comuns: materiais didáticos de química e tabelas universitárias — conjunto fechado de \~45-50 íons cobre sais/ácidos/bases/óxidos.
- `Create_vw_AnaliseCations.sql`: excluído do projeto.

---

## 8. Decisões Pendentes

*(nenhuma pendência no momento)*

---

## 9. Processo de Trabalho

- Abordagem adotada: levantamento de requisitos → modelagem → implementação, evitando o padrão anterior de "banco primeiro, requisitos depois".
- Backlog no GitHub: uma Issue por RF, com milestones "v1" e "v2".