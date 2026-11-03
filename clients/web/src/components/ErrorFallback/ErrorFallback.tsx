import { Alert, Button, Stack } from '@mantine/core'
import { toUserMessage } from '@/utils/apiErrors'
import type { FallbackProps } from 'react-error-boundary'

const ErrorFallback = ({ error, resetErrorBoundary }: FallbackProps) => (
  <Stack className="py-8" align="center">
    <Alert color="red" title="Si è verificato un errore" className="w-full max-w-xl">
      {toUserMessage(error)}
    </Alert>
    <Button variant="outline" onClick={resetErrorBoundary}>
      Riprova
    </Button>
  </Stack>
)

export default ErrorFallback
