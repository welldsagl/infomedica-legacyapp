import { QueryClient } from '@tanstack/react-query'
import { HttpError } from '@/utils/apiErrors'

// Niente retry sugli errori 4xx: ripetere una richiesta sbagliata non la rende giusta.
const shouldRetry = (failureCount: number, error: unknown): boolean =>
  !(error instanceof HttpError && error.status < 500) && failureCount < 2

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 30_000,
      retry: shouldRetry,
      refetchOnWindowFocus: false,
    },
  },
})
