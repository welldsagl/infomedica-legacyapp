import { QueryErrorResetBoundary } from '@tanstack/react-query'
import { ErrorBoundary } from 'react-error-boundary'
import { ErrorFallback } from '@/components'
import ClientiListContent from './components/ClientiListContent'

// Pagina di route: isola gli errori (anche delle query) dal resto dell'applicazione.
const ClientiListPage = () => (
  <QueryErrorResetBoundary>
    {({ reset }) => (
      <ErrorBoundary FallbackComponent={ErrorFallback} onReset={reset}>
        <ClientiListContent />
      </ErrorBoundary>
    )}
  </QueryErrorResetBoundary>
)

export default ClientiListPage
