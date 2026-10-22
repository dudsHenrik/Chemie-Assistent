import type { EntradaMassa, Divergencia, Quantidade, Relatorio } from './tipos';

type AoCalcular = (massas: EntradaMassa[]) => Promise<void>;

// Elementos e formatadores
const area = document.getElementById('relatorio') as HTMLElement;
const numero = new Intl.NumberFormat('pt-BR', { maximumFractionDigits: 4 });
const SUBSCRITOS = '₀₁₂₃₄₅₆₇₈₉';

// Exibição do relatório
export function mostrarRelatorio(
  r: Relatorio,
  massasPrevias: EntradaMassa[],
  aoCalcular: AoCalcular,
): void {
  const partes: HTMLElement[] = [criar('h2', formatarFormula(r.equacaoBalanceada))];

  const aviso = avisoDivergencias(r);
  if (aviso !== null) partes.push(aviso);

  partes.push(secao('Proporção', criar('p', formatarFormula(r.proporcao))));
  partes.push(secao('Compostos', tabelaCompostos(r)));

  partes.push(secao(
    'Informar massas',
    criar('p',
      'Informe a massa de um composto para calcular os demais, ' +
      'ou de dois ou mais reagentes para descobrir o limitante.',
      'ajuda'),
    formularioMassas(r, massasPrevias, aoCalcular),
  ));

  if (r.quantidades !== null) {
    partes.push(secao('Quantidades', tabelaQuantidades(r.quantidades)));
  }

  const limitante = secaoLimitante(r);
  if (limitante !== null) partes.push(limitante);

  area.className = '';
  area.replaceChildren(...partes);
  area.hidden = false;
}

export function mostrarErro(mensagem: string): void {
  area.className = 'erro';
  area.replaceChildren(criar('p', mensagem));
  area.hidden = false;
}

// Formatação e criação de elementos
function formatarFormula(texto: string): string {
  return texto
    .replace(/(?<=[A-Za-z)\]])\d+/g, (digitos) => {
      let resultado = '';
      for (let i = 0; i < digitos.length; i++) {
        resultado += SUBSCRITOS[Number(digitos[i])];
      }
      return resultado;
    })
    .replace(/\*/g, '·');
}

function criar(tag: string, texto = '', classe = ''): HTMLElement {
  const el = document.createElement(tag);
  el.textContent = texto;
  if (classe !== '') el.className = classe;
  return el;
}

function secao(titulo: string, ...conteudo: HTMLElement[]): HTMLElement {
  const div = criar('section');
  div.appendChild(criar('h3', titulo));
  for (let i = 0; i < conteudo.length; i++) {
    div.appendChild(conteudo[i]);
  }
  return div;
}

// Construção de tabelas
function tabela(
  cabecalhos: string[],
  linhas: string[][],
  destacar = -1,
): HTMLElement {
  const t = document.createElement('table');

  const trCabecalho = t.createTHead().insertRow();
  for (let i = 0; i < cabecalhos.length; i++) {
    trCabecalho.appendChild(criar('th', cabecalhos[i]));
  }

  const corpo = t.createTBody();
  for (let i = 0; i < linhas.length; i++) {
    const tr = corpo.insertRow();
    if (i === destacar) tr.className = 'limitante';

    for (let j = 0; j < linhas[i].length; j++) {
      tr.insertCell().textContent = linhas[i][j];
    }
  }

  const envolta = criar('div', '', 'tabela');
  envolta.appendChild(t);
  return envolta;
}

// Divergências de coeficientes
function descreverDivergencia(d: Divergencia): string {
  const digitou = d.digitado === 1
    ? 'não digitou coeficiente'
    : `digitou ${d.digitado}`;

  const correto = d.esperado === 1
    ? 'nenhum coeficiente (o 1 é implícito)'
    : `${d.esperado}`;

  return `${formatarFormula(d.composto)}: você ${digitou}; o correto é ${correto}.`;
}

function avisoDivergencias(r: Relatorio): HTMLElement | null {
  if (r.divergencias.length === 0) return null;

  const caixa = criar('div', '', 'aviso');
  caixa.appendChild(
    criar('p', 'Alguns coeficientes digitados não batem com o balanceamento:'));

  const lista = criar('ul');
  for (let i = 0; i < r.divergencias.length; i++) {
    const d = r.divergencias[i];
    lista.appendChild(criar('li', descreverDivergencia(d)));
  }

  caixa.appendChild(lista);
  return caixa;
}

