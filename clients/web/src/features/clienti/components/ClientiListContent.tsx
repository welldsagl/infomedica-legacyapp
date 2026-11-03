import { Group, LoadingOverlay, Stack, Title } from '@mantine/core'
import { ButtonLink, PageLoader } from '@/components'
import { useClienti } from '../hooks/useClienti'
import { useClientiPage } from '../hooks/useClientiPage'
import ClientiPager from './ClientiPager'
import ClientiTable from './ClientiTable'

const ClientiListContent = () => {
  const { page, setPage } = useClientiPage()
  const { clienti, totalCount, totalPages, isPending, isFetching } = useClienti(page)

  if (isPending) return <PageLoader />

  return (
    <Stack gap="md">
      <Group justify="space-between">
        <Title order={2}>Clienti</Title>
        <ButtonLink to="/clienti/nuovo">Nuovo cliente</ButtonLink>
      </Group>

      <div className="relative">
        <LoadingOverlay visible={isFetching} overlayProps={{ blur: 1 }} />
        <ClientiTable clienti={clienti} />
      </div>

      <ClientiPager
        page={page}
        totalPages={totalPages}
        totalCount={totalCount}
        onChange={setPage}
      />
    </Stack>
  )
}

export default ClientiListContent
