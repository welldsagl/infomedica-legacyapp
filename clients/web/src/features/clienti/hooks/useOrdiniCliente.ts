import { useQuery } from '@tanstack/react-query'
import { runPromise } from '@/utils/http'
import { getOrdiniCliente } from '../utils/clientiApi'
import { clientiKeys } from '../utils/queryKeys'

export const useOrdiniCliente = (clienteId: number) => {
  const query = useQuery({
    queryKey: clientiKeys.ordini(clienteId),
    queryFn: ({ signal }) => runPromise(getOrdiniCliente(clienteId, signal)),
    throwOnError: true,
  })

  const ordini = query.data ?? []
  const totale = ordini.reduce((sum, o) => sum + o.importo, 0)

  return { ordini, totale, isPending: query.isPending }
}
