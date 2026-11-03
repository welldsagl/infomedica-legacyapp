import { modals } from '@mantine/modals'
import { useDeleteCliente } from './useDeleteCliente'
import type { Cliente } from '../types'

export const useConfirmDeleteCliente = (cliente: Cliente) => {
  const mutation = useDeleteCliente()

  const confirmDelete = () => {
    modals.openConfirmModal({
      title: 'Eliminare il cliente?',
      children: `"${cliente.ragioneSociale}" verrà eliminato definitivamente.`,
      labels: { confirm: 'Elimina', cancel: 'Annulla' },
      confirmProps: { color: 'red' },
      onConfirm: () => {
        mutation.mutate(cliente.id)
      },
    })
  }

  return { confirmDelete, isDeleting: mutation.isPending }
}
