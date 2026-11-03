import { Group, Pagination, Text } from '@mantine/core'

type ClientiPagerProps = {
  page: number
  totalPages: number
  totalCount: number
  onChange: (page: number) => void
}

const ClientiPager = ({ page, totalPages, totalCount, onChange }: ClientiPagerProps) => (
  <Group justify="space-between">
    <Text size="sm" c="dimmed">
      {totalCount} clienti
    </Text>
    <Pagination value={page} total={totalPages} onChange={onChange} />
  </Group>
)

export default ClientiPager
