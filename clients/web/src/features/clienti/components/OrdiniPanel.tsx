import { Skeleton } from '@mantine/core'
import { useOrdiniCliente } from '../hooks/useOrdiniCliente'
import OrdiniTable from './OrdiniTable'

type OrdiniPanelProps = {
  clienteId: number
}

const OrdiniPanel = ({ clienteId }: OrdiniPanelProps) => {
  const { ordini, totale, isPending } = useOrdiniCliente(clienteId)

  if (isPending) return <Skeleton height={120} />
  return <OrdiniTable ordini={ordini} totale={totale} />
}

export default OrdiniPanel