// Tabelas de compostos e quantidades
function tabelaCompostos(r: Relatorio): HTMLElement {
  const linhas: string[][] = [];
  for (let i = 0; i < r.compostos.length; i++) {
    const c = r.compostos[i];
    linhas.push([
      formatarFormula(c.formula),
      String(c.coeficiente),
      c.ehReagente ? 'Reagente' : 'Produto',
      numero.format(c.massaMolar),
    ]);
  }

  return tabela(
    ['Composto', 'Coeficiente', 'Papel', 'Massa molar (g/mol)'],
    linhas,
  );
}

function tabelaQuantidades(quantidades: Quantidade[]): HTMLElement {
  const linhas: string[][] = [];
  for (let i = 0; i < quantidades.length; i++) {
    const q = quantidades[i];
    linhas.push([
      formatarFormula(q.formula),
      numero.format(q.mols),
      numero.format(q.massaGramas),
    ]);
  }

  return tabela(['Composto', 'Mols', 'Massa (g)'], linhas);
}

// Reagente limitante
function secaoLimitante(r: Relatorio): HTMLElement | null {
  if (r.limitante === null) return null;

  const l = r.limitante;
  const linhas: string[][] = [];
  for (let i = 0; i < l.linhas.length; i++) {
    const q = l.linhas[i];
    linhas.push([
      formatarFormula(q.formula),
      numero.format(q.mols),
      numero.format(q.massaGramas),
      q.ehReagente ? numero.format(q.excedenteMols ?? 0) : '—',
      q.ehReagente ? numero.format(q.excedenteGramas ?? 0) : '—',
    ]);
  }

  const nome = formatarFormula(l.linhas[l.indiceLimitante].formula);
  return secao(
    'Reagente limitante',
    criar('p', `O reagente limitante é ${nome}.`),
    tabela(
      ['Composto', 'Mols', 'Massa (g)', 'Excedente (mol)', 'Excedente (g)'],
      linhas,
      l.indiceLimitante,
    ),
  );
}

// Formulário de entrada de massas
function formularioMassas(
  r: Relatorio,
  previas: EntradaMassa[],
  aoCalcular: AoCalcular,
): HTMLElement {
  const form = document.createElement('form');
  form.className = 'massas';

  const campos: HTMLInputElement[] = [];
  const unidades: HTMLSelectElement[] = [];

  for (let i = 0; i < r.compostos.length; i++) {
    const c = r.compostos[i];

    const nome = criar('div', formatarFormula(c.formula));
    nome.appendChild(criar('span', c.ehReagente ? 'reagente' : 'produto', 'papel'));

    const entrada = document.createElement('input');
    entrada.type = 'number';
    entrada.step = 'any';
    entrada.min = '0';
    entrada.placeholder = 'Massa';
    entrada.setAttribute('aria-label', `Massa de ${c.formula}`);

    const unidade = document.createElement('select');
    unidade.add(new Option('g', 'g'));
    unidade.add(new Option('kg', 'kg'));

    for (let k = 0; k < previas.length; k++) {
      if (previas[k].indiceComposto === c.indice) {
        entrada.value = String(previas[k].valor);
        unidade.value = previas[k].unidade;
      }
    }

    campos.push(entrada);
    unidades.push(unidade);

    const linha = criar('div', '', 'massa-linha');
    linha.append(nome, entrada, unidade);
    form.appendChild(linha);
  }

  const erro = criar('p', '', 'erro');
  erro.hidden = true;

  const botao = criar('button', 'Calcular quantidades') as HTMLButtonElement;
  botao.type = 'submit';

  form.append(erro, botao);

  // Processamento do envio
  form.addEventListener('submit', async (evento) => {
    evento.preventDefault();
    erro.hidden = true;

    const massas: EntradaMassa[] = [];
    for (let i = 0; i < campos.length; i++) {
      const valor = campos[i].valueAsNumber;
      if (Number.isNaN(valor)) continue;

      massas.push({
        indiceComposto: i,
        valor,
        unidade: unidades[i].value as 'g' | 'kg',
      });
    }

    if (massas.length == 0) {
      erro.textContent = 'Informe a massa de pelo menos um composto.';
      erro.hidden = false;
      return;
    }

    if (massas.length > 1) {
      for (let i = 0; i < massas.length; i++) {
        if (!r.compostos[massas[i].indiceComposto].ehReagente) {
          erro.textContent =
            'Para informar mais de uma massa, use apenas reagentes. ' +
            'Para partir de um produto, informe só a massa dele.';
          erro.hidden = false;
          return;
        }
      }
    }

    botao.disabled = true;
    try {
      await aoCalcular(massas);
    } catch (e) {
      erro.textContent = e instanceof Error ? e.message : 'Erro inesperado.';
      erro.hidden = false;
    } finally {
      botao.disabled = false;
    }
  });

  return form;
}