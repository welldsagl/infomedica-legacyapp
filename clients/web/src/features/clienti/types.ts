import { Schema } from 'effect'
import { PagedResultSchema } from '@/types/api'

// ---------------------------------------------------------------------------
// Contratti dell'API (specchio di LegacyApp.Api.Contracts), validati a runtime.
// ---------------------------------------------------------------------------

export const ClienteSchema = Schema.Struct({
  id: Schema.Number,
  ragioneSociale: Schema.String,
  partitaIva: Schema.NullOr(Schema.String),
  email: Schema.NullOr(Schema.String),
  dataInserimento: Schema.Date, // stringa ISO -> Date
  attivo: Schema.Boolean,
})

export const PagedClientiSchema = PagedResultSchema(ClienteSchema)

export const OrdineSchema = Schema.Struct({
  id: Schema.Number,
  clienteId: Schema.Number,
  dataOrdine: Schema.Date,
  importo: Schema.Number,
  note: Schema.NullOr(Schema.String),
})

export const OrdiniSchema = Schema.Array(OrdineSchema)

export type Cliente = Schema.Schema.Type<typeof ClienteSchema>
export type Ordine = Schema.Schema.Type<typeof OrdineSchema>

/** Corpo di POST/PUT (ClienteRequest lato .NET). */
export type ClienteRequest = {
  readonly ragioneSociale: string
  readonly partitaIva: string | null
  readonly email: string | null
  readonly attivo: boolean
}

// ---------------------------------------------------------------------------
// Form: stesse regole di validazione dei DataAnnotations lato API.
// ---------------------------------------------------------------------------

const EMAIL_PATTERN = /^[^\s@]+@[^\s@]+\.[^\s@]+$/

export const ClienteFormSchema = Schema.Struct({
  ragioneSociale: Schema.Trim.pipe(
    Schema.nonEmptyString({ message: () => 'La ragione sociale è obbligatoria' }),
    Schema.maxLength(255, { message: () => 'Massimo 255 caratteri' })
  ),
  partitaIva: Schema.Trim.pipe(
    Schema.filter((s) => s === '' || /^\d{11}$/.test(s) || 'La partita IVA deve avere 11 cifre')
  ),
  email: Schema.Trim.pipe(
    Schema.filter((s) => s === '' || EMAIL_PATTERN.test(s) || 'Indirizzo email non valido'),
    Schema.maxLength(255, { message: () => 'Massimo 255 caratteri' })
  ),
  attivo: Schema.Boolean,
})

/** Valori grezzi gestiti da React Hook Form (input del form). */
export type ClienteFormValues = Schema.Schema.Encoded<typeof ClienteFormSchema>

/** Valori dopo la validazione (trim applicato). */
export type ClienteFormOutput = Schema.Schema.Type<typeof ClienteFormSchema>

// ---------------------------------------------------------------------------
// Parametri di ricerca della lista (?page=N)
// ---------------------------------------------------------------------------

export type ClientiSearch = {
  readonly page: number
}
