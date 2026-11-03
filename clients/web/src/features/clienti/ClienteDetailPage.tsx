import { QueryErrorResetBoundary } from '@tanstack/react-query'
import { ErrorBoundary } from 'react-error-boundary'
import { ErrorFallback } from '@/components'
import ClienteDetailContent from './components/ClienteDetailContent'

// Pagina di route: isola gli errori (anche delle query) dal resto dell'applicazione.
const ClienteDetailPage = () => (
  <QueryErrorResetBoundary>
    {({ reset }) => (
      <ErrorBoundary FallbackComponent={ErrorFallback} onReset={reset}>
        <ClienteDetailContent />
      </ErrorBoundary>
    )}
  </QueryErrorResetBoundary>
)

export default ClienteDetailPage
