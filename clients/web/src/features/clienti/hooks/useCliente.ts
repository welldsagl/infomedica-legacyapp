import { useQuery } from '@tanstack/react-query'
import { runPromise } from '@/utils/http'
import { getCliente } from '../utils/clientiApi'
import { clientiKeys } from '../utils/queryKeys'

export const useCliente = (id: number) => {
  const query = useQuery({
    queryKey: clientiKeys.detail(id),
    queryFn: ({ signal }) => runPromise(getCliente(id, signal)),
    throwOnError: true,
  })

  return { cliente: query.data, isPending: query.isPending }
}
