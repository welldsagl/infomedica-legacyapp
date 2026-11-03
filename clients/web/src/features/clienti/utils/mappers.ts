import type {
  Cliente,
  ClienteFormOutput,
  ClienteFormValues,
  ClienteRequest,
  ClientiSearch,
} from '../types'

const emptyToNull = (value: string): string | null => (value === '' ? null : value)

/** Valori del form -> corpo della richiesta (stringhe vuote -> null, come si aspetta l'API). */
export const toClienteRequest = (values: ClienteFormOutput): ClienteRequest => ({
  ragioneSociale: values.ragioneSociale,
  partitaIva: emptyToNull(values.partitaIva),
  email: emptyToNull(values.email),
  attivo: values.attivo,
})

export const emptyClienteForm: ClienteFormValues = {
  ragioneSociale: '',
  partitaIva: '',
  email: '',
  attivo: true,
}

export const toClienteFormValues = (cliente: Cliente): ClienteFormValues => ({
  ragioneSociale: cliente.ragioneSociale,
  partitaIva: cliente.partitaIva ?? '',
  email: cliente.email ?? '',
  attivo: cliente.attivo,
})

/** Validazione dei search params della route /clienti (?page=N). */
export const validateClientiSearch = (search: Record<string, unknown>): ClientiSearch => {
  const page = Number(search.page)
  return { page: Number.isInteger(page) && page > 0 ? page : 1 }
}

/** Parametro di route $id -> intero positivo, oppure null. */
export const parseClienteId = (raw: string | undefined): number | null => {
  const id = Number(raw)
  return Number.isInteger(id) && id > 0 ? id : null
}
