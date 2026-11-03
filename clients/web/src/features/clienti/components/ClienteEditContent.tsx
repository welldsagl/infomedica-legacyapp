import { Stack, Title } from '@mantine/core'
import { PageLoader } from '@/components'
import { useCliente } from '../hooks/useCliente'
import { useClienteId } from '../hooks/useClienteId'
import ClienteEditForm from './ClienteEditForm'

const ClienteEditContent = () => {
  const id = useClienteId()
  const { cliente, isPending } = useCliente(id)

  if (isPending || cliente === undefined) return <PageLoader />

  return (
    <Stack gap="md">
      <Title order={2}>Modifica cliente #{cliente.id}</Title>
      <ClienteEditForm cliente={cliente} />
    </Stack>
  )
}

export default ClienteEditContent
