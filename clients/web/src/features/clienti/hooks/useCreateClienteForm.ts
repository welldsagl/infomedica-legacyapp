import { useNavigate } from '@tanstack/react-router'
import { emptyClienteForm, toClienteRequest } from '../utils/mappers'
import { useClienteForm } from './useClienteForm'
import { useCreateCliente } from './useCreateCliente'
import type { SubmitEvent } from 'react'

export const useCreateClienteForm = () => {
  const navigate = useNavigate()
  const mutation = useCreateCliente()
  const { form, applyServerErrors } = useClienteForm(emptyClienteForm)

  // handleSubmit restituisce una Promise: la si consuma qui, il componente riceve un handler void
  const onSubmit = (event: SubmitEvent<HTMLFormElement>) => {
    void form.handleSubmit((values) => {
      mutation.mutate(toClienteRequest(values), { onError: applyServerErrors })
    })(event)
  }

  const cancel = () => {
    void navigate({ to: '/clienti', search: { page: 1 } })
  }

  return { form, onSubmit, cancel, isPending: mutation.isPending }
}
