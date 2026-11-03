import { requestJson, requestVoid } from '@/utils/http'
import { ClienteSchema, OrdiniSchema, PagedClientiSchema } from '../types'
import type { ClienteRequest } from '../types'

// Funzioni pure che descrivono le chiamate (Effect). Vengono eseguite solo negli hook.

export const PAGE_SIZE = 20

export const getClienti = (page: number, signal?: AbortSignal) =>
  requestJson(PagedClientiSchema, `/api/clienti?page=${page}&pageSize=${PAGE_SIZE}`, { signal })

export const getCliente = (id: number, signal?: AbortSignal) =>
  requestJson(ClienteSchema, `/api/clienti/${id}`, { signal })

export const getOrdiniCliente = (id: number, signal?: AbortSignal) =>
  requestJson(OrdiniSchema, `/api/clienti/${id}/ordini`, { signal })

export const createCliente = (request: ClienteRequest) =>
  requestJson(ClienteSchema, '/api/clienti', { method: 'POST', body: request })

export const updateCliente = (id: number, request: ClienteRequest) =>
  requestVoid(`/api/clienti/${id}`, { method: 'PUT', body: request })

export const deleteCliente = (id: number) => requestVoid(`/api/clienti/${id}`, { method: 'DELETE' })
