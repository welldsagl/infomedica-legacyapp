import { useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from '@tanstack/react-router'
import { notifications } from '@mantine/notifications'
import { toUserMessage } from '@/utils/apiErrors'
import { runPromise } from '@/utils/http'
import { deleteCliente } from '../utils/clientiApi'
import { clientiKeys } from '../utils/queryKeys'

export const useDeleteCliente = () => {
  const queryClient = useQueryClient()
  const navigate = useNavigate()

  return useMutation({
    mutationFn: (id: number) => runPromise(deleteCliente(id)),
    onSuccess: async (_, id) => {
      queryClient.removeQueries({ queryKey: clientiKeys.detail(id) })
      await queryClient.invalidateQueries({ queryKey: clientiKeys.lists() })
      notifications.show({ color: 'green', title: 'Cliente eliminato', message: `Cliente #${id}` })
      await navigate({ to: '/clienti', search: { page: 1 } })
    },
    // 409 dall'API: "Impossibile eliminare un cliente che ha ordini associati."
    onError: (error) => {
      notifications.show({
        color: 'red',
        title: 'Eliminazione non riuscita',
        message: toUserMessage(error),
      })
    },
  })
}
