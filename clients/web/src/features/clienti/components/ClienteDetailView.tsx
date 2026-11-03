import { Stack } from '@mantine/core'
import { useConfirmDeleteCliente } from '../hooks/useConfirmDeleteCliente'
import ClienteSummary from './ClienteSummary'
import OrdiniSection from './OrdiniSection'
import type { Cliente } from '../types'

type ClienteDetailViewProps = {
  cliente: Cliente
}

const ClienteDetailView = ({ cliente }: ClienteDetailViewProps) => {
  const { confirmDelete, isDeleting } = useConfirmDeleteCliente(cliente)

  return (
    <Stack gap="lg">
      <ClienteSummary cliente={cliente} onDelete={confirmDelete} isDeleting={isDeleting} />
      <OrdiniSection clienteId={cliente.id} />
    </Stack>
  )
}

export default ClienteDetailView
