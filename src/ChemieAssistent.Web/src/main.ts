import './style.css';
import { calcular } from './api';
import { mostrarErro, mostrarRelatorio } from './relatorio';
import type { EntradaMassa } from './tipos';

const campo = document.getElementById('equacao') as HTMLTextAreaElement;
const botao = document.getElementById('calcular') as HTMLButtonElement;

// ---------- Atalhos (+, ∗, →, ⇌) ----------
function inserirNoCursor(texto: string): void {
  campo.setRangeText(texto, campo.selectionStart, campo.selectionEnd, 'end');
  campo.focus();
}

const atalhos = document.querySelectorAll<HTMLButtonElement>('.atalho');

for (let i = 0; i < atalhos.length; i++) {
  const atalho = atalhos[i];
  atalho.addEventListener('click', () => {
    inserirNoCursor(atalho.dataset.texto ?? '');
  });
}

// ---------- Calcular ----------
async function aplicar(equacao: string, massas: EntradaMassa[]): Promise<void> {
  const relatorio = await calcular(equacao, massas);
  mostrarRelatorio(relatorio, massas, (novas) => aplicar(equacao, novas));
}

async function enviar(): Promise<void> {
  const equacao = campo.value.trim();
  if (equacao === '') return;

  botao.disabled = true;
  try {
    await aplicar(equacao, []);
  } catch (e) {
    mostrarErro(e instanceof Error ? e.message : 'Erro inesperado.');
  } finally {
    botao.disabled = false;
  }
}

botao.addEventListener('click', enviar);

campo.addEventListener('keydown', (evento) => {
  if (evento.key === 'Enter' && !evento.shiftKey) {
    evento.preventDefault();
    enviar();
  }
});