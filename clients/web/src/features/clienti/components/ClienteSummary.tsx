import { Badge, Button, Card, Group, SimpleGrid, Stack, Text, Title } from '@mantine/core'
import { ButtonLink } from '@/components'
import { formatDateTime, orDash } from '@/utils/format'
import type { Cliente } from '../types'

type ClienteSummaryProps = {
  cliente: Cliente
  onDelete: () => void
  isDeleting: boolean
}

type FieldProps = {
  label: string
  value: string
}

// Sotto-componente non esportato, usato solo qui
const Field = ({ label, value }: FieldProps) => (
  <Stack gap={2}>
    <Text size="xs" c="dimmed" tt="uppercase">
      {label}
    </Text>
    <Text>{value}</Text>
  </Stack>
)

const ClienteSummary = ({ cliente, onDelete, isDeleting }: ClienteSummaryProps) => (
  <Card withBorder radius="md" padding="lg">
    <Group justify="space-between" mb="md">
      <Group gap="sm">
        <Title order={2}>{cliente.ragioneSociale}</Title>
        <Badge color={cliente.attivo ? 'green' : 'gray'} variant="light">
          {cliente.attivo ? 'Attivo' : 'Non attivo'}
        </Badge>
      </Group>
      <Group gap="xs">
        <ButtonLink to="/clienti/$id/modifica" params={{ id: String(cliente.id) }} variant="light">
          Modifica
        </ButtonLink>
        <Button color="red" variant="subtle" onClick={onDelete} loading={isDeleting}>
          Elimina
        </Button>
      </Group>
    </Group>

    <SimpleGrid cols={{ base: 1, sm: 2, md: 4 }}>
      <Field label="Codice" value={String(cliente.id)} />
      <Field label="Partita IVA" value={orDash(cliente.partitaIva)} />
      <Field label="Email" value={orDash(cliente.email)} />
      <Field label="Inserito il" value={formatDateTime(cliente.dataInserimento)} />
    </SimpleGrid>
  </Card>
)

export default ClienteSummary
