import { useForm } from 'react-hook-form'
import { effectTsResolver } from '@hookform/resolvers/effect-ts'
import { toFieldErrors } from '@/utils/apiErrors'
import { ClienteFormSchema } from '../types'
import type { ClienteFormOutput, ClienteFormValues } from '../types'

const FORM_FIELDS: ReadonlyArray<keyof ClienteFormValues> = [
  'ragioneSociale',
  'partitaIva',
  'email',
  'attivo',
]

const isFormField = (name: string): name is keyof ClienteFormValues =>
  (FORM_FIELDS as ReadonlyArray<string>).includes(name)

/** Base comune a creazione e modifica: validazione client + errori di validazione dal server. */
export const useClienteForm = (defaultValues: ClienteFormValues) => {
  const form = useForm<ClienteFormValues, unknown, ClienteFormOutput>({
    resolver: effectTsResolver(ClienteFormSchema),
    defaultValues,
  })

  const applyServerErrors = (error: unknown) => {
    toFieldErrors(error).forEach(([field, message]) => {
      if (isFormField(field)) form.setError(field, { type: 'server', message })
      else form.setError('root.server', { type: 'server', message })
    })
  }

  return { form, applyServerErrors }
}
