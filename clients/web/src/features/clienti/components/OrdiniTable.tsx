import { Table, Text } from '@mantine/core'
import { formatCurrency, formatDate, orDash } from '@/utils/format'
import type { Ordine } from '../types'

type OrdiniTableProps = {
  ordini: ReadonlyArray<Ordine>
  totale: number
}

const OrdiniTable = ({ ordini, totale }: OrdiniTableProps) =>
  ordini.length === 0 ? (
    <Text c="dimmed">Nessun ordine per questo cliente.</Text>
  ) : (
    <Table striped verticalSpacing="xs">
      <Table.Thead>
        <Table.Tr>
          <Table.Th>#</Table.Th>
          <Table.Th>Data</Table.Th>
          <Table.Th className="text-right">Importo</Table.Th>
          <Table.Th>Note</Table.Th>
        </Table.Tr>
      </Table.Thead>
      <Table.Tbody>
        {ordini.map((o) => (
          <Table.Tr key={o.id}>
            <Table.Td>{o.id}</Table.Td>
            <Table.Td>{formatDate(o.dataOrdine)}</Table.Td>
            <Table.Td className="text-right">{formatCurrency(o.importo)}</Table.Td>
            <Table.Td>{orDash(o.note)}</Table.Td>
          </Table.Tr>
        ))}
      </Table.Tbody>
      <Table.Tfoot>
        <Table.Tr>
          <Table.Th colSpan={2}>Totale</Table.Th>
          <Table.Th className="text-right">{formatCurrency(totale)}</Table.Th>
          <Table.Th />
        </Table.Tr>
      </Table.Tfoot>
    </Table>
  )

export default OrdiniTable
