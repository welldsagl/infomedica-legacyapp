import { QueryErrorResetBoundary } from '@tanstack/react-query'
import { ErrorBoundary } from 'react-error-boundary'
import { ErrorFallback } from '@/components'
import ClienteEditContent from './components/ClienteEditContent'

// Pagina di route: isola gli errori (anche delle query) dal resto dell'applicazione.
const ClienteEditPage = () => (
  <QueryErrorResetBoundary>
    {({ reset }) => (
      <ErrorBoundary FallbackComponent={ErrorFallback} onReset={reset}>
        <ClienteEditContent />
      </ErrorBoundary>
    )}
  </QueryErrorResetBoundary>
)

export default ClienteEditPage
