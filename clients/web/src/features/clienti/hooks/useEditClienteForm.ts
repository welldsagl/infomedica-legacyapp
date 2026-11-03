import { useNavigate } from '@tanstack/react-router'
import { toClienteFormValues, toClienteRequest } from '../utils/mappers'
import { useClienteForm } from './useClienteForm'
import { useUpdateCliente } from './useUpdateCliente'
import type { Cliente } from '../types'
import type { SubmitEvent } from 'react'

export const useEditClienteForm = (cliente: Cliente) => {
  const navigate = useNavigate()
  const mutation = useUpdateCliente(cliente.id)
  const { form, applyServerErrors } = useClienteForm(toClienteFormValues(cliente))

  // handleSubmit restituisce una Promise: la si consuma qui, il componente riceve un handler void
  const onSubmit = (event: SubmitEvent<HTMLFormElement>) => {
    void form.handleSubmit((values) => {
      mutation.mutate(toClienteRequest(values), { onError: applyServerErrors })
    })(event)
  }

  const cancel = () => {
    void navigate({ to: '/clienti/$id', params: { id: String(cliente.id) } })
  }

  return { form, onSubmit, cancel, isPending: mutation.isPending }
}
