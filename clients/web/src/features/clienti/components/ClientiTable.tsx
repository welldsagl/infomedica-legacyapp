import { Badge, Table, Text } from '@mantine/core'
import { AnchorLink } from '@/components'
import { formatDate, orDash } from '@/utils/format'
import type { Cliente } from '../types'

type ClientiTableProps = {
  clienti: ReadonlyArray<Cliente>
}

const ClientiTable = ({ clienti }: ClientiTableProps) =>
  clienti.length === 0 ? (
    <Text c="dimmed" className="py-8 text-center">
      Nessun cliente presente.
    </Text>
  ) : (
    <Table.ScrollContainer minWidth={700}>
      <Table striped highlightOnHover verticalSpacing="sm">
        <Table.Thead>
          <Table.Tr>
            <Table.Th>#</Table.Th>
            <Table.Th>Ragione sociale</Table.Th>
            <Table.Th>Partita IVA</Table.Th>
            <Table.Th>Email</Table.Th>
            <Table.Th>Inserito il</Table.Th>
            <Table.Th>Stato</Table.Th>
          </Table.Tr>
        </Table.Thead>
        <Table.Tbody>
          {clienti.map((c) => (
            <Table.Tr key={c.id}>
              <Table.Td>{c.id}</Table.Td>
              <Table.Td>
                <AnchorLink to="/clienti/$id" params={{ id: String(c.id) }} fw={500}>
                  {c.ragioneSociale}
                </AnchorLink>
              </Table.Td>
              <Table.Td>{orDash(c.partitaIva)}</Table.Td>
              <Table.Td>{orDash(c.email)}</Table.Td>
              <Table.Td>{formatDate(c.dataInserimento)}</Table.Td>
              <Table.Td>
                <Badge color={c.attivo ? 'green' : 'gray'} variant="light">
                  {c.attivo ? 'Attivo' : 'Non attivo'}
                </Badge>
              </Table.Td>
            </Table.Tr>
          ))}
        </Table.Tbody>
      </Table>
    </Table.ScrollContainer>
  )

export default ClientiTable
