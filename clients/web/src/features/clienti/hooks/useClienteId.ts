import { useParams } from '@tanstack/react-router'
import { parseClienteId } from '../utils/mappers'

/** Legge e valida il parametro $id (vale per /clienti/$id e /clienti/$id/modifica). */
export const useClienteId = (): number => {
  const params = useParams({ strict: false })
  const id = parseClienteId(params.id)

  if (id === null) throw new Error('Identificativo cliente non valido')
  return id
}
