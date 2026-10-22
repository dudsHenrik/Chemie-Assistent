import type { EntradaMassa, ErroApi, Relatorio } from './tipos';

const URL_API = 'http://localhost:5018/api/calcular';

export async function calcular(
  equacao: string,
  massas: EntradaMassa[] = [],
): Promise<Relatorio> {
  const corpo = massas.length > 0 ? { equacao, massas } : { equacao };

  let resposta: Response;
  try {
    resposta = await fetch(URL_API, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(corpo),
    });
  } catch {
    throw new Error('Não foi possível conectar ao servidor.');
  }

  if (!resposta.ok) {
    const erro: ErroApi = await resposta.json();
    throw new Error(erro.detail ?? erro.title);
  }

  return resposta.json();
}