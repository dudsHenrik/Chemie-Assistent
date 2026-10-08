# Documento de Requisitos de Software (DRS)

## Projeto: Chemie Assistent

\---

## 1\. Visão Geral

O **Chemie Assistent** é uma calculadora estequiométrica acessada via web, combinando um banco de dados SQL Server (dados atômicos e configuração eletrônica) com uma aplicação backend/frontend.

**Objetivo duplo do projeto:**

* Produzir uma aplicação funcional e apresentável como peça de portfólio.
* Ganhar experiência prática com SQL Server e desenvolvimento web, aplicando conceitos de Engenharia de Software (levantamento de requisitos, modelagem, processo).

**Stack:**

* **Banco de dados:** SQL Server
* **Aplicação:** C#, TypeScript, HTML, CSS

\---

## 2\. Usuários-alvo

* Alunos de química
* Profissionais do ramo

\---

## 3\. Escopo do Produto

|Versão|Escopo|
|-|-|
|**v1**|Leitura e balanceamento de equações químicas (simples e complexas, sem íons), proporção estequiométrica, massa molar e reagente limitante|
|**Backlog**|Itens sem versão definida (ver seção 6)|

\---

## 4\. Requisitos Funcionais (v1)

|ID|Requisito|Descrição|
|-|-|-|
|RF01|Ler uma equação química informada pelo usuário|O sistema deve aceitar a equação completa em **um único campo de texto**, em notação padrão. O `+` separa os compostos e a seta (`->` ou `<->`, exibidas como → e ⇌) separa reagentes de produtos. São aceitos: compostos simples (`NaCl`), parênteses (`Ca(OH)2`), parênteses aninhados (`Al2(SO4)3`), colchetes (`K3\[PW12O40]`) e hidratos com `\*` (`CuSO4\*5H2O`). Íons e polímeros **não** são aceitos.|
|RF02|Balancear a equação|A partir da leitura (RF01), calcular os coeficientes estequiométricos que balanceiam a massa (número de átomos de cada elemento) em ambos os lados da equação.|
|RF03|Calcular a proporção estequiométrica|Com a equação balanceada, apresentar a proporção molar entre reagentes e produtos.|
|RF04|Aplicar a proporção a uma quantidade informada|O usuário informa a massa de **um** participante da reação (reagente ou produto); o sistema calcula as quantidades correspondentes dos demais.|
|RF05|Calcular massa molar|Massa molar de cada composto envolvido, a partir das massas atômicas de `Atomo`.|
|RF06|Identificar o reagente limitante|Quando o usuário informa a massa de mais de um reagente, o sistema identifica o reagente limitante e o excedente dos demais.|

**Dependência de dado:** RF01–RF06 dependem da tabela `Atomo` (símbolo, massa atômica).

### 4.1 Regras de leitura da fórmula

* **Coeficiente:** número antes do composto (`2H2O`). Campo separado da leitura da fórmula: o coeficiente digitado nunca altera a composição do composto. Ele multiplica o composto inteiro, hidrato incluído.
* **Índice interno:** número após o símbolo (`H2O`). **Índice externo:** número após o `)` (`Ca(OH)2`).
* **O 1 é implícito:** quantidade `1` escrita explicitamente (`Fe1O3`, `Fe1`) é erro (o correto é `FeO3`, `Fe`). Quantidade `0` também é erro.
* Maiúsculas e minúsculas são diferenciadas (`CO` ≠ `Co`).

### 4.2 Fluxo de processamento

1. Ler os compostos (reagentes e produtos).
2. Verificar divergência de elementos entre reagentes e produtos.
3. Balancear o número de mols.
4. Verificar se os coeficientes informados pelo usuário estão corretos.

### 4.3 Interface

* A equação é digitada em um único campo de texto.
* O cálculo só roda ao clicar em **Calcular** via endpoint.
* Os resultados aparecem em um relatório na parte inferior da tela, pensado para estudantes.
* Erros de preenchimento do usuário (ex.: coeficientes incorretos) são exibidos no relatório.

\---

## 5\. Requisitos Não Funcionais

|ID|Categoria|Descrição|
|-|-|-|
|RNF01|Precisão numérica|Massa atômica (u) com 6 casas decimais; massa molar (g/mol) com 4 casas, pois balanças analíticas não dão confiança além da 4ª casa. Sem acúmulo de erro de arredondamento.|
|RNF02|Validação de entrada|Entradas inválidas devem ser rejeitadas com mensagem clara, nunca produzindo resultado silenciosamente incorreto.|
|RNF03|Idioma|Interface somente em português (pt-BR). A tradução para inglês (en-US) foi abolida em 02/10/2026.|
|RNF04|Portabilidade|Acesso via navegador, sem instalação.|
|RNF05|Manutenibilidade|Schema e regras documentados o suficiente para que o backlog possa ser retomado sem retrabalho estrutural.|
|RNF06|Segurança|Sistema público, sem gravação de registros (sem autenticação de usuário nem persistência de dados pessoais).|
|RNF07|Conservação de massa|A soma das massas dos reagentes deve ser igual à soma das massas dos produtos em todo resultado calculado.|

\---

## 6\. Backlog (sem versão definida)

Nada abaixo será entregue na v1.

|ID|Item|Observações|
|-|-|-|
|BL01|Nomear compostos inorgânicos|Sais, ácidos, bases e óxidos nomeados automaticamente. Se retomado, começar por óxidos e ácidos binários.|
|BL02|Compor composto a partir de Cátion + Ânion|Decompor uma fórmula em cátion + ânion (ou montar a fórmula a partir de ambos), respeitando o balanço de carga.|
|BL03|Operações com íons|Leitura de fórmulas com carga (ex.: `\[Fe(CN)6]^4-`, `H+`, `OH-`) e balanceamento de carga na equação.|
|BL04|Cálculo de pH/pOH|Depende de concentração e, para ácidos/bases fracos, de constantes Ka/Kb ainda não modeladas.|
|BL05|Equilíbrio de reação|Sem levantamento detalhado.|
|BL06|Reação termodinâmica|Depende de dados de entalpia/entropia/energia livre de formação (fonte sugerida: NIST Chemistry WebBook), ainda não modelados.|

\---

## 7\. Decisões de Modelagem

* Chave natural (`Simbolo`) preferida a chave surrogate, quando estável e única.
* Configuração eletrônica normalizada em duas tabelas (`SubnivelEletronico` + `ConfiguracaoEletronica`), não em formato wide.
* Formatação de exibição (superscript/subscript) feita em *view* (via `TRANSLATE`), nunca armazenada junto ao dado bruto.
* `Create\_vw\_AnaliseCations.sql`: excluído do projeto.
* **Previsto para o backlog (BL01–BL03):** `Ion` (Formula, Carga, Nome, Tipo) como supertipo; `IonMonoatomico` como subtipo ligado a `Atomo`; Cátion/Ânion como views filtradas pelo sinal de `Carga`, com coluna Nox. Referência: conjunto fechado de \~45–50 íons de materiais didáticos e tabelas universitárias.

\---

## 8\. Processo de Trabalho

* Abordagem adotada: levantamento de requisitos → modelagem → implementação, evitando o padrão anterior de "banco primeiro, requisitos depois".
* Backlog no GitHub: uma Issue por RF, todas no milestone "v1". Os itens BL01–BL06 ficam em Issues sem milestone.

