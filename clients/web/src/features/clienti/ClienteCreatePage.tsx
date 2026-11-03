import { QueryErrorResetBoundary } from '@tanstack/react-query'
import { ErrorBoundary } from 'react-error-boundary'
import { ErrorFallback } from '@/components'
import ClienteCreateContent from './components/ClienteCreateContent'

// Pagina di route: isola gli errori (anche delle query) dal resto dell'applicazione.
const ClienteCreatePage = () => (
  <QueryErrorResetBoundary>
    {({ reset }) => (
      <ErrorBoundary FallbackComponent={ErrorFallback} onReset={reset}>
        <ClienteCreateContent />
      </ErrorBoundary>
    )}
  </QueryErrorResetBoundary>
)

export default ClienteCreatePage
