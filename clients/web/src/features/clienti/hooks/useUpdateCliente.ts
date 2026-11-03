import { useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from '@tanstack/react-router'
import { notifications } from '@mantine/notifications'
import { toFieldErrors, toUserMessage } from '@/utils/apiErrors'
import { runPromise } from '@/utils/http'
import { updateCliente } from '../utils/clientiApi'
import { clientiKeys } from '../utils/queryKeys'
import type { ClienteRequest } from '../types'

export const useUpdateCliente = (id: number) => {
  const queryClient = useQueryClient()
  const navigate = useNavigate()

  return useMutation({
    mutationFn: (request: ClienteRequest) => runPromise(updateCliente(id, request)),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: clientiKeys.all })
      notifications.show({ color: 'green', title: 'Modifiche salvate', message: `Cliente #${id}` })
      await navigate({ to: '/clienti/$id', params: { id: String(id) } })
    },
    onError: (error) => {
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
