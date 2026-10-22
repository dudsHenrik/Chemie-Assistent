export interface Divergencia {
  indice: number;
  composto: string;
  digitado: number;
  esperado: number;
}

export interface Composto {
  indice: number;
  formula: string;
  coeficiente: number;
  ehReagente: boolean;
  massaMolar: number;
}

export interface Quantidade {
  indice: number;
  formula: string;
  ehReagente: boolean;
  massaMolar: number;
  mols: number;
  massaGramas: number;
  excedenteMols: number | null;
  excedenteGramas: number | null;
}

export interface Limitante {
  indiceLimitante: number;
  linhas: Quantidade[];
}

export interface Relatorio {
  equacaoBalanceada: string;
  reversivel: boolean;
  situacao: string;
  divergencias: Divergencia[];
  proporcao: string;
  compostos: Composto[];
  quantidades: Quantidade[] | null;
  limitante: Limitante | null;
}

export interface ErroApi {
  title: string;
  detail?: string;
  codigo?: string;
}

export interface EntradaMassa {
  indiceComposto: number;
  valor: number;
  unidade: 'g' | 'kg';
}