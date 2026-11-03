import { useNavigate, useSearch } from '@tanstack/react-router'

/** Pagina corrente della lista, sincronizzata con l'URL (?page=N): link condivisibili e tasto "indietro". */
export const useClientiPage = () => {
  const { page } = useSearch({ from: '/clienti' })
  const navigate = useNavigate({ from: '/clienti' })

  const setPage = (next: number) => {
    void navigate({ search: { page: next } })
  }

  return { page, setPage }
}
