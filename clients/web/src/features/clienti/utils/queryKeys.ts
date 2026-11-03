// Chiavi gerarchiche: invalidare ['clienti'] aggiorna liste, dettagli e ordini.
export const clientiKeys = {
  all: ['clienti'] as const,
  lists: () => [...clientiKeys.all, 'list'] as const,
  list: (page: number) => [...clientiKeys.lists(), page] as const,
  detail: (id: number) => [...clientiKeys.all, 'detail', id] as const,
  ordini: (id: number) => [...clientiKeys.all, 'ordini', id] as const,
}
