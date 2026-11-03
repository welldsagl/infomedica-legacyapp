import { keepPreviousData, useQuery } from '@tanstack/react-query'
import { runPromise } from '@/utils/http'
import { getClienti } from '../utils/clientiApi'
import { clientiKeys } from '../utils/queryKeys'

export const useClienti = (page: number) => {
  const query = useQuery({
    queryKey: clientiKeys.list(page),
    queryFn: ({ signal }) => runPromise(getClienti(page, signal)),
    placeholderData: keepPreviousData, // niente "salto" della tabella cambiando pagina
    throwOnError: true, // gli errori vanno all'ErrorBoundary della pagina
  })

  const totalCount = query.data?.totalCount ?? 0
  const pageSize = query.data?.pageSize ?? 1

  return {
    clienti: query.data?.items ?? [],
    totalCount,
    totalPages: Math.max(1, Math.ceil(totalCount / pageSize)),
    isPending: query.isPending,
    isFetching: query.isFetching,
  }
}
