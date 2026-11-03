import { Stack } from '@mantine/core'
import { AnchorLink, PageLoader } from '@/components'
import { useCliente } from '../hooks/useCliente'
import { useClienteId } from '../hooks/useClienteId'
import ClienteDetailView from './ClienteDetailView'

const ClienteDetailContent = () => {
  const id = useClienteId()
  const { cliente, isPending } = useCliente(id)

  return (
    <Stack gap="md">
      <AnchorLink to="/clienti" search={{ page: 1 }} size="sm">
        ← Torna all'elenco
      </AnchorLink>
      {isPending || cliente === undefined ? (
        <PageLoader />
      ) : (
        <ClienteDetailView cliente={cliente} />
      )}
    </Stack>
  )
}

export default ClienteDetailContent
