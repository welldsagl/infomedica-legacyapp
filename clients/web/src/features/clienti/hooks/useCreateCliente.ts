import { useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from '@tanstack/react-router'
import { notifications } from '@mantine/notifications'
import { toFieldErrors, toUserMessage } from '@/utils/apiErrors'
import { runPromise } from '@/utils/http'
import { createCliente } from '../utils/clientiApi'
import { clientiKeys } from '../utils/queryKeys'
import type { ClienteRequest } from '../types'

export const useCreateCliente = () => {
  const queryClient = useQueryClient()
  const navigate = useNavigate()

  return useMutation({
    mutationFn: (request: ClienteRequest) => runPromise(createCliente(request)),
    onSuccess: async (cliente) => {
      await queryClient.invalidateQueries({ queryKey: clientiKeys.lists() })
      notifications.show({
        color: 'green',
        title: 'Cliente creato',
        message: cliente.ragioneSociale,
      })
      await navigate({ to: '/clienti/$id', params: { id: String(cliente.id) } })
    },
    onError: (error) => {
      // Gli errori di validazione finiscono sui campi del form; gli altri in una notifica
      if (toFieldErrors(error).length === 0) {
        notifications.show({
          color: 'red',
          title: 'Salvataggio non riuscito',
          message: toUserMessage(error),
        })
      }
    },
  })
}
