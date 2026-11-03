import { Card, Title } from '@mantine/core'
import { QueryErrorResetBoundary } from '@tanstack/react-query'
import { ErrorBoundary } from 'react-error-boundary'
import { ErrorFallback } from '@/components'
import OrdiniPanel from './OrdiniPanel'

type OrdiniSectionProps = {
  clienteId: number
}

// Boundary dedicato: se gli ordini non si caricano, l'anagrafica resta visibile.
const OrdiniSection = ({ clienteId }: OrdiniSectionProps) => (
  <Card withBorder radius="md" padding="lg">
    <Title order={3} mb="md">
      Ordini
    </Title>
    <QueryErrorResetBoundary>
      {({ reset }) => (
        <ErrorBoundary FallbackComponent={ErrorFallback} onReset={reset}>
          <OrdiniPanel clienteId={clienteId} />
        </ErrorBoundary>
      )}
    </QueryErrorResetBoundary>
  </Card>
)

export default OrdiniSection
